using SecurityServices.Client.Business;
using SecurityServices.Client.Business.Transactions;

namespace MauiApp1.Business
{
    public static class ConnectApplicationDeviceManager
    {
        #region [Fields]

        private static int _tryCount = 0;

        public static MobileUserAccountAdministrator mobileUserAccountAdministrator { get; set; }

        #endregion

        #region [Static Methods :: Tasks]

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

            if (string.Equals(previousAccessToken, currentAccessToken) == true)
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

        #endregion
    }
}
