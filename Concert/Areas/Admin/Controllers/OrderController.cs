using Concert.DataAccess.Data;
using Concert.DataAccess.Repository.IRepository;
using Concert.Models;
using Concert.Models.ViewModels;
using Concert.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace ConcertWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ROLE_ADMIN)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

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
            OrderVM orderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetailSongs = _unitOfWork.OrderDetailSong.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Song"),
                OrderDetailServices = _unitOfWork.OrderDetailService.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Service")
            };

            return View(orderVM);
        }

        #region ApiCalls

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();

            switch (status)
            {
                case "pending":
                    orderHeaderList = orderHeaderList.Where(u => u.PaymentStatus == SD.PAYMENT_STATUS_DELAYED_PAYMENT);
                    break;
                case "inprocess":
                    orderHeaderList = orderHeaderList.Where(u => u.OrderStatus == SD.STATUS_IN_PROCESS);
                    break;
                case "completed":
                    orderHeaderList = orderHeaderList.Where(u => u.OrderStatus == SD.STATUS_SHIPPED);
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