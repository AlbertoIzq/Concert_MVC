using Concert.DataAccess.Data;
using Concert.DataAccess.Repository.IRepository;
using Concert.Models;
using Concert.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConcertWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ROLE_ADMIN)]
    public class GenreController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public GenreController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Genre> genreList = _unitOfWork.Genre.GetAll().ToList();

            return View(genreList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Genre genre)
        {
            ValidateGenre(genre);

            if (ModelState.IsValid)
            {
                _unitOfWork.Genre.Add(genre);
                _unitOfWork.Save();
                TempData["success"] = "Genre created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Genre? genreFromDb = _unitOfWork.Genre.Get(u => u.Id == id);

            if (genreFromDb == null)
            {
                return NotFound();
            }

            return View(genreFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Genre genre)
        {
            ValidateGenre(genre);

            if (ModelState.IsValid)
            {
                _unitOfWork.Genre.Update(genre);
                _unitOfWork.Save();
                TempData["success"] = "Genre updated successfully";
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

            Genre? genreFromDb = _unitOfWork.Genre.Get(u => u.Id == id);

            if (genreFromDb == null)
            {
                return NotFound();
            }

            return View(genreFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Genre? genre = _unitOfWork.Genre.Get(u => u.Id == id);

            if (genre == null)
            {
                return NotFound();
            }

			var songsWithThisGenre = _unitOfWork.Song.GetAll(u => u.GenreId == id).ToList();

			if (songsWithThisGenre.Count() > 0)
			{
				TempData["warning"] = "You cannot delete this genre because it's already used in some songs";
			}
			else
			{
				_unitOfWork.Genre.Remove(genre);
				_unitOfWork.Save();
				TempData["success"] = "Genre deleted successfully";
			}

			
            return RedirectToAction("Index");
        }

        private void ValidateGenre(Genre genre)
        {
            if (genre.Name.Any(char.IsDigit))
            {
                ModelState.AddModelError("Name", "The Name can contain letters only.");
            }
        }
    }
}