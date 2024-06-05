using Concert.DataAccess.Data;
using Concert.DataAccess.Repository.IRepository;
using Concert.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConcertWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LanguageController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LanguageController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Language> languageList = _unitOfWork.Language.GetAll().ToList();

            return View(languageList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Language language)
        {
            ValidateLanguage(language);

            if (ModelState.IsValid)
            {
                _unitOfWork.Language.Add(language);
                _unitOfWork.Save();
                TempData["success"] = "Language created successfully";
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

            Language? languageFromDb = _unitOfWork.Language.Get(u => u.Id == id);

            if (languageFromDb == null)
            {
                return NotFound();
            }

            return View(languageFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Language language)
        {
            ValidateLanguage(language);

            if (ModelState.IsValid)
            {
                _unitOfWork.Language.Update(language);
                _unitOfWork.Save();
                TempData["success"] = "Language updated successfully";
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

            Language? languageFromDb = _unitOfWork.Language.Get(u => u.Id == id);

            if (languageFromDb == null)
            {
                return NotFound();
            }

            return View(languageFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Language? language = _unitOfWork.Language.Get(u => u.Id == id);

            if (language == null)
            {
                return NotFound();
            }

            _unitOfWork.Language.Remove(language);
            _unitOfWork.Save();
            TempData["success"] = "Language deleted successfully";
            return RedirectToAction("Index");
        }

        private void ValidateLanguage(Language language)
        {
            if (!language.Name.All(char.IsLetter))
            {
                ModelState.AddModelError("name", "The Name can contain letters only.");
            }
        }
    }
}