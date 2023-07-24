using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using DAOLayer.Net7.Nutrition;
using MauiApp1.Areas.Dashboard.TemporaryStubModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParentMiddleWare.Models;
using ParentMiddleWare.NutrientModels;
using static ParentMiddleWare.Models.NutrientRecipeModel;


namespace FitappApi.Net7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class NutritionController : BaseController
    {
        private readonly NutritionContext _context;
        public NutritionController(NutritionContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Route("GetDailyNutrientDetails")]
        public async Task<DailyNutrientDetails> GetDailyNutrientDetails(long UserId, string Date)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            DailyNutrientDetails model = new DailyNutrientDetails();
            model.DailyNutrientOverview = new NutrientOverview();
            model.MealDetails = new List<MealDetails> { };

            try
            {
                DateTime dDate = DateTime.Parse(Date, System.Globalization.CultureInfo.InvariantCulture).Date;

                var dday = await _context.FnsNutritionActualDay.Where(t => t.FkUserId == UserId && t.Date == dDate)

                    .Include(t => t.FnsNutritionActualMeal)
                    .ThenInclude(t => t.MealType)
                    .Include(t => t.FnsNutritionActualMeal)
                    .ThenInclude(t => t.FnsNutritionActualDish)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (dday == null) return model;

                model.DailyNutrientOverview.TargetCalories = (long)dday.DayCalorieTarget;

                double CURR_CALS = 0;
                double PROT = 0;
                double FAT = 0;
                double CRABS = 0;
                MealDetails CUSTOM_MEAL = new MealDetails();
                MealDetails C_MEAL = new MealDetails();
                bool IS_FIRST_CUSTOM_MEAL = true;

                CUSTOM_MEAL.TargetCalories = 0;
                CUSTOM_MEAL.MealSpecificNutrientOverview = new List<DishDetails>();
                CUSTOM_MEAL.MealName = "Custom";
                CUSTOM_MEAL.IsClickable = true;

                foreach (var meal in dday.FnsNutritionActualMeal)
                {
                    double MEAL_CURR_CALS = 0;
                    double MEAL_PROT = 0;
                    double MEAL_FAT = 0;
                    double MEAL_CRABS = 0;

                    C_MEAL = new MealDetails();
             
                    bool Is_Meal_Clickable = true;
                    if(meal.FnsNutritionActualDish == null || meal.FnsNutritionActualDish.Count <= 0)
                    {
                        Is_Meal_Clickable = false;
                    }
                    bool Has_MEAL = false;
                    C_MEAL.MealName = meal.MealType.Name;
                    C_MEAL.TargetCalories = (long)meal.MealCalorieTarget;
                    C_MEAL.MealSpecificNutrientOverview = new List<DishDetails>();

                    foreach (var dish in meal.FnsNutritionActualDish)
                    {
                        var DISH_C_MEAL = new DishDetails();

                        if (dish.IsComplete)
                        {
                            MEAL_CURR_CALS += dish.CalorieActual.HasValue ? dish.CalorieActual.Value : 0;
                            MEAL_PROT += dish.ProteinActual.HasValue ? dish.ProteinActual.Value : 0;
                            MEAL_FAT += dish.FatActual.HasValue ? dish.FatActual.Value : 0;
                            MEAL_CRABS += dish.CrabsActual.HasValue ? dish.CrabsActual.Value : 0;

                            CURR_CALS += dish.CalorieActual.HasValue ? dish.CalorieActual.Value : 0;
                            PROT += dish.ProteinActual.HasValue ? dish.ProteinActual.Value : 0;
                            CRABS += dish.CrabsActual.HasValue ? dish.CrabsActual.Value : 0;
                            FAT += dish.FatActual.HasValue ? dish.FatActual.Value : 0;
                            Has_MEAL = true;

                            DISH_C_MEAL.ImageUrl = dish.UploadPhotoReference;
                            DISH_C_MEAL.Name = dish.Name;
                            DISH_C_MEAL.Active = true;
                            DISH_C_MEAL.Carb += dish.CrabsActual.HasValue ? dish.CrabsActual.Value : 0;
                            DISH_C_MEAL.Sugar += dish.SugarActual.HasValue ? dish.SugarActual.Value : 0;
                            DISH_C_MEAL.Fibre += dish.FiberGramsActual.HasValue ? dish.FiberGramsActual.Value : 0;
                            DISH_C_MEAL.Protein += dish.ProteinActual.HasValue ? dish.ProteinActual.Value : 0;
                            DISH_C_MEAL.Fat += dish.FatActual.HasValue ? dish.FatActual.Value : 0;
                            DISH_C_MEAL.Calories += dish.CalorieActual.HasValue ? dish.CalorieActual.Value : 0;
                            DISH_C_MEAL.SaturatedFat += dish.SaturatedFatGramsActual.HasValue ? dish.SaturatedFatGramsActual.Value : 0;
                            DISH_C_MEAL.UnsaturatedFat += dish.UnsaturatedFatActual.HasValue ? dish.UnsaturatedFatActual.Value : 0;
                            DISH_C_MEAL.Servings = dish.NumberOfServingsConsumed;
                        }
                        else
                        {
                            DISH_C_MEAL.Active = false;
                            Is_Meal_Clickable = false;
                        }
                        if (meal.MealTypeId == 4)
                        {
                            CUSTOM_MEAL.MealSpecificNutrientOverview.Add(DISH_C_MEAL);
                            CUSTOM_MEAL.CurrentCalories += (long)MEAL_CURR_CALS;
                            CUSTOM_MEAL.ProteinGram += (long)MEAL_PROT;
                            CUSTOM_MEAL.FatGram += (long)MEAL_FAT;
                            CUSTOM_MEAL.CarbsGram += (long)MEAL_CRABS;
                        }
                        else
                        {
                            C_MEAL.MealSpecificNutrientOverview.Add(DISH_C_MEAL);
                        }
                    }
                    C_MEAL.IsClickable = Is_Meal_Clickable;

                    C_MEAL.CurrentCalories = (long)MEAL_CURR_CALS;
                    C_MEAL.ProteinGram = (long)MEAL_PROT;
                    C_MEAL.FatGram = (long)MEAL_FAT;
                    C_MEAL.CarbsGram = (long)MEAL_CRABS;

                    if (meal.MealTypeId == 4)
                    {
                        if (!Is_Meal_Clickable)
                        {
                            CUSTOM_MEAL.IsClickable = false;
                        }
                    }
                    else
                    {
                        model.MealDetails.Add(C_MEAL);
                    }     
                }


                if(CUSTOM_MEAL.MealSpecificNutrientOverview.Count > 0)
                {
                    model.MealDetails.Add(CUSTOM_MEAL);
                }

                model.DailyNutrientOverview.CurrentCalories = (long)CURR_CALS;

                model.DailyNutrientOverview.ProteinGram = (long)PROT;
                model.DailyNutrientOverview.CrabsGram = (long)CRABS;
                model.DailyNutrientOverview.FatGram = (long)FAT;

                return model;
            }
            catch (Exception ex)
            {
                return model;
            }
        }

        [HttpGet]
        [Route("GetNutrientsFirstPage")]
        public async Task<NutrientsDataResponse> GetNutrientsFirstPage(long UserId, string Date)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            NutrientsDataResponse model = new NutrientsDataResponse();
            model.TotalNutrientsModel = new List<TotalNutrients>();
            model.ProteinModel = new List<Protein>();
            model.CarbohydratesModel = new List<Carbohydrates>();
            model.FatModel = new List<Fat>();

            try
            {
                DateTime dDate = DateTime.Parse(Date, System.Globalization.CultureInfo.InvariantCulture).Date;

                var dday = await _context.FnsNutritionActualDay.Where(t => t.FkUserId == UserId && t.Date == dDate)

                    .Include(t => t.FnsNutritionActualMeal)
                    .ThenInclude(t => t.FnsNutritionActualDish)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (dday == null) return model;


                model.TargetFat = dday.UnsaturatedFatGramsTarget + dday.SaturatedFatGramsTarget + dday.FatGramsTarget;
                model.TargetCarbohydrates = dday.CrabsGramsTarget;
                model.TargetProtein = dday.ProteinGramsTarget;
                model.AvgTargetCalories = dday.DayCalorieTarget;


                // setup vars
                double AvgCurrentCalories = 0;
                // double AvgTargetCalories;
                double AverageProteinIntake = 0;
                double AverageCarbsIntake = 0;
                double AverageFatIntake = 0;
                double AverageSugarIntake = 0;

                foreach (var meal in dday.FnsNutritionActualMeal)
                {
                    foreach (var dish in meal.FnsNutritionActualDish)
                    {
                        if (dish.IsComplete)
                        {
                            AvgCurrentCalories += dish.CalorieActual.HasValue ? dish.CalorieActual.Value : 0;
                            AverageProteinIntake += dish.ProteinActual.HasValue ? dish.ProteinActual.Value : 0;
                            AverageCarbsIntake += dish.CrabsActual.HasValue ? dish.CrabsActual.Value : 0;
                            AverageSugarIntake += dish.SugarActual.HasValue ? dish.SugarActual.Value : 0;
                            AverageFatIntake += dish.FatActual.HasValue ? dish.FatActual.Value : 0;
                        }
                    }
                }

                model.ProteinModel.Add(new Protein() { ProteinIntakeCount = AverageProteinIntake, TransactionDate = dDate });
                model.FatModel.Add(new Fat() { FatIntakeCount = AverageProteinIntake, TransactionDate = dDate });
                model.CarbohydratesModel.Add(new Carbohydrates() { CarbsIntakeCount = AverageProteinIntake, TransactionDate = dDate });

                model.TotalNutrientsModel.Add(new TotalNutrients()
                {
                    TotalCarbs = AverageCarbsIntake,
                    AverageSugarIntake = AverageSugarIntake,
                    CaloriesTranscribedTotal = (int)AvgCurrentCalories,
                    CurrentCalories = AvgCurrentCalories,
                    TotalFat = AverageFatIntake,
                    TotalProtein = AverageProteinIntake
                });

                return model;
            }
            catch (Exception ex)
            {
                return model;
            }
        }


        [HttpGet]
        [Route("GetFavoritesAndHistory")]
        public async Task<NutrientrecipesForMeal> GetFavoritesAndHistory(long userId)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            NutrientrecipesForMeal mealList = new NutrientrecipesForMeal();

            var nday = await _context.FnsNutritionActualDish.Where(t => t.FkUserId == userId)
                .AsNoTracking()
                .OrderByDescending(t => t.Id)
                .Take(15)
                .ToListAsync();

            var fnday = await _context.FnsNutritionActualDish.Where(t => t.FkUserId == userId && t.IsFavorite)
                 .AsNoTracking()
                 .ToListAsync();

            foreach (var k in nday)
            {
                if (!k.IsComplete) continue;

                NutrientRecipeModel meal = new NutrientRecipeModel();
                meal.NutrientInformation = new RecipeNutrientInformation();
                meal.RecipeName = k.Name;
                meal.DisplayImageUrl = k.UploadPhotoReference;
                meal.RecipeID = k.Id;
                meal.NutrientInformation.Calories = k.CalorieActual;
                meal.NutrientInformation.Fiber = k.FiberGramsActual;
                meal.NutrientInformation.Fat = k.FatActual;
                meal.NutrientInformation.Carbohydrates = k.CrabsActual;
                meal.NutrientInformation.Protein = k.ProteinActual;
                meal.NutrientInformation.Sugar = k.SugarActual;
                meal.NutrientInformation.SaturatedFat = k.SaturatedFatGramsActual;
                meal.NutrientInformation.UnsaturatedFat = k.UnsaturatedFatActual;
                meal.IsFavorite = k.IsFavorite;
                
                mealList.History.Add(meal);
            }


            foreach (var k in fnday)
            {
                if (!k.IsComplete) continue;

                NutrientRecipeModel meal = new NutrientRecipeModel();
                meal.NutrientInformation = new RecipeNutrientInformation();
                meal.RecipeName = k.Name;
                meal.DisplayImageUrl = k.UploadPhotoReference;
                meal.RecipeID = k.Id;
                meal.NutrientInformation.Calories = k.CalorieActual;
                meal.NutrientInformation.Fiber = k.FiberGramsActual;
                meal.NutrientInformation.Fat = k.FatActual;
                meal.NutrientInformation.Carbohydrates = k.CrabsActual;
                meal.NutrientInformation.Protein = k.ProteinActual;
                meal.NutrientInformation.Sugar = k.SugarActual;
                meal.NutrientInformation.SaturatedFat = k.SaturatedFatGramsActual;
                meal.NutrientInformation.UnsaturatedFat = k.UnsaturatedFatActual;
                meal.IsFavorite = k.IsFavorite;

                if (k.IsFavorite)
                {
                    mealList.Favorite.Add(meal);
                }
            }
            return mealList;
        }


        [HttpGet]
        [Route("GetMealDishes")]
        public async Task<List<NutrientDish>> GetMealDishes(long mealId)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            List<NutrientDish> mealList = new List<NutrientDish>();

            var nday = await _context.FnsNutritionActualMeal.Where(t => t.Id == mealId)
                .Include(t => t.FnsNutritionActualDish)
                .AsNoTracking()
                .FirstAsync();

            foreach (var k in nday.FnsNutritionActualDish)
            {
                //   if (!k.IsComplete) continue;
                NutrientDish ndish = new NutrientDish();
                NutrientRecipeModel dish = new NutrientRecipeModel();
                dish.NutrientInformation = new RecipeNutrientInformation();
                dish.RecipeName = k.Name;
                dish.DisplayImageUrl = k.UploadPhotoReference;
                dish.RecipeID = k.Id;
                dish.NutrientInformation.Calories = k.CalorieActual * (k.ShareOfDishConsumed * k.NumberOfServingsConsumed);
                dish.NutrientInformation.Fiber = k.FiberGramsActual * (k.ShareOfDishConsumed * k.NumberOfServingsConsumed);
                dish.NutrientInformation.Fat = k.FatActual * (k.ShareOfDishConsumed * k.NumberOfServingsConsumed);
                dish.NutrientInformation.Carbohydrates = k.CrabsActual * (k.ShareOfDishConsumed * k.NumberOfServingsConsumed);
                dish.NutrientInformation.Protein = k.ProteinActual * (k.ShareOfDishConsumed * k.NumberOfServingsConsumed);
                dish.NutrientInformation.Sugar = k.SugarActual * (k.ShareOfDishConsumed * k.NumberOfServingsConsumed);
                dish.NutrientInformation.SaturatedFat = k.SaturatedFatGramsActual * (k.ShareOfDishConsumed * k.NumberOfServingsConsumed);
                dish.NutrientInformation.UnsaturatedFat = k.UnsaturatedFatActual * (k.ShareOfDishConsumed * k.NumberOfServingsConsumed);

                ndish.NumberOfServings = Convert.ToInt64(k.NumberOfServingsConsumed);
                ndish.PercentageEaten = k.ShareOfDishConsumed;
                ndish.Notes = k.Remarks;
                ndish.isFavorite = k.IsFavorite;
                ndish.Recipe = dish;

                mealList.Add(ndish);
            }

            return mealList;
        }

        [HttpPost]
        [Route("FavoriteDish")]
        public async Task<bool> FavoriteDish(long recipeId)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                var dish = await _context.FnsNutritionActualDish.Where(t => t.Id == recipeId).FirstAsync();
                dish.IsFavorite = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpPost]
        [Route("UnFavoriteDish")]
        public async Task<bool> UnFavoriteDish(long recipeId)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                var dish = await _context.FnsNutritionActualDish.Where(t => t.Id == recipeId).FirstAsync();
                dish.IsFavorite = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task UploadFromBinaryDataAsync(
                                  BlobContainerClient containerClient,
                                  string strm, string fileName)
        {

            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            byte[] buffer = Convert.FromBase64String(strm);
            BinaryData binaryData = new BinaryData(buffer);
            await blobClient.UploadAsync(binaryData, true);
        }

        [HttpPost]
        [Route("AddDishByPhoto")]
        public async Task<bool> AddDishByPhoto([FromQuery] string Name, [FromQuery] long MealId, [FromQuery] double share, [FromQuery] double portions, [FromQuery] string ContentType, [FromQuery] long userId, [FromQuery] bool FavoriteDish, [FromQuery] double offset, [FromQuery] string Note = "")
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                //Request.Body.Position = 0;

                var rawRequestBody = await new StreamReader(Request.Body).ReadToEndAsync();

                QueueClient queue = new QueueClient(ParentMiddleWare.MiddleWare.QueueConnectionString, "airusercontent");

                BlobServiceClient blobServiceClient = new BlobServiceClient(ParentMiddleWare.MiddleWare.QueueConnectionString);
                BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("dishes");
                await blobContainerClient.CreateIfNotExistsAsync();
                BlobClient blobClent = blobContainerClient.GetBlobClient(Guid.NewGuid().ToString() + ContentType);
                BlobHttpHeaders httpheaders = new BlobHttpHeaders()
                {
                    ContentType = ContentType
                };

                BinaryData data = new BinaryData(Convert.FromBase64String(rawRequestBody));
                await blobClent.UploadAsync(data);


                FnsNutritionActualDish dish = new FnsNutritionActualDish();
                dish.UploadPhotoReference = blobClent.Uri.ToString();
                dish.FkNutritionActualMealId = MealId;
                dish.FkDishTranscriptionTypeId = 1;
                dish.IsComplete = false;
                dish.IsError = false;
                dish.IsFavorite = false;
                dish.IsFrequent = false;
                dish.CreationTimestamp = DateTime.UtcNow;
                dish.Name = Name;
                dish.ShareOfDishConsumed = share;
                dish.NumberOfServingsConsumed = portions;
                dish.FkDishTypeId = 7;
                dish.FkUserId = userId;
                dish.Remarks = Note;

                if (FavoriteDish)
                {
                    dish.IsFavorite = true;
                }

                _context.Add(dish);
                await _context.SaveChangesAsync();

                if (null != await queue.CreateIfNotExistsAsync())
                {
                    //     Console.WriteLine("The queue was created.");
                }
                await queue.SendMessageAsync(dish.Id.ToString(), TimeSpan.FromSeconds(0), default);

                var meal = await _context.FnsNutritionActualMeal.Where(t => t.Id == MealId).FirstOrDefaultAsync();
                if (meal != null)
                {
                    meal.IsComplete = false;
                    meal.IsSkipped = false;
                    meal.IsOngoing = true;
                    meal.Timestamp = DateTime.UtcNow.AddHours(offset);
                }
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        [HttpPost]
        [Route("AddDishByPhotoCustomMeal")]
        public async Task<long> AddDishByPhotoCustomMeal([FromQuery] string Dateft, [FromQuery] string Name, [FromQuery] double share, [FromQuery] double portions, [FromQuery] string ContentType, [FromQuery] long userId, [FromQuery] bool FavoriteDish, [FromQuery] double offset, [FromQuery] string Note = "")
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                DateTime Date = DateTime.Parse(Dateft, System.Globalization.CultureInfo.InvariantCulture).Date;

                var nday = await _context.FnsNutritionActualDay.Where(t => t.FkUserId == userId && t.Date == Date)
                    .FirstOrDefaultAsync();

                // create new meal and attach it to that day

                var newMeal = new FnsNutritionActualMeal()
                {
                    ProteinGramsTarget = 0,
                    SugarGramsTarget = 0,
                    UnsaturatedFatGramsTarget = 0,
                    AlcoholGramsTarget = 0,
                    CrabsGramsTarget = 0,
                    FatGramsTarget = 0,
                    FiberGramsTarget = 0,
                    HasTarget = false,
                    IsComplete = false,
                    IsOngoing = false,
                    IsSkipped = false,
                    IsSnoozed = false,
                    MealCalorieMax = 0,
                    MealCalorieMin = 0,
                    MealCalorieTarget = 0,
                    MealTypeId = 4,
                    SnoozedTime = null,
                    IsDeleted = false,
                };

                // no meal yet, create one
                if (nday == null)
                {
                    nday = new FnsNutritionActualDay()
                    {
                        Date = Date,
                        AlcoholGramsTarget = 0,
                        CrabsGramsTarget = 0,
                        DayCalorieTarget = 0,
                        DayCalorieTargetMax = 0,
                        DayCalorieTargetMin = 0,
                        FatGramsTarget = 0,
                        FiberGramsTarget = 0,
                        FkUserId = userId,
                        ProteinGramsTarget = 0,
                        SugarGramsTarget = 0,
                        UnsaturatedFatGramsTarget = 0,
                        FnsNutritionActualMeal = new List<FnsNutritionActualMeal>()
                    };
                    _context.FnsNutritionActualDay.Add(nday);
                }
                if (nday.FnsNutritionActualMeal == null)
                {
                    nday.FnsNutritionActualMeal = new List<FnsNutritionActualMeal>();
                }
                nday.FnsNutritionActualMeal.Add(newMeal);
                newMeal.FkNutritionActualDay = nday;

                var rawRequestBody = await new StreamReader(Request.Body).ReadToEndAsync();

                QueueClient queue = new QueueClient(ParentMiddleWare.MiddleWare.QueueConnectionString, "airusercontent");

                BlobServiceClient blobServiceClient = new BlobServiceClient(ParentMiddleWare.MiddleWare.QueueConnectionString);
                BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("dishes");
                await blobContainerClient.CreateIfNotExistsAsync();
                BlobClient blobClent = blobContainerClient.GetBlobClient(Guid.NewGuid().ToString() + ContentType);
                BlobHttpHeaders httpheaders = new BlobHttpHeaders()
                {
                    ContentType = ContentType
                };

                BinaryData data = new BinaryData(Convert.FromBase64String(rawRequestBody));
                await blobClent.UploadAsync(data);


                FnsNutritionActualDish dish = new FnsNutritionActualDish();
                dish.UploadPhotoReference = blobClent.Uri.ToString();
                // dish.FkNutritionActualMealId = MealId;
                dish.FkNutritionActualMeal = newMeal;
                dish.FkDishTranscriptionTypeId = 1;
                dish.IsComplete = false;
                dish.IsError = false;
                dish.IsFavorite = false;
                dish.IsFrequent = false;
                dish.CreationTimestamp = DateTime.UtcNow;
                dish.Name = Name;
                dish.ShareOfDishConsumed = share;
                dish.NumberOfServingsConsumed = portions;
                dish.FkDishTypeId = 9;
                dish.FkUserId = userId;
                dish.Remarks = Note;

                if (FavoriteDish)
                {
                    dish.IsFavorite = true;
                }
                newMeal.FnsNutritionActualDish.Add(dish);
                // _context.Add(dish);
                await _context.SaveChangesAsync();
                var ll = _context.Update(nday);


                if (null != await queue.CreateIfNotExistsAsync())
                {
                    //     Console.WriteLine("The queue was created.");
                }
                await queue.SendMessageAsync(dish.Id.ToString(), TimeSpan.FromSeconds(0), default);

                var meal = await _context.FnsNutritionActualMeal.Where(t => t.Id == newMeal.Id).FirstOrDefaultAsync();
                if (null != meal)
                {
                    meal.IsComplete = false;
                    meal.IsSkipped = false;
                    meal.IsOngoing = true;
                    meal.Timestamp = DateTime.UtcNow.AddHours(offset);
                    meal.ScheduledTime = null;
                }
                await _context.SaveChangesAsync();


                return meal.Id;
            }
            catch (Exception)
            {
                return -1;
            }
        }


        [HttpPost]
        [Route("NotifyTranscriptionComplete")]
        public async Task<bool> NotifyTranscriptionComplete(long MealId)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                #region  APN
                HttpClient _httpClient = new HttpClient();
                _httpClient.PostAsync(string.Format("{0}/api/SendTranscriptionComplete?MealId={1}", ChatController.AzureFunctionURL, MealId), null);
                #endregion
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        [Route("AddDishByReference")]
        public async Task<bool> AddDishByReference(long MealId, long recipId, double share, double portions, long userId, bool FavoriteDish, double offset, string Note = "")
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                var dayMeal = await _context.FnsNutritionActualMeal.Where(t => t.Id == MealId)
                .Include(t => t.FnsNutritionActualDish)
                .FirstAsync();

                var refdish = await _context.FnsNutritionActualDish.Where(t => t.Id == recipId)
                    .FirstAsync();
                var dish = new FnsNutritionActualDish();

                dish.CreationTimestamp = DateTime.UtcNow;
                dish.ShareOfDishConsumed = share;
                dish.NumberOfServingsConsumed = portions;
                dish.FkReuseReferenceId = refdish.Id;
                dish.CompletionTimestamp = DateTime.UtcNow;
                dish.Name = refdish.Name;
                dish.Remarks = refdish.Remarks;
                dish.AlcoholGramsActual = refdish.AlcoholGramsActual;
                dish.UploadPhotoReference = refdish.UploadPhotoReference;
                dish.CalorieActual = refdish.CalorieActual;
                dish.CrabsActual = refdish.CrabsActual;
                dish.FatActual = refdish.FatActual;
                dish.FiberGramsActual = refdish.FiberGramsActual;
                dish.FkDishTypeId = refdish.FkDishTypeId;
                dish.FkTranscriber = refdish.FkTranscriber;
                dish.Remarks = refdish.Remarks;
                dish.TranscriberRemarks = refdish.TranscriberRemarks;
                dish.ProteinActual = refdish.ProteinActual;
                dish.SugarActual = refdish.SugarActual;
                dish.UnsaturatedFatActual = refdish.UnsaturatedFatActual;
                dish.FkUserId = userId;
                dish.FkDishTranscriptionTypeId = 2;
                dish.SaturatedFatGramsActual = refdish.SaturatedFatGramsActual;
                dish.Remarks = Note;
                dish.IsComplete = true;

                if (FavoriteDish)
                {
                    dish.IsFavorite = true;
                }

                dayMeal.FnsNutritionActualDish.Add(dish);
                await _context.SaveChangesAsync();

                var meal = await _context.FnsNutritionActualMeal.Where(t => t.Id == MealId).FirstOrDefaultAsync();
                if (meal != null)
                {
                    meal.IsComplete = true;
                    meal.IsOngoing = true;
                    foreach (var d in meal.FnsNutritionActualDish)
                    {
                        if(! d.IsComplete)
                        {
                            meal.IsComplete = false;
                            meal.IsOngoing = true;
                        }
                    }
                    meal.IsSkipped = false;
                    meal.Timestamp = DateTime.UtcNow.AddHours(offset);
                }
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpPost]
        [Route("AddDishByReferenceCustomMeal")]
        public async Task<long> AddDishByReferenceCustomMeal(string Dateft, long recipId, double share, double portions, long userId, bool FavoriteDish, double offset, string Note = "")
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            try
            {
                DateTime Date = DateTime.Parse(Dateft, System.Globalization.CultureInfo.InvariantCulture).Date;
                // check if that day already has meals
              //  DateTime Date = DateTime.UtcNow.Date;
                var dayMeal = await _context.FnsNutritionActualDay.Where(t => t.FkUserId == userId && t.Date == Date)
                    .FirstOrDefaultAsync();

                // create new meal and attach it to that day

                var newMeal = new FnsNutritionActualMeal()
                {
                    ProteinGramsTarget = 0,
                    SugarGramsTarget = 0,
                    UnsaturatedFatGramsTarget = 0,
                    AlcoholGramsTarget = 0,
                    CrabsGramsTarget = 0,
                    FatGramsTarget = 0,
                    FiberGramsTarget = 0,
                    HasTarget = false,
                    IsComplete = false,
                    IsOngoing = false,
                    IsSkipped = false,
                    IsSnoozed = false,
                    MealCalorieMax = 0,
                    MealCalorieMin = 0,
                    MealCalorieTarget = 0,
                    MealTypeId = 4,
                    SnoozedTime = null,
                    IsDeleted = false,
                };

                // no meal yet, create one
                if (dayMeal == null)
                {
                    dayMeal = new FnsNutritionActualDay()
                    {
                        Date = Date,
                        AlcoholGramsTarget = 0,
                        CrabsGramsTarget = 0,
                        DayCalorieTarget = 0,
                        DayCalorieTargetMax = 0,
                        DayCalorieTargetMin = 0,
                        FatGramsTarget = 0,
                        FiberGramsTarget = 0,
                        FkUserId = userId,
                        ProteinGramsTarget = 0,
                        SugarGramsTarget = 0,
                        UnsaturatedFatGramsTarget = 0,
                        FnsNutritionActualMeal = new List<FnsNutritionActualMeal>()
                    };
                    _context.FnsNutritionActualDay.Add(dayMeal);
                }
                if (dayMeal.FnsNutritionActualMeal == null)
                {
                    dayMeal.FnsNutritionActualMeal = new List<FnsNutritionActualMeal>();
                }
                dayMeal.FnsNutritionActualMeal.Add(newMeal);

                var refdish = await _context.FnsNutritionActualDish.Where(t => t.Id == recipId)
                    .FirstAsync();
                var dish = new FnsNutritionActualDish();

                dish.CreationTimestamp = DateTime.UtcNow;
                dish.ShareOfDishConsumed = share;
                dish.NumberOfServingsConsumed = portions;
                dish.FkReuseReferenceId = refdish.Id;
                dish.CompletionTimestamp = DateTime.UtcNow;
                dish.Name = refdish.Name;
                dish.Remarks = refdish.Remarks;
                dish.AlcoholGramsActual = refdish.AlcoholGramsActual;
                dish.UploadPhotoReference = refdish.UploadPhotoReference;
                dish.CalorieActual = refdish.CalorieActual;
                dish.CrabsActual = refdish.CrabsActual;
                dish.FatActual = refdish.FatActual;
                dish.FiberGramsActual = refdish.FiberGramsActual;
                dish.FkDishTypeId = refdish.FkDishTypeId;
                dish.FkTranscriber = refdish.FkTranscriber;
                dish.Remarks = refdish.Remarks;
                dish.TranscriberRemarks = refdish.TranscriberRemarks;
                dish.ProteinActual = refdish.ProteinActual;
                dish.SugarActual = refdish.SugarActual;
                dish.UnsaturatedFatActual = refdish.UnsaturatedFatActual;
                dish.SaturatedFatGramsActual = refdish.SaturatedFatGramsActual;
                dish.FkUserId = userId;
                dish.FkDishTranscriptionTypeId = 2;
                dish.Remarks = Note;
                dish.IsComplete = true;

                if (FavoriteDish)
                {
                    dish.IsFavorite = true;
                }

                newMeal.FnsNutritionActualDish.Add(dish);
                await _context.SaveChangesAsync();

                var meal = await _context.FnsNutritionActualMeal.Where(t => t.Id == newMeal.Id).FirstOrDefaultAsync();
                if (meal != null)
                {
                    meal.IsComplete = true;
                    meal.IsSkipped = false;
                    meal.IsOngoing = false;

                    meal.Timestamp = DateTime.UtcNow.AddHours(offset);
                    meal.ScheduledTime = null;
                }
                await _context.SaveChangesAsync();

                return newMeal.Id;
            }
            catch (Exception)
            {
                return -1;
            }
        }


        [HttpPost]
        [Route("SnoozeMeal")]
        public async Task<bool> SnoozeMeal(long MealId, int snoozeTimeMinutes)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var meal = await _context.FnsNutritionActualMeal
                 .Include(t => t.FkNutritionActualDay)
                 .Include(t => t.MealType)
             .Where(t => t.Id == MealId).FirstOrDefaultAsync();

            if (meal == null) return false;
            meal.IsSnoozed = true;
            if (meal.SnoozedTime.HasValue)
            {
                meal.SnoozedTime = meal.SnoozedTime.Value.AddMinutes(snoozeTimeMinutes);
            }
            else
            {
                meal.SnoozedTime = ExerciseController.GetDateFromMealName(meal, meal.FkNutritionActualDay).AddMinutes(snoozeTimeMinutes);
            }
            await _context.SaveChangesAsync();
            return true;
        }


        [HttpPost]
        [Route("SnoozeMealUndo")]
        public async Task<bool> SnoozeMealUndo(long MealId)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var meal = await _context.FnsNutritionActualMeal
            .Where(t => t.Id == MealId).FirstOrDefaultAsync();

            if (meal == null) return false;
            meal.IsSnoozed = false;
            meal.SnoozedTime = null;
            await _context.SaveChangesAsync();
            return true;
        }


        [HttpPost]
        [Route("UndoSkip")]
        public async Task<bool> UndoSkip(long MealId)
        {
            if (!CheckAuth()) throw new Exception("Unauthorized");
            var meal = await _context.FnsNutritionActualMeal
            .Where(t => t.Id == MealId).FirstOrDefaultAsync();

            if (meal == null) return false;
            meal.IsSkipped = false;
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
