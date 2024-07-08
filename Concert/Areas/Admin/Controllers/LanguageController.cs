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
    public class LanguageController : BaseController
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

            var songsWithThisLanguage = _unitOfWork.Song.GetAll(u => u.LanguageId == id).ToList();

            if (songsWithThisLanguage.Count() > 0)
            {
				TempData["warning"] = "You cannot delete this language because it's already used in some songs";
			}
            else
            {
				_unitOfWork.Language.Remove(language);
				_unitOfWork.Save();
				TempData["success"] = "Language deleted successfully";
			}
            
            return RedirectToAction("Index");
        }

        private void ValidateLanguage(Language language)
        {
            if (!language.Name.All(char.IsLetter))
            {
                ModelState.AddModelError("Name", "The Name can contain letters only.");
            }
        }
    }
}