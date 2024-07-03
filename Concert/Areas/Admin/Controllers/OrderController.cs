using Concert.DataAccess.Data;
using Concert.DataAccess.Repository.IRepository;
using Concert.Models;
using Concert.Models.ViewModels;
using Concert.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        #region ApiCalls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<OrderHeader> orderHeaderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            return Json(new { data = orderHeaderList });
        }

        #endregion
    }
}