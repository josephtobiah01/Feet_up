using Newtonsoft.Json;
using ParentMiddleWare;
using ParentMiddleWare.Models;

namespace SupplementApi.Net7
{
    public class SupplementApi : MiddleWare
    {
        public static async Task<bool> TakeDose(long DoseId, float UnitCountActual,  bool isCustomerAdded = false, bool isFreeEntry = false, string FreeEntryName = "")
        {
            DateTime DateTakenUTC = DateTime.UtcNow;
            string dateString = string.Format("{0}/{1}/{2} {3}:{4}:{5}", DateTakenUTC.Month, DateTakenUTC.Day, DateTakenUTC.Year, DateTakenUTC.Hour, DateTakenUTC.Minute, DateTakenUTC.Second);
            using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Supplement/TakeDose?DoseId={1}&UnitCountActual={2}", BaseUrl, DoseId, UnitCountActual), null))
            {
                try
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrEmpty(apiResponse))
                    {
                        return JsonConvert.DeserializeObject<bool>(apiResponse);
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                return false;
            }
        }

        public static async Task<bool> TakeDoseUndo(long DoseId)
        {
            DateTime DateTakenUTC = DateTime.UtcNow;
            string dateString = string.Format("{0}/{1}/{2} {3}:{4}:{5}", DateTakenUTC.Month, DateTakenUTC.Day, DateTakenUTC.Year, DateTakenUTC.Hour, DateTakenUTC.Minute, DateTakenUTC.Second);
            using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Supplement/TakeDoseUndo?DoseId={1}", BaseUrl, DoseId), null))
            {
                try
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrEmpty(apiResponse))
                    {
                        return JsonConvert.DeserializeObject<bool>(apiResponse);
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                return false;
            }

        }

        public static async Task<bool> SnoozeDose(long DoseId, int MinutesSnoozed)
        {
            using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Supplement/SnoozeDose?DoseId={1}&MinutesSnoozed={2}", BaseUrl, DoseId, MinutesSnoozed), null))
            {
                try
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrEmpty(apiResponse))
                    {
                        return JsonConvert.DeserializeObject<bool>(apiResponse);
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                return false;
            }
        }

        public static async Task<bool> UndoSnooze(long DoseId)
        {
            using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Supplement/SnoozeDose?DoseId={1}", BaseUrl, DoseId), null))
            {
                try
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrEmpty(apiResponse))
                    {
                        return JsonConvert.DeserializeObject<bool>(apiResponse);
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                return false;
            }
        }


        // new
        //public static async Task<bool> SnoozeSupplement(long SupplementId, int MinutesSnoozed)
        //{
        //    return true;
        //}

        // new
        //public static async Task<bool> UndoSnoozeSupplement(long SupplementId)
        //{
        //    return true;
        //}

        //new 
        public static async Task<bool> UndoSkipSupplement(long DoseId)
        {
            using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Supplement/UndoSkipSupplement?DoseId={1}", BaseUrl, DoseId), null))
            {
                try
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrEmpty(apiResponse))
                    {
                        return JsonConvert.DeserializeObject<bool>(apiResponse);
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                return false;
            }
        }




        public static async Task<List<NdSupplementList>> GetAllSupplments()
        {
            List<NdSupplementList> lst = new List<NdSupplementList>();
            using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Supplement/GetAllSupplments?UserId={1}", BaseUrl, UserID)))
            {
                try
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrEmpty(apiResponse))
                    {
                        return JsonConvert.DeserializeObject<List<NdSupplementList>>(apiResponse);
                    }
                }
                catch (Exception ex)
                {
                    return lst;
                }
                return lst;
            }
        }
    }
}