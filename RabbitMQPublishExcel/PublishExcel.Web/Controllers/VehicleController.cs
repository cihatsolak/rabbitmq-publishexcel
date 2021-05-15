using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublishExcel.Web.Models.Contexts;
using PublishExcel.Web.Models.Core;
using PublishExcel.Web.Models.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PublishExcel.Web.Controllers
{
    [Authorize]
    public class VehicleController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;

        public VehicleController(UserManager<IdentityUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateVehicleExcel()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            string fileName = $"vehiclelist-excel-{Guid.NewGuid().ToString().Substring(0, 10)}";

            UserFile userFile = new()
            {
                UserId = user.Id,
                FileName = fileName,
                FileStatus = FileStatus.Creating
            };

            await _context.UserFiles.AddAsync(userFile);
            await _context.SaveChangesAsync();

            TempData["StartCreatingExcel"] = true;

            return RedirectToAction(nameof(Files));
        }

        [HttpGet]
        public async Task<IActionResult> Files()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var files = await _context.UserFiles.Where(p => p.UserId.Equals(user.Id)).ToListAsync();
            return View(files);
        }

    }
}
