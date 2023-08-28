using ParentMiddleWare.Models.DeviceIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare
{
    public static class ConnectedDeviceDataStorageManager
    {
        public static GoogleFit? googleFit {  get; set; }

        public static AppleHealthKit? appleHealthKit {  get; set; }


        public static void ClearGoogleFitData()
        {
            googleFit = null;
        }

        public static void ClearAppleHealthKitData()
        {
            appleHealthKit = null;
        }

        public static void ClearProperties()
        {
            googleFit = null;
            appleHealthKit = null;
        }

    }
}
