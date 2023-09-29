using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;
using InventorySystem.Models.ViewModels;
using InventorySystem.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;

namespace InventorySystem.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Inventory)]
    public class InventoryController : Controller
    {
        private readonly IUnitWork _unitWork;

        [BindProperty]
        public InventoryVM inventoryVM { get; set; }

        public InventoryController(IUnitWork unitWork)
        {
            _unitWork = unitWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NewInventory()
        {
            inventoryVM = new()
            {
                Inventory = new Models.Inventory(),
                StoreList = _unitWork.Inventory.GetAllDropDownList("Store")
            };

            inventoryVM.Inventory.State = false;
            // Obtener el Id del Usuario desde la sesion
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            inventoryVM.Inventory.UserId = claim.Value;
            inventoryVM.Inventory.InitialDate = DateTime.Now;
            inventoryVM.Inventory.FinalDate = DateTime.Now;

            return View(inventoryVM);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _unitWork.StoreProduct.GetAll(includeProperties: "Store,Product");
            return Json(new { data = all });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewInventory(InventoryVM inventoryVM)
        {
            if (ModelState.IsValid)
            {
                inventoryVM.Inventory.InitialDate = DateTime.Now;
                inventoryVM.Inventory.FinalDate = DateTime.Now;
                await _unitWork.Inventory.Add(inventoryVM.Inventory);
                await _unitWork.Save();
                return RedirectToAction(nameof(DetailInventory), new { id = inventoryVM.Inventory.Id });
            }

            inventoryVM.StoreList = _unitWork.Inventory.GetAllDropDownList("Store");
            return View(inventoryVM);
        }

        public async Task<IActionResult> More(int id) // Recibe Id del detalle
        {
            inventoryVM = new InventoryVM();
            var detail = await _unitWork.InventoryDetail.Get(id);
            inventoryVM.Inventory = await _unitWork.Inventory.Get(detail.InventoryId);

            detail.Amount += 1;
            await _unitWork.Save();
            return RedirectToAction(nameof(DetailInventory), new { id = inventoryVM.Inventory.Id });
        }

        public async Task<IActionResult> Less(int id) // Recibe Id del detalle
        {
            inventoryVM = new InventoryVM();
            var detail = await _unitWork.InventoryDetail.Get(id);
            inventoryVM.Inventory = await _unitWork.Inventory.Get(detail.InventoryId);

            if(detail.Amount == 1)
            {
                _unitWork.InventoryDetail.Remove(detail);
                await _unitWork.Save();
            }
            else
            {
                detail.Amount -= 1;
                await _unitWork.Save();
            }

            return RedirectToAction(nameof(DetailInventory), new { id = inventoryVM.Inventory.Id });
        }

        public async Task<IActionResult> GenerateStock(int id) // Id del inventario
        {
            var inventory = await _unitWork.Inventory.Get(id);
            var detailList = await _unitWork.InventoryDetail.GetAll(d => d.InventoryId == id);
            // Obtener el Id del Usuario desde la sesion
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);


            foreach (var item in detailList)
            {
                var storeProduct = new StoreProduct();
                storeProduct = await _unitWork.StoreProduct.GetFirst(s => s.ProductId == item.ProductId && s.StoreId == inventory.StoreId);

                if(storeProduct != null) // El registro de Stock existe, hay que actualizar las cantidades
                {
                    await _unitWork.KardexInventory.RegisterKardex(storeProduct.Id, "Input", "Inventory record", storeProduct.Amount, item.Amount, claim.Value);
                    storeProduct.Amount += item.Amount;
                    await _unitWork.Save();
                }
                else // Registro de Stock no existe, hay que crearlo
                {
                    storeProduct = new StoreProduct();
                    storeProduct.StoreId = inventory.StoreId;
                    storeProduct.ProductId = item.ProductId;
                    storeProduct.Amount = item.Amount;
                    await _unitWork.StoreProduct.Add(storeProduct);
                    await _unitWork.Save();
                    await _unitWork.KardexInventory.RegisterKardex(storeProduct.Id, "Input", "Initial inventory", 0, item.Amount, claim.Value);
                }
            }

            // Actualizar la cabecera del Inventario
            inventory.State = true;
            inventory.FinalDate = DateTime.Now;
            await _unitWork.Save();
            TempData[SD.Success] = "Stock has been generated successfully";
            return RedirectToAction("Index");
        }

        public IActionResult KardexProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult KardexProduct(string initialDateId, string finalDateId, int productId)
        {
            return  RedirectToAction(nameof(KardexProductResult), new { initialDateId, finalDateId, productId });
        }

        public async Task<IActionResult> KardexProductResult(string initialDateId, string finalDateId, int productId)
        {
            KardexInventoryVM kardexInventoryVM = new KardexInventoryVM();
            kardexInventoryVM.Product = new Product();
            kardexInventoryVM.Product = await _unitWork.Product.Get(productId);

            kardexInventoryVM.InitialDate = DateTime.Parse(initialDateId);
            kardexInventoryVM.FinalDate = DateTime.Parse(finalDateId).AddHours(23).AddMinutes(59);

            kardexInventoryVM.KardexInventoryList = await _unitWork.KardexInventory.GetAll(
                k => k.StoreProduct.ProductId == productId && 
                (k.RegistrationDate >= kardexInventoryVM.InitialDate && 
                k.RegistrationDate <= kardexInventoryVM.FinalDate), 
                includeProperties: "StoreProduct,StoreProduct.Product,StoreProduct.Store",
                orderBy: o => o.OrderBy(o => o.RegistrationDate)
            );

            return View(kardexInventoryVM);
        }

        #region API

        public async Task<IActionResult> DetailInventory(int id)
        {
            inventoryVM = new()
            {
                Inventory = await _unitWork.Inventory.GetFirst(i => i.Id == id, includeProperties: "Store"),
                InventoryDetails = await _unitWork.InventoryDetail.GetAll(d => d.InventoryId == id, includeProperties: "Product,Product.Brand")
            };

            return View(inventoryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailInventory(int inventoryId, int productId, int amountId)
        {
            inventoryVM = new InventoryVM();
            inventoryVM.Inventory = await _unitWork.Inventory.GetFirst(i => i.Id == inventoryId);
            var storeProduct = await _unitWork.StoreProduct.GetFirst(s => s.ProductId == productId && s.StoreId == inventoryVM.Inventory.StoreId);
            var detail = await _unitWork.InventoryDetail.GetFirst(d => d.InventoryId == inventoryId && d.ProductId == productId);

            if(detail == null)
            {
                inventoryVM.InventoryDetail = new InventoryDetail();
                inventoryVM.InventoryDetail.ProductId = productId;
                inventoryVM.InventoryDetail.InventoryId = inventoryId;

                if (storeProduct != null)
                    inventoryVM.InventoryDetail.PreviousStock = storeProduct.Amount;
                else
                    inventoryVM.InventoryDetail.PreviousStock = 0;

                inventoryVM.InventoryDetail.Amount = amountId;
                await _unitWork.InventoryDetail.Add(inventoryVM.InventoryDetail);
                await _unitWork.Save();
            }
            else
            {
                detail.Amount += amountId;
                await _unitWork.Save();
            }

            return RedirectToAction(nameof(DetailInventory), new { id = inventoryId });
        }

        [HttpGet]
        public async Task<IActionResult> SearchProduct(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                var productList = await _unitWork.Product.GetAll(p => p.State == true);
                var data = productList.Where(p => p.SerialNumber.Contains(term, StringComparison.OrdinalIgnoreCase) || 
                                                  p.Description.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();

                return Ok(data);
            }

            return Ok();
        }

        #endregion API
    }
}