using ConcertWeb.Data;
using ConcertWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace Concert.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            List<Category> objCategoryList = _db.Categories.ToList();

            return View(objCategoryList);
        }
    }
}
