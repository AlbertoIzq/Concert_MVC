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
    public class ServiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Service> ServiceList = _unitOfWork.Service.GetAll().ToList();

            return View(ServiceList);
        }

        // Upsert = Update + Insert(Edit)
        public IActionResult Upsert(int? id)
        {
            Service service = new Service();

            // Update, otherwise is Create
            if (id != null && id != 0)
            {
                service = _unitOfWork.Service.Get(u => u.Id == id);

                if (service == null)
                {
                    return NotFound();
                }
            }

            return View(service);
        }

        [HttpPost]
        public IActionResult Upsert(Service service)
        {
            ValidateService(service);

            if (ModelState.IsValid)
            {
                // Create
                if (service.Id == 0)
                {
                    _unitOfWork.Service.Add(service);
                    _unitOfWork.Save();
                    TempData["success"] = "Service created successfully";
                }
                // Update
                else
                {
                    _unitOfWork.Service.Update(service);
                    _unitOfWork.Save();
                    TempData["success"] = "Service updated successfully";
                }

                return RedirectToAction("Index");
            }
            else
            {
                return View(service);
            }
        }

        private void ValidateService(Service Service)
        {
            // Validation not needed at the moment
        }

        #region ApiCalls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Service> ServiceList = _unitOfWork.Service.GetAll().ToList();
            return Json(new { data = ServiceList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Service serviceToBeDeleted = _unitOfWork.Service.Get(u => u.Id == id);
            if (serviceToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            // Cannot delete default service
            if (id == 1)
            {
                return Json(new { success = false, message = "Error while deleting, you cannot delete default service" });
            }

            _unitOfWork.Service.Remove(serviceToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Service deleted successfully" });
        }

        #endregion
    }
}