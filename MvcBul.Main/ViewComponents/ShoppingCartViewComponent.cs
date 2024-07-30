using Microsoft.AspNetCore.Mvc;
using MvcBul.DataAccess.Repository.IRepository;
using MvcBul.Utility;
using System.Security.Claims;

namespace MvcBul.Main.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                var countOfShippingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == userId.Value).Count();
                if (HttpContext.Session.GetInt32(SD.SessionCart) == null)
                {
                    HttpContext.Session.SetInt32(SD.SessionCart, countOfShippingCarts);
                }

                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }

            return View(0);
        }
    }
}
