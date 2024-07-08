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
    public class CompanyController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Company> CompanyList = _unitOfWork.Company.GetAll().ToList();

            return View(CompanyList);
        }

        // Upsert = Update + Insert(Edit)
        public IActionResult Upsert(int? id)
        {
            Company company = new Company();

            // Update, otherwise is Create
            if (id != null && id != 0)
            {
                company = _unitOfWork.Company.Get(u => u.Id == id);

                if (company == null)
                {
                    return NotFound();
                }
            }

            return View(company);
        }

        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            ValidateCompany(company);

            if (ModelState.IsValid)
            {
                // Create
                if (company.Id == 0) 
                {
                    _unitOfWork.Company.Add(company);
                    _unitOfWork.Save();
                    TempData["success"] = "Company created successfully";
                }
                // Update
                else
                {
                    _unitOfWork.Company.Update(company);
                    _unitOfWork.Save();
                    TempData["success"] = "Company updated successfully";
                }
                
                return RedirectToAction("Index");
            }
            else
            {   
                return View(company);
            }
        }

        private void ValidateCompany(Company Company)
        {   
            // Validation not needed at the moment
        }

        #region ApiCalls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> CompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = CompanyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Company companyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            if (companyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Company.Remove(companyToBeDeleted);
            _unitOfWork.Save();
 
            return Json(new { success = true, message = "Company deleted successfully" });
        }

        #endregion
    }
}