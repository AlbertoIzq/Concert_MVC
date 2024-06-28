using Concert.DataAccess.Repository.IRepository;
using Concert.Models;
using Concert.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ConcertWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class SetListController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
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

            };

            // Calculate Order total
            int songCount = SetListVM.SongList.Count();
            foreach (var setListService in SetListVM.ServiceList)
            {
                setListService.PriceVariable = songCount * setListService.Service.PricePerSong;
                SetListVM.OrderTotal += setListService.PriceVariable + setListService.Service.PriceFixed;
            }

            return View(SetListVM);
        }
    }
}
