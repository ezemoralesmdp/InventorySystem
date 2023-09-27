using InventorySystem.DataAccess;
using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitWork _unitWork;
        private readonly ApplicationDbContext _db;

        public UserController(IUnitWork unitWork, ApplicationDbContext db)
        {
            _unitWork = unitWork;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region API

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userList = await _unitWork.User.GetAll();
            var userRole = await _db.UserRoles.ToListAsync();
            var roles = await _db.Roles.ToListAsync();

            foreach (var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
            }

            return Json(new { data = userList });
        }

        [HttpPost]
        public async Task<IActionResult> LockUnlock([FromBody] string id)
        {
            var user = await _unitWork.User.GetFirst(u => u.Id == id);

            if (user == null)
                return Json(new { success = false, message = "User error" });

            if(user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
            {
                //Usuario bloqueado
                user.LockoutEnd = DateTime.Now;
            }
            else
                user.LockoutEnd = DateTime.Now.AddYears(1000);

            await _unitWork.Save();
            return Json(new { success = true, message = "Successful operation" });
        }

        #endregion API
    }
}