using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PublishExcel.Web.Controllers
{
    [Authorize]
    public class VehicleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
