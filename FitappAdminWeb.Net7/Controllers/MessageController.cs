using Microsoft.AspNetCore.Mvc;

namespace FitappAdminWeb.Net7.Controllers
{
    public class MessageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
