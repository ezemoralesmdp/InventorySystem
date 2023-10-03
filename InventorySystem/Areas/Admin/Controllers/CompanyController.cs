using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models.ViewModels;
using InventorySystem.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventorySystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitWork _unitWork;

        public CompanyController(IUnitWork unitWork)
        {
            _unitWork = unitWork;
        }

        public async Task<IActionResult> Upsert()
        {
            CompanyVM companyVM = new()
            {
                Company = new Models.Company(),
                StoreList = _unitWork.Inventory.GetAllDropDownList("Store")
            };

            companyVM.Company = await _unitWork.Company.GetFirst();

            if(companyVM.Company == null)
                companyVM.Company = new Models.Company();

            return View(companyVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(CompanyVM companyVM)
        {
            if (ModelState.IsValid)
            {
                TempData[SD.Success] = "Successfully generated company";
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

                if(companyVM.Company.Id == 0)
                {
                    companyVM.Company.CreatedById = claim.Value;
                    companyVM.Company.UpdatedById = claim.Value;
                    companyVM.Company.CreationDate = DateTime.Now;
                    companyVM.Company.UpdateDate = DateTime.Now;
                    await _unitWork.Company.Add(companyVM.Company);
                }
                else // Update company
                {
                    companyVM.Company.CreatedById = claim.Value;
                    companyVM.Company.UpdatedById = claim.Value;
                    _unitWork.Company.Update(companyVM.Company);
                }

                await _unitWork.Save();
                return RedirectToAction("Index", "Home", new { Area = "Inventory" });
            }

            TempData[SD.Error] = "Error saving company";
            return View(companyVM);
        }
    }
}