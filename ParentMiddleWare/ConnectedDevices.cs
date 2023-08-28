using ParentMiddleWare.Models.DeviceIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ParentMiddleWare.Enumeration.ConnectedDevicesEnumeration;

namespace ParentMiddleWare
{
    public static class ConnectedDevices
    {

        #region [Public :: Fields]

        private static bool _isAppleConnected { get; set; }

        public static bool isAppleConnected
        { 
            get
            { 
                return _isAppleConnected; 
            } 
        }

        private static bool _isGoogleConnected { get; set; }

        public static bool isGoogleConnected 
        {
            get
            {
                return _isGoogleConnected;
            }
        }

        #endregion

        #region [Methods :: Tasks]

        private static bool UpdateGoogleFit(string federatedIndividualId, DeviceFitness deviceFitness)
        {
            bool isUpdateComplete = false;

            isUpdateComplete = true;

            return isUpdateComplete;
        }

        private static bool UpdateAppleHealthKit(string federatedIndividualId, DeviceFitness deviceFitness)
        {
            bool isUpdateComplete = false;

            isUpdateComplete = true;

            return isUpdateComplete;
        }

        private static DeviceFitness GetGoogleFitData(string federatedIndividualId)
        {
            try
            {
                GoogleFit googleFit = new GoogleFit();
                return googleFit;
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {

            }           
        }

        private static DeviceFitness GetAppleHealthKitData(string federatedIndividualId)
        {
            try
            {
                DeviceFitness deviceFitness = new DeviceFitness();

                return deviceFitness;
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {

            }            
        }

        #endregion

        #region [Public Methods :: Tasks] 

        public static void GetConnectedDevice(string federatedIndividualId)
        {
            GoogleFit googleFit = null;
            AppleHealthKit appleHealthKit = null;

            googleFit = (GoogleFit)GetGoogleFitData(federatedIndividualId);

            if (googleFit != null)
            {
                _isGoogleConnected = true;
            }
            else
            {
                _isGoogleConnected = false;
            }

            appleHealthKit = (AppleHealthKit)GetAppleHealthKitData(federatedIndividualId);

            if(appleHealthKit != null)
            {
                _isAppleConnected = true;
            }
            else
            {
                _isAppleConnected = false;
            }            
        }

        public static DeviceFitness UpdateFitnessData(string federatedIndividualId, DeviceFitness deviceFitness, ConnectedDevice connectedDevice)
        {
            try
            {
                switch (connectedDevice)
                {
                    case ConnectedDevice.Unknown:
                        break;

                    case ConnectedDevice.Google_Fit:

                        UpdateGoogleFit(federatedIndividualId, deviceFitness);
                        break;

                    case ConnectedDevice.Apple_Health_Kit:

                        UpdateAppleHealthKit(federatedIndividualId, deviceFitness);
                        break;

                    default:

                        break;
                }

                return deviceFitness;
            }
            catch (Exception ex)
            {
                return deviceFitness;
            }
            finally
            {                
            }           
        }

        #endregion


    }
}
