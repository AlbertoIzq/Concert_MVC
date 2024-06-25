using Concert.DataAccess.Repository.IRepository;
using Concert.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ConcertWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Song> songList = _unitOfWork.Song.GetAll(includeProperties: "Genre,Language");

            return View(songList);
        }

        public IActionResult Details(int songId)
        {
            SetListSong setListSong = new()
            {
                Song = _unitOfWork.Song.Get(u => u.Id == songId, includeProperties: "Genre,Language"),
                SongId = songId,
                Order = 1 // By default
            };

            return View(setListSong);
        }

        /// <summary>
        /// [Authorize] because if someone is posting, they must be an authorized user (no matter which role)
        /// i.e. they have to be logged into the website
        /// </summary>
        /// <param name="setListSong"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public IActionResult Details(SetListSong setListSong)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            setListSong.ApplicationUserId = userId;

            var setlistSongsFromDb = _unitOfWork.SetListSong.GetAll(u => u.ApplicationUserId == userId);

            if (setlistSongsFromDb != null)
            {
                // If it already exists in the db
                if (setlistSongsFromDb.Where(u => u.SongId == setListSong.SongId).FirstOrDefault() != null)
                {
                    TempData["warning"] = "Song already added to Set list";
                }
                // If it doesn't, save it as the last song added to setlist
                else
                {
                    var orderedList = setlistSongsFromDb.OrderBy(u => u.Order).ToList();
                    if (orderedList.Count > 0)
                    {
                        setListSong.Order = orderedList.Last().Order + 1;
                    }
                    else
                    {
                        setListSong.Order = 1;
                    }

                    _unitOfWork.SetListSong.Add(setListSong);
                    _unitOfWork.Save();
                    TempData["success"] = "Song added successfully to Set list";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Services()
        {
            List<Service> serviceList = _unitOfWork.Service.GetAll().OrderBy(u => u.Name).ToList();
            int indexDefaultService = serviceList.FindIndex(u => u.Id == 1);

            Service defaultService = serviceList[indexDefaultService];
            for (int i = indexDefaultService; i > 0 ; i--)
                serviceList[i] = serviceList[i - 1];
            serviceList[0] = defaultService;

            return View(serviceList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region ApiCalls

        [HttpPost]
        public IActionResult Services(int? id)
        {
            return Json(new { success = true, message = "Service added successfully to Set list" });

            /*
            Song songToBeDeleted = _unitOfWork.Song.Get(u => u.Id == id);
            if (songToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            // Delete the old image
            DeleteOldImage(songToBeDeleted);

            _unitOfWork.Song.Remove(songToBeDeleted);
            _unitOfWork.Save();
            //TempData["success"] = "Song deleted successfully";
            return Json(new { success = true, message = "Song deleted successfully" });
            */
        }

        #endregion
    }
}
