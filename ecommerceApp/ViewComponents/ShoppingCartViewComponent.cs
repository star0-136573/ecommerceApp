using ecommerceApp.Repository.IRepository;
using ecommerceApp.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ecommerceApp.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {

    
        private readonly IShoppingCartRepository _dbcart;
        public ShoppingCartViewComponent(IShoppingCartRepository dbcart)
        {
            _dbcart = dbcart;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {

                if (HttpContext.Session.GetInt32(SD.SessionCart) == null)
                {
                    HttpContext.Session.SetInt32(SD.SessionCart,
                    _dbcart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
                }

                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }

    }
}
