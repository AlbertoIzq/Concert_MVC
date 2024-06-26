using Concert.DataAccess.Repository.IRepository;
using Concert.Models;
using Concert.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ConcertWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class SetListController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
