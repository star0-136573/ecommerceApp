using ecommerceApp.Models;
using ecommerceApp.Models.ViewModels;
using ecommerceApp.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ecommerceApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _db; 
        private readonly ICategoryRepository _dbCty;
        private readonly IWebHostEnvironment _webHostEnv;

        public ProductController(IProductRepository db,ICategoryRepository dbc, IWebHostEnvironment webHostEnv)
        {
            _dbCty = dbc;
            _db = db;
            _webHostEnv = webHostEnv;
        }

        public IActionResult Index()
        {
            List<Product> product = _db.GetAll(includeProperties: "Category").ToList();

            return View(product);
        }

        public IActionResult Create()
        {
            ProductVM productVM = new()
            {
                CategoryList = _dbCty.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            return View(productVM);
        }

        [HttpPost]
        public IActionResult Create(ProductVM productVM, IFormFile file)
        {

            if (ModelState.IsValid && file != null)
            {
                   
              
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(_webHostEnv.WebRootPath, @"images\product");

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                

                _db.Add(productVM.Product);
                _db.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _dbCty.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }

        }


        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            ProductVM productVM = new()
            {
                CategoryList = _dbCty.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            productVM.Product = _db.Get(u => u.Id == id);

            if (productVM.Product == null)
            {
                return NotFound();
            }
            
            return View(productVM);
        }
        [HttpPost]
        public IActionResult Edit(ProductVM productVM, IFormFile file)
        {

            if (ModelState.IsValid)
            {
                
              
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(_webHostEnv.WebRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        
                        var oldImagePath =
                            Path.Combine(_webHostEnv.WebRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                



              
                
                _db.Update(productVM.Product);
                
                _db.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _dbCty.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }

        }

       /* public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? productFromDb = _db.Get(u => u.Id == id);


            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Product? obj = _db.Get(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }
            _db.Remove(obj);
            _db.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");
        }*/

        #region  apicall


        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _db.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }


        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var toDelete = _db.Get(u => u.Id == id);
            if (toDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath =
                           Path.Combine(_webHostEnv.WebRootPath,
                           toDelete.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _db.Remove(toDelete);
            _db.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion




    }


}
