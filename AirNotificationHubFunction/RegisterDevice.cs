using DAOLayer.Net7.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AirNotificationHubFunction
{

    public static class RegisterDevice
    {
        private static INotificationHubClient hub;
        [FunctionName("RegisterDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            try
            {
              //  log.LogInformation("C# HTTP trigger function processed a request.");

                string InstallationId = req.Query["InstallationId"];
                string userId = req.Query["userId"];


                hub = new NotificationHubClient(
                  "Endpoint=sb://airns1.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=NSSFc3/djIcdHKt7JXDitQ2g6b3RyMTwIFLR5OrziQM=",
                  "airnotificationhub"
                  );

                var installation = new Installation
                {
                    InstallationId = InstallationId,
                    PushChannel = "general",
                    UserId = userId,
                    Platform = NotificationPlatform.Apns
                };


                await hub.CreateOrUpdateInstallationAsync(installation);

                return new OkObjectResult("OK");
            }
            catch (Exception ex)
            {
                return new OkObjectResult(ex.Message + ex.StackTrace);
            }
        }



        [FunctionName("Run2")]
        public static async Task<IActionResult> Run2(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            try
            {

                long userId = long.Parse(req.Query["userId"]);


                hub = new NotificationHubClient(
                  "Endpoint=sb://airns1.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=NSSFc3/djIcdHKt7JXDitQ2g6b3RyMTwIFLR5OrziQM=",
                  "airnotificationhub"
                  );


                using (UserContext _uContext = new UserContext() )
                {
                    try
                    {
                        var DeviceList = await _uContext.Apn.Where(t => t.FkUserId == userId && t.IsActive == true).ToListAsync();
                        if (DeviceList == null)
                        {
                            return new OkObjectResult(false);
                        }
                        foreach (var device in DeviceList)
                        {

                            var jsonPayload = "{\"aps\":{\"alert\":\"Notification Hub test notification\"}}";
                            var n = new AppleNotification(jsonPayload);
                            NotificationOutcome outcome = await hub.SendNotificationAsync(n, "RegistrationId:" + device.DeviceId + "/");
                        }

                    }
                    catch (Exception ex)
                    {
                 
                    }
                }
            }
            catch (Exception ex)
            {
                return new OkObjectResult(false);
            }
            return new OkObjectResult(true);

        }
    }
}
