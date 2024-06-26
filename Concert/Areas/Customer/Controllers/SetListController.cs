using Concert.DataAccess.Repository.IRepository;
using Concert.Models;
using Concert.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Diagnostics;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    }
}
