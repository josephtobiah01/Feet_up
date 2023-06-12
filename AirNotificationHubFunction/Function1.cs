using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AirNotificationHubFunction
{
    public class Function1
    {
        private static INotificationHubClient hub;
        [FunctionName("Function1")]
        public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            if (myTimer.IsPastDue)
            {
                log.LogInformation("Timer is running late!");
            }
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            //var namespaceManager = new NamespaceManager("connection string");

            //var hub = await namespaceManager.GetNotificationHubAsync("hubname", CancellationToken.None);

             hub = new NotificationHubClient(
                "Endpoint=sb://airns1.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=NSSFc3/djIcdHKt7JXDitQ2g6b3RyMTwIFLR5OrziQM=",
                "airnotificationhub"
                );



        }


        public void ChatThread()
        {

        }


        public async Task<bool> SendAppleNotification(string UserId)
        {
            try
            {
                var jsonPayload = "{\"aps\":{\"alert\":\"Notification Hub test notification\"}}";
                var n = new AppleNotification(jsonPayload);
                NotificationOutcome outcome = await hub.SendNotificationAsync(n, "$UserId:" + UserId + "/");
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }











    public class AppleInstallation : Installation
    {
        /// <summary>
        /// Creates a new instance of the AppleInstallation class.
        /// </summary>
        public AppleInstallation()
        {
            Platform = NotificationPlatform.Apns;
        }

        /// <summary>
        /// Creates a new instance of the AppleInstallation class.
        /// </summary>
        /// <param name="installationId">The unique identifier for the installation.</param>
        /// <param name="deviceToken">The APNs device token to use for the PushChannel.</param>
        public AppleInstallation(string installationId, string deviceToken) : this()
        {
            InstallationId = installationId ?? throw new ArgumentNullException(nameof(installationId));
            PushChannel = deviceToken ?? throw new ArgumentNullException(nameof(deviceToken));
        }
    }
}
