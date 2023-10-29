using ecommerceApp.Models;
using ecommerceApp.Repository.IRepository;
using ecommerceApp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ecommerceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productdb;
        private readonly IShoppingCartRepository _cartdb;
        public HomeController(ILogger<HomeController> logger, IProductRepository db, IShoppingCartRepository cartdb)
        {
            _logger = logger;
            _productdb = db;
            _cartdb = cartdb;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart,
                _cartdb.GetAll(u => u.ApplicationUserId == claim.Value).Count());
            }
            IEnumerable<Product> productList = _productdb.GetAll(includeProperties: "Category");
            return View(productList);

          
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _productdb.Get(u => u.Id == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = productId
            };

            return View(cart);
        }


        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = _cartdb.Get(u => u.ApplicationUserId == userId &&
        u.ProductId == shoppingCart.ProductId);

            if (cartFromDb != null)
            {
                //shopping cart exists
                cartFromDb.Count += shoppingCart.Count;
                _cartdb.Update(cartFromDb);
            }
            else
            {
                //add cart record
                _cartdb.Add(shoppingCart);
            }

            TempData["success"] = "Cart updated successfully";
            _cartdb.Save();

            
            return RedirectToAction(nameof(Index));
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}