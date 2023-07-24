using DAOLayer.Net7.User;
using FitappApi.Net7.IdentityContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using ParentMiddleWare.ApiModels;
using System;
using System.Net.Http.Json;
using UserApi.Net7.Models;

namespace FitappApi.Net7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _appContext;
        private readonly UserContext _uContext;



        public UserController(UserContext uContext, ApplicationDbContext appContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appContext = appContext;
            _uContext = uContext;
        }

      
        [HttpPost]
        [Route("RegisterDevice")]
        public async Task<bool> RegisterDevice(string RegistrationId, long UserId, string Platform)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                var apnList = await _uContext.Apn.Where(t => t.FkUserId == UserId && t.DeviceId == RegistrationId && t.Platform == Platform).ToListAsync();
                if (apnList == null || apnList.Count < 1)
                {
                    var apn = new Apn()
                    {
                        DeviceId = RegistrationId,
                        FkUserId = UserId,
                        IsActive = true,
                        Platform = Platform,
                        Timestamp = DateTime.UtcNow,
                        LastActive = DateTime.UtcNow,
                    };
                    _uContext.Apn.Add(apn);
                    await _uContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    foreach (var apn in apnList)
                    {
                        apn.LastActive = DateTime.UtcNow;
                        apn.IsActive = true;
                        await _uContext.SaveChangesAsync();
                        return true;
                    }
                }

                        //else
                        //{
                        //    foreach (var apn in apnList)
                        //    {
                        //        //  if (apn.IsActive == false)

                        //        apn.IsActive = false;
                        //        //  apn.Platform = Platform;
                        //        //    apn.Timestamp = DateTime.UtcNow;
                        //        apn.LastActive = DateTime.UtcNow;
                        //        await _uContext.SaveChangesAsync();
                        //    }
                        //    var napn = new Apn()
                        //    {
                        //        DeviceId = RegistrationId,
                        //        FkUserId = UserId,
                        //        IsActive = true,
                        //        Platform = Platform,
                        //        Timestamp = DateTime.UtcNow,
                        //        LastActive = DateTime.UtcNow,
                        //    };
                        //    _uContext.Apn.Add(napn);
                        //    await _uContext.SaveChangesAsync();
                        //    return true;
                        //}
                        return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [HttpPost]
        [Route("UnRegisterDevice")]
        public async Task<bool> UnRegisterDevice(string RegistrationId, long UserId, string Platform)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
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

        [HttpPost]
        [Route("RegisterBarcode")]
        public async Task<bool> RegisterBarcode([FromBody] GeneralApiModel model)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                var user = await _uContext.User.Where(t => t.Id == model.UserId).FirstOrDefaultAsync();
                if (user == null)
                {
                    return false;
                }
                else
                {
                    user.BarcodeString = model.param1;
                    user.IsNewBarcode = true;
                    await _uContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpPost]
        [Route("CreateUser")]
        public async Task<UserOpResult> CreateUser([FromBody] CreateUserModel model)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
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
        [Route("UpdateOffset")]
        public async Task<bool> UpdateOffset([FromBody] SetOffsetModel model)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var FitAppuser = await _uContext.User.Where(t => t.Id == model.UserId && t.IsActive == true).FirstOrDefaultAsync();
            if (FitAppuser == null)
            {
                return false;
            }
            FitAppuser.LastKnownTimeOffset = model.Offset;
            await _uContext.SaveChangesAsync();
            return true;
        }

        [HttpPost]
        [Route("LoginWINDOWS")]
        public async Task<UserOpResult> LoginWINDOWS([FromBody] LoginUserModel model)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var result = await _signInManager.PasswordSignInAsync(model.Username,
                         model.Password, false, lockoutOnFailure: true);
            var ret = new UserOpResult()
            {
                isSuccess = result.Succeeded,
            };

            if (ret.isSuccess)
            {
                // var k = await _userManager.get.GetUserIdAsync(new ApplicationUser() { UserName = model.Username });
                var user = _appContext.Users.Where(t => t.UserName == model.Username).First();
                //    _userManager.getu
                var FitAppuser = await _uContext.User.Where(t => t.FkFederatedUser == user.Id && t.IsActive == true).FirstOrDefaultAsync();
                if (FitAppuser == null)
                {
                    ret.isSuccess = false;
                    return ret;
                }
                ret.UserId = FitAppuser.Id;
                ret.UserName = model.Username;
              //  FitAppuser.LastKnownTimeOffset = model.Offset;
               // await _uContext.SaveChangesAsync();
            }
            else
            {
                if (result.IsNotAllowed) ret.Errors.Add("IsNotAllowed");
                if (result.IsLockedOut) ret.Errors.Add("IsLockedOut");
                if (result.RequiresTwoFactor) ret.Errors.Add("RequiresTwoFactor");

            }

            return ret;
        }


        [HttpPost]
        [Route("GetUserInfo")]
        public async Task<UserOpResult> GetUserInfo([FromBody] GeneralApiModel model)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var ret = new UserOpResult()
            {
            };

            var FitAppuser = await _uContext.User.Where(t => t.Id == model.UserId).FirstOrDefaultAsync();
            if (FitAppuser == null)
            {
                return ret;
            }

            ret.Email = FitAppuser.Email;
            ret.Height = FitAppuser.Height.HasValue ? FitAppuser.Height.Value.ToString() : "unknown";
            ret.Last_recorded_weight = FitAppuser.Weight.HasValue ? FitAppuser.Weight.Value.ToString() : "unknown";

            if (FitAppuser.Dob.HasValue)
            {
                int age = 0;
                age = DateTime.Now.Year - FitAppuser.Dob.Value.Year;
                if (DateTime.Now.DayOfYear < FitAppuser.Dob.Value.DayOfYear)
                {
                    age--;
                }
                ret.Age = age.ToString();
            }

            return ret;
        }

        
        [HttpPost]
        [Route("Login")]
        public async Task<UserOpResult> Login([FromBody] LoginUserModel model)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
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
                FitAppuser.LastKnownTimeOffset = model.Offset;
                await _uContext.SaveChangesAsync();


                ret.Email = FitAppuser.Email;
                ret.Height = FitAppuser.Height.HasValue ? FitAppuser.Height.Value.ToString() : "unknown";
                ret.Last_recorded_weight = FitAppuser.Weight.HasValue ? FitAppuser.Weight.Value.ToString() : "unknown";
                
                if(FitAppuser.Dob.HasValue)
                {
                    int age = 0;
                    age = DateTime.Now.Year - FitAppuser.Dob.Value.Year;
                    if (DateTime.Now.DayOfYear < FitAppuser.Dob.Value.DayOfYear)
                    {
                        age--;
                    }
                    ret.Age = age.ToString();
                }

                return ret;
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
            if (!CheckAuth()) throw new Exception("Unauthorized");
            List<UserOpResult> ret = new List<UserOpResult>();
            var appusers =  _appContext.Users.ToList();

            foreach (var au in appusers)
            {
                var FitAppuser = _uContext.User.Where(t => t.FkFederatedUser == au.Id).First();
                ret.Add(new UserOpResult() { UserId = FitAppuser.Id, UserName = FitAppuser.UserLevel.ToString() });
            }
            return ret;

        }
    }
}
