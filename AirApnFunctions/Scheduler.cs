
using DAOLayer.Net7.User;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace AirApnFunctions
{
    public class Scheduler
    {
        private readonly ILogger _logger;

        public Scheduler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Scheduler>();
        }

        [Function("Schedule")]
        // public async Task Schedule([TimerTrigger("0 */5 * * * *")] MyInfo myTimer)
         public async Task Schedule([TimerTrigger("0 */30 * * * *")] MyInfo myTimer)
      //  public async Task Schedule([TimerTrigger("0 */1 * * * *")] MyInfo myTimer)
        {
            await DoSchedule();
        }


        public static async Task DoSchedule()
        {
            var options = new DbContextOptionsBuilder<UserContext>().UseSqlServer(Push.connectionString).Options;
            try
            {
                DateTime UTCNOW = DateTime.UtcNow;
                using (UserContext _uContext = new UserContext(options))
                {
                    try
                    {
                        foreach (var u in await _uContext.User.Where(t => t.IsActive == true && t.UserLevel < 1000).AsNoTracking().ToListAsync())
                        {
                            //  if (Push.ONLY_TEST_MANUAL) return;
                            if (u.UserLevel <= 0) continue;
                            if (Push.TEST) continue;
                            if (!u.LastKnownTimeOffset.HasValue) continue;
                            try { await Push.DoScanNutrients(u.Id, UTCNOW, u.LastKnownTimeOffset.Value); }
                            catch (Exception ex)
                            {
                                if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace, u.Id);
                            }

                            try { await Push.ScanSupplements(u.Id, UTCNOW, u.LastKnownTimeOffset.Value); }
                            catch (Exception ex)
                            {
                                if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace, u.Id);
                            }

                            try { await Push.ScanTraningSessions(u.Id, UTCNOW, u.LastKnownTimeOffset.Value); }
                            catch (Exception ex)
                            {
                                if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace, u.Id);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                if (Push.LOGERROR) await Push.LogError(ex.Message, ex.StackTrace);
            }
        }

        [Function("TriggerPush")]
        public async Task<HttpResponseData> TriggerPush([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            try
            {

                await DoSchedule();
            }
            catch (Exception ex)
            {
                if (Push.LOGERROR)
                {
                    await Push.LogError(ex.Message, ex.StackTrace);
                    return req.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }

    //public class MyInfo
    //{
    //    public MyScheduleStatus ScheduleStatus { get; set; }

    //    public bool IsPastDue { get; set; }
    //}

    //public class MyScheduleStatus
    //{
    //    public DateTime Last { get; set; }

    //    public DateTime Next { get; set; }

    //    public DateTime LastUpdated { get; set; }
    //}
}
