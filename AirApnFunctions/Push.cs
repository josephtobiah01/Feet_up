
using DAOLayer.Net7.Exercise;
using DAOLayer.Net7.Nutrition;
using DAOLayer.Net7.Supplement;
using DAOLayer.Net7.User;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.NotificationHubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Statics;
using System.Net;

namespace AirApnFunctions
{


    public class PushId
    {
        public string DeviceID { get; set; }
        public string Platform { get; set; }
        public long UserId;
    }


    public class Push
    {
    //    public static bool ONLY_TEST_MANUAL = false;


        public static bool LOG = true;
        public static bool LOGERROR = true;
        //prod
        // when to send notifcation i.e 5 minutes before card time
        //public static int PUSHMINOFFSET = 6;
        //// how long register spawn, i.e register for 30 minutes, double registrations are blocked by code
        //public static int REGISTERTIME = 30;
        //// look at databse +/- this in hours
        //public static int SCANTIME = 2;

        //test
        public static int PUSHMINOFFSET = 5;
        // public static int PUSHMINOFFSET = 60;

        //public static int REGISTERTIME = 40;
        public static int REGISTERTIME = 40;
        public double DELETE_OFFSET = 1.5;

        public static bool REMEMBER_DEVICES = true;


        // DEBUG
        public static string PrimaryConnectionString = "Endpoint=sb://aitnotificationhubsandbox.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=kf3Etl/3r2Yxn8xwWQl73/gREdPknXGOnIk3hVRYALU=";
        public static string HubName = "aitnotificationhubsandbox";
        public static string connectionString = "Data Source=air-sql-prod-sea.database.windows.net;Initial Catalog=FitApp-test;Persist Security Info=True;User ID=FitApp;Password=f^2kL$f78DFbjh79H$";
        public static bool TEST = true;



        // TEST
        //public static string PrimaryConnectionString = "Endpoint=sb://airnotif.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=Ppu/iR2p2IizU13e17jUlolCyABSqcp1K5cf4Yt7ir0=";
        //public static string HubName = "airnotificationhub";
        //public static string connectionString = "Data Source=air-sql-prod-sea.database.windows.net;Initial Catalog=FitApp-test;Persist Security Info=True;User ID=FitApp;Password=f^2kL$f78DFbjh79H$";
        //public static bool TEST = false;


        // PROD
        //public static string PrimaryConnectionString = "Endpoint=sb://airnotif.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=Ppu/iR2p2IizU13e17jUlolCyABSqcp1K5cf4Yt7ir0=";
        //public static string HubName = "airnotificationhub";
        //public static string connectionString = "Data Source=air-sql-prod-sea.database.windows.net;Initial Catalog=FitApp-prod;Persist Security Info=True;User ID=FitApp;Password=f^2kL$f78DFbjh79H$";
        //public static bool TEST = false;



        public static NotificationHubClient nhClient = NotificationHubClient.CreateClientFromConnectionString(PrimaryConnectionString, HubName);


        private readonly ILogger _logger;

        public Push(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Push>();
        }


        [Function("Push")]
        public async Task Run([TimerTrigger("0 */2 * * * *")] MyInfo myTimer)
        // public async Task Run([TimerTrigger("0 */1 * * * *")] MyInfo myTimer)
        {
            try
            {
                if(TEST) { return; }
                await DoRunPush();
            }
            catch (Exception ex)
            {
                if (LOGERROR)
                {
                    await LogError(ex.Message, ex.StackTrace);
                }
            }
        }


