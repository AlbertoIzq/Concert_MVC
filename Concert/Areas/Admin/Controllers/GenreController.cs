using Concert.DataAccess.Data;
using Concert.DataAccess.Repository.IRepository;
using Concert.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConcertWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GenreController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public GenreController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Genre> objGenreList = _unitOfWork.Genre.GetAll().ToList();

            return View(objGenreList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Genre obj)
        {
            ValidateGenre(obj);

            if (ModelState.IsValid)
            {
                _unitOfWork.Genre.Add(obj);
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
        public IActionResult Edit(Genre obj)
        {
            ValidateGenre(obj);

            if (ModelState.IsValid)
            {
                _unitOfWork.Genre.Update(obj);
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
            Genre? obj = _unitOfWork.Genre.Get(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Genre.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Genre deleted successfully";
            return RedirectToAction("Index");
        }

        private void ValidateGenre(Genre genre)
        {
            if (genre.Name.Any(char.IsDigit))
            {
                ModelState.AddModelError("name", "The Name can contain letters only.");
            }
        }
    }
}