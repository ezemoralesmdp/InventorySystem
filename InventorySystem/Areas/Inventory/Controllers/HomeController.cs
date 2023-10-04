using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;
using InventorySystem.Models.ErrorViewModels;
using InventorySystem.Models.Specifications;
using InventorySystem.Models.ViewModels;
using InventorySystem.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace InventorySystem.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitWork _unitWork;

        [BindProperty]
        public ShoppingCartVM shoppingCartVM { get; set; }

        public HomeController(ILogger<HomeController> logger, IUnitWork unitWork)
        {
            _logger = logger;
            _unitWork = unitWork;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string search = "", string actualSearch = "")
        {
            // Controlar sesion
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim != null)
            {
                var shoppingCartList = await _unitWork.ShoppingCart.GetAll(s => s.UserId == claim.Value);
                var productsNumber = shoppingCartList.Count(); // Número de registros
                HttpContext.Session.SetInt32(SD.ssShoppingCarts, productsNumber);
            }

            if (!String.IsNullOrEmpty(search))
                pageNumber = 1;
            else
                search = actualSearch;

            ViewData["ActualSearch"] = search;

            if(pageNumber < 1)
                pageNumber = 1;

            Parameters parameters = new()
            {
                PageNumber = pageNumber,
                PageSize = 8
            };

            var result = _unitWork.Product.GetAllPaginated(parameters);

            if (!String.IsNullOrEmpty(search))
                result = _unitWork.Product.GetAllPaginated(parameters, p => p.Description.Contains(search));

            ViewData["TotalPages"] = result.MetaData.TotalPages;
            ViewData["TotalRecords"] = result.MetaData.TotalCount;
            ViewData["PageSize"] = result.MetaData.PageSize;
            ViewData["PageNumber"] = pageNumber;
            ViewData["Previous"] = "disabled";
            ViewData["Next"] = "";

            if (pageNumber > 1)
                ViewData["Previous"] = "";

            if (result.MetaData.TotalPages <= pageNumber)
                ViewData["Next"] = "disabled";

            return View(result);
        }

        public async Task<IActionResult> Detail(int id)
        {
            shoppingCartVM = new ShoppingCartVM();
            shoppingCartVM.Company = await _unitWork.Company.GetFirst();
            shoppingCartVM.Product = await _unitWork.Product.GetFirst(p => p.Id == id, includeProperties: "Brand,Category");
            var storeProduct = await _unitWork.StoreProduct.GetFirst(s => s.ProductId == id && s.StoreId == shoppingCartVM.Company.StoreSellId);

            shoppingCartVM.Stock = (storeProduct == null) ? 0 : storeProduct.Amount;
            
            shoppingCartVM.ShoppingCart = new ShoppingCart()
            {
                Product = shoppingCartVM.Product,
                ProductId = shoppingCartVM.Product.Id
            };

            return View(shoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Detail(ShoppingCartVM shoppingCartVM)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCartVM.ShoppingCart.UserId = claim.Value;

            ShoppingCart shoppingCartDB = await _unitWork.ShoppingCart.GetFirst(
                                            s => s.UserId == claim.Value && 
                                            s.ProductId == shoppingCartVM.ShoppingCart.ProductId
                                        );

            if (shoppingCartDB == null)
                await _unitWork.ShoppingCart.Add(shoppingCartVM.ShoppingCart);
            else
            {
                shoppingCartDB.Amount += shoppingCartVM.ShoppingCart.Amount;
                _unitWork.ShoppingCart.Update(shoppingCartDB);
            }

            await _unitWork.Save();
            TempData[SD.Success] = "Product added to shopping cart successfully";

            // Agregar valor a la sesion
            var shoppingCartList = await _unitWork.ShoppingCart.GetAll(s => s.UserId == claim.Value);
            var productsNumber = shoppingCartList.Count(); // Número de registros
            HttpContext.Session.SetInt32(SD.ssShoppingCarts, productsNumber);

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