using ecommerceApp.Models;
using ecommerceApp.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ecommerceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _db;
        public HomeController(ILogger<HomeController> logger, IProductRepository db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _db.GetAll(includeProperties: "Category");
            return View(productList);

          
        }


        public IActionResult Details(int productId)
        {
            Product product = _db.Get(u => u.Id == productId, includeProperties: "Category");
            return View(product);
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