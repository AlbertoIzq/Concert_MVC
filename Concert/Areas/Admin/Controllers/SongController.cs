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
                songVM.Song = _unitOfWork.Song.Get(u => u.Id == id, includeProperties: "SongImages");

                if (songVM.Song == null)
                {
                    return NotFound();
                }
            }

            return View(songVM);
        }

        [HttpPost]
        public IActionResult Upsert(SongVM songVM, List<IFormFile> files)
        {
            ValidateSong(songVM.Song, files);

            if (ModelState.IsValid)
            {
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

                // Create
                // Copy uploaded image file and save ImageUrl
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string internalPath = @"images\songs\song-" + songVM.Song.Id;
                        string songPath = Path.Combine(wwwRootPath, internalPath);

                        if (!Directory.Exists(songPath))
                        {
                            Directory.CreateDirectory(songPath);
                        }

                        using (var fileStream = new FileStream(Path.Combine(songPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        SongImage songImage = new()
                        {
                            ImageUrl = "\\" + Path.Combine(internalPath, fileName),
                            SongId = songVM.Song.Id,
                        };

                        if (songVM.Song.SongImages == null)
                        {
                            songVM.Song.SongImages = new List<SongImage>();
                        }

                        songVM.Song.SongImages.Add(songImage);
                        // This would be a way to do it
                        // _unitOfWork.SongImage.Add(songImage);
                    }

                    _unitOfWork.Song.Update(songVM.Song);
                    _unitOfWork.Save();
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

        public IActionResult DeleteImage(int imageId)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            var imageToBeDeleted = _unitOfWork.SongImage.Get(u => u.Id == imageId);
            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    // Delete the old image
                    var oldImagePath = imageToBeDeleted.ImageUrl.TrimStart(Path.PathSeparator);
                    var oldImageFullPath = Path.Combine(wwwRootPath, oldImagePath);

                    if (System.IO.File.Exists(oldImageFullPath))
                    {
                        System.IO.File.Delete(oldImageFullPath);
                    }
                }

                _unitOfWork.SongImage.Remove(imageToBeDeleted);
                _unitOfWork.Save();

                TempData["success"] = "Image deleted successfully";
            }

            return RedirectToAction(nameof(Upsert), new { id = imageToBeDeleted.SongId });
        }

        private void ValidateSong(Song song, List<IFormFile> files)
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
            else if (files == null && song.SongImages == null)
            {
                ModelState.AddModelError("Song.ImageUrl", "You have to add an image file.");
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
            //DeleteOldImage(songToBeDeleted);
            /// @todo

            _unitOfWork.Song.Remove(songToBeDeleted);
            _unitOfWork.Save();
            //TempData["success"] = "Song deleted successfully";
            return Json(new { success = true, message = "Song deleted successfully" });
        }

        #endregion
    }
}