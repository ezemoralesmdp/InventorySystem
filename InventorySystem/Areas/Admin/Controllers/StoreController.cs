using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;
using InventorySystem.Utils;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StoreController : Controller
    {
        private readonly IUnitWork _unitWork;

        public StoreController(IUnitWork unitWork)
        {
            _unitWork = unitWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Store store = new();

            if (!id.HasValue)
            {
                store.State = true;
                return View(store);
            }

            store = await _unitWork.Store.Get(id.GetValueOrDefault());

            if (store == null)
                return NotFound();

            return View(store);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Store store)
        {
            if (ModelState.IsValid)
            {
                if (store.Id == 0)
                {
                    await _unitWork.Store.Add(store);
                    TempData[SD.Success] = "Store successfully created";
                }
                else
                {
                    _unitWork.Store.Update(store);
                    TempData[SD.Success] = "Store successfully updated";
                }

                await _unitWork.Save();
                return RedirectToAction(nameof(Index));
            }

            TempData[SD.Success] = "Error saving store";
            return View();
        }

        #region API

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _unitWork.Store.GetAll();
            return Json(new { data = all });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var store = await _unitWork.Store.Get(id);

            if (store == null)
                return Json(new { success = false, message = "Error when trying to find the store" });

            _unitWork.Store.Remove(store);
            await _unitWork.Save();
            return Json(new { success = true, message = "Store successfully removed" });
        }

        [ActionName("ValidateName")]
        public async Task<IActionResult> ValidateName(string name, int id = 0)
        {
            bool value = false;
            var list = await _unitWork.Store.GetAll();

            if(id == 0)
                value = list.Any(s => s.Name.ToLower().Trim() == name.ToLower().Trim());
            else
                value = list.Any(s => s.Name.ToLower().Trim() == name.ToLower().Trim() && s.Id != id);

            if (value)
                return Json(new { data = true });

            return Json(new { data = false });
        }

        #endregion API
    }
}