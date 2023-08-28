using MauiApp1.Business.DeviceServices;
using MauiApp1.Business.UserServices;
using ParentMiddleWare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserApi.Net7;

namespace MauiApp1.Business
{
    public class DeviceUserAccountManager
    {
        public static async Task ForceUserSignOut()
        {
            string FkFederatedUser = MiddleWare.FkFederatedUser;
            bool userSignedOutSuccessful = UserHandler.LogOffuser();

            if (userSignedOutSuccessful == true)
            {
                try
                {
                    await UserMiddleware.UnRegisterDevice(await PushRegistration.CheckPermission(), FkFederatedUser, PushRegistration.GetPlatform());
                }
                catch (Exception ex)
                {

                }               
               
            }
           
           
        }
    }
}
