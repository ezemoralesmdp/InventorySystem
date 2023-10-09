using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;
using InventorySystem.Models.ViewModels;
using InventorySystem.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace InventorySystem.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitWork _unitWork;
        private string _webUrl;

        [BindProperty]
        public ShoppingCartVM shoppingCartVM { get; set; }

        public ShoppingCartController(IUnitWork unitWork, IConfiguration configuration)
        {
            _unitWork = unitWork;
            _webUrl = configuration.GetValue<string>("DomainUrls:WEB_URL");
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCartVM = new ShoppingCartVM();
            shoppingCartVM.Order = new Models.Order();
            shoppingCartVM.ShoppingCartList = await _unitWork.ShoppingCart.GetAll(u => u.UserId == claim.Value, includeProperties: "Product");

            shoppingCartVM.Order.TotalOrder = 0;
            shoppingCartVM.Order.UserId = claim.Value;

            foreach (var list in shoppingCartVM.ShoppingCartList)
            {
                list.Price = list.Product.Price; // Siempre mostrar el precio actual del producto
                shoppingCartVM.Order.TotalOrder += (list.Price * list.Amount);
            }

            return View(shoppingCartVM);
        }

        public async Task<IActionResult> More(int shoppingCartId)
        {
            var shoppingCart = await _unitWork.ShoppingCart.GetFirst(s => s.Id == shoppingCartId);
            shoppingCart.Amount += 1;
            await _unitWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Less(int shoppingCartId)
        {
            var shoppingCart = await _unitWork.ShoppingCart.GetFirst(s => s.Id == shoppingCartId);

            if(shoppingCart.Amount == 1)
            {
                // Remover el registro y actualizar la sesion
                var shoppingCartList = await _unitWork.ShoppingCart.GetAll(s => s.UserId == shoppingCart.UserId);
                var numberProducts = shoppingCartList.Count();
                _unitWork.ShoppingCart.Remove(shoppingCart);
                await _unitWork.Save();
                HttpContext.Session.SetInt32(SD.ssShoppingCarts, numberProducts - 1);
            }
            else
            {
                shoppingCart.Amount -= 1;
                await _unitWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int shoppingCartId)
        {
            // Remover el registro y actualizar la sesion
            var shoppingCart = await _unitWork.ShoppingCart.GetFirst(s => s.Id == shoppingCartId);
            var shoppingCartList = await _unitWork.ShoppingCart.GetAll(s => s.UserId == shoppingCart.UserId);
            var numberProducts = shoppingCartList.Count();
            _unitWork.ShoppingCart.Remove(shoppingCart);
            await _unitWork.Save();
            HttpContext.Session.SetInt32(SD.ssShoppingCarts, numberProducts - 1);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Proceed()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCartVM = new ShoppingCartVM()
            {
                Order = new Models.Order(),
                ShoppingCartList = await _unitWork.ShoppingCart.GetAll(s => s.UserId == claim.Value, includeProperties: "Product"),
                Company = await _unitWork.Company.GetFirst()
            };

            shoppingCartVM.Order.TotalOrder = 0;
            shoppingCartVM.Order.User = await _unitWork.User.GetFirst(u => u.Id == claim.Value);

            foreach (var list in shoppingCartVM.ShoppingCartList)
            {
                list.Price = list.Product.Price;
                shoppingCartVM.Order.TotalOrder += (list.Price * list.Amount);
            }

            shoppingCartVM.Order.ClientNames = shoppingCartVM.Order.User.Names + " " + shoppingCartVM.Order.User.LastName;
            shoppingCartVM.Order.Telephone = shoppingCartVM.Order.User.PhoneNumber;
            shoppingCartVM.Order.Address = shoppingCartVM.Order.User.Address;
            shoppingCartVM.Order.Country = shoppingCartVM.Order.User.Country;
            shoppingCartVM.Order.City = shoppingCartVM.Order.User.City;

            // Controlar Stock
            foreach (var list in shoppingCartVM.ShoppingCartList)
            {
                // Capturar el stock de cada producto
                var product = await _unitWork.StoreProduct.GetFirst(s => s.ProductId == list.ProductId && s.StoreId == shoppingCartVM.Company.StoreSellId);

                if (list.Amount > product.Amount)
                {
                    TempData[SD.Error] = "The quantity of the product: " + list.Product.Description + " exceeds the current quantity (" + product.Amount + ")";
                    return RedirectToAction("Index");
                }
            }

            return View(shoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Proceed(ShoppingCartVM shoppingCartVM)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCartVM.ShoppingCartList = await _unitWork.ShoppingCart.GetAll(s => s.UserId == claim.Value, includeProperties: "Product");

            shoppingCartVM.Company = await _unitWork.Company.GetFirst();
            shoppingCartVM.Order.TotalOrder = 0;
            shoppingCartVM.Order.UserId = claim.Value;
            shoppingCartVM.Order.OrderDate = DateTime.Now;

            foreach (var list in shoppingCartVM.ShoppingCartList)
            {
                list.Price = list.Product.Price;
                shoppingCartVM.Order.TotalOrder += (list.Price + list.Amount);
            }

            //Controlar Stock
            foreach (var list in shoppingCartVM.ShoppingCartList)
            {
                var product = await _unitWork.StoreProduct.GetFirst(s => s.ProductId == list.ProductId && s.StoreId == shoppingCartVM.Company.StoreSellId);

                if(list.Amount > product.Amount)
                {
                    TempData[SD.Error] = $"The quantity of the product {list.Product.Description} exceeds the current Stock ({product.Amount})";
                    return RedirectToAction("Index");
                }
            }

            shoppingCartVM.Order.OrderState = SD.PendingStatus;
            shoppingCartVM.Order.PaymentState = SD.PaymentStatusPending;
            await _unitWork.Order.Add(shoppingCartVM.Order);
            await _unitWork.Save();

            // Grabar Detalle Orden
            foreach (var list in shoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = list.ProductId,
                    OrderId = shoppingCartVM.Order.Id,
                    Price = list.Price,
                    Amount = list.Amount
                };

                await _unitWork.OrderDetail.Add(orderDetail);
                await _unitWork.Save();
            }

            // Stripe
            var user = await _unitWork.User.GetFirst(u => u.Id == claim.Value);
            var options = new SessionCreateOptions
            {
                SuccessUrl = _webUrl + $"Inventory/ShoppingCart/OrderConfirmation?id={shoppingCartVM.Order.Id}",
                CancelUrl = _webUrl + $"Inventory/ShoppingCart/Index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = user.Email
            };

            foreach(var list in shoppingCartVM.ShoppingCartList)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(list.Price * 100), // Siempre precio * 100
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = list.Product.Description
                        }
                    },
                    Quantity = list.Amount
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            _unitWork.Order.UpdatePaymentStripeId(shoppingCartVM.Order.Id, session.Id, session.PaymentIntentId);
            await _unitWork.Save();

            Response.Headers.Add("Location", session.Url); // Redirecciona a Stripe
            return new StatusCodeResult(303);
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = await _unitWork.Order.GetFirst(o => o.Id == id, includeProperties: "User");
            var service = new SessionService();
            Session session = service.Get(order.SessionId);
            var shoppingCart = await _unitWork.ShoppingCart.GetAll(u => u.UserId == order.UserId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitWork.Order.UpdatePaymentStripeId(id, session.Id, session.PaymentIntentId);
                _unitWork.Order.UpdateState(id, SD.ApprovedStatus, SD.PaymentStatusApproved);
                await _unitWork.Save();

                // Disminuir Stock de la bodega de Venta
                var company = await _unitWork.Company.GetFirst();
                foreach (var list in shoppingCart)
                {
                    var storeProduct = new StoreProduct();
                    storeProduct = await _unitWork.StoreProduct.GetFirst(s => s.ProductId == list.ProductId && s.StoreId == company.StoreSellId);

                    await _unitWork.KardexInventory.RegisterKardex(storeProduct.Id, "Out", "Sell - Order #" + id, storeProduct.Amount, list.Amount, order.UserId);
                    storeProduct.Amount -= list.Amount;
                    await _unitWork.Save();
                }
            }

            // Borramos el carro de compras y la sesion de carro de compras
            List<ShoppingCart> shoppingCartList = shoppingCart.ToList();
            _unitWork.ShoppingCart.RemoveRange(shoppingCartList);
            await _unitWork.Save();
            HttpContext.Session.SetInt32(SD.ssShoppingCarts, 0);

            return View(id);
        }
    }
}