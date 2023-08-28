using Microsoft.AspNetCore.Mvc;

namespace FitappApi.Net7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : BaseController
    {
        public readonly string _env;
        public HomeController(IConfiguration configuration)
        {
            _env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        }

        [HttpGet]
        public async Task<string> Index()
        {
            return _env;
        }
    }
}
