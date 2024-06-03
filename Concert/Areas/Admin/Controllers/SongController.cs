using Concert.DataAccess.Data;
using Concert.DataAccess.Repository.IRepository;
using Concert.Models;
using Concert.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConcertWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SongController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SongController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Song> objSongList = _unitOfWork.Song.GetAll().ToList();

            return View(objSongList);
        }

        public IActionResult Create()
        {
            SongVM songVM = new()
            {
                GenreList = _unitOfWork.Genre.GetAll().Select(u => new SelectListItem()
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Song = new Song()
            };

            return View(songVM);
        }

        [HttpPost]
        public IActionResult Create(SongVM songVM)
        {
            ValidateSong(songVM.Song);

            if (ModelState.IsValid)
            {
                _unitOfWork.Song.Add(songVM.Song);
                _unitOfWork.Save();
                TempData["success"] = "Song created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                songVM.GenreList = _unitOfWork.Genre.GetAll().Select(u => new SelectListItem()
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(songVM);
            }
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Song? songFromDb = _unitOfWork.Song.Get(u => u.Id == id);

            if (songFromDb == null)
            {
                return NotFound();
            }

            return View(songFromDb);
        }

        [HttpPost]
        public IActionResult Edit(SongVM songVM)
        {
            ValidateSong(songVM.Song);

            if (ModelState.IsValid)
            {
                _unitOfWork.Song.Update(songVM.Song);
                _unitOfWork.Save();
                TempData["success"] = "Song updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Song? songFromDb = _unitOfWork.Song.Get(u => u.Id == id);

            if (songFromDb == null)
            {
                return NotFound();
            }

            return View(songFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Song? obj = _unitOfWork.Song.Get(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Song.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Song deleted successfully";
            return RedirectToAction("Index");
        }

        private void ValidateSong(Song song)
        {   
            if (char.IsAsciiLetterLower(song.Artist.ElementAt(0)))
            {
                ModelState.AddModelError("artist", "The Artist Name must start with a capital letter.");
            }

            if (char.IsAsciiLetterLower(song.Title.ElementAt(0)))
            {
                ModelState.AddModelError("title", "The Song Title must start with a capital letter.");
            }
        }
    }
}