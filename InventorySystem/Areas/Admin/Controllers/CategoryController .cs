using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;
using InventorySystem.Utils;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitWork _unitWork;

        public CategoryController(IUnitWork unitWork)
        {
            _unitWork = unitWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Category category = new();

            if (!id.HasValue)
            {
                category.State = true;
                return View(category);
            }

            category = await _unitWork.Category.Get(id.GetValueOrDefault());

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    await _unitWork.Category.Add(category);
                    TempData[SD.Success] = "Category successfully created";
                }
                else
                {
                    _unitWork.Category.Update(category);
                    TempData[SD.Success] = "Category successfully updated";
                }

                await _unitWork.Save();
                return RedirectToAction(nameof(Index));
            }

            TempData[SD.Success] = "Error saving category";
            return View();
        }

        #region API

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _unitWork.Category.GetAll();
            return Json(new { data = all });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitWork.Category.Get(id);

            if (category == null)
                return Json(new { success = false, message = "Error when trying to find the category" });

            _unitWork.Category.Remove(category);
            await _unitWork.Save();
            return Json(new { success = true, message = "Category successfully removed" });
        }

        [ActionName("ValidateName")]
        public async Task<IActionResult> ValidateName(string name, int id = 0)
        {
            bool value = false;
            var list = await _unitWork.Category.GetAll();

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