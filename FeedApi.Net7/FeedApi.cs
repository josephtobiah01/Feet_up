using FeedApi.Net7.Models;
using Newtonsoft.Json;
using ParentMiddleWare;
using ParentMiddleWare.Models;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Http.Json;
using System.Numerics;
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


        public static async Task<long> GetDailyPlanIdAsync(string FkFederatedUser, DateTime date)
        {
            string dateString = string.Format("{0}/{1}/{2} {3}:{4}:{5}", date.Month, date.Day, date.Year, date.Hour, date.Minute, date.Second);
            using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Exercise/GetDailyPlanId?FkFederatedUser={1}&Dateft={2}", BaseUrl, FkFederatedUser, dateString)))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(apiResponse))
                {
                    return JsonConvert.DeserializeObject<long>(apiResponse);
                }
                return -1;
            }
        }

        public static async Task GetDailyPlanId(string FkFederatedUser, DateTime date)
        {
            try
            {
                //Trace.TraceError("GetDailyPlanId: " + FkFederatedUser);
                string dateString = string.Format("{0}/{1}/{2} 12:00:00", date.Month, date.Day, date.Year);
                using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Exercise/GetDailyPlanId?FkFederatedUser={1}&Dateft={2}", BaseUrl, FkFederatedUser, dateString)))
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
                                //  Trace.TraceError("SetPlanid: " + FkFederatedUser);
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

        public static async Task<List<FeedItem>> GetDailyFeedAsync( DateTime date, int includeDetails = 1)
        {
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
                     await GetDailyPlanId(MiddleWare.FkFederatedUser, DateTime.Now);
                    if (MiddleWare.DailyPlanId.Keys.Contains(DateTime.Parse(dString, System.Globalization.CultureInfo.InvariantCulture)))
                    {
                        PlanId = MiddleWare.DailyPlanId[DateTime.Parse(dString, System.Globalization.CultureInfo.InvariantCulture)];
                    }
                }
                if (PlanId <= 0)
                {

                }

                List<FeedItem> FeedList = new List<FeedItem>();

                using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Exercise/GetDailyFeed?PlanId={1}&Dateft={2}&FkFederatedUser={3}", BaseUrl, PlanId, dString, MiddleWare.FkFederatedUser)))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
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

                foreach (var t in FeedList)
                {
                    if (t.Status != FeedItemStatus.Completed && t.Status != FeedItemStatus.Ongoing)
                    {
                        if (t.Date.Subtract(DateTime.Now).TotalMinutes < - MiddleWare.AutoSkippedTimeout)
                        {
                            t.Status = FeedItemStatus.Skipped;
                            // skip in api
                        }
                    }
                }
                return FeedList;
            }
            catch (Exception ex)
            {
                Trace.TraceError("GetDailyFeedAsync error: " + ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        public static async Task<FeedItem> GetFeedItem(string feedItemId)
        {
            try
            {
                using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Exercise/GetFeedItem?feedItemId={1}", BaseUrl, feedItemId)))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<FeedItem>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}