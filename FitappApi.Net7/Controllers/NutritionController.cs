using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using DAOLayer.Net7.Nutrition;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParentMiddleWare.Models;
using UserApi.Net7.Models;
using static ParentMiddleWare.Models.NutrientRecipeModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FitappApi.Net7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class NutritionController : ControllerBase
    {
        private readonly NutritionContext _context;
        public NutritionController(NutritionContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Route("GetFavoritesAndHistory")]
        public async Task<NutrientrecipesForMeal> GetFavoritesAndHistory(long userId)
        {
            NutrientrecipesForMeal mealList = new NutrientrecipesForMeal();
            
            var nday = await _context.FnsNutritionActualDish.Where(t => t.FkUserId == userId)
                .AsNoTracking()
                .OrderByDescending(t =>t.Id)
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
                meal.IsFavorite = k.IsFavorite;

                //if (k.IsFavorite)
                //{
                //    mealList.Favorite.Add(meal);
                //}

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
            List<NutrientDish> mealList = new List<NutrientDish>();

            var nday = await _context.FnsNutritionActualMeal.Where(t => t.Id == mealId)
                .Include(t=>t.FnsNutritionActualDish)
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
                dish.NutrientInformation.Protein = k.ProteinActual* (k.ShareOfDishConsumed * k.NumberOfServingsConsumed);


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
            try
            {
                var dish = await _context.FnsNutritionActualDish.Where(t => t.Id == recipeId).FirstAsync();
                dish.IsFavorite = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
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
        public async Task<bool> AddDishByPhoto([FromQuery] string Name,  [FromQuery] long MealId, [FromQuery] double share, [FromQuery] double portions, [FromQuery] string ContentType, [FromQuery] long userId, [FromQuery] bool FavoriteDish, [FromQuery] double offset, [FromQuery] string Note = "")
        {
            try
            {
                //Request.Body.Position = 0;

                var rawRequestBody = await new StreamReader(Request.Body).ReadToEndAsync();

                QueueClient queue = new QueueClient(ParentMiddleWare.MiddleWare.QueueConnectionString, "airusercontent");
           
                BlobServiceClient blobServiceClient = new BlobServiceClient(ParentMiddleWare.MiddleWare.QueueConnectionString);
                BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("dishes");
                await blobContainerClient.CreateIfNotExistsAsync();
                BlobClient blobClent = blobContainerClient.GetBlobClient(Guid.NewGuid().ToString()  + ContentType);
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
                dish.NumberOfServingsConsumed =  portions;
                dish.FkDishTypeId = 7;
                dish.FkUserId = userId;
                dish.Remarks = Note;

                if(FavoriteDish)
                {
                    dish.IsFavorite = true;
                }
            
                _context.Add(dish);
                await _context.SaveChangesAsync();

                if (null != await queue.CreateIfNotExistsAsync())
                {
               //     Console.WriteLine("The queue was created.");
                }
                await  queue.SendMessageAsync(dish.Id.ToString(), TimeSpan.FromSeconds(0), default);

                var meal = await _context.FnsNutritionActualMeal.Where(t => t.Id == MealId).FirstOrDefaultAsync();
                meal.IsComplete = false;
                meal.IsSkipped = false;
                meal.IsOngoing = true;
                meal.Timestamp = DateTime.UtcNow.AddHours(offset);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [HttpPost]
        [Route("AddDishByPhotoCustomMeal")]
        public async Task<long> AddDishByPhotoCustomMeal([FromQuery] string Name, [FromQuery] double share, [FromQuery] double portions, [FromQuery] string ContentType, [FromQuery] long userId, [FromQuery] bool FavoriteDish, [FromQuery] double offset, [FromQuery] string Note = "")
        {
            try
            {
                // check if that day already has meals
                DateTime Date = DateTime.UtcNow.Date;
                var nday = await _context.FnsNutritionActualDay.Where(t => t.FkUserId == userId && t.Date >= Date && t.Date < Date.AddDays(1))
                //    .AsNoTracking()
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
                if(nday.FnsNutritionActualMeal == null)
                {
                    nday.FnsNutritionActualMeal = new List<FnsNutritionActualMeal>();
                }
                nday.FnsNutritionActualMeal.Add(newMeal);
                newMeal.FkNutritionActualDay = nday;
              //  nday.FnsNutritionActualMeal.Add(newMeal);
           //     _context.Update(nday);


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
                meal.IsComplete = false;
                meal.IsSkipped = false;
                meal.IsOngoing = true;
                meal.Timestamp = DateTime.UtcNow.AddHours(offset);
                await _context.SaveChangesAsync();

                return meal.Id;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        [HttpPost]
        [Route("AddDishByReference")]
        public async Task<bool> AddDishByReference(long MealId, long recipId, double share, double portions, long userId, bool FavoriteDish,  double offset, string Note = "")
        {
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
                dish.Remarks = Note;

                if (FavoriteDish)
                {
                    dish.IsFavorite = true;
                }

                dayMeal.FnsNutritionActualDish.Add(dish);
                await _context.SaveChangesAsync();

                var meal = await _context.FnsNutritionActualMeal.Where(t => t.Id == MealId).FirstOrDefaultAsync();
                meal.IsComplete = false;
                meal.IsSkipped = false;
                meal.IsOngoing = true;
                meal.Timestamp = DateTime.UtcNow.AddHours(offset);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpPost]
        [Route("AddDishByReferenceCustomMeal")]
        public async Task<long> AddDishByReferenceCustomMeal(long recipId, double share, double portions, long userId, bool FavoriteDish, double offset, string Note = "")
        {
            try
            {
                // check if that day already has meals
                DateTime Date = DateTime.UtcNow.Date;
                var dayMeal = await _context.FnsNutritionActualDay.Where(t => t.FkUserId == userId && t.Date >= Date && t.Date < Date.AddDays(1))
                   // .AsNoTracking()
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
                dish.FkUserId = userId;
                dish.FkDishTranscriptionTypeId = 2;
                dish.Remarks = Note;

                if (FavoriteDish)
                {
                    dish.IsFavorite = true;
                }

                newMeal.FnsNutritionActualDish.Add(dish);
                await _context.SaveChangesAsync();

                var meal = await _context.FnsNutritionActualMeal.Where(t => t.Id == newMeal.Id).FirstOrDefaultAsync();
                meal.IsComplete = false;
                meal.IsSkipped = false;
                meal.IsOngoing = true;
              
                meal.Timestamp = DateTime.UtcNow.AddHours(offset);
                await _context.SaveChangesAsync();

                return newMeal.Id;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }


        [HttpPost]
        [Route("SnoozeMeal")]
        public async Task<bool> SnoozeMeal(long MealId, int snoozeTimeMinutes)
        {

            var meal = await _context.FnsNutritionActualMeal
                .Include(t=>t.FkNutritionActualDay)
                .Include(t => t.MealType)
            .Where(t => t.Id == MealId).FirstOrDefaultAsync();

            DateTime date = meal.FkNutritionActualDay.Date;
            string dString = string.Format("{0}/{1}/{2} 12:00:00", date.Month, date.Day, date.Year);

            meal.IsSnoozed = true;
            if(meal.SnoozedTime.HasValue)
            {
                meal.SnoozedTime = meal.SnoozedTime.Value.AddMinutes(snoozeTimeMinutes);
            }
            else
            {
                meal.SnoozedTime = ExerciseController.GetDateFromMealName(meal.MealType.Name, dString).AddMinutes(snoozeTimeMinutes);
            }
            await _context.SaveChangesAsync();
            return true;
        }


        [HttpPost]
        [Route("SnoozeMealUndo")]
        public async Task<bool> SnoozeMealUndo(long MealId)
        {

            var meal = await _context.FnsNutritionActualMeal
            .Where(t => t.Id == MealId).FirstOrDefaultAsync();

            meal.IsSnoozed = false;
            meal.SnoozedTime = null;
            await _context.SaveChangesAsync();

            return true;
        }


        [HttpPost]
        [Route("UndoSkip")]
        public async Task<bool> UndoSkip(long MealId)
        {

            var meal = await _context.FnsNutritionActualMeal
            .Where(t => t.Id == MealId).FirstOrDefaultAsync();

            meal.IsSkipped = false;
            await _context.SaveChangesAsync();

            return true;
        }

        [HttpPost]
        [Route("InitNutrion")]
        public async void InitNutrion()
        {
            try
            {
                DateTime utcnow = DateTime.Now;
                List<UserOpResult> users = new List<UserOpResult>();
                var appusers = _context.User.ToList();

                foreach (var au in appusers)
                {
                    users.Add(new UserOpResult() { UserId = au.Id, UserName = au.UserLevel.ToString() });
                }

                foreach (var u in users)
                {
                    if (u.UserName != "100") continue;

                    for (int i = 0; i < 40; i++)
                    {


                        FnsNutritionActualDay nday = new FnsNutritionActualDay();
                        nday.AlcoholGramsTarget = 100;
                        nday.SugarGramsTarget = 90;
                        nday.FiberGramsTarget = 250;
                        nday.UnsaturatedFatGramsTarget = 10;
                        nday.FatGramsTarget = 125;
                        nday.ProteinGramsTarget = 180;
                        nday.CrabsGramsTarget = 515;
                        nday.Date = utcnow.AddDays(i);
                        nday.DayCalorieTarget = 2500;
                        nday.DayCalorieTargetMax = 3500;
                        nday.DayCalorieTargetMin = 2000;
                        nday.FkUserId = u.UserId;


                        FnsNutritionActualMeal mmeal1 = new FnsNutritionActualMeal();
                        FnsNutritionActualMeal mmeal2 = new FnsNutritionActualMeal();
                        FnsNutritionActualMeal mmeal3 = new FnsNutritionActualMeal();
                        mmeal1.AlcoholGramsTarget = 100;
                        mmeal1.SugarGramsTarget = 110;
                        mmeal1.FiberGramsTarget = 120;
                        mmeal1.UnsaturatedFatGramsTarget = 130;
                        mmeal1.FatGramsTarget = 140;
                        mmeal1.CrabsGramsTarget = 150;
                        mmeal1.ProteinGramsTarget = 160;
                        mmeal1.MealCalorieTarget = 1100;
                        mmeal1.MealCalorieMin = 1200;
                        mmeal1.MealCalorieMax = 1300;
                        mmeal1.HasTarget = true;
                        mmeal1.MealTypeId = 1;
                        mmeal1.FkNutritionActualDay = nday;

                        mmeal2.AlcoholGramsTarget = 200;
                        mmeal2.SugarGramsTarget = 210;
                        mmeal2.FiberGramsTarget = 220;
                        mmeal2.UnsaturatedFatGramsTarget = 230;
                        mmeal2.FatGramsTarget = 240;
                        mmeal2.CrabsGramsTarget = 250;
                        mmeal2.ProteinGramsTarget = 260;
                        mmeal2.MealCalorieTarget = 2100;
                        mmeal2.MealCalorieMin = 2200;
                        mmeal2.MealCalorieMax = 2300;
                        mmeal2.HasTarget = true;
                        mmeal2.MealTypeId = 2;
                        mmeal2.FkNutritionActualDay = nday;

                        mmeal3.AlcoholGramsTarget = 300;
                        mmeal3.SugarGramsTarget = 310;
                        mmeal3.FiberGramsTarget = 320;
                        mmeal3.UnsaturatedFatGramsTarget = 330;
                        mmeal3.FatGramsTarget = 340;
                        mmeal3.CrabsGramsTarget = 350;
                        mmeal3.ProteinGramsTarget = 360;
                        mmeal3.MealCalorieTarget = 3100;
                        mmeal3.MealCalorieMin = 3200;
                        mmeal3.MealCalorieMax = 3300;
                        mmeal3.HasTarget = true;
                        mmeal3.MealTypeId = 3;
                        mmeal3.FkNutritionActualDay = nday;




                        _context.Add(nday);
                        _context.Add(mmeal1);
                        _context.Add(mmeal2);
                        _context.Add(mmeal3);
                        _context.SaveChanges();
                    }
                }
            }
            catch(Exception e)
            {

            }
        }


        //[HttpGet]
        //[Route("GetUserByID")]
        //public User GetUserByID(long ID)
        //{

        //}

        

    }
}
