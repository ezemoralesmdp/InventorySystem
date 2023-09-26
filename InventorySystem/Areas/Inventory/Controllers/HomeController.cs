using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;
using InventorySystem.Models.ErrorViewModels;
using InventorySystem.Models.Specifications;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InventorySystem.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitWork _unitWork;

        public HomeController(ILogger<HomeController> logger, IUnitWork unitWork)
        {
            _logger = logger;
            _unitWork = unitWork;
        }

        public IActionResult Index(int pageNumber = 1, string search = "", string actualSearch = "")
        {
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