using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Business
{
    public class DeviceInformationManager
    {
        public const string DEVICE_IDENTIFIER = "Device_Identifier";

        public Guid DeviceId { get; set; }

        public string DeviceName { get; set; }

        public string DeviceType { get; set;}

        public string DeviceDescription { get; set; }

        #region [Method :: Task]

        public void CreateDeviceId()
        {
            this.DeviceId = Guid.NewGuid();
        }

        #endregion
    }
}
