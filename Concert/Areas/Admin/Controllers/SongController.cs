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

        // Upsert = Update + Insert(Edit)
        public IActionResult Upsert(int? id)
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

            // Update, otherwise is Create
            if (id != null && id != 0)
            {
                songVM.Song = _unitOfWork.Song.Get(u => u.Id == id);

                if (songVM.Song == null)
                {
                    return NotFound();
                }
            }

            return View(songVM);
        }

        [HttpPost]
        public IActionResult Upsert(SongVM songVM, IFormFile? file)
        {
            ValidateSong(songVM.Song);

            if (ModelState.IsValid)
            {
                // Create
                _unitOfWork.Song.Add(songVM.Song);
                _unitOfWork.Save();
                TempData["success"] = "Song created successfully";
                return RedirectToAction("Index");

                /* // Update
                _unitOfWork.Song.Update(songVM.Song);
                _unitOfWork.Save();
                TempData["success"] = "Song updated successfully";
                return RedirectToAction("Index");
                 */
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