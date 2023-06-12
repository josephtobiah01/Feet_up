using Microsoft.EntityFrameworkCore;
using System.Net;
using AirApnFunctions.Models;
using Azure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using System.Configuration;

namespace AirApnFunctions
{
    public class Test
    {
        private readonly ILogger _logger;
        private static INotificationHubClient hub;
       // private readonly UserContext _uContext;


        public Test(ILoggerFactory loggerFactory) //, UserContext uContext)
        {
         //   _uContext = uContext;
            _logger = loggerFactory.CreateLogger<Test>();
        }

        [Function("Test")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");


            var response = req.CreateResponse(HttpStatusCode.OK);
            try
            {
                long userId = long.Parse(req.Query["userId"]);

                hub = new NotificationHubClient(
                  "Endpoint=sb://airns1.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=NSSFc3/djIcdHKt7JXDitQ2g6b3RyMTwIFLR5OrziQM=",
                  "airnotificationhub"
                  );

                string connectionString = "Data Source=c88-sqlserver-sea-test.database.windows.net;Initial Catalog=FitApp-test_dev;Persist Security Info=True;User ID=FitApp;Password=\"f^2kL;$f78DFbjh79\""; //ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                //string connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
                var options = new DbContextOptionsBuilder<FitAppContext>().UseSqlServer(connectionString).Options;
               // _context = new FitAppContext(options);
                using (FitAppContext _uContext = new FitAppContext(options))
                {
                    try
                    {
                        var DeviceList = await _uContext.Apn.Where(t => t.FkUserId == userId && t.IsActive == true).ToListAsync();
                        if (DeviceList == null || DeviceList.Count <= 0)
                        {
                            response.WriteString("No such Number");
                        }
                        foreach (var device in DeviceList)
                        {


                            //   var jsonPayload = "{\"aps\":{\"alert\":\"Notification Hub test notification\"}}";

                            var jsonPayload = "{\"aps\":{\n\n\"alert\" :\"Got Milk?\",\n\"badge\" :\"7\",\n\"Content-available\" : \"1\",\n\"sound\" : \"\"\n}}";

                            var n = new AppleNotification(jsonPayload);
                            NotificationOutcome outcome = await hub.SendNotificationAsync(n, "RegistrationId:" + device.DeviceId + "");
                        }

                    }
                    catch (Exception ex)
                    {
                        response.WriteString(ex.Message + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                response.WriteString(ex.Message + ex.StackTrace);
            }
      
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }
    }
}
