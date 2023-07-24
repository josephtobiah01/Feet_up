using FitnessData.Common;
using MauiApp1.Areas.BarcodeScanning.Views;
using MauiApp1.Areas.Security.Views;
using MauiApp1.Business;
using MauiApp1.Business.DeviceServices;
using Microsoft.Maui.Authentication;
using Newtonsoft.Json;
using ParentMiddleWare;
using UserApi.Net7;

namespace MauiApp1.Pages.Profile
{
    //
    // Summary:
    //     Web Authenticator Result parsed from the callback Url.
    //
    // Remarks:
    //     All of the querystring or url fragment properties are parsed into a dictionary
    //     and can be accessed by their key.

    public class WebAuthenticatorResultProxy
    {
        [JsonConstructor]
        public WebAuthenticatorResultProxy()
        {

        }

        public Dictionary<string, string> Properties { get; set; }


    }


    public partial class ViewProfile 
    {
        #region [Fields]

        private bool _hideConnectedAppButton = true;
        #endregion

        #region[Initialization]

        protected override async void OnInitialized()
        {
            base.OnInitialized();
            await  RefreshMenu();
        }

        #endregion

        #region [Methods :: EventHandlers :: Controls]

        private async Task GoToScanPage()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new BarcodeScannerContentPage());
        }

        public async void ClosePage()
        {
            await App.Current.MainPage.Navigation.PopAsync();
        }

        private void ConnectedApps_Clicked()
        {
            ConnectApplicationToGoogleFit();
        }

        #endregion

        #region [Methods :: Tasks]

        private async Task RefreshMenu()
        {
            string encryptedAccessToken = string.Empty;
            string accessToken = string.Empty;
            bool isAppConnected = false;
            bool userSignedIn = MiddleWare.UserID > 0;
            await Task.Run(() =>
            {
                if (userSignedIn == true)
                {
                    encryptedAccessToken = Preferences.Default.Get(
                        ConnectApplicationDeviceManager.ACCESS_TOKEN_STRING, string.Empty);

                    accessToken = EncryptDecryptManager.EncryptDecrypt(
                        encryptedAccessToken, 300);

                    ConnectApplicationDeviceManager.mobileUserAccountTransactionManager =
                                      new SecurityServices.Client.Business.Transactions.MobileUserAccountTransactionManager(
                                          ConnectApplicationDeviceManager.APPCONNECTENDPOINT, accessToken);

                    isAppConnected = ConnectApplicationDeviceManager.mobileUserAccountTransactionManager
                        .IsAlreadyConnected(MiddleWare.UserID);

                    if (isAppConnected == true &&
                        string.IsNullOrWhiteSpace(accessToken) == false)
                    {
                        _hideConnectedAppButton = true;
                    }
                    else
                    {
                        _hideConnectedAppButton = false;
                    }
                }
                else
                {
                    _hideConnectedAppButton = true;
                }

                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            });
            

            
        }

        private async void ConnectApplicationToGoogleFit()
        {
            //string url = "https://security-development.ageinreverse.me/mobileauth/Google";
            string url = "https://security.ageinreverse.me/mobileauth/Google";
            string expiresInString = string.Empty;
            string refreshTokenExpiresInString = string.Empty;
            string accessToken = string.Empty;
            string refreshToken = string.Empty;
            string emailAddress = string.Empty;
            string accessTokenKey = string.Empty;
            string encryptedAccessTokenKey = string.Empty;
            DateTimeOffset? expiresIn = null;
            DateTimeOffset? refreshTokenExpiresIn = null;
            string encryptedAccessToken = string.Empty;
            string encryptedRefreshToken = string.Empty;
            string encryptedEmailAddress = string.Empty;
            string encryptedExpiresIn = string.Empty;
            string encryptedRefreshTokenExpiresIn = string.Empty;

            try
            {
                encryptedAccessTokenKey = Preferences.Default.Get(
                    ConnectApplicationDeviceManager.ACCESS_TOKEN_STRING, string.Empty);

                accessTokenKey = EncryptDecryptManager.EncryptDecrypt(
                    encryptedAccessTokenKey, 300);

                ConnectApplicationDeviceManager.mobileUserAccountTransactionManager =
                                  new SecurityServices.Client.Business.Transactions.MobileUserAccountTransactionManager(
                                      ConnectApplicationDeviceManager.APPCONNECTENDPOINT, accessTokenKey);

                bool isAppConnected = ConnectApplicationDeviceManager.mobileUserAccountTransactionManager.IsAlreadyConnected(MiddleWare.UserID);
                if (isAppConnected == false
                    || string.IsNullOrWhiteSpace(accessTokenKey) == true)
                {
#if ANDROID || IOS

                    WebAuthenticatorResult webAuthenticatorResult =
                        await WebAuthenticator.Default.AuthenticateAsync(new Uri(url), new Uri("myapp://")
                    );

                    if (webAuthenticatorResult != null)
                    {
                        accessToken = webAuthenticatorResult.AccessToken;
                        refreshToken = webAuthenticatorResult.RefreshToken;
                        emailAddress = webAuthenticatorResult.Properties["email"].ToString();
                        expiresIn = ConnectApplicationDeviceManager.ExtractAccessTokenExpirationDate(webAuthenticatorResult);
                        refreshTokenExpiresIn = ConnectApplicationDeviceManager.ExtractRefreshTokenExpirationDate(webAuthenticatorResult); ;

                        if (expiresIn.HasValue == true)
                        {
                            expiresInString = JsonConvert.SerializeObject(expiresIn.Value);
                        }

                        if (refreshTokenExpiresIn.HasValue == true)
                        {
                            refreshTokenExpiresInString = JsonConvert.SerializeObject(refreshTokenExpiresIn.Value);
                        }

                        encryptedAccessToken = EncryptDecryptManager.EncryptDecrypt(accessToken, 300);
                        encryptedRefreshToken = EncryptDecryptManager.EncryptDecrypt(refreshToken, 300);
                        encryptedEmailAddress = EncryptDecryptManager.EncryptDecrypt(emailAddress, 300);
                        encryptedExpiresIn = EncryptDecryptManager.EncryptDecrypt(expiresInString, 300);
                        encryptedRefreshTokenExpiresIn = EncryptDecryptManager.EncryptDecrypt(refreshTokenExpiresInString, 300);

                        Preferences.Default.Set(ConnectApplicationDeviceManager.ACCESS_TOKEN_STRING, encryptedAccessToken);
                        Preferences.Default.Set(ConnectApplicationDeviceManager.REFRESH_TOKEN_STRING, encryptedRefreshToken);
                        Preferences.Default.Set(ConnectApplicationDeviceManager.EMAIL_ADDRESS_STRING, encryptedEmailAddress);
                        Preferences.Default.Set(ConnectApplicationDeviceManager.EXPIRES_IN_STRING, encryptedExpiresIn);
                        Preferences.Default.Set(ConnectApplicationDeviceManager.REFRESH_EXPIRES_IN_STRING, encryptedRefreshTokenExpiresIn);


                        ConnectApplicationDeviceManager.mobileUserAccountTransactionManager.ConnectGoogleAccount(MiddleWare.UserID, MiddleWare.UserName, MiddleWare.UserName, emailAddress, string.Empty,
                  accessToken, refreshToken, expiresIn, refreshTokenExpiresIn);

                        await RefreshMenu();
                        StateHasChanged();

                        await App.Current.MainPage.DisplayAlert("Google Fit Connected Successfully", "It may take some time for apps and devices to sync data with Google Fit.", "OK");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Unable to Connect Google Fit", "", "OK");
                    }
#endif
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Notification", "You are already connected with Age In Reverse.", "OK");
                }
            }
            catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
            {
                await App.Current.MainPage.DisplayAlert("Connect App", "Connect to Google Fit was cancelled.", "OK");
            }            
            catch (Exception ex)
            {
                //await App.Current.MainPage.DisplayAlert("Connect to Google Fit", ex.Message + ex.StackTrace, "OK");
                await App.Current.MainPage.DisplayAlert("Connect to Google Fit", "An error occured while connecting to google fit. Please check your connection and try again.", "OK");
            }
            finally
            {

            }

        }


        //public static async Task<FitnessService> IsAppConnected()
        //{
        //    try
        //    {
        //        FitnessService fitnessService = new FitnessService();
        //       // await App.Current.MainPage.DisplayAlert("Enter IsAppConnected", "Enter IsAppConnected", "OK");
        //        string fitnessServiceToken = Preferences.Default.Get("fitness_service", string.Empty);

        //        if (string.IsNullOrWhiteSpace(fitnessServiceToken) == true)
        //        {
        //         //   await App.Current.MainPage.DisplayAlert("Token is null", "Token is null", "OK");
        //            return null;
        //        }
        //        else
        //        {
        //            //await App.Current.MainPage.DisplayAlert("Token not null, deserialize next", fitnessServiceToken, "OK");
        //            //   FitnessService fitnessService = JsonConvert.DeserializeObject<FitnessService>(fitnessServiceToken);
        //            //  JsonReader jreader = new JsonReader()
        //            WebAuthenticatorResultProxy webAuthenticatorResult = System.Text.Json.JsonSerializer.Deserialize<WebAuthenticatorResultProxy>(fitnessServiceToken);

        //            WebAuthenticatorResult webauthreal = new WebAuthenticatorResult(webAuthenticatorResult.Properties);

        //            fitnessService.WebAuthenticatorResult = webauthreal;

        //            if (fitnessService.WebAuthenticatorResult.ExpiresIn.HasValue == true)
        //            {
        //                if (DateTimeOffset.Now >= fitnessService.WebAuthenticatorResult.ExpiresIn.Value)
        //                {
        //                    ClearFitnessServiceStorage();
        //              //      await App.Current.MainPage.DisplayAlert("Token elapsed", "Token is elapsed", "OK");
        //                    return null;
        //                }
        //                else
        //                {
        //               //     await App.Current.MainPage.DisplayAlert("Token OK", "Token is OK", "OK");
        //                    ViewProfile._fitnessService = fitnessService;
        //                    return fitnessService;
        //                }
        //            }
        //            else
        //            {
        //              //  await App.Current.MainPage.DisplayAlert("Token elapsed NOVAL", "Token is elapsed NOVAL", "OK");
        //                return null;
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await App.Current.MainPage.DisplayAlert(ex.Message, ex.StackTrace, "OK");
        //        return null;
        //        //throw ex;
        //    }
        //}

        private async Task SignOutUser()
        {

            bool userSignedOut = await App.Current.MainPage.DisplayAlert("Sign Out", "Are you sure you want to sign out", "Sign out", "Cancel");

            if (userSignedOut == true)
            {
                long userId = MiddleWare.UserID;
                bool userSignedOutSuccessful = MauiApp1.Pages.Index.LogOffuser();

                if (userSignedOutSuccessful == true)
                {
                    try
                    {
                        await UserMiddleware.UnRegisterDevice(await PushRegistration.CheckPermission(), userId, PushRegistration.GetPlatform());
                    }
                    catch (Exception ex)
                    {

                    }
                    if (HTMLBridge.RefreshData != null)
                    {
                        HTMLBridge.RefreshData.Invoke(this, null);
                    }
                    await App.Current.MainPage.Navigation.PushAsync(new MVPLoginContentPage());
                }
                ViewProfile.ClearFitnessServiceStorage();
                StateHasChanged();
            }
            else
            {
                StateHasChanged();
            }
        }

        public static void ClearFitnessServiceStorage()
        {
            Preferences.Default.Set(ConnectApplicationDeviceManager.ACCESS_TOKEN_STRING, string.Empty);
            Preferences.Default.Set(ConnectApplicationDeviceManager.REFRESH_TOKEN_STRING, string.Empty);
            Preferences.Default.Set(ConnectApplicationDeviceManager.EMAIL_ADDRESS_STRING, string.Empty);
            Preferences.Default.Set(ConnectApplicationDeviceManager.EXPIRES_IN_STRING, string.Empty);
            Preferences.Default.Set(ConnectApplicationDeviceManager.REFRESH_EXPIRES_IN_STRING, string.Empty);
        }
        #endregion

    }
}