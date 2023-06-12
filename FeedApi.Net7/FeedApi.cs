using FeedApi.Net7.Models;
using Newtonsoft.Json;
using ParentMiddleWare;
using ParentMiddleWare.Models;
using System.Diagnostics;
using System.Diagnostics.Metrics;
//using static System.Runtime.InteropServices.JavaScript.JSType;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FeedApi.Net7
{
    public class FeedApi : MiddleWare
    {
        public static async Task<List<int>> GetConfig()
        {
            try
            {
                using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Exercise/GetConfig", BaseUrl)))
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrEmpty(apiResponse))
                    {
                        return JsonConvert.DeserializeObject<List<int>>(apiResponse);
                    }
                    List<int> list = new List<int>();
                    list.Add(60);
                    list.Add(60);
                    list.Add(15);
                    list.Add(1);
                    return list;
                }
            }
            catch (Exception ex)
            {
                List<int> list = new List<int>();
                list.Add(60);
                list.Add(60);
                list.Add(15);
                list.Add(1);
                return list;
            }
        }


        public static async Task<long> GetDailyPlanIdAsync(long userId, DateTime date)
        {
            string dateString = string.Format("{0}/{1}/{2} {3}:{4}:{5}", date.Month, date.Day, date.Year, date.Hour, date.Minute, date.Second);
            using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Exercise/GetDailyPlanId?UserId={1}&Dateft={2}", BaseUrl, UserID, dateString)))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(apiResponse))
                {
                    return JsonConvert.DeserializeObject<long>(apiResponse);
                }
                return -1;
            }
        }

        public static async Task GetDailyPlanId(long userId, DateTime date)
        {
            try
            {
               // Trace.TraceError("GetDailyPlanId: " + userId);
                string dateString = string.Format("{0}/{1}/{2} 12:00:00", date.Month, date.Day, date.Year);
                using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Exercise/GetDailyPlanId?UserId={1}&Dateft={2}", BaseUrl, UserID, dateString)))
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var _result = JsonConvert.DeserializeObject<List<EmDailyPlan>>(apiResponse);

                    if (_result != null && _result.Count > 0)
                    {
                        MiddleWare.DailyPlanId = new Dictionary<DateTime, long>();
                        foreach (var item in _result)
                        {
                            try
                            {
                              //  Trace.TraceError("SetPlanid: " + userId);
                                string dString = string.Format("{0}/{1}/{2} 12:00:00", item.StartDay.Value.Month, item.StartDay.Value.Day, item.StartDay.Value.Year);
                                MiddleWare.DailyPlanId.Add(DateTime.Parse(dString, System.Globalization.CultureInfo.InvariantCulture), item.Id);
                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {

            }
        }
    


 

        //public static async Task<List<FeedItem>> GetDailyFeedAsync(int includeDetails = 1)
        //{
        //  //  return await GetDailyFeedAsync(DateTime.UtcNow, includeDetails);
        //}

        public static async Task<List<FeedItem>> GetDailyFeedAsync( DateTime date, int includeDetails = 1)
        {
            DateTime Timestamp1 = DateTime.Now;

            try
            {
                string dString = string.Format("{0}/{1}/{2} 12:00:00", date.Month, date.Day, date.Year);

                long PlanId = -1;
                if (MiddleWare.DailyPlanId.Keys.Contains(DateTime.Parse(dString, System.Globalization.CultureInfo.InvariantCulture)))
                {
                    PlanId = MiddleWare.DailyPlanId[DateTime.Parse(dString, System.Globalization.CultureInfo.InvariantCulture)];
                }
    
                if(PlanId <= 0)
                {
                     await GetDailyPlanId(MiddleWare.UserID, DateTime.Now);
                    if (MiddleWare.DailyPlanId.Keys.Contains(DateTime.Parse(dString, System.Globalization.CultureInfo.InvariantCulture)))
                    {
                        PlanId = MiddleWare.DailyPlanId[DateTime.Parse(dString, System.Globalization.CultureInfo.InvariantCulture)];
                    }
                }
                if (PlanId <= 0)
                {

                    /////////////////////// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                  //  return new List<FeedItem>();
                }
                    List<FeedItem> FeedList = new List<FeedItem>();

                //  string dateString = string.Format("{0}/{1}/{2} 12:00:0 AM", date.Month, date.Day, date.Year);

                //  string dateString = string.Format("{0}/{1}/{2} {3}:{4}:{5}", date.Month, date.Day, date.Year, date.Hour, date.Minute, date.Second);

                DateTime Timestamp11 = DateTime.Now;
                DateTime Timestamp33;
                using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Exercise/GetDailyFeed?PlanId={1}&Dateft={2}&UserId={3}", BaseUrl, PlanId, dString, MiddleWare.UserID)))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                     Timestamp33 = DateTime.Now;
                    if (!String.IsNullOrEmpty(apiResponse))
                    {
                        FeedList = JsonConvert.DeserializeObject<List<FeedItem>>(apiResponse);
                    }
                }
                DateTime Timestamp22 = DateTime.Now;
                if (FeedList == null)
                {
                    FeedList = new List<FeedItem>();
                }

                // manually add Supplement item
                //FeedItem supp = new FeedItem();
                //supp.ItemType = FeedItemType.SupplementItem;
                //supp.SupplementFeedItem = new SupplementFeedItem();
                //supp.SupplementFeedItem.SupplementEntries = new List<SupplementEntry>();
               
                //SupplementEntry supplementEntry = new SupplementEntry();
                //supplementEntry.Supplementname = "Sample Tablet";
                //supplementEntry.SupplementId = -1;
                //supplementEntry.DoseId = -2;
                //supplementEntry.UnitCount = 3;
                //supplementEntry.DoseWarningLimit = 5;
                //supplementEntry.DoseHardLimit = 7;
                //supplementEntry.UnitName = "Tablet";
                //supplementEntry.is_Weight = false;
                //supplementEntry.is_Volume = false;
                //supplementEntry.is_Count  = true;
                //supplementEntry.Instructions = "Drink with whole glass of water";
                //supplementEntry.Requires_source_of_fat = true;
                //supplementEntry.Take_after_meal = true;
                //supplementEntry.Take_before_sleep = false;
                //supplementEntry.Take_on_empty_stomach = false;
                //supplementEntry.ScheduledTime = DateTime.UtcNow.AddHours(DateTimeOffset.Now.Offset.Hours).AddMinutes(20);

                //SupplementEntry supplementEntry2 = new SupplementEntry();
                //supplementEntry.Supplementname = "Sample Powder";
                //supplementEntry2.SupplementId = -10;
                //supplementEntry2.DoseId = -20;
                //supplementEntry2.UnitCount = 1;
                //supplementEntry2.DoseWarningLimit = 2;
                //supplementEntry2.DoseHardLimit = 2;
                //supplementEntry2.UnitName = "mg";
                //supplementEntry2.is_Weight = true;
                //supplementEntry2.is_Volume = false;
                //supplementEntry2.is_Count = false;
                //supplementEntry2.Instructions = "Eat raw";
                //supplementEntry2.Requires_source_of_fat = false;
                //supplementEntry2.Take_after_meal = false;
                //supplementEntry2.Take_before_sleep = false;
                //supplementEntry2.Take_on_empty_stomach = true;
                //supplementEntry2.ScheduledTime = DateTime.UtcNow.AddHours(DateTimeOffset.Now.Offset.Hours).AddMinutes(20);

                //SupplementEntry supplementEntry3 = new SupplementEntry();
                //supplementEntry.Supplementname = "Sample Volume Supplement";
                //supplementEntry3.SupplementId = -100;
                //supplementEntry3.DoseId = -200;
                //supplementEntry3.UnitCount = 5;
                //supplementEntry3.DoseWarningLimit = 10;
                //supplementEntry3.DoseHardLimit = 15;
                //supplementEntry3.UnitName = "scm";
                //supplementEntry3.is_Weight = false;
                //supplementEntry3.is_Volume = true;
                //supplementEntry3.is_Count = false;
                //supplementEntry3.Instructions = "Just look at it";
                //supplementEntry3.Requires_source_of_fat = false;
                //supplementEntry3.Take_after_meal = false;
                //supplementEntry3.Take_before_sleep = true;
                //supplementEntry3.Take_on_empty_stomach = false;
                //supplementEntry3.ScheduledTime = DateTime.UtcNow.AddHours(DateTimeOffset.Now.Offset.Hours).AddMinutes(20);



                //supp.SupplementFeedItem.SupplementEntries.Add(supplementEntry);
                //supp.SupplementFeedItem.SupplementEntries.Add(supplementEntry2);
                //supp.SupplementFeedItem.SupplementEntries.Add(supplementEntry3);

                ////  item.TraningSession = t;
                //supp.Title = "Sample Supplement";
                //supp.Date = DateTime.UtcNow.AddHours(DateTimeOffset.Now.Offset.Hours).AddMinutes(20);
                //supp.Status = FeedItemStatus.Scheduled;
                //supp.Text = new List<TextPair>();
                //supp.Text.Add(new TextPair(TextCategory.Description, "Sample Description"));
                //supp.Text.Add(new TextPair(TextCategory.Supplement_Count, " 2 Before meal, 2 After meal"));

                //FeedList.Add(supp);
                ////


                foreach (var t in FeedList)
                {
                    if (t.Status != FeedItemStatus.Completed)
                    {
                        if (t.Date.Subtract(DateTime.Now).TotalMinutes < - MiddleWare.AutoSkippedTimeout)
                        {
                            t.Status = FeedItemStatus.Skipped;
                            // skip in api
                        }
                    }
                }

                DateTime Timestamp2 = DateTime.Now;
                Trace.TraceError("Millisec for whole call: " + Timestamp2.Subtract(Timestamp1).TotalMilliseconds);
                Trace.TraceError("Millisec for API  call: " + Timestamp22.Subtract(Timestamp11).TotalMilliseconds);
                Trace.TraceError("Millisec for API  call without Serialisation: " + Timestamp33.Subtract(Timestamp11).TotalMilliseconds);
                return FeedList;
            }
            catch (Exception ex)
            {
                Trace.TraceError("GetDailyFeedAsync error: " + ex.Message + ex.StackTrace);
                throw ex;
               // return new List<FeedItem>(); 
            }
        }

    }
}