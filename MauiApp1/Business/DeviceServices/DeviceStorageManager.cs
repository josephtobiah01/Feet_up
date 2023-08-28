using MauiApp1.Exceptions;
using Newtonsoft.Json;
using ParentMiddleWare;
using ParentMiddleWare.Enumeration;
using ParentMiddleWare.Models.DeviceIntegration;
using System.Net.Mail;

namespace MauiApp1.Business.DeviceServices
{
    public class DeviceStorageManager
    {
        #region [Fields]

        public static bool HasErrorReadingStorage = false;

        #endregion

        #region [Static Methods :: Tasks]

        public static async Task SetDataInStorageWithEncryption(string token, string value, bool useSecureStorage, int encryptionKey)
        {
            string encryptedValue = string.Empty;

            try
            {
                switch (useSecureStorage)
                {
                    case true:

                        await SecureStorage.Default.SetAsync(token, value);
                        break;

                    case false:

                        if (string.IsNullOrWhiteSpace(value) == true)
                        {
                            Preferences.Default.Set(token, value);
                        }
                        else
                        {
                            encryptedValue = EncryptDecryptManager.EncryptDecrypt(value, encryptionKey);
                            Preferences.Default.Set(token, encryptedValue);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                //await App.Current.MainPage.DisplayAlert(ex.Message, ex.StackTrace,
                //         "OK");
                // log error
                throw new DeviceStorageException("Error occured while saving stored value", ex.InnerException);
            }
            finally
            {

            }

        }

        public static async Task<string> GetDataInStorageWithEncryption(string token, string defaultValue, bool useSecureStorage, int encryptionKey)
        {
            //await App.Current.MainPage.DisplayAlert("ENTER", token,
            //                                      "OK");
            string storedValue = string.Empty;
            string unencryptedValue = string.Empty;

            try
            {
                switch (useSecureStorage)
                {
                    case true:

                        unencryptedValue = await SecureStorage.Default.GetAsync(token);
                        //await App.Current.MainPage.DisplayAlert("GET", unencryptedValue,
                        //                                         "OK");
                        if (unencryptedValue == null)
                        {
                            //await App.Current.MainPage.DisplayAlert("NULL", "Token is null",
                            //                               "OK");
                            unencryptedValue = defaultValue;
                        }
                        break;

                    case false:

                        storedValue = Preferences.Default.Get(token, defaultValue);
                        if (string.IsNullOrWhiteSpace(storedValue) == true)
                        {
                            unencryptedValue = storedValue;
                        }
                        else
                        {
                            unencryptedValue = EncryptDecryptManager.EncryptDecrypt(storedValue, encryptionKey);
                        }
                        break;
                }

                return unencryptedValue;
            }
            catch (Exception ex)
            {
                //       await App.Current.MainPage.DisplayAlert(ex.Message, ex.StackTrace,
                //"OK");
                //log error
                throw new DeviceStorageException("Error occured while retreiving stored value", ex.InnerException);
            }
            finally
            {

            }
        }

        public static void ClearFitnessServiceStorage(bool useSecureStorage)
        {
            try
            {
                ClearGoogleFitFitnessServiceStorage(useSecureStorage);
                ClearAppleHealthKitFitnessServiceStorage(useSecureStorage);
            }
            catch (Exception ex)
            {
                throw new DeviceStorageException("Error occured while clearing stored value", ex.InnerException);
            }
            finally
            {

            }
        }

        #endregion

        #region [Static Methods :: Tasks :: GoogleFit]

        public static async void InitializeGoogleFitData()
        {
            string accessToken = string.Empty;
            string refreshToken = string.Empty;
            string emailAddress = string.Empty;
            DateTimeOffset expiresIn = DateTimeOffset.MinValue;
            DateTimeOffset refreshExpiresIn = DateTimeOffset.MinValue;

            string ExpiresInString = null;
            string RefreshExpiresInString = null;

            bool ExpiresInParsed = false;
            bool RefreshExpiresInParsed = false;

            try
            {
                accessToken = await DeviceStorageManager.GetDataInStorageWithEncryption(
               GoogleFit.ACCESS_TOKEN, string.Empty,
               MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);

                if (string.IsNullOrWhiteSpace(accessToken) == false)
                {
                    refreshToken = await DeviceStorageManager.GetDataInStorageWithEncryption(
               GoogleFit.REFRESH_TOKEN, string.Empty,
               MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);

                    emailAddress = await DeviceStorageManager.GetDataInStorageWithEncryption(
            GoogleFit.EMAIL_ADDRESS, string.Empty,
            MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);

                    ExpiresInString = await DeviceStorageManager.GetDataInStorageWithEncryption(
            GoogleFit.EXPIRES_IN, string.Empty,
            MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);

                    RefreshExpiresInString = await DeviceStorageManager.GetDataInStorageWithEncryption(
            GoogleFit.REFRESH_EXPIRES_IN, string.Empty,
            MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);

                    ExpiresInParsed = DateTimeOffset.TryParse(ExpiresInString, out expiresIn);
                    RefreshExpiresInParsed = DateTimeOffset.TryParse(RefreshExpiresInString, out refreshExpiresIn);

                    ConnectedDeviceDataStorageManager.googleFit = new GoogleFit();
                    ConnectedDeviceDataStorageManager.googleFit.AccessToken = accessToken;
                    ConnectedDeviceDataStorageManager.googleFit.RefreshToken = refreshToken;

                    if (ExpiresInParsed == true)
                    {
                        ConnectedDeviceDataStorageManager.googleFit.ExpiresIn = expiresIn;
                    }
                    else
                    {
                        ConnectedDeviceDataStorageManager.googleFit.ExpiresIn = null;
                    }

                    if (RefreshExpiresInParsed == true)
                    {
                        ConnectedDeviceDataStorageManager.googleFit.RefreshExpiresIn = expiresIn;
                    }
                    else
                    {
                        ConnectedDeviceDataStorageManager.googleFit.RefreshExpiresIn = null;
                    }
                }
            }
            catch (Exception ex)
            {
                HasErrorReadingStorage = true;
            }
            finally
            {
                ConnectedDevices.UpdateFitnessData(MiddleWare.UserID + "",
                    ConnectedDeviceDataStorageManager.googleFit,
                    ConnectedDevicesEnumeration.ConnectedDevice.Google_Fit
                    );
            }
        }

        public static async void SaveGoogleFitData(WebAuthenticatorResult webAuthenticatorResult)
        {
            string expiresInString = string.Empty;
            string refreshTokenExpiresInString = string.Empty;
            string emailAddress = string.Empty;
            try
            {
                ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.AccessToken = webAuthenticatorResult.AccessToken;
                ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.RefreshToken = webAuthenticatorResult.RefreshToken;
                emailAddress = Uri.UnescapeDataString(webAuthenticatorResult.Properties["email"].ToString());
                ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.EmailAddress = emailAddress;
                ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.ExpiresIn = ConnectApplicationDeviceManager.ExtractAccessTokenExpirationDate(webAuthenticatorResult);
                ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.RefreshExpiresIn = ConnectApplicationDeviceManager.ExtractRefreshTokenExpirationDate(webAuthenticatorResult); ;

                if (ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.ExpiresIn.HasValue == true)
                {
                    expiresInString = JsonConvert.SerializeObject(ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.ExpiresIn.Value);
                }

                if (ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.RefreshExpiresIn.HasValue == true)
                {
                    refreshTokenExpiresInString = JsonConvert.SerializeObject(ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.RefreshExpiresIn.Value);
                }

                await DeviceStorageManager.SetDataInStorageWithEncryption(GoogleFit.ACCESS_TOKEN,
                    ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.AccessToken, MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);

                await DeviceStorageManager.SetDataInStorageWithEncryption(GoogleFit.REFRESH_TOKEN,
                  ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.RefreshToken, MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);

                await DeviceStorageManager.SetDataInStorageWithEncryption(GoogleFit.EMAIL_ADDRESS,
                 ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.EmailAddress, MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);

                await DeviceStorageManager.SetDataInStorageWithEncryption(GoogleFit.EXPIRES_IN,
                 expiresInString, MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);

                await DeviceStorageManager.SetDataInStorageWithEncryption(GoogleFit.REFRESH_EXPIRES_IN,
                 refreshTokenExpiresInString, MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);
            }
            catch (Exception ex)
            {                
                throw;
            }
            finally
            {
                ConnectedDevices.UpdateFitnessData(MiddleWare.UserID + "",
                    ConnectedDeviceDataStorageManager.googleFit,
                    ConnectedDevicesEnumeration.ConnectedDevice.Google_Fit
                    );
            }
        }

        private static void ClearGoogleFitFitnessServiceStorage(bool useSecureStorage)
        {
            try
            {
                switch (useSecureStorage)
                {
                    case true:

                        SecureStorage.Default.Remove(GoogleFit.ACCESS_TOKEN);
                        SecureStorage.Default.Remove(GoogleFit.REFRESH_TOKEN);
                        SecureStorage.Default.Remove(GoogleFit.EMAIL_ADDRESS);
                        SecureStorage.Default.Remove(GoogleFit.EXPIRES_IN);
                        SecureStorage.Default.Remove(GoogleFit.REFRESH_EXPIRES_IN);
                        break;

                    case false:

                        Preferences.Default.Remove(GoogleFit.ACCESS_TOKEN);
                        Preferences.Default.Remove(GoogleFit.REFRESH_TOKEN);
                        Preferences.Default.Remove(GoogleFit.EMAIL_ADDRESS);
                        Preferences.Default.Remove(GoogleFit.EXPIRES_IN);
                        Preferences.Default.Remove(GoogleFit.REFRESH_EXPIRES_IN);
                        break;
                }
            }
            catch (Exception ex)
            {
                //await App.Current.MainPage.DisplayAlert(ex.Message, ex.StackTrace,
                //         "OK");
                // log error
                throw new DeviceStorageException("Error occured while saving stored value", ex.InnerException);
            }
            finally
            {
                ConnectedDeviceDataStorageManager.ClearGoogleFitData();
                ConnectedDevices.UpdateFitnessData(MiddleWare.UserID + "",
                    ConnectedDeviceDataStorageManager.googleFit,
                    ConnectedDevicesEnumeration.ConnectedDevice.Google_Fit
                    );
            }
        }

        #endregion

        #region [Static Methods :: Tasks :: Apple Health Kit]

        public static async void InitializeAppleHealthKitData()
        {
            string accessToken = string.Empty;
            string refreshToken = string.Empty;
            string emailAddress = string.Empty;
            DateTimeOffset expiresIn = DateTimeOffset.MinValue;
            DateTimeOffset refreshExpiresIn = DateTimeOffset.MinValue;

            string ExpiresInString = null;
            string RefreshExpiresInString = null;

            bool ExpiresInParsed = false;
            bool RefreshExpiresInParsed = false;

            try
            {
                accessToken = await DeviceStorageManager.GetDataInStorageWithEncryption(
               AppleHealthKit.ACCESS_TOKEN, string.Empty,
               MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);

                if (string.IsNullOrWhiteSpace(accessToken) == false)
                {
                    refreshToken = await DeviceStorageManager.GetDataInStorageWithEncryption(
               AppleHealthKit.REFRESH_TOKEN, string.Empty,
               MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);

                    emailAddress = await DeviceStorageManager.GetDataInStorageWithEncryption(
            AppleHealthKit.EMAIL_ADDRESS, string.Empty,
            MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);

                    ExpiresInString = await DeviceStorageManager.GetDataInStorageWithEncryption(
            AppleHealthKit.EXPIRES_IN, string.Empty,
            MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);

                    RefreshExpiresInString = await DeviceStorageManager.GetDataInStorageWithEncryption(
            AppleHealthKit.REFRESH_EXPIRES_IN, string.Empty,
            MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);

                    ExpiresInParsed = DateTimeOffset.TryParse(ExpiresInString, out expiresIn);
                    RefreshExpiresInParsed = DateTimeOffset.TryParse(RefreshExpiresInString, out refreshExpiresIn);

                    ConnectedDeviceDataStorageManager.appleHealthKit = new AppleHealthKit();
                    ConnectedDeviceDataStorageManager.appleHealthKit.AccessToken = accessToken;
                    ConnectedDeviceDataStorageManager.appleHealthKit.RefreshToken = refreshToken;

                    if (ExpiresInParsed == true)
                    {
                        ConnectedDeviceDataStorageManager.appleHealthKit.ExpiresIn = expiresIn;
                    }
                    else
                    {
                        ConnectedDeviceDataStorageManager.appleHealthKit.ExpiresIn = null;
                    }

                    if (RefreshExpiresInParsed == true)
                    {
                        ConnectedDeviceDataStorageManager.appleHealthKit.RefreshExpiresIn = expiresIn;
                    }
                    else
                    {
                        ConnectedDeviceDataStorageManager.appleHealthKit.RefreshExpiresIn = null;
                    }
                }
            }
            catch (Exception ex)
            {
                HasErrorReadingStorage = true;
            }
            finally
            {
                ConnectedDevices.UpdateFitnessData(MiddleWare.UserID + "",
                  ConnectedDeviceDataStorageManager.appleHealthKit,
                  ConnectedDevicesEnumeration.ConnectedDevice.Apple_Health_Kit
                  );
            }
        }

        public static async void SaveAppleHealthKitData(WebAuthenticatorResult webAuthenticatorResult)
        {
            string expiresInString = string.Empty;
            string refreshTokenExpiresInString = string.Empty;

            try
            {
                ParentMiddleWare.ConnectedDeviceDataStorageManager.appleHealthKit.AccessToken = webAuthenticatorResult.AccessToken;
                ParentMiddleWare.ConnectedDeviceDataStorageManager.appleHealthKit.RefreshToken = webAuthenticatorResult.RefreshToken;
                ParentMiddleWare.ConnectedDeviceDataStorageManager.appleHealthKit.EmailAddress = webAuthenticatorResult.Properties["email"].ToString();
                ParentMiddleWare.ConnectedDeviceDataStorageManager.appleHealthKit.ExpiresIn = ConnectApplicationDeviceManager.ExtractAccessTokenExpirationDate(webAuthenticatorResult);
                ParentMiddleWare.ConnectedDeviceDataStorageManager.appleHealthKit.RefreshExpiresIn = ConnectApplicationDeviceManager.ExtractRefreshTokenExpirationDate(webAuthenticatorResult); ;

                if (ParentMiddleWare.ConnectedDeviceDataStorageManager.appleHealthKit.ExpiresIn.HasValue == true)
                {
                    expiresInString = JsonConvert.SerializeObject(ParentMiddleWare.ConnectedDeviceDataStorageManager.appleHealthKit.ExpiresIn.Value);
                }

                if (ParentMiddleWare.ConnectedDeviceDataStorageManager.appleHealthKit.RefreshExpiresIn.HasValue == true)
                {
                    refreshTokenExpiresInString = JsonConvert.SerializeObject(ParentMiddleWare.ConnectedDeviceDataStorageManager.appleHealthKit.RefreshExpiresIn.Value);
                }

                await DeviceStorageManager.SetDataInStorageWithEncryption(AppleHealthKit.ACCESS_TOKEN,
                    ParentMiddleWare.ConnectedDeviceDataStorageManager.appleHealthKit.AccessToken, MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);

                await DeviceStorageManager.SetDataInStorageWithEncryption(AppleHealthKit.REFRESH_TOKEN,
                  ParentMiddleWare.ConnectedDeviceDataStorageManager.appleHealthKit.RefreshToken, MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);

                await DeviceStorageManager.SetDataInStorageWithEncryption(AppleHealthKit.EMAIL_ADDRESS,
                 ParentMiddleWare.ConnectedDeviceDataStorageManager.appleHealthKit.EmailAddress, MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);

                await DeviceStorageManager.SetDataInStorageWithEncryption(AppleHealthKit.EXPIRES_IN,
                 expiresInString, MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);

                await DeviceStorageManager.SetDataInStorageWithEncryption(AppleHealthKit.REFRESH_EXPIRES_IN,
                 refreshTokenExpiresInString, MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);
            }
            catch (Exception ex)
            {                
                throw;
            }
            finally
            {
                ConnectedDevices.UpdateFitnessData(MiddleWare.UserID + "",
                  ConnectedDeviceDataStorageManager.appleHealthKit,
                  ConnectedDevicesEnumeration.ConnectedDevice.Apple_Health_Kit
                  );
            }
        }

        private static void ClearAppleHealthKitFitnessServiceStorage(bool useSecureStorage)
        {
            try
            {
                switch (useSecureStorage)
                {
                    case true:

                        SecureStorage.Default.Remove(AppleHealthKit.ACCESS_TOKEN);
                        SecureStorage.Default.Remove(AppleHealthKit.REFRESH_TOKEN);
                        SecureStorage.Default.Remove(AppleHealthKit.EMAIL_ADDRESS);
                        SecureStorage.Default.Remove(AppleHealthKit.EXPIRES_IN);
                        SecureStorage.Default.Remove(AppleHealthKit.REFRESH_EXPIRES_IN);
                        break;

                    case false:

                        Preferences.Default.Remove(AppleHealthKit.ACCESS_TOKEN);
                        Preferences.Default.Remove(AppleHealthKit.REFRESH_TOKEN);
                        Preferences.Default.Remove(AppleHealthKit.EMAIL_ADDRESS);
                        Preferences.Default.Remove(AppleHealthKit.EXPIRES_IN);
                        Preferences.Default.Remove(AppleHealthKit.REFRESH_EXPIRES_IN);
                        break;
                }
            }
            catch (Exception ex)
            {
                //await App.Current.MainPage.DisplayAlert(ex.Message, ex.StackTrace,
                //         "OK");
                // log error
                throw new DeviceStorageException("Error occured while saving stored value", ex.InnerException);
            }
            finally
            {
                ConnectedDeviceDataStorageManager.ClearAppleHealthKitData();
                ConnectedDevices.UpdateFitnessData(MiddleWare.UserID + "",
                 ConnectedDeviceDataStorageManager.appleHealthKit,
                 ConnectedDevicesEnumeration.ConnectedDevice.Apple_Health_Kit
                 );
            }
        }

        #endregion

        #region [Static Methods :: Tasks :: Device Information]

        public static async Task<Guid> GetDeviceIdStorageData()
        {
            Guid deviceId = Guid.Empty;
            string deviceIdString = string.Empty;
            bool guidValid = false;

            try
            {
                deviceIdString = await DeviceStorageManager.GetDataInStorageWithEncryption(
               DeviceInformationManager.DEVICE_IDENTIFIER, string.Empty,
               MiddleWare.UseSecuredStorage, MiddleWare.DeviceIntegrationEncryptionId);

                guidValid = Guid.TryParse(deviceIdString, out deviceId);

                return deviceId;
            }
            catch (Exception ex)
            {
                HasErrorReadingStorage = true;

                return deviceId;
            }
            finally
            {
                
            }
        }

        public static async void SaveDeviceIdData(Guid deviceId)
        {
            try
            {               
                await DeviceStorageManager.SetDataInStorageWithEncryption(DeviceInformationManager.DEVICE_IDENTIFIER,
                    deviceId.ToString(), 
                    MiddleWare.UseSecuredStorage, 
                    MiddleWare.DeviceIntegrationEncryptionId);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
               
            }
        }

        private static void ClearDeviceIdStorage(bool useSecureStorage)
        {
            try
            {
                switch (useSecureStorage)
                {
                    case true:

                        SecureStorage.Default.Remove(DeviceInformationManager.DEVICE_IDENTIFIER);
                        break;

                    case false:

                        Preferences.Default.Remove(DeviceInformationManager.DEVICE_IDENTIFIER);
                        break;
                }
            }
            catch (Exception ex)
            {
                //await App.Current.MainPage.DisplayAlert(ex.Message, ex.StackTrace,
                //         "OK");
                // log error
                throw new DeviceStorageException("Error occured while clearing stored value", ex.InnerException);
            }
            finally
            {
              
            }
        }

        #endregion
    }
}
