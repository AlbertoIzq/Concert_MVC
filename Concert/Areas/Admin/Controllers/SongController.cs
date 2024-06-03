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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SongController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Song> songList = _unitOfWork.Song.GetAll().ToList();

            return View(songList);
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
                        var oldImagePath = songVM.Song.ImageUrl.TrimStart(Path.PathSeparator);
                        var oldImageFullPath = Path.Combine(wwwRootPath , oldImagePath);

                        if(System.IO.File.Exists(oldImageFullPath))
                        {
                            System.IO.File.Delete(oldImageFullPath);
                        }
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
            Song? song = _unitOfWork.Song.Get(u => u.Id == id);

            if (song == null)
            {
                return NotFound();
            }

            _unitOfWork.Song.Remove(song);
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