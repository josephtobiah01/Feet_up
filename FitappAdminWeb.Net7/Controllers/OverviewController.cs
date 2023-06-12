using FitappAdminWeb.Net7.Classes.Base;
using FitappAdminWeb.Net7.Classes.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FitappAdminWeb.Net7.Controllers
{
    public class OverviewController : BaseController
    {
        MessageRepository _messagerepo;

        public OverviewController(MessageRepository messagerepo)
            : base(messagerepo)
        {
            _messagerepo = messagerepo;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
