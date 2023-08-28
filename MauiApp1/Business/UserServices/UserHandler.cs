using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParentMiddleWare;
using ParentMiddleWare.ApiModels;
using UserApi.Net7.Models;
using UserApi.Net7;
using MauiApp1.Business.DeviceServices;
using MauiApp1.Exceptions;
using MauiApp1.Business;
using MauiApp1.Areas.Security.Views;
using Newtonsoft.Json;

namespace MauiApp1.Business.UserServices
{
    public static class UserHandler
    {
        public static async Task<bool> SetupUser()
        {
            try
            {
                MiddleWare.FkFederatedUser = string.Empty;
                var user = await GetLoggedInUser();
                if (user != null)
                {
                    bool IsSuccessful = await LoginUser(new MinUserModel() { FkFederatedUser = user.FkFederatedUser, ID = user.ID, NM = user.NM });
                    if (IsSuccessful)
                    {
                        MauiApp1.Pages.Index.EnableFooter();
                        MauiApp1.Shared.NavMenu.isLoggedIn = true;
                    }
                }
            }
            catch { return false; }
            return true;
        }

        public static async Task<UserOpResult> LoginUserManually(string UserName, string Password)
        {
#if WINDOWS
            return await UserApi.Net7.UserMiddleware.LoginUserWINDOWS(UserName, Password);
#elif IOS
            return await UserApi.Net7.UserMiddleware.LoginUser(UserName, Password);
#elif ANDROID
            return await UserApi.Net7.UserMiddleware.LoginUser(UserName, Password);
#endif
        }

        // call this immedeatly after login
        public static async Task<bool> SaveUser(MinUserModel user)
        {
            string serializeUserObject = string.Empty;
            try
            {
                serializeUserObject = JsonConvert.SerializeObject(user);

                await DeviceStorageManager.SetDataInStorageWithEncryption("user_token",
                           serializeUserObject, MiddleWare.UseSecuredStorage, MiddleWare.DefaultEncryptionId);
                return true;
            }
            catch (DeviceStorageException deviceStorageException)
            {
                await ForceSignOutUser();
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static async Task ForceSignOutUser()
        {
            await DeviceUserAccountManager.ForceUserSignOut();
            //if (HTMLBridge.RefreshData != null)
            //{
            //    HTMLBridge.RefreshData.Invoke(null, null);
            //}
            if (App.Current.MainPage != null)
            {
                //await App.Current.MainPage.Navigation.PushAsync(new MVPLoginContentPage());
                App.Current.MainPage = new NavigationPage(new MVPLoginContentPage());
                await Application.Current.MainPage.Navigation.PopToRootAsync();
            }
        }

        // call this when the app starts -- if it returnsother than null, then call "LoginUser" function
        public static async Task<MinUserModel> GetLoggedInUser()
        {
            try
            {
                //var Token = EncryptDecrypt(Preferences.Default.Get("user_token", "Unknown"), 200);
                string Token = await DeviceStorageManager.GetDataInStorageWithEncryption("user_token", "Unknown",
            MiddleWare.UseSecuredStorage, MiddleWare.DefaultEncryptionId);

                if (Token == null || Token == "Unknown")
                {
                    return null;
                }
                return JsonConvert.DeserializeObject<MinUserModel>(Token);
            }
            catch (DeviceStorageException deviceStorageException)
            {
                await ForceSignOutUser();
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Set the icon on top right from "login" to "Logoff"
        // Navigate to mainpage after login
        // Set the name on the ui to the " user.UserName"
        public static async Task<bool> LoginUser(MinUserModel user)
        {
            MiddleWare.FkFederatedUser = user.FkFederatedUser;
            MiddleWare.UserID = user.ID;
            MiddleWare.UserName = user.NM;

            await FeedApi.Net7.FeedApi.GetDailyPlanId(MiddleWare.FkFederatedUser, DateTime.Now);

            DateTime date = DateTime.Now;
            string dString = string.Format("{0}/{1}/{2} 12:00:00", date.Month, date.Day, date.Year);

            long PlanId = -1;
            if (MiddleWare.DailyPlanId.Keys.Contains(DateTime.Parse(dString, System.Globalization.CultureInfo.InvariantCulture)))
            {
                PlanId = MiddleWare.DailyPlanId[DateTime.Parse(dString, System.Globalization.CultureInfo.InvariantCulture)];
            }

            //GetUserInfo
            MauiApp1.Pages.Index.EnableFooter();
            MauiApp1.Shared.NavMenu.isLoggedIn = true;
            // sync by design
            UserMiddleware.GetUserInfo();
            return true;
        }

        public static bool LogOffuser()
        {
            MiddleWare.FkFederatedUser = string.Empty;
            MiddleWare.DailyPlanId = new Dictionary<DateTime, long>();
            MiddleWare.UserName = "Guest";
            MauiApp1.Pages.Index.DisableFooter();
            MauiApp1.Shared.NavMenu.isLoggedIn = false;
            Preferences.Default.Clear();
            try
            {
                SecureStorage.Default.RemoveAll();
            }
            catch (Exception ex)
            {

            }
            return true;
        }


        public static string EncryptDecrypt(string szPlainText, int szEncryptionKey)
        {
            StringBuilder szInputStringBuild = new StringBuilder(szPlainText);
            StringBuilder szOutStringBuild = new StringBuilder(szPlainText.Length);
            char Textch;
            for (int iCount = 0; iCount < szPlainText.Length; iCount++)
            {
                Textch = szInputStringBuild[iCount];
                Textch = (char)(Textch ^ szEncryptionKey);
                szOutStringBuild.Append(Textch);
            }
            return szOutStringBuild.ToString();
        }

    }
}
