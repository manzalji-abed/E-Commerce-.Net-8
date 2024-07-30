using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcBul.DataAccess.Repository.IRepository;
using MvcBul.Models;
using MvcBul.Models.ViewModels;
using MvcBul.Utility;
using Newtonsoft.Json.Linq;
using Stripe.Checkout;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using System.Security.Policy;

namespace MvcBul.Main.Controllers
{
    [Authorize]
    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == userId, includeProps: "Product"),
                OrderHeader=new()
            };

            foreach(var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Product.ProductImages = _unitOfWork.ProductImage.GetAll(u => u.ProductId == cart.Product.Id).ToList();
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }

        public IActionResult Plus (int cardId)
        {
            var cardFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cardId, tracked:true);
            cardFromDb.Count++;
            _unitOfWork.ShoppingCart.Update(cardFromDb);
            _unitOfWork.Save();
            return RedirectToAction("Index");


        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user=_unitOfWork.ApplicationUser.Get(u=>u.Id== userId);

			var existingOrderHeader = _unitOfWork.OrderHeader.Get(
	        u => u.UserId == userId && u.OrderStatus == SD.StatusPending);


			ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == userId, includeProps: "Product"),
				OrderHeader = existingOrderHeader ?? new OrderHeader
				{
					UserId =userId,
                    ApplicationUser = user,
                    Name=user.Name,
                    PhoneNumber=user.PhoneNumber,
                    StreetAddress=user.StreetAddress,
                    City=user.City,
                    State=user.State,
                    PostalCode=user.PostalCode
                },
            };

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == userId, includeProps: "Product");

            var existingOrderHeader = _unitOfWork.OrderHeader.Get(
            u => u.UserId == userId && u.OrderStatus == SD.StatusPending);

            if (existingOrderHeader != null)
            {
                ShoppingCartVM.OrderHeader = existingOrderHeader;
            }

            else
            {
                ShoppingCartVM.OrderHeader.ApplicationUser = null;

				foreach (var cart in ShoppingCartVM.ShoppingCartList)
                {
                    cart.Price = GetPriceBasedOnQuantity(cart);
                    ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
                }


                if (user.CompanyId.GetValueOrDefault() == 0)
                {
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                    ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;

                }
                else
                {
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                    ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
                }
                if (ShoppingCartVM.OrderHeader.Id == 0)
                {
                    _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
					_unitOfWork.Save();
				}
			}


			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				OrderDetail orderDetail = new()
				{
					ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count,
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            if (user.CompanyId.GetValueOrDefault() == 0)
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;

            }

            if (user.CompanyId.GetValueOrDefault() == 0)
            {
                //it is a regular customer account and we need to capture payment
                //stripe logic
                var domain = "https://localhost:7292/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + $"cart/index",
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
                            Currency = "cad",
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
                _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            return RedirectToAction(nameof(OrderConfirmation),new { id = ShoppingCartVM.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProps: "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                //this is an order by customer

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }


            }

            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
                .GetAll(u => u.UserId == orderHeader.UserId).ToList();

            _unitOfWork.ShoppingCart.DeleteRange(shoppingCarts);
            HttpContext.Session.Clear();
            _unitOfWork.Save();
            return View(id);
        }
        public IActionResult Minus(int cardId)
        {
            var cardFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cardId, tracked:true);
            if (cardFromDb.Count <= 1)
            {
                var countOfShippingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == cardFromDb.UserId).Count()-1;
                HttpContext.Session.SetInt32(SD.SessionCart, countOfShippingCarts);
                _unitOfWork.ShoppingCart.Delete(cardFromDb);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            cardFromDb.Count--;
            _unitOfWork.ShoppingCart.Update(cardFromDb);
            _unitOfWork.Save();
            return RedirectToAction("Index");

        }

        public IActionResult Delete(int cardId)
        {
            var cardFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cardId, tracked:true);
            if (cardFromDb != null)
            {
                var countOfShippingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == cardFromDb.UserId).Count()-1;
                HttpContext.Session.SetInt32(SD.SessionCart, countOfShippingCarts);
                _unitOfWork.ShoppingCart.Delete(cardFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        private double GetPriceBasedOnQuantity(ShoppingCart cart)
        {
            if (cart.Count <= 50)
            {
                return cart.Product.Price;
            }
            else
            {
                if(cart.Count <= 100)
                {
                    return cart.Product.Price50;
                }
                else
                {
                    return cart.Product.Price100;
                }
            }
        }
    }
}
