using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.Models.DeviceIntegration
{
    public class GoogleFit : DeviceFitness
    {
        public const string NAME = "GoogleFit";
        public const string PAST_ACCESS_TOKEN = "GoogleFitPastAccessToken";
        public const string ACCESS_TOKEN = "GoogleFitAccessToken";
        public const string REFRESH_TOKEN = "GoogleFitRefreshToken";
        public const string EMAIL_ADDRESS = "GoogleFitEmailAddress";
        public const string EXPIRES_IN = "GoogleFitExpiresIn";
        public const string REFRESH_EXPIRES_IN = "GoogleFitRefreshExpiresIn";

        //public string AccessToken { get; set; }

        //public string RefreshToken { get; set; }

        //public string EmailAddress { get; set; }

        //public DateTimeOffset? ExpiresIn { get; set; }

        //public DateTimeOffset? RefreshExpiresIn { get; set; }
    }
}
