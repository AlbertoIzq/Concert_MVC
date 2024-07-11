using Concert.DataAccess.Data;
using Concert.DataAccess.Repository.IRepository;
using Concert.Models;
using Concert.Models.ViewModels;
using Concert.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Diagnostics;
using System.Security.Claims;

namespace ConcertWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
			OrderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetailSongs = _unitOfWork.OrderDetailSong.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Song"),
                OrderDetailServices = _unitOfWork.OrderDetailService.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Service")
            };

            return View(OrderVM);
        }

        [HttpPost]
        [Authorize(Roles = SD.ROLE_ADMIN + "," + SD.ROLE_EMPLOYEE)]
		public IActionResult UpdateOrderDetail()
		{
            var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);

            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
			orderHeaderFromDb.Surname = OrderVM.OrderHeader.Surname;
			orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
			orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
			orderHeaderFromDb.City = OrderVM.OrderHeader.City;
			orderHeaderFromDb.State = OrderVM.OrderHeader.State;
			orderHeaderFromDb.Country = OrderVM.OrderHeader.Country;
			orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;

            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Order details updated successfully";

			return RedirectToAction(nameof(Details), new {orderId = orderHeaderFromDb.Id});
		}

        [HttpPost]
        [Authorize(Roles = SD.ROLE_ADMIN + "," + SD.ROLE_EMPLOYEE)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.STATUS_IN_PROCESS);
            _unitOfWork.Save();
            TempData["success"] = "Order details updated successfully";

            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.ROLE_ADMIN + "," + SD.ROLE_EMPLOYEE)]
        public IActionResult Confirm(int orderId)
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId);
            orderHeader.OrderStatus = SD.STATUS_CONFIRMED;
            orderHeader.ConfirmationDate = DateTime.Now;

            if (orderHeader.PaymentStatus == SD.PAYMENT_STATUS_DELAYED_PAYMENT)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(SD.PAYMENT_DELAYED_DAYS));
            }
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["success"] = "Order confirmed successfully";

            return RedirectToAction(nameof(Details), new { orderId = orderId });
        }

        [HttpPost]
        [Authorize(Roles = SD.ROLE_ADMIN + "," + SD.ROLE_EMPLOYEE)]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);

            // We refund the customer only if the payment was already done
            if (orderHeader.PaymentStatus == SD.PAYMENT_STATUS_APPROVED)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.STATUS_CANCELLED, SD.PAYMENT_STATUS_REFUNDED);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.STATUS_CANCELLED, SD.PAYMENT_STATUS_CANCELLED);
            }

            _unitOfWork.Save();
            TempData["success"] = "Order cancelled successfully";

            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        [ActionName("Details")]
        [HttpPost]
        public IActionResult Details_PAY_NOW()
        {
            // We populate them again because when we're posting we may loose some information
            OrderVM.OrderHeader = _unitOfWork.OrderHeader.
                Get(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
            OrderVM.OrderDetailSongs = _unitOfWork.OrderDetailSong.
                GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id, includeProperties: "Song");
            OrderVM.OrderDetailServices = _unitOfWork.OrderDetailService.
                GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id, includeProperties: "Service");

            // Stripe logic
            var service = new SessionService();
            string successShortUrl = $"Admin/Order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.Id}";
            string cancelShortUrl = $"Admin/Order/Details?orderId={OrderVM.OrderHeader.Id}";
            var listSongs = OrderVM.OrderDetailSongs;
            var listServices = OrderVM.OrderDetailServices;
            var options = Methods.SetStripeOptions(successShortUrl, cancelShortUrl, listSongs, listServices);
            Session session = service.Create(options);

            // PaymentIntentId will be null because it's only populated once payment is successful
            _unitOfWork.OrderHeader.UpdateStripePaymentId(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);

            // This means that we are redirecting to a new Url
            return new StatusCodeResult(303);
        }

        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderHeaderId, includeProperties: "ApplicationUser");

            // It's an order by a company
            if (orderHeader.PaymentStatus == SD.PAYMENT_STATUS_DELAYED_PAYMENT)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus , SD.PAYMENT_STATUS_APPROVED);
                    _unitOfWork.Save();
                }
            }

            return View(orderHeaderId);
        }

        #region ApiCalls

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();

            // If user isn't admin or employee, then he/she can only see his/her own orders
            if (!User.IsInRole(SD.ROLE_ADMIN) && !User.IsInRole(SD.ROLE_EMPLOYEE))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                
                orderHeaderList = orderHeaderList.Where(u => u.ApplicationUserId == userId).ToList();
            }

            switch (status)
            {
                case "pending":
                    orderHeaderList = orderHeaderList.Where(u => u.PaymentStatus == SD.PAYMENT_STATUS_DELAYED_PAYMENT);
                    break;
                case "inprocess":
                    orderHeaderList = orderHeaderList.Where(u => u.OrderStatus == SD.STATUS_IN_PROCESS);
                    break;
                case "completed":
                    orderHeaderList = orderHeaderList.Where(u => u.OrderStatus == SD.STATUS_CONFIRMED);
                    break;
                case "approved":
                    orderHeaderList = orderHeaderList.Where(u => u.OrderStatus == SD.STATUS_APPROVED);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaderList });
        }

        #endregion
    }
}