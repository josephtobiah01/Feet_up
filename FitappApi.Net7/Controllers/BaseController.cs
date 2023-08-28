using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using ParentMiddleWare.ApiModels;

namespace FitappApi.Net7.Controllers
{
    public class BaseController : ControllerBase
    {

        [NonAction]
        public bool CheckAuth()
        {
            StringValues auth = string.Empty;
            Request.Headers.TryGetValue("Authorization", out auth);
            if (string.IsNullOrEmpty(auth) || auth != "8ed4497d-f8ac-44bc-a68b-c6cb2a2f13a4")
            {
                return false;
            }
            return true;
        }

        //[HttpGet]
        //[Route("GetTest")]
        //public async Task<bool> GetTest()
        //{
        //    return true;
        //}


        //[HttpPost]
        //[Route("PostTest")]
        //public async Task<bool> PostTest()
        //{
        //    return true;
        //}
    }
}
