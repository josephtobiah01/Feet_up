using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.Models.DeviceIntegration
{
    public class AppleHealthKit : DeviceFitness
    {
        public const string NAME = "AppleHealthKit";
        public const string PAST_ACCESS_TOKEN = "AppleHealthKitPastAccessToken";
        public const string ACCESS_TOKEN = "AppleHealthKitAccessToken";
        public const string REFRESH_TOKEN = "AppleHealthKitRefreshToken";
        public const string EMAIL_ADDRESS = "AppleHealthKitEmailAddress";
        public const string EXPIRES_IN = "AppleHealthKitExpiresIn";
        public const string REFRESH_EXPIRES_IN = "AppleHealthKitRefreshExpiresIn";

        //public string AccessToken { get; set; }

        //public string RefreshToken { get; set; }

        //public string EmailAddress { get; set; }

        //public DateTimeOffset? ExpiresIn { get; set; }

        //public DateTimeOffset? RefreshExpiresIn { get; set; }
    }
}
