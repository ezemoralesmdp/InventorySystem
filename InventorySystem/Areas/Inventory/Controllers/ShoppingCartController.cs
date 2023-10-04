using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models.ViewModels;
using InventorySystem.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventorySystem.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitWork _unitWork;

        [BindProperty]
        public ShoppingCartVM shoppingCartVM { get; set; }

        public ShoppingCartController(IUnitWork unitWork)
        {
            _unitWork = unitWork;
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
    }
}