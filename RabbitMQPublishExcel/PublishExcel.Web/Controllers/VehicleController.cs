using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublishExcel.Shared.Models;
using PublishExcel.Web.Models.Contexts;
using PublishExcel.Web.Models.Core;
using PublishExcel.Web.Models.Enums;
using PublishExcel.Web.Services.RabbitMQ;
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

        private readonly RabbitMQPublisher _rabbitMQPublisher;

        public VehicleController(UserManager<IdentityUser> userManager, AppDbContext context, RabbitMQPublisher rabbitMQPublisher)
        {
            _userManager = userManager;
            _context = context;
            _rabbitMQPublisher = rabbitMQPublisher;
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

            _rabbitMQPublisher.Publish(new CreateExcelMessage //RabbitMQ'ye bildiriyorum.
            {
                FileId = userFile.Id
            });

            TempData["StartCreatingExcel"] = true;

            return RedirectToAction(nameof(Files));
        }

        [HttpGet]
        public async Task<IActionResult> Files()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var files = await _context.UserFiles.Where(p => p.UserId.Equals(user.Id)).OrderByDescending(p => p.CreatedDate).ToListAsync();
            return View(files);
        }

    }
}
