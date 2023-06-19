using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Common.DeviceIntegration
{
    public static class DeviceIntegrationEnumerations
    {
        /// <summary>
        /// This is a sample only in preparation for calories burned but there is no enumerations yet.
        /// </summary>
        public enum CaloriesExpendedTypes
        {
            Passive = 1,
            Active = 2
        }

        public enum SleepStageTypes
        {
            AwakeDuringSleepCycle = 1,
            Sleep = 2,
            OutOfBed = 3,
            LightSleep = 4,
            DeepSleep = 5,
            Rem = 6
        }
    }
}
