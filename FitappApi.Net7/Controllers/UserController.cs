using DAOLayer.Net7.User;
using FitappApi.Net7.IdentityContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Http.Json;
using UserApi.Net7.Models;

namespace FitappApi.Net7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
     //   private readonly ExerciseContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _appContext;
        private readonly UserContext _uContext;



        public UserController(UserContext uContext, ApplicationDbContext appContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
         //   _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _appContext = appContext;
            _uContext = uContext;
        }

          [HttpPost]
        [Route("RegisterDevice")]
        public async Task<bool> RegisterDevice(string RegistrationId, long UserId)
        {
            try
            {
                var apn = await _uContext.Apn.Where(t => t.FkUserId == UserId && t.DeviceId == RegistrationId).FirstOrDefaultAsync();
                if (apn == null)
                {
                    apn = new Apn()
                    {
                        DeviceId = RegistrationId,
                        FkUserId = UserId,
                        IsActive = true
                    };
                    _uContext.Apn.Add(apn);
                    await _uContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    if (apn.IsActive == false)
                    {
                        apn.IsActive = true;
                        await _uContext.SaveChangesAsync();
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex) 
            {
                return false;
            }
        }


        [HttpPost]
        [Route("UnRegisterDevice")]
        public async Task<bool> UnRegisterDevice(string RegistrationId, long UserId)
        {
            try
            {
                var apn = await _uContext.Apn.Where(t => t.FkUserId == UserId && t.DeviceId == RegistrationId).FirstOrDefaultAsync();
                if (apn == null)
                {
                    return true;
                }
                else
                {
                    _uContext.Apn.Remove(apn);
                    await _uContext.SaveChangesAsync();
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        //[HttpPost]
        //[Route("SendAPN")]
        //public async Task<bool> SendAPN(long UserId, string Message, string Badge, string Alarm, string Iamge)
        //{
        //    try
        //    {
        //        var DeviceList = await _uContext.Apn.Where(t => t.FkUserId == UserId && t.IsActive == true).ToListAsync();
        //        if(DeviceList == null)
        //        {
        //            return false;
        //        }
        //        foreach(var device in DeviceList)
        //        {

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}


        [HttpPost]
        [Route("CreateUser")]
        public async Task<UserOpResult> CreateUser([FromBody] CreateUserModel model)
        {
            var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
           
            var ret = new UserOpResult()
            {
                isSuccess = result.Succeeded,
            };

            if(!ret.isSuccess) 
            {
                foreach(var x in result.Errors)
                {
                    ret.Errors.Add(x.Description);
                }
            }
            else
            {
                // Succeded, Now Create the Fitapp User

                // get the UserId of the ASP NET IDENTITY USER
                var aspuser = _appContext.Users.Where(t => t.UserName == model.Username).First();
                User FitappUser = new User()
                {
                    Email = model.Email,
                    FkFederatedUser = aspuser.Id,
                    UserLevel = 100,
                    LastKnownTimeOffset = DateTimeOffset.UtcNow
                };
                _uContext.User.Add(FitappUser);
                int code =  await _uContext.SaveChangesAsync();
            }
            return ret;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<UserOpResult> Login([FromBody] LoginUserModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username,
                         model.Password, false, lockoutOnFailure: true);
            var ret = new UserOpResult()
            {
                isSuccess = result.Succeeded,
            };

            if(ret.isSuccess)
            {
                // var k = await _userManager.get.GetUserIdAsync(new ApplicationUser() { UserName = model.Username });
                var user = _appContext.Users.Where(t => t.UserName == model.Username).First();
                 //    _userManager.getu
                var FitAppuser = await _uContext.User.Where(t => t.FkFederatedUser == user.Id && t.IsActive == true).FirstOrDefaultAsync();
                if(FitAppuser == null)
                {
                    ret.isSuccess = false;
                    return ret;
                }
                ret.UserId = FitAppuser.Id;
                ret.UserName = model.Username;
            }
            else
            {
                if (result.IsNotAllowed) ret.Errors.Add("IsNotAllowed");
                if (result.IsLockedOut) ret.Errors.Add("IsLockedOut");
                if (result.RequiresTwoFactor) ret.Errors.Add("RequiresTwoFactor");

            }

            return ret;
        }


        [HttpGet]
        [Route("GetAll")]
        public async Task<List<UserOpResult>> GetAll()
        {
            List<UserOpResult> ret = new List<UserOpResult>();
            var appusers =  _appContext.Users.ToList();

            foreach (var au in appusers)
            {
                var FitAppuser = _uContext.User.Where(t => t.FkFederatedUser == au.Id).First();
                ret.Add(new UserOpResult() { UserId = FitAppuser.Id, UserName = FitAppuser.UserLevel.ToString() });
            }
            return ret;

        }
        


        //[HttpGet]
        //[Route("GetUserByID")]
        //public User? GetUserByID(long ID)
        //{
        //    using (var ctx = new UserContext())
        //    {
        //        ctx.Configuration.LazyLoadingEnabled = false;
        //        return ctx.Users.Where(t => t.ID == ID)
        //                         .FirstOrDefault();
        //    }
        //}

        //[HttpPost]
        //[Route("CreateUser")]
        //public async Task CreateUser(User User)
        //{
        //    using var ctx = new UserContext();
        //    ctx.Users.Add(User);
        //    await ctx.SaveChangesAsync();
        //}


        //[HttpPost]
        //[Route("DeleteUser")]
        //public async Task DeleteUser(User User)
        //{
        //    using var ctx = new UserContext();
        //    ctx.Users.Remove(User);
        //    await ctx.SaveChangesAsync();
        //}


        //[HttpPost]
        //[Route("AddUserConnections")]
        //public async Task AddUserConnections(UserPair Pair)
        //{
        //    using var ctx = new UserContext();
        //    Pair.User1.ConnectedUsers.Add(new ConnectedUsers() { Parent= Pair.User1, Child = Pair.User2 });
        //    Pair.User2.ConnectedUsers.Add(new ConnectedUsers() { Parent = Pair.User2, Child = Pair.User1 });
        //    await ctx.SaveChangesAsync();
        //}

        ////[HttpPost]
        ////[Route("api/User/AddUserConnections")]
        ////public async Task AddUserConnections(User Parent, User Child)
        ////{
        ////    using var ctx = new UserContext();
        ////    Parent.ConnectedUsers.Add(new ConnectedUsers() { Parent = Parent, Child = Child });
        ////    Child.ConnectedUsers.Add(new ConnectedUsers() { Parent = Child, Child = Parent });
        ////    await ctx.SaveChangesAsync();
        ////}

    }
}
