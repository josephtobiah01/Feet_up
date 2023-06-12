using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace MauiApp1.Threads
{
    public class BackgroundWorker
    {
        //  private readonly SentryMauiOptions _options;

        public BackgroundWorker()
        {
            //    _options = options.Value;
        }

        public async Task DoWorkAsync()
        {
            // Here a completely new scope is created - detached from any current scope.
            // No information is copied over from the current scope.  It is starting clean.
            //  var scope = new Scope(_options);

            try
            {

            //    FeedApi.Net7.FeedApi.DailyPlanId =  await FeedApi.Net7.FeedApi.GetDailyPlanId(FeedApi.Net7.FeedApi.UserID, DateTime.Now);
                // You'll need to add breadcrumbs and other items directly on that scope
                //  scope.AddBreadcrumb("Doing a thing");
                // await Task.Delay(100);
                //  scope.AddBreadcrumb("Doing another thing");
                //  await Task.Delay(100);

                //     throw new Exception("Something went wrong!");
            }
            catch (Exception ex)
            {
                // When you capture events, pass the scope in as a parameter.
                //  SentrySdk.CaptureEvent(new SentryEvent(ex), scope);
            }
        }
    }
}
