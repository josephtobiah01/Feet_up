using Newtonsoft.Json;
using ParentMiddleWare;
using ParentMiddleWare.ApiModels;
using ParentMiddleWare.Models;
using ParentMiddleWare.NutrientModels;
using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using static ParentMiddleWare.Models.NutrientRecipeModel;


namespace ImageApi.Net7
{
    public class NutritionApi : MiddleWare
    {
        public static async Task<NutrientrecipesForMeal> GetFavoritesAndHistory()
        {
            using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Nutrition/GetFavoritesAndHistory?FkFederatedUser={1}", BaseUrl, MiddleWare.FkFederatedUser)))
            {
                try
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrEmpty(apiResponse))
                    {
                        return JsonConvert.DeserializeObject<NutrientrecipesForMeal>(apiResponse);
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return null;
            }
        }


        public static async Task<DailyNutrientDetails> GetDailyNutrientDetails(DateTime Date)
        {
            string dString = string.Format("{0}/{1}/{2} 12:00:00", Date.Month, Date.Day, Date.Year);
            using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Nutrition/GetDailyNutrientDetails?FkFederatedUser={1}&Date={2}", BaseUrl, FkFederatedUser, dString)))
            {
                try
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrEmpty(apiResponse))
                    {
                        return JsonConvert.DeserializeObject<DailyNutrientDetails>(apiResponse);
                    }
                }
                catch (Exception ex)
                {
                    return new DailyNutrientDetails();
                }
                return new DailyNutrientDetails();
            }
        }


        public static async Task<NutrientsDataResponse> GetNutrientsFirstPage(DateTime Date)
        {
            string dString = string.Format("{0}/{1}/{2} 12:00:00", Date.Month, Date.Day, Date.Year);
            using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Nutrition/GetNutrientsFirstPage?FkFederatedUser={1}&Date={2}", BaseUrl, FkFederatedUser, dString)))
            {
                try
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrEmpty(apiResponse))
                    {
                        return JsonConvert.DeserializeObject<NutrientsDataResponse>(apiResponse);
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return null;
            }
        }

        public static async Task<List<NutrientDish>> GetMealDishes(long MealId)
        {
            using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Nutrition/GetMealDishes?mealId={1}", BaseUrl, MealId)))
            {
                try
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrEmpty(apiResponse))
                    {
                        return JsonConvert.DeserializeObject<List<NutrientDish>>(apiResponse);
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return null;
            }
        }

        public static async Task<bool> FavoriteDish(long dishId)
        {
            //using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Nutrition/FavoriteDish?recipeId={1}", BaseUrl, dishId), null))
            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/Nutrition/FavoriteDish", BaseUrl), dishId))
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



        public static async Task<bool> UnFavoriteDish(long dishId)
        {
            //using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Nutrition/UnFavoriteDish?recipeId={1}", BaseUrl, dishId), null))
            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/Nutrition/UnFavoriteDish", BaseUrl), dishId))
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


        public static async Task<bool> AddDishByPhoto(string Name, string Note, long MealId, double share, double portions, string ContentType, string ImageData, bool FavoriteDish = false)
        {
            //servings and portions seem swapped in the backend?
            StringContent sc = new StringContent(ImageData, Encoding.UTF8, "application/json");
            //using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Nutrition/AddDishByPhoto?Name={1}&MealId={2}&share={3}&portions={4}&ContentType={5}&userId={6}&Note={7}&FavoriteDish={8}&offset={9}", BaseUrl, Name, MealId, share, portions, ContentType, ParentMiddleWare.MiddleWare.UserID, Note, FavoriteDish, TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours), sc))
            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/Nutrition/AddDishByPhoto", BaseUrl), new GeneralApiModel { param1 = Name, longparam1 = MealId, doubleparam1 = share, doubleparam2 = portions, param2 = ContentType, FkFederatedUser = ParentMiddleWare.MiddleWare.FkFederatedUser, param3 = Note, boolparam1 = FavoriteDish, doubleparam3 = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours, param4 = ImageData }))
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

        public static async Task<long> AddDishByPhotoCustomMeal(string Name, string Note, double share, double portions, string ContentType, string ImageData, bool FavoriteDish = false)
        {
            //servings and portions seem swapped in the backend?
            StringContent sc = new StringContent(ImageData, Encoding.UTF8, "application/json");
            DateTime date = DateTime.Now;
            string dString = string.Format("{0}/{1}/{2} 12:00:00", date.Month, date.Day, date.Year);
            //using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Nutrition/AddDishByPhotoCustomMeal?Dateft={1}&Name={2}&share={3}&portions={4}&ContentType={5}&userId={6}&Note={7}&FavoriteDish={8}&offset={9}", BaseUrl, dString, Name, share, portions, ContentType, ParentMiddleWare.MiddleWare.UserID, Note, FavoriteDish, TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours), sc))
            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/Nutrition/AddDishByPhotoCustomMeal", BaseUrl), new GeneralApiModel { param1 = dString, param2 = Name, doubleparam1 = share, doubleparam2 = portions, param3 = ContentType, FkFederatedUser = ParentMiddleWare.MiddleWare.FkFederatedUser, param4 = Note, boolparam1 = FavoriteDish, doubleparam3 = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours, param5 = ImageData }))
            {
                try
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrEmpty(apiResponse))
                    {
                        return JsonConvert.DeserializeObject<long>(apiResponse);
                    }
                }
                catch (Exception ex)
                {
                    return -1;
                }
                return -1;
            }
        }

        public static async Task<bool> AddDishByReference(long recipId, string note, long MealId, double share, double portions, bool FavoriteDish = false)
        {
            //servings and portions seem swapped in the backend?
            //using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Nutrition/AddDishByReference?recipId={1}&MealId={2}&share={3}&portions={4}&userId={5}&FavoriteDish={6}&offset={7}&Note={8}", BaseUrl, recipId, MealId, share, portions, MiddleWare.UserID, FavoriteDish, TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours, note), null))
            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/Nutrition/AddDishByReference", BaseUrl), new GeneralApiModel { longparam1 = recipId, longparam2 = MealId, doubleparam1 = share, doubleparam2 = portions, FkFederatedUser = MiddleWare.FkFederatedUser, boolparam1 = FavoriteDish, doubleparam3 = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours, param1 = note }))
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

        public static async Task<long> AddDishByReferenceCustomMeal(long recipId, string note, double share, double portions, bool FavoriteDish = false)
        {
            DateTime date = DateTime.Now;
            string dString = string.Format("{0}/{1}/{2} 12:00:00", date.Month, date.Day, date.Year);
            //servings and portions seem swapped in the backend?
            //using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Nutrition/AddDishByReferenceCustomMeal?Dateft={1}&recipId={2}&share={3}&portions={4}&userId={5}&FavoriteDish={6}&offset={7}&Note={8}", BaseUrl, dString, recipId, share, portions, MiddleWare.UserID, FavoriteDish, TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours, note), null))
            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/Nutrition/AddDishByReferenceCustomMeal", BaseUrl), new GeneralApiModel { param1 = dString, longparam1 = recipId, doubleparam1 = share, doubleparam2 = portions, FkFederatedUser = MiddleWare.FkFederatedUser, boolparam1 = FavoriteDish, doubleparam3 = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours, param2 = note }))
            {
                try
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrEmpty(apiResponse))
                    {
                        return JsonConvert.DeserializeObject<long>(apiResponse);
                    }
                }
                catch (Exception ex)
                {
                    return -1;
                }
                return -1;
            }
        }

        // new
        public static async Task<bool> SnoozeMeal(long mealId, int snoozeTimeMinutes)
        {
            //using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Nutrition/SnoozeMeal?mealId={1}&snoozeTimeMinutes={2}", BaseUrl, mealId, snoozeTimeMinutes), null))
            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/Nutrition/SnoozeMeal}", BaseUrl), new GeneralApiModel { longparam1 = mealId, intparam1 = snoozeTimeMinutes }))
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
        public static async Task<bool> SnoozeMealUndo(long mealId)
        {
            //using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Nutrition/SnoozeMealUndo?mealId={1}", BaseUrl, mealId), null))
            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/Nutrition/SnoozeMealUndo", BaseUrl), mealId))
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

        public static async Task<bool> SkipMeal(long mealId)
        {
            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/Nutrition/SkipMeal", BaseUrl), mealId))
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

        public static async Task<bool> UndoSkip(long mealId)
        {
            //using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Nutrition/UndoSkip?mealId={1}", BaseUrl, mealId), null))
            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/Nutrition/UndoSkip", BaseUrl), mealId))
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
    }
}