        public async Task DoRunPush()
        {
            try
            {
                //   if (Push.ONLY_TEST_MANUAL) return;
                DateTime UTCNOW = DateTime.UtcNow;
                DateTime UTCNOW_TRIGGER = DateTime.UtcNow.AddMinutes(PUSHMINOFFSET);
                var options = new DbContextOptionsBuilder<UserContext>().UseSqlServer(Push.connectionString).Options;

                using (UserContext _uContext = new UserContext(options))
                {

                    var scheduleList = await _uContext.ApnSchedule.Where(t => UTCNOW_TRIGGER >= t.DateToTriggerUtc)
                        .ToListAsync();

                    if (scheduleList == null) return;

                    foreach (var schedule in scheduleList)
                    {
                        try
                        {
                            if (UTCNOW_TRIGGER >= schedule.DateToTriggerUtc)
                            {
                                if (schedule.DateToTriggerUtc.AddMinutes((REGISTERTIME * DELETE_OFFSET)) < UTCNOW)
                                {
                                    _uContext.ApnSchedule.Remove(schedule);
                                }
                                else if (await ShouldSendCheckIfComplete(schedule, schedule.Category))
                                {
                                    await DispatchPushMessageFromSchedule(schedule, schedule.Category);
                                    schedule.HasSent = true;
                                }
                                else
                                {
                         //           _uContext.ApnSchedule.Remove(schedule);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (LOGERROR)
                            {
                                await LogError(ex.Message, ex.StackTrace);
                            }
                        }
                    }
                    await _uContext.SaveChangesAsync();
                }


            }
            catch (Exception ex)
            {
                if (LOGERROR)
                {
                    await LogError(ex.Message, ex.StackTrace);
                }
            }
        }
        


        public async static Task<bool> ShouldSendCheckIfComplete(ApnSchedule obj, string Category)
        {
            try
            {
                if (Category == Strings.NOTIF_TRAINING)
                {
                    if (obj.HasSent == true) return false;
                    var options = new DbContextOptionsBuilder<ExerciseContext>().UseSqlServer(connectionString).Options;
                    using (ExerciseContext _uContext = new ExerciseContext(options))
                    {
                        var tsession = await _uContext.EdsTrainingSession.Where(t => t.Id == obj.ItemId).FirstOrDefaultAsync();
                        if (tsession == null) return false;
                        if (tsession.EndTimeStamp.HasValue) return false;
                        if (tsession.IsSkipped) return false;
                        return true;
                    }
                }
                else if (Category == Strings.NOTIF_NUTRIENT)
                {
                    if (obj.HasSent == true) return false;
                    var options = new DbContextOptionsBuilder<NutritionContext>().UseSqlServer(connectionString).Options;
                    using (NutritionContext _uContext = new NutritionContext(options))
                    {
                        var meal = await _uContext.FnsNutritionActualMeal.Where(t => t.Id == obj.ItemId).FirstOrDefaultAsync();
                        if (meal == null) return false;
                        if (meal.IsComplete) return false;
                        if (meal.IsOngoing) return false;
                        if (meal.IsSkipped) return false;
                        return true;
                    }
                }
                else if (Category == Strings.NOTIF_SUPPLEMENT)
                {
                    if (obj.HasSent == true) return false;
                    var options = new DbContextOptionsBuilder<SupplementContext>().UseSqlServer(connectionString).Options;
                    using (SupplementContext _uContext = new SupplementContext(options))
                    {
                        var dose = await _uContext.NdsSupplementScheduleDose.Where(t => t.Id == obj.ItemId).FirstOrDefaultAsync();
                        if (dose == null) return false;
                        if (dose.IsComplete) return false;
                        if (dose.IsSkipped) return false;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace);
                return false;
            }
            return false;
        }



        [Function("SendTranscriptionComplete")]
        public async Task<HttpResponseData> SendTranscriptionComplete([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            try
            {
                long userId = long.Parse(req.Query["MealId"]);
                await SendSendTranscriptionComplete(userId);
                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Function("SendCUSTOMPush")]
        public async Task<HttpResponseData> SendCUSTOMPush([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            try
            {
                long userId = long.Parse(req.Query["userId"]);
                string ca = req.Query["Category"];
                string pl = req.Query["Payload"];
                await SendCUSTOMPushMessage(userId, ca, pl);
                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }


        //     var outcome = await nhClient.SendDirectNotificationAsync(message, DeviceID);
        [Function("SendTest")]
        public async Task<HttpResponseData> SendTest([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed SendChat");

            try
            {
                string DeviceID = req.Query["DeviceID"];
                var message = GetPushMessageString
                (
                  "D",
                   "You’ve got a new message",
                  "Your Trainer or Nutritionist have send you a message",
                  "0",
                   Strings.NOTIF_CHAT,
                   "",
                   "1"
                );

                var outcome = await nhClient.SendDirectNotificationAsync(GenerateChatPushMessage("D", message), DeviceID);
                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Function("SendChat")]
        public async Task<HttpResponseData> SendChat([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed SendChat");

            try
            {
                long userId = long.Parse(req.Query["userId"]);
                await SendChatPushMessage(userId);
                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Function("SayHello")]
        public async Task<HttpResponseData> SayHello([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            return req.CreateResponse(HttpStatusCode.OK);
        }

        [Function("LogIn")]
        public async Task<HttpResponseData> LogIn([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            return req.CreateResponse(HttpStatusCode.OK);
        }


        public static async Task<List<PushId>> GetDeviceForUser(long UserId)
        {
            List<PushId> UserDevices = new List<PushId>();
            var options = new DbContextOptionsBuilder<UserContext>().UseSqlServer(Push.connectionString).Options;
            using (UserContext _uContext = new UserContext(options))
            {
                // Get devices for user
                var DeviceList = await _uContext.Apn.Where(t => t.FkUserId == UserId && t.IsActive == true).ToListAsync();
                if (DeviceList == null || DeviceList.Count <= 0)
                {
                    if (Push.LOGERROR) await Push.LogError("DEVICELIST ERROR", "USER: " + UserId);
                    return new List<PushId>();
                }
                foreach (var device in DeviceList)
                {
                    if (device.IsActive.HasValue && device.IsActive == true)
                    {
                        if (device.IsActive == false)
                        {
                            try
                            {
                                if (device.LastActive < DateTime.UtcNow.AddDays(-8))
                                {
                                    _uContext.Remove(device);
                                    _uContext.SaveChanges();
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        PushId pid = new PushId() { DeviceID = device.DeviceId, Platform = device.Platform.Trim(), UserId = UserId };
                        UserDevices.Add(pid);
                    }
                }
            }
            return UserDevices;
        }


        public static async Task SendSendTranscriptionComplete(long MealId)
        {
            var options = new DbContextOptionsBuilder<NutritionContext>().UseSqlServer(connectionString).Options;
            using (NutritionContext _uContext = new NutritionContext(options))
            {
                var meal = _uContext.FnsNutritionActualMeal.Where(t => t.Id == MealId)
                    .Include(t => t.FkNutritionActualDay)
                    .Include(t => t.MealType)
                    .AsNoTracking()
                    .FirstOrDefault();

                if (meal != null)
                {
                    long UserId = meal.FkNutritionActualDay.FkUserId;
                    string mealmane = meal.MealType.Name;
                    var DeviceList = await GetDeviceForUser(UserId);
                    foreach (var x in DeviceList)
                    {
                        var message = GetPushMessageString
                        (
                            x.Platform,
                             mealmane + " transcribed",
                             "Your Nutritionist has completed your meal transcription",
                             "0",
                             Strings.NOTIF_TRANSCRIPT,
                             "ME" + MealId,
                             "1"
                          );

                        await DispatchPushMessage(x.Platform, GenerateChatPushMessage(x.Platform, message), x.DeviceID, "SendTransc", UserId);
                    }
                }
            }
        }

        public static async Task SendChatPushMessage(long User)
        {
            var DeviceList = await GetDeviceForUser(User);
            foreach (var x in DeviceList)
            {
                var message = GetPushMessageString
                (
                    x.Platform,
                     "You’ve got a new message",
                     "Your Trainer or Nutritionist have send you a message",
                     "0",
                     Strings.NOTIF_CHAT,
                     "",
                     "1"
                  );
                await DispatchPushMessage(x.Platform, GenerateChatPushMessage(x.Platform, message), x.DeviceID, "SendChat", User);
            }
        }


        public static async Task SendCUSTOMPushMessage(long User, string Category, string Payload)
        {
            var DeviceList = await GetDeviceForUser(User);
            foreach (var x in DeviceList)
            {
                var message = GetPushMessageString
                (
                    x.Platform,
                     "New " + Category,
                     "You got a new ." + Category,
                     "0",
                     Category,
                     Payload,
                     "1"
                  );
                await DispatchPushMessage(x.Platform, GenerateChatPushMessage(x.Platform, message), x.DeviceID, "SendChat", User);
            }
        }


        public static async Task DispatchPushMessageFromSchedule(ApnSchedule n, string category)
        {
            try
            {
                if (n.HasSent) return;
                n.HasSent = true;

                await DispatchPushMessage(n.Platform, GenerateChatPushMessage(n.Platform, n.Message), n.DeviceId, category, n.UserId);

            }
            catch (Exception ex)
            {
                if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace, n.UserId);
            }
        }



        public static async Task DispatchPushMessage(string platform, Notification message, string DeviceID, string category, long UserId)
        {
            try
            {
                if (platform == "D")
                {
                    //   var outcome = await nhClient.SendNotificationAsync(message, "$InstallationId:{" + DeviceID + "}");
                    //    var outcome = await nhClient.SendNotificationAsync(message, "$InstallationId:{" + DeviceID + "}");

                    var outcome = await nhClient.SendDirectNotificationAsync(message, DeviceID);

                    if (outcome != null)
                    {
                        //  outcome.State.
                    }
                }
                else if (platform == "A")
                {

                //  var outcome = await nhClient.SendNotificationAsync(message, "$InstallationId:{" + DeviceID + "}");
                    var outcome = await nhClient.SendDirectNotificationAsync(message, DeviceID);
                    //if (outcome != null)
                    //{
                    //    int count = 0;
                    //    while ((outcome.State == NotificationOutcomeState.Enqueued || outcome.State == NotificationOutcomeState.Processing) && ++count < 10)
                    //    {
                    //        var outcomeDetails = await nhClient.GetNotificationOutcomeDetailsAsync(outcome.NotificationId);
                    //        Thread.Sleep(2000);
                    //    }
                    //}
                    //    var outcomeApnsByDeviceId2 = await nhClient.SendDirectNotificationAsync(message, DeviceID);
                }
                if (LOG)
                {
                    var options = new DbContextOptionsBuilder<UserContext>().UseSqlServer(connectionString).Options;
                    using (UserContext _uContext = new UserContext(options))
                    {
                        ApnLogs logs = new ApnLogs();
                        logs.Category = category;
                        logs.Timestamp = DateTime.Now;
                        logs.FkUserId = UserId;
                        logs.Devicekey = DeviceID;
                        logs.Title = "";
                        logs.Platform = platform;
                        _uContext.ApnLogs.Add(logs);
                        await _uContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace, UserId);
            }
        }

        public static Notification GenerateChatPushMessage(string platofrm, string nstring)
        {
            try
            {
                if (platofrm == "A")
                {
                    return new AppleNotification(nstring);
                }
                else if (platofrm == "D")
                {
                    return new FcmNotification(nstring);
                }
                else return new FcmNotification("");
            }
            catch (Exception ex)
            {
                if (Push.LOGERROR) Push.LogError(ex.Message, ex.StackTrace);
                return null;
            }
        }

        public static string GetPushMessageString(string platform, string Title, string Message, string badge, string ACTION, string PARAM1, string Content)
        {
            if (platform == "A")
            {
                return "{\"aps\":{\"alert\":{\"title\":\"" + Title + "\",\"body\":\"" + Message + "\"},\"data\":{\"aaction\":\""+ ACTION + "\",\"param1\":\""+ PARAM1 + "\"},\"badge\":" + badge + ",\"content-available\":" + Content + ",\"interruption-level\":\"active\"}}";
            }
            else if (platform == "D")
            {
                return "{\"notification\":{\"Title\":\"" + Title + "\",\"body\":\"" + Message + "\"},\"data\":{\"aaction\":\""+ ACTION + "\",\"param1\":\"" + PARAM1 + "\"},\"priority\":\"high\",\"badge\":\"" + badge + "\"}";
            }
            else return "";
        }


        public static async Task DoScanNutrients(long UserId, DateTime UTCNOW, DateTimeOffset offset)
        {
            var options = new DbContextOptionsBuilder<NutritionContext>().UseSqlServer(connectionString).Options;

            using (NutritionContext _uContext = new NutritionContext(options))
            {
                DateTime Sdate = DateTime.UtcNow.Add(offset.Offset).Date;

                //   DateTime mdate = Sdate.Date.AddDays(-1);
                //   DateTime vdate = DateTime.UtcNow.Date.AddDays(1);
                //  DateTime vdate = DateTime.UtcNow.Date.AddDays(1);
                //  var days = await _uContext.FnsNutritionActualDay.Where(t => t.FkUserId == UserId && t.Date > mdate && t.Date < vdate)
                var days = await _uContext.FnsNutritionActualDay.Where(t => t.FkUserId == UserId && t.Date.Date == Sdate)
                      .Include(t => t.FnsNutritionActualMeal)
                      .ThenInclude(t => t.MealType)
                      .AsNoTracking()
                      .ToListAsync();

                if (days != null && days.Count > 0)
                {
                    foreach (var day in days)
                    {
                        try
                        {
                            foreach (var meal in day.FnsNutritionActualMeal)
                            {
                                try
                                {
                                    var mealactualdate = GetDateFromMealName(meal, day);
                                    if (mealactualdate == DateTime.MinValue) continue;
                                    var ddate = GetDateFromMealName(meal, day).Add(-1 * offset.Offset);
                                    if (CheckRegistertime(ddate, UTCNOW))
                                    {
                                        // don't send for complete
                                        if (meal.IsComplete) continue;

                                        string Title = "Time for " + meal.MealType.Name + ".";
                                        string Message = "Don't forget to track your food intake to stay on top of your goals";
                                        await DoQueueMessageNutrients(UTCNOW, UserId, Message, Title, ddate, meal.Id, "ME" + meal.Id);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace, UserId);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace, UserId);
                        }
                    }
                }
            }
        }


        public static async Task<bool> DoQueueMessageNutrients(DateTime UTCNOW, long UserId, string message, string Title, DateTime ddate, long Id, string PARAM1)
        {
            return await SchedulePushNotificationsObject.AddNutrients(new SchedulePushNotification()
            {
                Title = Title,
                UserId = UserId,
                Message = message,
                DateUTC = ddate,
                ID = Id,
                PARAM1 = PARAM1
            }, UTCNOW);
        }

        public static async Task<bool> DoQueueMessageTranining(DateTime UTCNOW, long UserId, string message, string Title, DateTime ddate, long Id, string PARAM1)
        {
            return await SchedulePushNotificationsObject.AddTraningSessions(new SchedulePushNotification()
            {
                Title = Title,
                UserId = UserId,
                Message = message,
                DateUTC = ddate,
                ID = Id,
                PARAM1 = PARAM1
            }, UTCNOW);
        }
        public static async Task<bool> DoQueueMessageSupplements(DateTime UTCNOW, long UserId, string message, string Title, DateTime ddate, long Id, string PARAM1)
        {

            return await SchedulePushNotificationsObject.AddSupplements(new SchedulePushNotification()
            {
                Title = Title,
                UserId = UserId,
                Message = message,
                DateUTC = ddate,
                ID = Id,
                PARAM1 = PARAM1
            }, UTCNOW);
        }

        public static bool CheckRegistertime(DateTime feeditemDate, DateTime UTCNOW)
        {
            double minutes = UTCNOW.Subtract(feeditemDate).TotalMinutes;
            if (minutes > 0) return false;
            if (minutes * -1 <= REGISTERTIME)
            {
                return true;
            }
            return false;
        }

        public static DateTime GetDateFromMealName(FnsNutritionActualMeal meal, FnsNutritionActualDay day)
        {
            if (meal.ScheduledTime.HasValue)
            {
                return meal.ScheduledTime.Value;
            }
            if (meal.MealType.Defaulttime.HasValue)
            {
                return day.Date.Date.Add(meal.MealType.Defaulttime.Value);
            }
            return DateTime.MinValue;
        }

        public static async Task ScanTraningSessions(long UserId, DateTime UTCNOW, DateTimeOffset offset)
        {
            var options = new DbContextOptionsBuilder<ExerciseContext>().UseSqlServer(connectionString).Options;

            using (ExerciseContext _uContext = new ExerciseContext(options))
            {
                try
                {
                    var plan = await _uContext.Eds12weekPlan.Where(t => t.FkCustomerId == UserId && t.IsCurrent == true)
                          .Include(t => t.EdsWeeklyPlan)
                          .ThenInclude(t => t.EdsDailyPlan)
                          .ThenInclude(t => t.EdsTrainingSession)
                          .AsNoTracking()
                          .ToListAsync();

                    if (plan != null && plan.Count > 0)
                    {
                        foreach (var p in plan)
                        {
                            try
                            {
                                foreach (var week in p.EdsWeeklyPlan)
                                {
                                    try
                                    {
                                        foreach (var day in week.EdsDailyPlan)
                                        {
                                            try
                                            {
                                                foreach (var session in day.EdsTrainingSession)
                                                {
                                                    if (!session.StartDateTime.HasValue) continue;
                                                    var ddate = session.StartDateTime.Value.Add(-1 * offset.Offset);
                                                    if (CheckRegistertime(ddate, UTCNOW))
                                                    {
                                                        // dont send alert for finished session
                                                        if (session.EndTimeStamp.HasValue) continue;

                                                        string Message = "Boost your fitness with your scheduled activity.";
                                                        string Title = "Time for your workout";
                                                        await DoQueueMessageTranining(UTCNOW, UserId, Message, Title, ddate, session.Id, "TR" + session.Id);
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace, UserId);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace, UserId);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace, UserId);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace, UserId);
                }
            }
        }

        public static async Task ScanSupplements(long UserId, DateTime UTCNOW, DateTimeOffset offset)
        {
            var options = new DbContextOptionsBuilder<SupplementContext>().UseSqlServer(connectionString).Options;

            using (SupplementContext _uContext = new SupplementContext(options))
            {

                DateTime mdate = DateTime.UtcNow.Add(offset.Offset).Date;

                var s_schedule = await _uContext.NdsSupplementSchedulePerDate.Where(t => t.CustomerId == UserId && t.Date == mdate)
                      .Include(t => t.NdsSupplementSchedule)
                      .ThenInclude(t => t.NdsSupplementScheduleDose)
                      .AsNoTracking()
                      .ToListAsync();

                if (s_schedule != null && s_schedule.Count > 0)
                {
                    foreach (var sdate in s_schedule)
                    {
                        try
                        {
                            bool doDose = true;
                            foreach (var schedule in sdate.NdsSupplementSchedule)
                            {
                                try
                                {
                                    //                    bool doDose = true;
                                    foreach (var dose in schedule.NdsSupplementScheduleDose)
                                    {
                                        try
                                        {
                                            if (doDose)
                                            {
                                                // dont send for completed doses
                                                if (!dose.ScheduledTime.HasValue) continue;
                                                if (dose.CompletionTime.HasValue) continue;

                                                //  var ddate = (sdate.Date.AddHours(dose.ScheduledTime.Value.Hour).AddMinutes(dose.ScheduledTime.Value.Minute)).Add(-1 * offset.Offset);
                                                DateTime ddate;
                                                if (dose.IsSnoozed.HasValue && dose.IsSnoozed.Value == true && dose.SnoozedTime.HasValue)
                                                {
                                                    ddate = sdate.Date.AddHours(dose.SnoozedTime.Value.Hour).Add(-1 * offset.Offset);
                                                }
                                                else
                                                {
                                                    ddate = sdate.Date.AddHours(dose.ScheduledTime.Value.Hour).Add(-1 * offset.Offset);
                                                }
                                                if (CheckRegistertime(ddate, UTCNOW))
                                                {
                                                    doDose = false;
                                                    string Message = "Don't forget to log your supplements in the app";
                                                    string Title = "Take your supplements";
                                                    await DoQueueMessageSupplements(UTCNOW, UserId, Message, Title, ddate, dose.Id, "SU" + ddate.Hour);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace, UserId);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace, UserId);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace, UserId);
                        }
                    }
                }
            }
        }

        public static async Task LogError(string message, string? stack = null, long userId = -1)
        {
            if (LOGERROR)
            {
                var options = new DbContextOptionsBuilder<UserContext>().UseSqlServer(connectionString).Options;
                using (UserContext _uContext = new UserContext(options))
                {
                    ApnErrorLog log = new ApnErrorLog();
                    log.Timestamp = DateTime.UtcNow;
                    log.Message = message;
                    log.Stack = stack;
                    log.FkUsrId = userId;
                    _uContext.Add(log);
                    await _uContext.SaveChangesAsync();
                }
            }
        }

        [Function("PrintStack")]
        public async Task<string> PrintStack([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            string mmm = "PUSH QUEUE" + System.Environment.NewLine + System.Environment.NewLine;
            DateTime UTCNOW = DateTime.UtcNow;
            var options = new DbContextOptionsBuilder<UserContext>().UseSqlServer(Push.connectionString).Options;

            using (UserContext _uContext = new UserContext(options))
            {
                try
                {
                    var scheduleList = await _uContext.ApnSchedule.ToListAsync();
                    if (scheduleList == null) return mmm;

                    foreach (var schedule in scheduleList)
                    {
                        mmm += "Created: " + schedule.CreatedUtc
                            + " | Valid Till: " + schedule.DateToTriggerUtc.AddMinutes(REGISTERTIME * DELETE_OFFSET)
                            + " | User: " + schedule.UserId
                            + " |" + schedule.Category
                            + " | TriggerUTC: " + schedule.DateToTriggerUtc
                            + " | IsSent: " + schedule.HasSent
                            + System.Environment.NewLine;
                    }
                    return mmm;
                }
                catch (Exception ex)
                {
                    return mmm;
                }
            }
        }

    }


    
    


    public static class SchedulePushNotificationsObject
    {
        public static async Task<bool> AddSchedule(SchedulePushNotification n, DateTime UTCNOW, string Category)
        {
            try
            {
                var options = new DbContextOptionsBuilder<UserContext>().UseSqlServer(Push.connectionString).Options;
                using (UserContext _uContext = new UserContext(options))
                {
                    List<PushId> UserDevices = new List<PushId>();
                    // Get devices for user
                    var DeviceList = await _uContext.Apn.Where(t => t.FkUserId == n.UserId && t.IsActive == true).ToListAsync();
                    if (DeviceList == null || DeviceList.Count <= 0)
                    {
                        if (Push.LOGERROR) await Push.LogError("DEVICELIST ERROR", "USER: " + n.UserId);
                        return false;
                    }
                    foreach (var device in DeviceList)
                    {
                        if (device.IsActive.HasValue && device.IsActive == true)
                        {
                            if (device.IsActive == false)
                            {
                                try
                                {
                                    if (device.LastActive < DateTime.UtcNow.AddDays(-8))
                                    {
                                        _uContext.Remove(device);
                                        _uContext.SaveChanges();
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                            PushId pid = new PushId() { DeviceID = device.DeviceId, Platform = device.Platform.Trim(), UserId = n.UserId };
                            UserDevices.Add(pid);
                        }
                    }


                    foreach (var pid in UserDevices)
                    {
                        //
                        var check = await _uContext.ApnSchedule.Where(t => t.ItemId == n.ID && t.DeviceId == pid.DeviceID).FirstOrDefaultAsync();
                        if (check != null) return false;

                        var nmessage = Push.GetPushMessageString(pid.Platform, n.Title, n.Message, "1", Category, n.PARAM1, "1");

                        ApnSchedule schedule = new ApnSchedule()
                        {
                            UserId = n.UserId,
                            CreatedUtc = UTCNOW,
                            DateToTriggerUtc = n.DateUTC,
                            HasSent = false,
                            Message = nmessage,
                            DeviceId = pid.DeviceID,
                            Platform = pid.Platform,
                            ItemId = n.ID,
                            Category = Category
                        };
                        _uContext.ApnSchedule.Add(schedule);

                    }
                    await _uContext.SaveChangesAsync();
                    return true;
                }
            }
            catch(Exception ex)
            {
                if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace);
                return false;
            }
        }

        public static async Task<bool> AddNutrients(SchedulePushNotification n, DateTime UTCNOW)
        {
            return await AddSchedule(n, UTCNOW, Strings.NOTIF_NUTRIENT);
        }

        public static async Task<bool> AddSupplements(SchedulePushNotification n, DateTime UTCNOW)
        {
            return await AddSchedule(n, UTCNOW, Strings.NOTIF_SUPPLEMENT);
        }

        public static async Task<bool> AddTraningSessions(SchedulePushNotification n, DateTime UTCNOW)
        {
            return await AddSchedule(n, UTCNOW, Strings.NOTIF_TRAINING);
        }
    }




    public class SchedulePushNotification
    {
        public long UserId { get; set; }
        public string Title { set; get; }
        public string Message { set;get; }
        public string PARAM1 { get; set; }
        public DateTime DateUTC { get; set; }
        public long ID { get; set; }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
