using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;
using InventorySystem.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class BrandController : Controller
    {
        private readonly IUnitWork _unitWork;

        public BrandController(IUnitWork unitWork)
        {
            _unitWork = unitWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Brand brand = new();

            if (!id.HasValue)
            {
                brand.State = true;
                return View(brand);
            }

            brand = await _unitWork.Brand.Get(id.GetValueOrDefault());

            if (brand == null)
                return NotFound();

            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Brand brand)
        {
            if (ModelState.IsValid)
            {
                if (brand.Id == 0)
                {
                    await _unitWork.Brand.Add(brand);
                    TempData[SD.Success] = "Brand successfully created";
                }
                else
                {
                    _unitWork.Brand.Update(brand);
                    TempData[SD.Success] = "Brand successfully updated";
                }

                await _unitWork.Save();
                return RedirectToAction(nameof(Index));
            }

            TempData[SD.Success] = "Error saving brand";
            return View();
        }

        #region API

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _unitWork.Brand.GetAll();
            return Json(new { data = all });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _unitWork.Brand.Get(id);

            if (brand == null)
                return Json(new { success = false, message = "Error when trying to find the brand" });

            _unitWork.Brand.Remove(brand);
            await _unitWork.Save();
            return Json(new { success = true, message = "Brand successfully removed" });
        }

        [ActionName("ValidateName")]
        public async Task<IActionResult> ValidateName(string name, int id = 0)
        {
            bool value = false;
            var list = await _unitWork.Brand.GetAll();

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