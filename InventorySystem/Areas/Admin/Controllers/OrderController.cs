using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;
using InventorySystem.Models.ViewModels;
using InventorySystem.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventorySystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitWork _unitWork;

        [BindProperty]
        public OrderDetailVM orderDetailVM { get; set; }

        public OrderController(IUnitWork unitWork)
        {
            _unitWork = unitWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(int id)
        {
            orderDetailVM = new OrderDetailVM()
            {
                Order = await _unitWork.Order.GetFirst(o => o.Id == id, includeProperties: "User"),
                OrderDetailList = await _unitWork.OrderDetail.GetAll(d => d.OrderId == id, includeProperties: "Product")
            };

            return View(orderDetailVM);
        }

        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Process(int id)
        {
            var order = await _unitWork.Order.GetFirst(o => o.Id == id);
            order.OrderState = SD.ProcessingStatus;
            await _unitWork.Save();
            TempData[SD.Success] = "Order changed to status In process";
            return RedirectToAction(nameof(Detail), new { id = id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> SendOrder(OrderDetailVM orderDetailVM)
        {
            var order = await _unitWork.Order.GetFirst(o => o.Id == orderDetailVM.Order.Id);
            order.OrderState = SD.SentStatus;
            order.Carrier = orderDetailVM.Order.Carrier;
            order.DeliveryNumber = orderDetailVM.Order.DeliveryNumber;
            order.DeliveryDate = DateTime.Now;
            await _unitWork.Save();
            TempData[SD.Success] = "Order changed to status Sent";
            return RedirectToAction(nameof(Detail), new { id = orderDetailVM.Order.Id });
        }

        #region API

        [HttpGet]
        public async Task<IActionResult> GetOrderList(string state)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<Order> all;

            if (User.IsInRole(SD.Role_Admin)) // Validar el Rol del usuario
                all = await _unitWork.Order.GetAll(includeProperties: "User");
            else
                all = await _unitWork.Order.GetAll(o => o.UserId == claim.Value, includeProperties: "User");

            // Validar el Estado
            switch (state)
            {
                case "approved":
                    all = all.Where(o => o.OrderState == SD.ApprovedStatus);
                    break;
                case "completed":
                    all = all.Where(o => o.OrderState == SD.SentStatus);
                    break;
                default:
                    break;
            }

            return Json(new { data = all });
        }

        #endregion API
    }
}