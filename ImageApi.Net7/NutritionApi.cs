using MauiApp1.Areas.Dashboard.TemporaryStubModel;
using Newtonsoft.Json;
using ParentMiddleWare;
using ParentMiddleWare.Models;
using ParentMiddleWare.NutrientModels;
using System.Text;
using System.Text.Encodings.Web;
using static ParentMiddleWare.Models.NutrientRecipeModel;


namespace ImageApi.Net7
{
    public class NutritionApi : MiddleWare
    {
        public static async Task<NutrientrecipesForMeal> GetFavoritesAndHistory()
        {
            using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Nutrition/GetFavoritesAndHistory?userId={1}", BaseUrl, MiddleWare.UserID)))
            {
                try
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrEmpty(apiResponse))
                    {
                        return JsonConvert.DeserializeObject<NutrientrecipesForMeal>(apiResponse);
                    }
                }
                catch(Exception ex)
                {
                    return null;
                }
                return null;
            }
        }


        public static async Task<DailyNutrientDetails> GetDailyNutrientDetails(DateTime Date)
        {
            string dString = string.Format("{0}/{1}/{2} 12:00:00", Date.Month, Date.Day, Date.Year);
            using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Nutrition/GetDailyNutrientDetails?UserId={1}&Date={2}", BaseUrl, UserID, dString)))
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
            using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Nutrition/GetNutrientsFirstPage?UserId={1}&Date={2}", BaseUrl, UserID, dString)))
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
            using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Nutrition/FavoriteDish?recipeId={1}", BaseUrl, dishId), null))
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
            using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Nutrition/UnFavoriteDish?recipeId={1}", BaseUrl, dishId), null))
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


        public static async Task<bool> AddDishByPhoto(string Name, string Note,  long MealId,  double share, double portions, string ContentType, string ImageData, bool FavoriteDish = false)
        {
            //servings and portions seem swapped in the backend?
            StringContent sc = new StringContent(ImageData, Encoding.UTF8, "application/json");
            using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Nutrition/AddDishByPhoto?Name={1}&MealId={2}&share={3}&portions={4}&ContentType={5}&userId={6}&Note={7}&FavoriteDish={8}&offset={9}", BaseUrl, Name, MealId, share, portions, ContentType, ParentMiddleWare.MiddleWare.UserID, Note, FavoriteDish, TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours), sc))
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
            using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Nutrition/AddDishByPhotoCustomMeal?Dateft={1}&Name={2}&share={3}&portions={4}&ContentType={5}&userId={6}&Note={7}&FavoriteDish={8}&offset={9}", BaseUrl, dString, Name, share, portions, ContentType, ParentMiddleWare.MiddleWare.UserID, Note, FavoriteDish, TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours), sc))
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
            using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Nutrition/AddDishByReference?recipId={1}&MealId={2}&share={3}&portions={4}&userId={5}&FavoriteDish={6}&offset={7}&Note={8}", BaseUrl, recipId, MealId, share, portions, MiddleWare.UserID, FavoriteDish, TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours, note), null))
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

        public static async Task<long> AddDishByReferenceCustomMeal(long recipId, string note,  double share, double portions, bool FavoriteDish = false)
        {
            DateTime date = DateTime.Now;
            string dString = string.Format("{0}/{1}/{2} 12:00:00", date.Month, date.Day, date.Year);
            //servings and portions seem swapped in the backend?
            using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Nutrition/AddDishByReferenceCustomMeal?Dateft={1}&recipId={2}&share={3}&portions={4}&userId={5}&FavoriteDish={6}&offset={7}&Note={8}", BaseUrl, dString, recipId, share, portions, MiddleWare.UserID, FavoriteDish, TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours, note), null))
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
            using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Nutrition/SnoozeMeal?mealId={1}&snoozeTimeMinutes={2}", BaseUrl, mealId, snoozeTimeMinutes), null))
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
            using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Nutrition/SnoozeMealUndo?mealId={1}", BaseUrl, mealId), null))
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
        public static async Task<bool> UndoSkip(long mealId)
        {
            using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Nutrition/UndoSkip?mealId={1}", BaseUrl, mealId), null))
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
        public static async Task<bool> Skip(long mealId)
        {
            return false;
            //using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Nutrition/UndoSkip?mealId={1}", BaseUrl, mealId), null))
            //{
            //    try
            //    {
            //        string apiResponse = await response.Content.ReadAsStringAsync();
            //        if (!String.IsNullOrEmpty(apiResponse))
            //        {
            //            return JsonConvert.DeserializeObject<bool>(apiResponse);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        return false;
            //    }
            //    return false;
            //}
        }
    }
}