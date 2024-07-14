using Concert.DataAccess.Repository.IRepository;
using Concert.Models;
using Concert.Models.ViewModels;
using Concert.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace ConcertWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class SetListController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public SetListVM SetListVM { get; set; }

        public SetListController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            SetListVM = new SetListVM()
            {
                SongList = _unitOfWork.SetListSong.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Song"),
                ServiceList = _unitOfWork.SetListService.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Service"),
                OrderHeader = new()
            };

            // Add service by default if is isn't already included
            if (SetListVM.ServiceList.ToList().Where(u => u.ServiceId == 1).ToList() == null)
            {
                SetListService setListServiceDefault = new SetListService();
                setListServiceDefault.ApplicationUserId = userId;
                setListServiceDefault.ServiceId = 1;
                _unitOfWork.SetListService.Add(setListServiceDefault);
                _unitOfWork.Save();
            }

            // Calculate Order total
            int songCount = SetListVM.SongList.Count();
            foreach (var setListService in SetListVM.ServiceList)
            {
                setListService.PriceVariable = songCount * setListService.Service.PricePerSong;
                SetListVM.OrderHeader.OrderTotal += setListService.PriceVariable + setListService.Service.PriceFixed;
            }

			return View(SetListVM);
        }

        [HttpGet]
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            SetListVM = new SetListVM()
            {
                SongList = _unitOfWork.SetListSong.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Song"),
                ServiceList = _unitOfWork.SetListService.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Service"),
                OrderHeader = new()
            };

            SetListVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            SetListVM.OrderHeader.Name = SetListVM.OrderHeader.ApplicationUser.Name;
            SetListVM.OrderHeader.Surname = SetListVM.OrderHeader.ApplicationUser.Surname;
            SetListVM.OrderHeader.PhoneNumber = SetListVM.OrderHeader.ApplicationUser.PhoneNumber;
            SetListVM.OrderHeader.StreetAddress = SetListVM.OrderHeader.ApplicationUser.StreetAddress;
            SetListVM.OrderHeader.City = SetListVM.OrderHeader.ApplicationUser.City;
            SetListVM.OrderHeader.State = SetListVM.OrderHeader.ApplicationUser.State;
            SetListVM.OrderHeader.Country = SetListVM.OrderHeader.ApplicationUser.Country;
            SetListVM.OrderHeader.PostalCode = SetListVM.OrderHeader.ApplicationUser.PostalCode;

            // Calculate Order total
            int songCount = SetListVM.SongList.Count();
            foreach (var setListService in SetListVM.ServiceList)
            {
                setListService.PriceVariable = songCount * setListService.Service.PricePerSong;
                SetListVM.OrderHeader.OrderTotal += setListService.PriceVariable + setListService.Service.PriceFixed;
            }

            return View(SetListVM);
        }

        [HttpPost]
        [ActionName("Summary")]
		public IActionResult SummaryPOST()
		{
            ValidateSetListVM(SetListVM);

			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			SetListVM.SongList = _unitOfWork.SetListSong.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Song");
			SetListVM.ServiceList = _unitOfWork.SetListService.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Service");
			SetListVM.OrderHeader.ApplicationUserId = userId;

			// Calculate Order total
			int songCount = SetListVM.SongList.Count();
			foreach (var setListService in SetListVM.ServiceList)
			{
				setListService.PriceVariable = songCount * setListService.Service.PricePerSong;
				SetListVM.OrderHeader.OrderTotal += setListService.PriceVariable + setListService.Service.PriceFixed;
			}

			if (ModelState.IsValid)
			{
				ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

				SetListVM.OrderHeader.OrderDate = System.DateTime.Now;

				// It's a regular customer account and we need to capture payment
				// GetValueOrDefault() because it can be null
				if (applicationUser.CompanyId.GetValueOrDefault() == 0)
				{
					SetListVM.OrderHeader.OrderStatus = SD.STATUS_PENDING;
					SetListVM.OrderHeader.PaymentStatus = SD.PAYMENT_STATUS_PENDING;
				}
				// It's a company user
				else
				{
					SetListVM.OrderHeader.OrderStatus = SD.STATUS_APPROVED;
					SetListVM.OrderHeader.PaymentStatus = SD.PAYMENT_STATUS_DELAYED_PAYMENT;
				}

                // Create OrderHeader, OrderDetailSong and OrderDetailService in database
                CreateOrderHeaderAndDetailSongAndServiceInDb();

				// If it's a regular customer account and we need to capture payment
				if (applicationUser.CompanyId.GetValueOrDefault() == 0)
				{
                    // Stripe logic
                    string successShortUrl = $"Customer/SetList/OrderConfirmation/{SetListVM.OrderHeader.Id}";
                    string cancelShortUrl = "Customer/SetList/Index";

                    // For SetStripeOptions method refactorized which is used also in OrderController
                    var listSongs = new List<OrderDetailSong>();
                    foreach (var setListSong in SetListVM.SongList)
                    {
                        var orderDetailSong = new OrderDetailSong()
                        {
                            SongId = setListSong.SongId,
                            Song = setListSong.Song
                        };
                        listSongs.Add(orderDetailSong);
                    }
                    var listServices = new List<OrderDetailService>();
                    foreach (var setListService in SetListVM.ServiceList)
                    {
                        var orderDetailService = new OrderDetailService()
                        {
                            ServiceId = setListService.ServiceId,
                            Service = setListService.Service
                        };
                        listServices.Add(orderDetailService);
                    }

                    var service = new SessionService();
                    var options = UtilityMethods.SetStripeOptions(successShortUrl, cancelShortUrl, listSongs, listServices);
                    Session session = service.Create(options);

                    // PaymentIntentId will be null because it's only populated once payment is successful
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(SetListVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.Save();
                    Response.Headers.Add("Location", session.Url);

                    // This means that we are redirecting to a new Url
                    return new StatusCodeResult(303);
				}

                return RedirectToAction(nameof(OrderConfirmation), new { id = SetListVM.OrderHeader.Id });
			}

			return View(SetListVM);
		}

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");

			// It's an order by a costumer
			if (orderHeader.PaymentStatus != SD.PAYMENT_STATUS_DELAYED_PAYMENT)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
					_unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.STATUS_APPROVED, SD.PAYMENT_STATUS_APPROVED);
                    _unitOfWork.Save();
				}
                HttpContext.Session.Clear();
            }

            // Empty SetListSong
            List<SetListSong> setListSongs = _unitOfWork.SetListSong.
                GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.SetListSong.RemoveRange(setListSongs);
            _unitOfWork.Save();

			// Empty SetListSong except default service
			List<SetListService> setListServices = _unitOfWork.SetListService.
				GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            setListServices = setListServices.Where(u => u.ServiceId != 1).ToList();
			_unitOfWork.SetListService.RemoveRange(setListServices);
			_unitOfWork.Save();

			return View(id);
        }

		public IActionResult SetBefore(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var setListSongs = _unitOfWork.SetListSong.GetAll(u => u.ApplicationUserId == userId).OrderBy(u => u.Order).ToList();
            int indexSetListSong = setListSongs.FindIndex(u => u.Id == id);
            int order = setListSongs[indexSetListSong].Order;

            if (order > 1)
            {
                setListSongs[indexSetListSong].Order--;
                setListSongs[indexSetListSong - 1].Order++;
                _unitOfWork.SetListSong.Update(setListSongs[indexSetListSong]);
                _unitOfWork.SetListSong.Update(setListSongs[indexSetListSong - 1]);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult SetAfter(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var setListSongs = _unitOfWork.SetListSong.GetAll(u => u.ApplicationUserId == userId).OrderBy(u => u.Order).ToList();
            int indexSetListSong = setListSongs.FindIndex(u => u.Id == id);
            int order = setListSongs[indexSetListSong].Order;
            int maxOrder = setListSongs.Last().Order;

            if (order < maxOrder)
            {
                setListSongs[indexSetListSong].Order++;
                setListSongs[indexSetListSong + 1].Order--;
                _unitOfWork.SetListSong.Update(setListSongs[indexSetListSong]);
                _unitOfWork.SetListSong.Update(setListSongs[indexSetListSong + 1]);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveSong(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var setListSong = _unitOfWork.SetListSong.GetAll(u => u.ApplicationUserId == userId).Where(u => u.Id == id).FirstOrDefault();

            if (setListSong != null)
            {
                // Update other songs order after removing current song
                var setListSongs = _unitOfWork.SetListSong.GetAll(u => u.ApplicationUserId == userId).OrderBy(u => u.Order).ToList();
                int indexSetListSong = setListSongs.FindIndex(u => u.Id == id);

                for (int i = indexSetListSong + 1; i < setListSongs.Count(); i++)
                {
                    setListSongs[i].Order--;
                    _unitOfWork.SetListSong.Update(setListSongs[i]);
                    _unitOfWork.Save();
                }
                // Remove current song
                _unitOfWork.SetListSong.Remove(setListSong);
                _unitOfWork.Save();
                TempData["success"] = "Song removed successfully from Set list";
                HttpContext.Session.SetInt32(SD.SESSION_SETLIST,
                    _unitOfWork.SetListSong.GetAll(u => u.ApplicationUserId == userId).Count());
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveService(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var setListService = _unitOfWork.SetListService.GetAll(u => u.ApplicationUserId == userId).Where(u => u.Id == id).FirstOrDefault();

            if (setListService != null)
            {
                if (setListService.ServiceId == 1)
                {
                    TempData["warning"] = "Error while deleting, you cannot delete default service";
                }
                else
                {
                    _unitOfWork.SetListService.Remove(setListService);
                    _unitOfWork.Save();
                }
            }

            return RedirectToAction(nameof(Index));
        }

		private void ValidateSetListVM(SetListVM setListVM)
		{
            if (!setListVM.OrderHeader.PhoneNumber.IsNullOrEmpty() &&
                setListVM.OrderHeader.PhoneNumber.Any(char.IsAsciiLetter))
            {
				ModelState.AddModelError("OrderHeader.PhoneNumber", "Phone number cannot contain letters.");
			}

			if (setListVM.OrderHeader.ConcertDate <= DateTime.Now)
			{
				ModelState.AddModelError("OrderHeader.ConcertDate", "Concert date cannot be in the past");
			}

		}

        /// <summary>
        /// Create OrderHeader, OrderDetailSong and OrderDetailService in the database
        /// </summary>
        private void CreateOrderHeaderAndDetailSongAndServiceInDb()
        {
			// Create OrderHeader
			_unitOfWork.OrderHeader.Add(SetListVM.OrderHeader);
			_unitOfWork.Save();

			// Create OrderDetailSong
			foreach (var setListSong in SetListVM.SongList)
			{
				OrderDetailSong orderDetailSong = new()
				{
					SongId = setListSong.SongId,
					Order = setListSong.Order,
					OrderHeaderId = SetListVM.OrderHeader.Id
				};
				_unitOfWork.OrderDetailSong.Add(orderDetailSong);
				_unitOfWork.Save();
			}

			// Create OrderDetailService
			foreach (var setListService in SetListVM.ServiceList)
			{
				OrderDetailService orderDetailService = new()
				{
					ServiceId = setListService.ServiceId,
					OrderHeaderId = SetListVM.OrderHeader.Id
				};
				_unitOfWork.OrderDetailService.Add(orderDetailService);
				_unitOfWork.Save();
			}
		}
	}
}
