using System.Web.Mvc;

public class ErrorController : Controller
{

    [HttpGet]
    public ActionResult Error()
    {
        return View();
    }
}