using Concert.DataAccess.Repository.IRepository;
using Concert.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ConcertWeb.ViewComponents
{
    public class SetListViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public SetListViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(SD.SESSION_SETLIST) == null)
                {
                    HttpContext.Session.SetInt32(SD.SESSION_SETLIST,
                        _unitOfWork.SetListSong.GetAll(u => u.ApplicationUserId == claim.Value).Count());
                }
                
                return View(HttpContext.Session.GetInt32(SD.SESSION_SETLIST));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
