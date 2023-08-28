using Newtonsoft.Json;
using ParentMiddleWare;
using ParentMiddleWare.ApiModels;
using ParentMiddleWare.Models;
using System.Net.Http.Json;

namespace SupplementApi.Net7
{
    public class SupplementApi : MiddleWare
    {
        public static async Task<bool> TakeDose(long DoseId, float UnitCountActual,  bool isCustomerAdded = false, bool isFreeEntry = false, string FreeEntryName = "")
        {
            DateTime DateTakenUTC = DateTime.UtcNow;
            string dateString = string.Format("{0}/{1}/{2} {3}:{4}:{5}", DateTakenUTC.Month, DateTakenUTC.Day, DateTakenUTC.Year, DateTakenUTC.Hour, DateTakenUTC.Minute, DateTakenUTC.Second);
            //using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Supplement/TakeDose?UserId={1}&DoseId={2}&UnitCountActual={3}", BaseUrl, UserID, DoseId, UnitCountActual), null))
            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/Supplement/TakeDose", BaseUrl), new GeneralApiModel { FkFederatedUser = FkFederatedUser, longparam1 = DoseId, floatparam1 = UnitCountActual }))
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
            //using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Supplement/TakeDoseUndo?DoseId={1}", BaseUrl, DoseId), null))
            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/Supplement/TakeDoseUndo", BaseUrl ), DoseId))
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
            //using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Supplement/SnoozeDose?DoseId={1}&MinutesSnoozed={2}", BaseUrl, DoseId, MinutesSnoozed), null))
            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/Supplement/SnoozeDose", BaseUrl), new GeneralApiModel { longparam1 = DoseId, intparam1 = MinutesSnoozed }))
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
            //using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Supplement/SnoozeDose?DoseId={1}", BaseUrl, DoseId), null))
            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/Supplement/UndoSnooze", BaseUrl), DoseId))
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


        public static async Task<bool> UndoSkipSupplement(long DoseId)
        {
            //using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Supplement/UndoSkipSupplement?DoseId={1}", BaseUrl, DoseId), null))
            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/Supplement/UndoSkipSupplement", BaseUrl), DoseId))
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
        public static async Task<bool> SkipSupplement(long DoseId)
        {
            //using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Supplement/SkipSupplement?DoseId={1}", BaseUrl, DoseId), null))
            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/Supplement/SkipSupplement", BaseUrl), DoseId))
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
            using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Supplement/GetAllSupplments?FkFederatedUser={1}", BaseUrl, FkFederatedUser)))
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