using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.Models.DeviceIntegration
{
    public class DeviceFitness
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string EmailAddress { get; set; }

        public DateTimeOffset? ExpiresIn { get; set; }

        public DateTimeOffset? RefreshExpiresIn { get; set; }
    }
}
