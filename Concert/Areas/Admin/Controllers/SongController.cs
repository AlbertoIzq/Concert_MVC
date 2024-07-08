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
    public class SongController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SongController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Song> songList = _unitOfWork.Song.GetAll(includeProperties:"Genre,Language").ToList();

            return View(songList);
        }

        // Upsert = Update + Insert(Edit)
        public IActionResult Upsert(int? id)
        {
            SongVM songVM = new()
            {
                GenreList = _unitOfWork.Genre.GetAll().OrderBy(u => u.Name).Select(u => new SelectListItem()
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                LanguageList = _unitOfWork.Language.GetAll().OrderBy(u => u.Name).Select(u => new SelectListItem()
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
            ValidateSong(songVM.Song, file);

            if (ModelState.IsValid)
            {
                // Create
                // Copy uploaded image file and save ImageUrl
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    const string internalPath = @"images\song";
                    string songPath = Path.Combine(wwwRootPath, internalPath);

                    // If we upload a new image
                    if (!string.IsNullOrEmpty(songVM.Song.ImageUrl))
                    {
                        // Delete the old image
                        DeleteOldImage(songVM.Song);
                    }

                    using (var fileStream = new FileStream(Path.Combine(songPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    songVM.Song.ImageUrl = Path.Combine(internalPath, fileName);
                }

                // Create
                if (songVM.Song.Id == 0) 
                {
                    _unitOfWork.Song.Add(songVM.Song);
                    _unitOfWork.Save();
                    TempData["success"] = "Song created successfully";
                }
                // Update
                else
                {
                    _unitOfWork.Song.Update(songVM.Song);
                    _unitOfWork.Save();
                    TempData["success"] = "Song updated successfully";
                }
                
                return RedirectToAction("Index");
            }
            else
            {
                // When returning back to the view it expects the dropdown to be populated
                songVM.GenreList = _unitOfWork.Genre.GetAll().Select(u => new SelectListItem()
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                songVM.LanguageList = _unitOfWork.Language.GetAll().Select(u => new SelectListItem()
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(songVM);
            }
        }

        private void ValidateSong(Song song, IFormFile? file)
        {   
            if (char.IsAsciiLetterLower(song.Artist.ElementAt(0)))
            {
                ModelState.AddModelError("Song.Artist", "The Artist Name must start with a capital letter.");
            }

            if (char.IsAsciiLetterLower(song.Title.ElementAt(0)))
            {
                ModelState.AddModelError("Song.Title", "The Song Title must start with a capital letter.");
            }

            // If we create a song without adding an image or we edit a song that doesn't have an image
            else if (file == null && song.ImageUrl == null)
            {
                ModelState.AddModelError("Song.ImageUrl", "You have to add an image file.");
            }
        }

        private void DeleteOldImage(Song song)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            // Delete the old image
            var oldImagePath = song.ImageUrl.TrimStart(Path.PathSeparator);
            var oldImageFullPath = Path.Combine(wwwRootPath, oldImagePath);

            if (System.IO.File.Exists(oldImageFullPath))
            {
                System.IO.File.Delete(oldImageFullPath);
            }
        }

        #region ApiCalls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Song> songList = _unitOfWork.Song.GetAll(includeProperties: "Genre,Language").ToList();
            return Json(new { data = songList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
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
        }

        #endregion
    }
}