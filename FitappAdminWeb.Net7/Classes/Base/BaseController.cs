using FitappAdminWeb.Net7.Classes.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FitappAdminWeb.Net7.Classes.Base
{
    public abstract class BaseController : Controller
    {
        MessageRepository _messagerepo;

        public BaseController(MessageRepository messagerepo)
        {
            _messagerepo = messagerepo;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ViewBag.RoomWithConcernsCount = await _messagerepo.GetRoomsWithConcernsCount();
            await base.OnActionExecutionAsync(context, next);
        }
    }
}
