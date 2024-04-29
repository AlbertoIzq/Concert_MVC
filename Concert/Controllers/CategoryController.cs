using Microsoft.AspNetCore.Mvc;

namespace Concert.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
