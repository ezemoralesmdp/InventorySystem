using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models.ViewModels;
using InventorySystem.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Inventory)]
    public class ProductController : Controller
    {
        private readonly IUnitWork _unitWork;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public ProductController(IUnitWork unitWork, IWebHostEnvironment webHostEnviroment)
        {
            _unitWork = unitWork;
            _webHostEnviroment = webHostEnviroment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new Models.Product(),
                CategoryList = _unitWork.Product.GetAllDropDownList("Category"),
                BrandList = _unitWork.Product.GetAllDropDownList("Brand"),
                FatherList = _unitWork.Product.GetAllDropDownList("Product")
            };

            // Create new Product
            if (id == null)
            {
                productVM.Product.State = true;
                return View(productVM);
            }
            else
            {
                productVM.Product = await _unitWork.Product.Get(id.GetValueOrDefault());
                if (productVM.Product == null)
                    return NotFound();
                
                return View(productVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnviroment.WebRootPath; //Se captura directorio wwwroot

                if(productVM.Product.Id == 0)
                {
                    //Create
                    string upload = webRootPath + SD.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = fileName + extension;
                    await _unitWork.Product.Add(productVM.Product);
                }
                else
                {
                    //Update
                    var objProduct = await _unitWork.Product.GetFirst(p => p.Id == productVM.Product.Id, isTracking: false);
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + SD.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        //Delete previous image
                        var previousFile = Path.Combine(upload, objProduct.ImageUrl);
                        if (System.IO.File.Exists(previousFile))
                            System.IO.File.Delete(previousFile);

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVM.Product.ImageUrl = fileName + extension;
                    }
                    else
                    {
                        productVM.Product.ImageUrl = objProduct.ImageUrl;
                    }

                    _unitWork.Product.Update(productVM.Product);
                }

                TempData[SD.Success] = "Succesful transaction!";
                await _unitWork.Save();
                return View("Index");
            }
            
            //If not valid
            productVM.CategoryList = _unitWork.Product.GetAllDropDownList("Category");
            productVM.BrandList = _unitWork.Product.GetAllDropDownList("Brand");
            productVM.FatherList = _unitWork.Product.GetAllDropDownList("Product");

            return View(productVM);
        }

        #region API

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _unitWork.Product.GetAll(includeProperties: "Category,Brand");
            return Json(new { data = all });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitWork.Product.Get(id);

            if (product == null)
                return Json(new { success = false, message = "Error when trying to find the product" });

            //Remove image
            string upload = _webHostEnviroment.WebRootPath + SD.ImagePath;
            var previousFile = Path.Combine(upload, product.ImageUrl);
            
            if (System.IO.File.Exists(previousFile))
                System.IO.File.Delete(previousFile);

            _unitWork.Product.Remove(product);
            await _unitWork.Save();
            return Json(new { success = true, message = "Product successfully removed" });
        }

        [ActionName("ValidateSerial")]
        public async Task<IActionResult> ValidateSerial(string serial, int id = 0)
        {
            bool value = false;
            var list = await _unitWork.Product.GetAll();

            if(id == 0)
                value = list.Any(s => s.SerialNumber.ToLower().Trim() == serial.ToLower().Trim());
            else
                value = list.Any(s => s.SerialNumber.ToLower().Trim() == serial.ToLower().Trim() && s.Id != id);

            if (value)
                return Json(new { data = true });

            return Json(new { data = false });
        }

        #endregion API
    }
}