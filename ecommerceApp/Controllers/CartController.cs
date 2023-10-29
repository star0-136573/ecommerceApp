using ecommerceApp.Models;
using ecommerceApp.Models.ViewModels;
using ecommerceApp.Repository.IRepository;
using ecommerceApp.Utility;
using Habanero.BO.ClassDefinition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Stripe;
using Stripe.Checkout;
using Stripe.Issuing;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace ecommerceApp.Controllers
{
    [Authorize]
    public class CartController : Controller
    {

        private readonly IShoppingCartRepository _dbcart;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        private IApplicationUserRepository _appUser { get; set; }
		private IOrderDetailRepository _ordDt { get; set; }
		public CartController(IShoppingCartRepository dbcart, IApplicationUserRepository appUser, IOrderDetailRepository ordDt)
        {
            _dbcart = dbcart;
            _appUser = appUser;
            _ordDt = ordDt; 
        }


        public IActionResult Index()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _dbcart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),

				OrderDetail = new()
			};

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPrice(cart);
                ShoppingCartVM.OrderDetail.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }


        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _dbcart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderDetail = new()
            };

            ShoppingCartVM.OrderDetail.ApplicationUser = _appUser.Get(u => u.Id == userId);

            ShoppingCartVM.OrderDetail.Name = ShoppingCartVM.OrderDetail.ApplicationUser.Name;
            ShoppingCartVM.OrderDetail.PhoneNumber = ShoppingCartVM.OrderDetail.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderDetail.StreetAddress = ShoppingCartVM.OrderDetail.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderDetail.City = ShoppingCartVM.OrderDetail.ApplicationUser.City;
            ShoppingCartVM.OrderDetail.State = ShoppingCartVM.OrderDetail.ApplicationUser.State;
            ShoppingCartVM.OrderDetail.PostalCode = ShoppingCartVM.OrderDetail.ApplicationUser.PostalCode;



            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPrice(cart);
                ShoppingCartVM.OrderDetail.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }



		[HttpPost]
		[ActionName("Summary")]
		public IActionResult SummaryPOST()
		{
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

           ShoppingCartVM.ShoppingCartList = _dbcart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product");

           ShoppingCartVM.OrderDetail.OrderDate = System.DateTime.Now;
           ShoppingCartVM.OrderDetail.ApplicationUserId = userId;

           ShoppingCartVM.OrderDetail.ApplicationUser = _appUser.Get(u => u.Id == userId);





            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPrice(cart);
               ShoppingCartVM.OrderDetail.OrderTotal += (cart.Price * cart.Count);

            }



          



         
       



            //it is a regular customer account and we need to capture payment
            //stripe logic
            var domain = "https://localhost:7254/";
            var options = new SessionCreateOptions
            {
                /* SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderDetail.Id}",*/
                SuccessUrl = domain+$"Cart/Confirmation",
                CancelUrl = domain + "Home/index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };
			

			foreach (var item in ShoppingCartVM.ShoppingCartList)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                        Currency = "GBP",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }


            var service = new SessionService();
            Session session = service.Create(options);
         

            ShoppingCartVM.OrderDetail.PaymentId = session.Id;
			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				OrderDetail orderDetail = new()
				{
					ProductId = cart.ProductId,
					Product = cart.Product,
					Id = ShoppingCartVM.OrderDetail.Id,
					Price = cart.Price,
					Count = cart.Count,
					OrderTotal = ShoppingCartVM.OrderDetail.OrderTotal,
					ApplicationUserId = userId,
                    PaymentId = session.Id,

					Name = ShoppingCartVM.OrderDetail.ApplicationUser.Name,
					PhoneNumber = ShoppingCartVM.OrderDetail.ApplicationUser.PhoneNumber,
					StreetAddress = ShoppingCartVM.OrderDetail.ApplicationUser.StreetAddress,
					City = ShoppingCartVM.OrderDetail.ApplicationUser.City,
					State = ShoppingCartVM.OrderDetail.ApplicationUser.State,
					PostalCode = ShoppingCartVM.OrderDetail.ApplicationUser.PostalCode,
				};
				_ordDt.Add(orderDetail);
				_ordDt.Save();


			}

			Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);


            return RedirectToAction(nameof(Confirmation), new { id = session.Id });

        
		}


        public IActionResult Confirmation(string id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            /*var list = _ordDt.GetAll(u => u.PaymentId == id,
                includeProperties: "Product");
            ViewData["paymentInfo"] = list;*/
            var cartFromDb = _dbcart.GetAll(u => u.ApplicationUserId == userId);
           

            _dbcart.RemoveRange(cartFromDb);

            _dbcart.Save();
            return View(id) ;
        }













			public IActionResult Plus(int cartId)
        {
            var cartFromDb = _dbcart.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _dbcart.Update(cartFromDb);
            _dbcart.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _dbcart.Get(u => u.Id == cartId);
            if (cartFromDb.Count <= 1)
            {
                //remove that from cart
                _dbcart.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count -= 1;
                _dbcart.Update(cartFromDb);
            }

            _dbcart.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _dbcart.Get(u => u.Id == cartId);
            _dbcart.Remove(cartFromDb);
            _dbcart.Save();
            return RedirectToAction(nameof(Index));
        }




        private double GetPrice(ShoppingCart cart)
        {
           
                return cart.Product.Price;
           
        }
    }
}
