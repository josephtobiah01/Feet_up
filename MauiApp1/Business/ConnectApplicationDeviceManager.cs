using FitnessData.Common;
using Newtonsoft.Json;
using SecurityServices.Client.Business.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Business
{
    public static class ConnectApplicationDeviceManager
    {

        private static int _tryCount = 0;

        //public const string URL = "https://security-development.ageinreverse.me/mobileauth/Google";
        public const string URL = "https://security.ageinreverse.me/mobileauth/Google";
        public const string PAST_ACCESS_TOKEN_STRING = "pastAccessToken";
        public const string ACCESS_TOKEN_STRING = "accessToken";
        public const string REFRESH_TOKEN_STRING = "refreshToken";
        public const string EMAIL_ADDRESS_STRING = "emailAddress";
        public const string EXPIRES_IN_STRING = "expiresIn";
        public const string REFRESH_EXPIRES_IN_STRING = "refreshExpiresIn";

        //public const string APPCONNECTENDPOINT = "https://security-development.ageinreverse.me/en-US/v1/REST";
        public const string APPCONNECTENDPOINT = "https://security.ageinreverse.me/en-US/v1/REST";

        public static MobileUserAccountTransactionManager mobileUserAccountTransactionManager { get; set; }

        public static void ClearFitnessServiceStorage()
        {
            Preferences.Default.Set(ACCESS_TOKEN_STRING, string.Empty);
        }

        public static DateTimeOffset? ExtractAccessTokenExpirationDate(WebAuthenticatorResult webAuthenticatorResult)
        {
            DateTimeOffset accessTokenExpirationDate;
            string accessTokenExpiresAtValue;
            DateTimeOffset accessTokenExpiresAtDateTimeOffset;
            KeyValuePair<string, string?> accessTokenExpiresAtKeyValuePair = webAuthenticatorResult.Properties.FirstOrDefault(item => item.Key == "expires_in");



            if (accessTokenExpiresAtKeyValuePair.Equals(default(KeyValuePair<string, string?>)))
            {
                return null;
            }
            else
            {
                accessTokenExpiresAtValue = accessTokenExpiresAtKeyValuePair.Value;
                long accessTokenUnixTimeSeconds;
                bool successful = long.TryParse(accessTokenExpiresAtValue, out accessTokenUnixTimeSeconds);



                if (successful == false)
                {
                    return null;
                }
                else
                {
                    try
                    {
                        accessTokenExpirationDate = DateTimeOffset.FromUnixTimeSeconds(accessTokenUnixTimeSeconds);



                        return accessTokenExpirationDate;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }

        public static DateTimeOffset? ExtractRefreshTokenExpirationDate(WebAuthenticatorResult webAuthenticatorResult)
        {
            DateTimeOffset refreshTokenExpirationDate;
            string refreshTokenExpiresAtValue;
            DateTimeOffset refreshTokenExpiresAtDateTimeOffset;
            KeyValuePair<string, string?> refreshTokenExpiresAtKeyValuePair = webAuthenticatorResult.Properties.FirstOrDefault(item => item.Key == "refresh_token_expires_in");



            if (refreshTokenExpiresAtKeyValuePair.Equals(default(KeyValuePair<string, string?>)))
            {
                return null;
            }
            else
            {
                refreshTokenExpiresAtValue = refreshTokenExpiresAtKeyValuePair.Value;
                long refreshTokenUnixTimeSeconds;
                bool successful = long.TryParse(refreshTokenExpiresAtValue, out refreshTokenUnixTimeSeconds);



                if (successful == false)
                {
                    return null;
                }
                else
                {
                    try
                    {
                        refreshTokenExpirationDate = DateTimeOffset.FromUnixTimeSeconds(refreshTokenUnixTimeSeconds);



                        return refreshTokenExpirationDate;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }
    
        public static bool isUserEnteredConnectAppLoop(string previousAccessToken, string currentAccessToken)
        {

            if(string.Equals(previousAccessToken,currentAccessToken) == true)
            {
                _tryCount++;
                if (_tryCount > 3)
                {
                    _tryCount = 0;
                    return true;
                }
                else
                {
                    _tryCount = 0; 
                    return false;
                }                
            }
            else
            {
                _tryCount = 0;
                return false;
            }
        }
    }
}
