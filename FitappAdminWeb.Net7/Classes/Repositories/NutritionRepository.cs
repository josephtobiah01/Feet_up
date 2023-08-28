using AutoMapper;
using DAOLayer.Net7.Nutrition;
using FitappAdminWeb.Net7.Classes.DTO;
using FitappAdminWeb.Net7.Classes.Utilities;
using FitappAdminWeb.Net7.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FitappAdminWeb.Net7.Classes.Repositories
{
    public class NutritionRepository
    {
        NutritionContext _nutcontext;
        FitAppAPIUtil _apiutil;
        IConfiguration _config;
        ILogger<NutritionRepository> _logger;
        IMapper _mapper;

        const int ACTUALDAY_DEFAULT_DATERANGE = 7;
        const long TRANSCRIPTIONTYPEID_PHOTO = 1;
        readonly string API_NOTIFYTRANSCRIPTIONCOMPLETE = "/api/Nutrition/NotifyTranscriptionComplete";
        readonly string APPSETTINGKEY_MAINAPI_DOMAIN = "MainApi_Domain";
        readonly int CUSTOM_MEALTYPE_ID = 4;

        public NutritionRepository(NutritionContext nutritionContext, ILogger<NutritionRepository> logger, IConfiguration config, IMapper mapper, FitAppAPIUtil apiutil)
        {
            _nutcontext = nutritionContext;
            _logger = logger;
            _mapper = mapper;
            _config = config;
            _apiutil = apiutil;
        }

        public async Task<FnsNutritionActualDay?> GetActualDayByDate(long userId, DateTime dt)
        {
            return await _nutcontext.FnsNutritionActualDay.Where(r => r.FkUserId == userId && r.Date.Date == dt.Date)
                .Include(r => r.FnsNutritionActualMeal)
                    .ThenInclude(r => r.FnsNutritionActualDish)
                        .ThenInclude(r => r.FkDishType)
                .Include(r => r.FnsNutritionActualMeal)
                    .ThenInclude(r => r.MealType)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CheckIfActualDayCanBeAddedByDate(long userId, DateTime dt)
        {
            return (await _nutcontext.FnsNutritionActualDay.CountAsync(r => r.FkUserId == userId && r.Date.Date == dt.Date)) == 0;
        }

        public async Task<FnsNutritionActualDay?> GetActualDayById(long dayId)
        {
            return await _nutcontext.FnsNutritionActualDay.FirstOrDefaultAsync(r => r.Id == dayId);
        }

        public async Task<List<FnsNutritionActualMeal>> GetMealsInDay(long dayId)
        {
            return await _nutcontext.FnsNutritionActualMeal.Where(r => r.FkNutritionActualDayId == dayId)
                .Include(r => r.MealType).ToListAsync();
        }

        public async Task<List<FnsNutritionActualDay>> GetActualDaysByDateRange(long userId, DateTime start, DateTime? end = null)
        {
            if (!end.HasValue)
            {
                end = start;
            }

            return await _nutcontext.FnsNutritionActualDay.Where(r => r.FkUserId == userId && r.Date.Date >= start.Date && r.Date.Date <= end.Value.Date)
                .OrderBy(r => r.Date)
                .Include(r => r.FnsNutritionActualMeal)
                    .ThenInclude(r => r.FnsNutritionActualDish)
                        .ThenInclude(r => r.FkDishType)
                .Include(r => r.FnsNutritionActualMeal)
                    .ThenInclude(r => r.MealType)
                .ToListAsync();
        }

        public async Task<List<FnsNutritionActualDish>> GetTranscriptionList(bool includeTestAccounts = false)
        {
            return await _nutcontext.FnsNutritionActualDish.Where(r => !r.IsComplete && r.FkDishTranscriptionTypeId == TRANSCRIPTIONTYPEID_PHOTO && (includeTestAccounts || r.FkNutritionActualMeal.FkNutritionActualDay.FkUser.IsActive == true))
                .OrderBy(r => r.CreationTimestamp)
                .Include(r => r.FkDishTranscriptionType)
                .Include(r => r.FkDishType)
                .Include(r => r.FkNutritionActualMeal)
                    .ThenInclude(r => r.FkNutritionActualDay)
                        .ThenInclude(r => r.FkUser)
                .Include(r => r.FkNutritionActualMeal)
                    .ThenInclude(r => r.MealType)
                .ToListAsync();
        }

        public async Task<FnsNutritionActualDish?> GetActualDishById(long id)
        {
            return await _nutcontext.FnsNutritionActualDish.Where(r => r.Id == id)
                .Include(r => r.FkDishType)
                .Include(r => r.FkNutritionActualMeal)
                    .ThenInclude(r => r.FkNutritionActualDay)
                        .ThenInclude(r => r.FkUser)
                .Include(r => r.FkNutritionActualMeal)
                    .ThenInclude(r => r.MealType)
                .SingleOrDefaultAsync();
        }

        public async Task<FnsNutritionActualDish?> GetActualDishById_MinimalInclude(long id)
        {
            return await _nutcontext.FnsNutritionActualDish.Where(r => r.Id == id)
                .Include(r => r.FkDishType)
                .Include(r => r.FkUser)
                .SingleOrDefaultAsync();
        }

        public async Task<List<SelectListItem>> GetDishListSelectItems(long userid)
        {
            return await _nutcontext.FnsNutritionActualDish.Where(r => r.FkNutritionActualMeal.FkNutritionActualDay.FkUserId == userid && r.IsComplete && r.FkReuseReference == null)
                .Select(r => new SelectListItem()
                {
                    Text = $"{r.Name} (ID: {r.Id})", Value = r.Id.ToString() 
                }).ToListAsync();
        }

        public async Task<FnsNutritionActualDish?> UpdateActualDish(DishEditFormModel input)
        {
            using (_logger.BeginScope("UpdateActualDish"))
            {
                try
                {
                    
                    FnsNutritionActualDish currDish = await GetActualDishById(input.Id);
                    if (currDish == null)
                    {
                        _logger.LogError("Cannot edit dish. Cannot find dish id {id}", input.Id);
                        return null;
                    }
                    bool firstTranscription = !currDish.IsComplete;

                    _mapper.Map(input, currDish);

                    //check if all dishes in meal are completed is already completed
                    if (currDish.IsError)
                    {
                        currDish.ErrorTimestamp = DateTime.UtcNow;                      
                    }
                    else
                    {
                        currDish.IsComplete = true;
                        currDish.CompletionTimestamp = DateTime.UtcNow;
                        if (!currDish.FkNutritionActualMeal.IsComplete) 
                        {
                            //we don't really reverse meal completion if it is completed earlier.
                            currDish.FkNutritionActualMeal.IsComplete = await AreAllDishesCompleteInMeal(currDish.FkNutritionActualMealId);
                            currDish.FkNutritionActualMeal.IsOngoing = !currDish.FkNutritionActualMeal.IsComplete;
                        }
                    }
                    
                    _nutcontext.Update(currDish);
                    await _nutcontext.SaveChangesAsync();

                    //Call NotifyTranscriptionComplete API for this current dish when all dishes are complete
                    if (firstTranscription && currDish.IsComplete && currDish.FkNutritionActualMeal.IsComplete)
                    {
                        try
                        {
                            HttpClient client = _apiutil.GetHttpClient();
                            var request = _apiutil.BuildRequest(API_NOTIFYTRANSCRIPTIONCOMPLETE, HttpMethod.Post, currDish.FkNutritionActualMealId);
                            _ = Task.Run(() => client.SendAsync(request));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to call GetMealDishes API on dish id {id}", currDish.Id);
                        }
                    }

                    return currDish;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update dish {data}", JsonConvert.SerializeObject(input));
                    return null;
                }
            }
        }

        public async Task<bool> AreAllDishesCompleteInMeal(long mealId)
        {
            var meal = await _nutcontext.FnsNutritionActualMeal.FirstOrDefaultAsync(r => r.Id == mealId);
            if (meal == null)
            {
                _logger.LogWarning("Cannot find meal with ID {id}", mealId);
                return false;
            }

            var incompleteDishCount = await _nutcontext.FnsNutritionActualDish.CountAsync(r => r.FkNutritionActualMealId == mealId && r.IsComplete == false);
            return (incompleteDishCount - 1) <= 0; //we offset this by one to take into the account the current unsaved to-be-completed item
        }

        //NOTE: Only used for testing the dish queue.
        public async Task<FnsNutritionActualDish?> AddDishToPhotoTranscribeQueue(DateTime dt, TimeSpan time, long mealTypeId, long dishTypeId,string imageUrl, long userId)
        {
            using (_logger.BeginScope("AddDishToPhotoTranscribeQueue"))
            {
                try
                {

                    FnsTranscriptionType photoTransType = await _nutcontext.FnsTranscriptionType.FirstOrDefaultAsync(r => r.Name == "Photo");
                    if (photoTransType == null)
                    {
                        _logger.LogError("Cannot add dish. No transcription type found of name 'Photo' found");
                        return null;
                    }

                    FmsDishType dishType = await _nutcontext.FmsDishType.FirstOrDefaultAsync(r => r.Id == dishTypeId);
                    if (dishType == null)
                    {
                        _logger.LogError("Cannot add dish. No dish Type of id {dishTypeId}", dishTypeId);
                        return null;
                    }

                    FnsNutritionActualDay actualDay = await _nutcontext.FnsNutritionActualDay.FirstOrDefaultAsync(r => r.Date.Date == dt.Date && r.FkUserId == userId);
                    if (actualDay == null)
                    {
                        _logger.LogError("Cannot add dish. No day found on {dt} for User {user}", dt, userId);
                        return null;
                    }

                    FnsNutritionActualMeal actualMeal = await _nutcontext.FnsNutritionActualMeal
                        .FirstOrDefaultAsync(r => r.MealTypeId == mealTypeId && r.FkNutritionActualDayId == actualDay.Id);
                    if (actualMeal == null)
                    {
                        _logger.LogError("Cannot add dish. No meal found on day {dayId} with meal type {mealTypeId}", actualDay.Id, mealTypeId);
                        return null;
                    }

                    FnsNutritionActualDish newDish = new FnsNutritionActualDish()
                    {
                        CreationTimestamp = dt.Add(time),
                        UploadPhotoReference = imageUrl,
                        FkDishTranscriptionTypeId = photoTransType.Id,
                        FkNutritionActualMealId = actualMeal.Id,
                        FkDishTypeId = dishType.Id,                        
                        Remarks = "Added in Backend"
                    };

                    _nutcontext.FnsNutritionActualDish.Add(newDish);
                    await _nutcontext.SaveChangesAsync();
                    return newDish;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to add dish");
                    return null;
                }
            }           
        }

        public async Task<bool> AddNutritionalDailyPlans(ActualDay_DTO input)
        {
            using (_logger.BeginScope("AddNutritionalDailyPlans"))
            {
                try
                {
                    FnsNutritionActualDay mappedDay = _mapper.Map<FnsNutritionActualDay>(input);
                    foreach (var meal in input.Meals)
                    {
                        var mappedMeal = _mapper.Map<FnsNutritionActualMeal>(meal);
                        mappedDay.FnsNutritionActualMeal.Add(mappedMeal);
                    }

                    bool isOkToAdd = await CheckIfActualDayCanBeAddedByDate(mappedDay.FkUserId, mappedDay.Date);
                    if (!isOkToAdd)
                    {
                        throw new InvalidOperationException($"Cannot add daily plan. Food Daily plan for {mappedDay.Date} already exists!");
                    }
                    _nutcontext.FnsNutritionActualDay.Add(mappedDay);

                    //query a list of all possible conflicting actual_days. We cannot add an ActualDay if it exists in this query
                    var actualDay_lists = await GetInvalidDatesForMealPlanExtrapolation(input.FkUserId, input.Date, input.DaysToExtrapolate);

                    //try to extrapolate to future days
                    for (var i = 1; i <= input.DaysToExtrapolate; i++)
                    {
                        var currDate = mappedDay.Date.Date.AddDays(i);
                        if (actualDay_lists.Contains(currDate))
                        {
                            _logger.LogWarning("A plan already exist for this day {date}. Skipping..", currDate);
                            continue;
                        }

                        //add a copy of a plan
                        var newDailyPlan = _mapper.Map<FnsNutritionActualDay>(input);
                        foreach (var meal in input.Meals)
                        {
                            //custom meals are NOT copied
                            if (meal.MealTypeId == CUSTOM_MEALTYPE_ID)
                            {
                                continue;
                            }

                            var newMeal = _mapper.Map<FnsNutritionActualMeal>(meal);
                            if (newMeal.ScheduledTime.HasValue)
                            {
                                newMeal.ScheduledTime = currDate.Add(newMeal.ScheduledTime.Value.TimeOfDay);
                            }
                            newDailyPlan.FnsNutritionActualMeal.Add(newMeal);
                        }
                        newDailyPlan.Date = currDate;

                        _nutcontext.Add(newDailyPlan);
                    }

                    int rowChanges = await _nutcontext.SaveChangesAsync();
                    _logger.LogInformation("Insert OK. Row Changes {rc}", rowChanges);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to add fns daily plan");
                    return false;
                }
            }
        }

        public async Task<bool> EditNutritionalDailyPlans(ActualDay_DTO input)
        {
            using (_logger.BeginScope("EditNutritionalDailyPlans"))
            {
                try
                {
                    var currDay = await _nutcontext.FnsNutritionActualDay.Where(r => r.Id == input.Id).Include(r => r.FnsNutritionActualMeal).FirstOrDefaultAsync();
                    if (currDay == null)
                    {
                        throw new InvalidOperationException($"Cannot find actual day with id {input.Id}");
                    }

                    _mapper.Map(input, currDay);

                    //map meals
                    foreach (var inputMeal in input.Meals)
                    {
                        var currMeal = currDay.FnsNutritionActualMeal.FirstOrDefault(r => r.Id == inputMeal.Id);
                        if ((currMeal != null) && (inputMeal.Id != 0))
                        {
                            //Edit the meal row
                            _mapper.Map(inputMeal, currMeal);
                            continue;
                        }
                        //else add a new meal row
                        var newMealRow = _mapper.Map<FnsNutritionActualMeal>(inputMeal);
                        newMealRow.Id = 0;
                        currDay.FnsNutritionActualMeal.Add(newMealRow);
                    }

                    //await CopyDayToFutureDays(input);
                    await CopyDayToFutureDays_V2(input);

                    int rowChanges = await _nutcontext.SaveChangesAsync();
                    _logger.LogInformation($"Updated Actual Day ID {input.Id}. Rows Changed: {rowChanges}");
                    return true;
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to edit nutritional daily plans");
                    return false;
                }
            }
        }

        /// <summary>
        /// This returns a list of days that do not contain any dish data and are therefore replaceable
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startDate"></param>
        /// <param name="daysToExtrapolate"></param>
        /// <returns></returns>
        private async Task<List<FnsNutritionActualDay>> GetActualDaysToReplace(long userId, DateTime startDate, int daysToExtrapolate)
        {
            DateTime endDate = startDate.AddDays(daysToExtrapolate);
            var actualday_list = await _nutcontext.FnsNutritionActualDay.Where(r => r.FkUserId == userId && 
                r.Date > startDate && r.Date <= endDate &&
                r.FnsNutritionActualMeal.Count(s => s.FnsNutritionActualDish.Count > 0) == 0)
                .Include(r => r.FnsNutritionActualMeal)
                .ToListAsync();

            return actualday_list;
        }

        private async Task<List<FnsNutritionActualDay>> GetActualDaysToReplace_V2(long userId, DateTime startDate, int daysToExtrapolate)
        {
            DateTime endDate = startDate.AddDays(daysToExtrapolate);
            var actualday_list = await _nutcontext.FnsNutritionActualDay.Where(r => r.FkUserId == userId &&
                r.Date > startDate && r.Date <= endDate)
                .Include(r => r.FnsNutritionActualMeal)
                    .ThenInclude(r => r.FnsNutritionActualDish)
                        .ThenInclude(r => r.FkDishType)
                .Include(r => r.FnsNutritionActualMeal)
                    .ThenInclude(r => r.MealType)
                .ToListAsync();

            return actualday_list;
        }

        private async Task CopyDayToFutureDays(ActualDay_DTO input)
        {
            var actualDayList = await GetActualDaysToReplace(input.FkUserId, input.Date, input.DaysToExtrapolate);
            for (var i = 0; i < input.DaysToExtrapolate; i++)
            {
                var currDate = input.Date.AddDays(i + 1);
                var currDays = actualDayList.Where(r => r.Date.Date == currDate.Date).ToList();

                //add a day row if there is no day there.

                foreach (var currDay in currDays)
                {
                    if (currDay != null)
                    {
                        //delete actual day from db.
                        //Note: Might need to drill down to meals depending on db schema
                        _nutcontext.FnsNutritionActualMeal.RemoveRange(currDay.FnsNutritionActualMeal.ToArray());
                        _nutcontext.FnsNutritionActualDay.Remove(currDay);
                    }

                    //add a new actual day row based on input
                    var newDayRow = _mapper.Map<FnsNutritionActualDay>(input);
                    newDayRow.Id = 0;
                    newDayRow.Date = currDate;

                    foreach (var inputMeal in input.Meals)
                    {
                        //skip copying meal type 4 (custom meals)
                        if (inputMeal.MealTypeId == CUSTOM_MEALTYPE_ID)
                        {
                            continue;
                        }

                        var newMealRow = _mapper.Map<FnsNutritionActualMeal>(inputMeal);
                        newMealRow.Id = 0;
                        newMealRow.FkNutritionActualDayId = 0;

                        if (newMealRow.ScheduledTime.HasValue)
                        {
                            newMealRow.ScheduledTime = currDate.Add(newMealRow.ScheduledTime.Value.TimeOfDay);
                        }
                        newDayRow.FnsNutritionActualMeal.Add(newMealRow);

                    }
                    _nutcontext.FnsNutritionActualDay.Add(newDayRow);
                }
            }
        }

        private async Task CopyDayToFutureDays_V2(ActualDay_DTO input)
        {
            var actualDayList = await GetActualDaysToReplace_V2(input.FkUserId, input.Date, input.DaysToExtrapolate);
            for (var i = 0; i < input.DaysToExtrapolate; i++)
            {
                var currDate = input.Date.AddDays(i + 1);
                var currDays = actualDayList.Where(r => r.Date.Date == currDate.Date).ToList();
                bool hasData = currDays.Any(r => r.FnsNutritionActualMeal.Any(s => s.FnsNutritionActualDish.Count > 0));

                if (hasData) continue; //If dishes already exist in any plan in the date, we skip this date

                //delete actual day from db.
                foreach (var currDay in currDays)
                {
                    if (currDay != null)
                    {                       
                        _nutcontext.FnsNutritionActualMeal.RemoveRange(currDay.FnsNutritionActualMeal.ToArray());
                        _nutcontext.FnsNutritionActualDay.Remove(currDay);
                    }
                }

                //add a new actual day row based on input
                var newDayRow = _mapper.Map<FnsNutritionActualDay>(input);
                newDayRow.Id = 0;
                newDayRow.Date = currDate;

                foreach (var inputMeal in input.Meals)
                {
                    //skip copying meal type 4 (custom meals)
                    if (inputMeal.MealTypeId == CUSTOM_MEALTYPE_ID)
                    {
                        continue;
                    }

                    var newMealRow = _mapper.Map<FnsNutritionActualMeal>(inputMeal);
                    newMealRow.Id = 0;
                    newMealRow.FkNutritionActualDayId = 0;

                    if (newMealRow.ScheduledTime.HasValue)
                    {
                        newMealRow.ScheduledTime = currDate.Add(newMealRow.ScheduledTime.Value.TimeOfDay);
                    }

                    if (newMealRow.IsComplete)
                    {
                        newMealRow.IsComplete = false;
                    }

                    if (newMealRow.IsOngoing)
                    {
                        newMealRow.IsOngoing = false;
                    }

                    newDayRow.FnsNutritionActualMeal.Add(newMealRow);

                }
                _nutcontext.FnsNutritionActualDay.Add(newDayRow);
            }
        }

        private async Task<List<DateTime>> GetInvalidDatesForMealPlanExtrapolation(long userId, DateTime startDate, int daysToExtrapolate)
        {
            List<DateTime> datesToCheck = new List<DateTime>();
            for (var i = 1; i <= daysToExtrapolate; i++)
            {
                datesToCheck.Add(startDate.Date.AddDays(i));
            }
            
            return await _nutcontext.FnsNutritionActualDay.Where(r =>
                        r.FkUserId == userId &&
                        datesToCheck.Contains(r.Date.Date))
                        //r.FnsNutritionActualMeal.Where(r => r.FnsNutritionActualDish.Count > 0).Select(r => r.Id).Count() > 0)
                        .Select(r => r.Date).ToListAsync();
        }

        public async Task<List<FnsNutritionActualMeal>> GetMealsByDay(DateTime day)
        {
            return await _nutcontext.FnsNutritionActualMeal.Where(r => r.FkNutritionActualDay.Date.Date == day.Date)
                .ToListAsync();
        }

        public async Task<List<FnsMealType>> GetMealTypes()
        {
            return await _nutcontext.FnsMealType.Where(r => !r.IsDeleted).ToListAsync();
        }

        public async Task<List<FmsDishType>> GetDishTypes()
        {
            return await _nutcontext.FmsDishType.Where(r => !r.IsDeleted).ToListAsync();
        }

        public async Task<List<User>> GetUsers()
        {
            return await _nutcontext.User.Where(r => r.UserLevel > 0 && r.UserLevel <= 1000).ToListAsync();
        }

        public async Task<User?> GetUserById(long userId)
        {
            return await _nutcontext.User.Where(r => r.Id == userId && r.UserLevel > 0 && r.UserLevel <= 1000).FirstOrDefaultAsync();
        }

        public async Task<List<FnsTranscriptionType>> GetTranscriptionTypes()
        {
            return await _nutcontext.FnsTranscriptionType.Where(r => r.IsActive).ToListAsync();
        }

        public async Task<List<FnsErrorCode>> GetFnsErrorCodes()
        {
            return await _nutcontext.FnsErrorCode.Where(r => r.IsActive).ToListAsync();
        }

        /// <summary>
        /// Returns the latest date that contains an ActualDay
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<DateTime?> GetLatestDateWithActualDay(long userId)
        {
            return await _nutcontext.FnsNutritionActualDay.Where(r => r.FkUserId == userId).OrderByDescending(r => r.Date).Select(r => r.Date).FirstOrDefaultAsync();
        }

        #region Nutrition 
        DayActualNutritionValueCalculationModel CalculateTotalValues(FnsNutritionActualDay actualDay)
        {
            DayActualNutritionValueCalculationModel dayTotal = new DayActualNutritionValueCalculationModel()
            {
                DayId = actualDay.Id
            };

            foreach (var meal in actualDay.FnsNutritionActualMeal)
            {
                MealActualNutritionalValueCalculationModel mealTotal = new MealActualNutritionalValueCalculationModel() { MealId = meal.Id };

                foreach (var dish in meal.FnsNutritionActualDish)
                {
                    if (!dish.IsComplete)
                    {
                        continue;
                    }
                    DishActualNutritionalValueCalculationModel dishTotal = new DishActualNutritionalValueCalculationModel() { DishId = dish.Id };
                    dishTotal.TotalCalories += (dish.CalorieActual ?? 0) * dish.ShareOfDishConsumed * dish.NumberOfServingsConsumed;
                    dishTotal.TotalCarbs += (dish.CrabsActual ?? 0) * dish.ShareOfDishConsumed * dish.NumberOfServingsConsumed;
                    dishTotal.TotalProtein += (dish.ProteinActual ?? 0) * dish.ShareOfDishConsumed * dish.NumberOfServingsConsumed;
                    dishTotal.TotalSugars += (dish.SugarActual ?? 0) * dish.ShareOfDishConsumed * dish.NumberOfServingsConsumed;
                    dishTotal.TotalFat += (dish.FatActual ?? 0) * dish.ShareOfDishConsumed * dish.NumberOfServingsConsumed;
                    dishTotal.TotalSaturatedFat += (dish.SaturatedFatGramsActual ?? 0) * dish.ShareOfDishConsumed * dish.NumberOfServingsConsumed;
                    dishTotal.TotalUnsaturatedFat += (dish.UnsaturatedFatActual ?? 0) * dish.ShareOfDishConsumed * dish.NumberOfServingsConsumed;
                    dishTotal.TotalFiber += (dish.FiberGramsActual ?? 0) * dish.ShareOfDishConsumed * dish.NumberOfServingsConsumed;
                    dishTotal.TotalAlcohol += (dish.AlcoholGramsActual ?? 0) * dish.ShareOfDishConsumed * dish.NumberOfServingsConsumed;
                    
                    mealTotal.TotalCalories += dishTotal.TotalCalories;
                    mealTotal.TotalCarbs += dishTotal.TotalCarbs;
                    mealTotal.TotalProtein += dishTotal.TotalProtein;
                    mealTotal.TotalSugars += dishTotal.TotalSugars;
                    mealTotal.TotalFat += dishTotal.TotalFat;
                    mealTotal.TotalSaturatedFat += dishTotal.TotalSaturatedFat;
                    mealTotal.TotalUnsaturatedFat += dishTotal.TotalUnsaturatedFat;
                    mealTotal.TotalFiber += dishTotal.TotalFiber;
                    mealTotal.TotalAlcohol += dishTotal.TotalAlcohol;
                    mealTotal.DishTotal.Add(dishTotal);

                    dayTotal.TotalCalories += dishTotal.TotalCalories;
                    dayTotal.TotalCarbs += dishTotal.TotalCarbs;
                    dayTotal.TotalProtein += dishTotal.TotalProtein;
                    dayTotal.TotalSugars += dishTotal.TotalSugars;
                    dayTotal.TotalFat += dishTotal.TotalFat;
                    dayTotal.TotalSaturatedFat += dishTotal.TotalSaturatedFat;
                    dayTotal.TotalUnsaturatedFat += dishTotal.TotalUnsaturatedFat;
                    dayTotal.TotalFiber += dishTotal.TotalFiber;
                    dayTotal.TotalAlcohol += dishTotal.TotalAlcohol;
                }
                dayTotal.MealTotals.Add(mealTotal);
            }

            return dayTotal;
        }

        public async Task<List<DayActualNutritionValueCalculationModel>> GetTotalNutritionalValueForActualDays(List<FnsNutritionActualDay> actualDays)
        {
            using (_logger.BeginScope("GetTotalNutritionalValueForActualDays"))
            {
                if (actualDays == null)
                    throw new ArgumentNullException(nameof(actualDays));

                foreach (var day in actualDays)
                {
                    //ensure that the actualDay already has meals and dishes loaded. If not, load them
                    await _nutcontext.Entry(day).Collection(r => r.FnsNutritionActualMeal).Query().Include(r => r.FnsNutritionActualDish).LoadAsync();
                }

                List<DayActualNutritionValueCalculationModel> results = actualDays.Select(r => CalculateTotalValues(r)).ToList();
                return results;
            }
        }
        #endregion
    }
}
