using AutoMapper;
using FitappAdminWeb.Net7.Classes.Base;
using FitappAdminWeb.Net7.Classes.Repositories;
using FitappAdminWeb.Net7.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Globalization;
using X.PagedList;

namespace FitappAdminWeb.Net7.Controllers
{
    public class NutritionController : BaseController
    {
        MessageRepository _messagerepo;
        NutritionRepository _nutrepo;
        ILogger<NutritionController> _logger;
        IMapper _mapper;

        const string ADD_DISHFAILED_ERROR = "adddishfailed";

        public NutritionController(MessageRepository messagerepo, NutritionRepository nutrepo, ILogger<NutritionController> logger, IMapper mapper)
            : base(messagerepo)
        {
            _messagerepo = messagerepo;
            _nutrepo = nutrepo;
            _logger = logger;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditPlan(long userId, DateTime? start = null, long? id = null, bool copy = false)
        {
            DailyMealEditModel vm = new DailyMealEditModel();
            vm.IsCopy = copy;
            var initialDate = start ?? DateTime.Now.Date;

            long targetUserId = userId;
            if (id.HasValue)
            {
                var dailyPlan = await _nutrepo.GetActualDayById(id.Value);
                if (dailyPlan != null)
                {
                    vm.Date = copy ? null : dailyPlan.Date;
                    vm.Id = dailyPlan.Id;
                }
                else
                {
                    vm.IsCopy = false;
                    vm.Date = initialDate;
                }
            }
            else
            {
                vm.IsCopy = false;
                vm.Date = initialDate;
            }

            var user = await _nutrepo.GetUserById(targetUserId);
            if (user == null)
            {
                return RedirectToAction("DailyPlan");
            }
            vm.CurrentUser = user;

            var mealtypes = await _nutrepo.GetMealTypes();
            mealtypes.RemoveAll(x => x.Id == 4);
            vm.MealTypeList = new SelectList(mealtypes, "Id", "Name").ToList();

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> DailyPlan(long userId, DateTime? start = null, DateTime? end = null)
        {
            NutritionDailyPlanViewModel vm = new NutritionDailyPlanViewModel();
            if (!start.HasValue)
            {
                start = DateTime.Now.Date;
            }
            vm.Day = start.Value;

            var s = new string("papa");
            var user = await _nutrepo.GetUserById(userId);
            if (user == null)
            {
                _logger.LogWarning("NutritionDailyPlan: Cannot find user {id}.", userId);
                return RedirectToAction("Index", "Home");
            }
            vm.CurrentUser = user;
            vm.LatestDayWithPlan = await _nutrepo.GetLatestDateWithActualDay(user.Id);

            vm.DailyPlans = await _nutrepo.GetActualDaysByDateRange(user.Id, start.Value, end);
            vm.TotalValues = await _nutrepo.GetTotalNutritionalValueForActualDays(vm.DailyPlans);
            return View(vm);
        }

        //[HttpGet]
        //public async Task<IActionResult> DailyPlan(long userId, DateTime? dt = null)
        //{
        //    NutritionDailyPlanViewModel vm = new NutritionDailyPlanViewModel();
        //    if (!dt.HasValue)
        //    {
        //        dt = DateTime.Now.Date;
        //    }

        //    var user = await _nutrepo.GetUserById(userId);
        //    if (user == null)
        //    {
        //        _logger.LogWarning("NutritionDailyPlan: Cannot find user {id}.", userId);
        //        return RedirectToAction("Index", "Home");
        //    }
        //    vm.CurrentUser = user;

        //    var dailyplan = await _nutrepo.GetActualDayByDate(userId, dt.Value);
        //    vm.CurrentDailyPlan = dailyplan;

        //    return View(vm);
        //}

        [HttpGet]
        public async Task<IActionResult> MealQueue(int? page, bool IncludeTestAccounts)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            MealQueueViewModel vm = new MealQueueViewModel() { IncludeTestAccounts = IncludeTestAccounts };
            vm.DishesForTranscription = await _nutrepo.GetTranscriptionList(vm.IncludeTestAccounts);
            vm.PagingDishesForTranscription = new PagedList<DAOLayer.Net7.Nutrition.FnsNutritionActualDish>(vm.DishesForTranscription, pageNumber, pageSize);

            if (Request.RouteValues.ContainsKey("error"))
            {
                ViewData["error"] = Request.RouteValues["error"];
            }

            var users = await _nutrepo.GetUsers();
            vm.UserList = users.Select(r => new SelectListItem()
            {
                Text = $"{r.FirstName} {r.LastName} (ID: {r.Id})",
                Value = r.Id.ToString()
            }).ToList();

            var mealTypes = await _nutrepo.GetMealTypes();
            vm.MealTypeList = new SelectList(mealTypes, "Id", "Name").ToList();

            var dishTypes = await _nutrepo.GetDishTypes();
            vm.DishTypeList = new SelectList(dishTypes, "Id", "Name").ToList();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddDishToQueue(MealQueueAddDishModel DishToAdd)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("MealQueue", new { error = ADD_DISHFAILED_ERROR });
                }

                var result = await _nutrepo.AddDishToPhotoTranscribeQueue(
                    DishToAdd.Day, DishToAdd.Time, DishToAdd.MealTypeId.Value, DishToAdd.DishTypeId.Value, 
                    DishToAdd.ImageUrl, DishToAdd.UserId.Value);

                if (result != null)
                {
                    return RedirectToAction("MealQueue");
                }
                return RedirectToAction("MealQueue", new { error = ADD_DISHFAILED_ERROR });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add Dish to Queue");
                return RedirectToAction("MealQueue", new { error = ADD_DISHFAILED_ERROR });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DishEditor(long id)
        {
            //var DishesForTranscription = await _nutrepo.GetTranscriptionList();
            var currDish = await _nutrepo.GetActualDishById(id);
            if (currDish == null)
            {
                return NotFound();
            }

            MealEditDishModel vm = new(currDish);
            vm.Data = _mapper.Map<DishEditFormModel>(currDish);
            vm.SubmittedBy = currDish.FkNutritionActualMeal.FkNutritionActualDay.FkUser;
            vm.SubmittedOn = currDish.CreationTimestamp;
            vm.MealId = currDish.FkNutritionActualMealId;
            vm.MealType = currDish.FkNutritionActualMeal.MealType.Name;
            vm.UserRemarks = currDish.Remarks;

            var dishTypes = await _nutrepo.GetDishTypes();
            vm.DishTypeList = new SelectList(dishTypes, "Id", "Name").ToList();

            var transcriptionTypes = await _nutrepo.GetTranscriptionTypes();
            vm.TranscriptionTypes = new SelectList(transcriptionTypes, "Id", "Name").ToList();

            var errorCode = await _nutrepo.GetFnsErrorCodes();
            vm.ErrorCode = new SelectList(errorCode, "Id", "Name", "DefaultErrorMessage", "DefaultErrorHeadline").ToList();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DishEditor(DishEditFormModel data) 
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("DishEditor", new { error = "invalid_data" });
                }

                var currDish = await _nutrepo.UpdateActualDish(data);
                return RedirectToAction("MealQueue");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to edit dish");
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Overview(long userId, DateTime? startdate = null, DateTime? enddate = null, long? chartDisplay = null)
        {
            NutritionDailyPlanGraphViewModel vm = new NutritionDailyPlanGraphViewModel();

            chartDisplay ??= 0;

            startdate ??= DateTime.Now.Date.AddDays(-7); //Get most recent week

            enddate ??= startdate.Value.AddDays(7);

            var user = await _nutrepo.GetUserById(userId);
            if (user == null)
            {
                _logger.LogWarning("NutritionDailyPlan: Cannot find user {id}.", userId);
                return RedirectToAction("Index", "Home");
            }

            vm.CurrentUser = user;
            vm.Day = startdate.Value;
            vm.EndDay = enddate.Value;
            vm.ChartDisplayId = (long)chartDisplay;

            vm.DailyPlans = await _nutrepo.GetActualDaysByDateRange(user.Id, startdate.Value, enddate);
            vm.TotalValues = await _nutrepo.GetTotalNutritionalValueForActualDays(vm.DailyPlans);

            foreach (DAOLayer.Net7.Nutrition.FnsNutritionActualDay dailyPlan in vm.DailyPlans)
            {
                foreach (Nutrients name in Enum.GetValues(typeof(Nutrients)))
                {
                    vm.NutritionListRawGraphData.Add(GetNutritionDailyPlan(name, dailyPlan, vm.TotalValues));
                }
            }

            var queryDataChart = vm.NutritionListRawGraphData.GroupBy(x => x.Name).Where(g => g.Count() > 0).Select(y => y).ToList();

            if (chartDisplay == 1)//Weekly
            {
                foreach (var item in queryDataChart)
                {
                    vm.NutritionListChartDisplayData.Add(GetNutritionWeeklyChartData(item));
                }
            }
            else if (chartDisplay == 2)//Mothly
            {
                foreach (var item in queryDataChart)
                {
                    vm.NutritionListChartDisplayData.Add(GetNutritionMonthlyChartData(item));
                }
            }
            else //Daily
            {
                foreach (var item in queryDataChart)
                {
                    vm.NutritionListChartDisplayData.Add(GetNutritionDailyChartData(item));
                }
            }
            return View(vm);
        }

        static NutritionRawGraphData GetNutritionDailyPlan(Nutrients name, DAOLayer.Net7.Nutrition.FnsNutritionActualDay dailyPlan, List<DayActualNutritionValueCalculationModel> totalValues)
        {
            // Gets the Calendar instance associated with a CultureInfo.
            CultureInfo localCultureInfo = new CultureInfo(CultureInfo.CurrentCulture.Name);
            Calendar localCalendar = localCultureInfo.Calendar;

            // Gets the DTFI properties required by GetWeekOfYear.
            CalendarWeekRule calendarWeekRule = localCultureInfo.DateTimeFormat.CalendarWeekRule;
            DayOfWeek firstDayOfWeek = localCultureInfo.DateTimeFormat.FirstDayOfWeek;

            var nutritionGraphData = new NutritionRawGraphData();
            nutritionGraphData.Name = name.ToString();
            nutritionGraphData.Date = dailyPlan.Date;
            nutritionGraphData.Month = dailyPlan.Date.Month;
            nutritionGraphData.Week = localCalendar.GetWeekOfYear(dailyPlan.Date, calendarWeekRule, firstDayOfWeek);

            switch (name)
            {
                case Nutrients.Calories:
                    nutritionGraphData.TotalValue = totalValues.Find(match: x => x.DayId == dailyPlan.Id).TotalCalories;
                    nutritionGraphData.TargetTotalValue = dailyPlan.DayCalorieTarget;
                    break;
                case Nutrients.Protein:
                    nutritionGraphData.TotalValue = totalValues.Find(match: x => x.DayId == dailyPlan.Id).TotalProtein;
                    nutritionGraphData.TargetTotalValue = dailyPlan.ProteinGramsTarget;
                    break;
                case Nutrients.Carbohydrates:
                    nutritionGraphData.TotalValue = totalValues.Find(match: x => x.DayId == dailyPlan.Id).TotalCarbs;
                    nutritionGraphData.TargetTotalValue = dailyPlan.CrabsGramsTarget;
                    break;
                case Nutrients.Sugars:
                    nutritionGraphData.TotalValue = totalValues.Find(match: x => x.DayId == dailyPlan.Id).TotalSugars;
                    nutritionGraphData.TargetTotalValue = dailyPlan.SugarGramsTarget;
                    break;
                case Nutrients.Fat:
                    nutritionGraphData.TotalValue = totalValues.Find(match: x => x.DayId == dailyPlan.Id).TotalFat;
                    nutritionGraphData.TargetTotalValue = dailyPlan.FatGramsTarget;
                    break;
                case Nutrients.SaturatedFat:
                    nutritionGraphData.TotalValue = totalValues.Find(match: x => x.DayId == dailyPlan.Id).TotalSaturatedFat;
                    nutritionGraphData.TargetTotalValue = dailyPlan.SaturatedFatGramsTarget;
                    break;
                case Nutrients.UnsaturatedFat:
                    nutritionGraphData.TotalValue = totalValues.Find(match: x => x.DayId == dailyPlan.Id).TotalUnsaturatedFat;
                    nutritionGraphData.TargetTotalValue = dailyPlan.UnsaturatedFatGramsTarget;
                    break;
                case Nutrients.Fiber:
                    nutritionGraphData.TotalValue = totalValues.Find(match: x => x.DayId == dailyPlan.Id).TotalFiber;
                    nutritionGraphData.TargetTotalValue = dailyPlan.FiberGramsTarget;
                    break;
                case Nutrients.Alcohol:
                    nutritionGraphData.TotalValue = totalValues.Find(match: x => x.DayId == dailyPlan.Id).TotalAlcohol;
                    nutritionGraphData.TargetTotalValue = dailyPlan.AlcoholGramsTarget;
                    break;
            }
            return nutritionGraphData;
        }
        static NutritionChartDisplayData GetNutritionDailyChartData(IGrouping<string?, NutritionRawGraphData> ListOfData)
        {
            var chartData = new NutritionChartDisplayData();
            chartData.Name = ListOfData.Key;
            foreach (var item in ListOfData)
            {
                chartData.GraphDataActual.Add(item.TotalValue);
                chartData.GraphDataTarget.Add(item.TargetTotalValue);
                chartData.GraphLabel.Add(item.Date.ToShortDateString());
            }
            return chartData;
        }
        static NutritionChartDisplayData GetNutritionMonthlyChartData(IGrouping<string?, NutritionRawGraphData> ListOfData)
        {
            var chartData = new NutritionChartDisplayData();
            chartData.Name = ListOfData.Key;
            var queryData = ListOfData.GroupBy(x => x.Month).Where(g => g.Count() > 1).Select(y => y).ToList();
            foreach (var item in queryData)
            {
                chartData.GraphDataActual.Add(item.Select(y => y.TotalValue).Sum());
                chartData.GraphDataTarget.Add(item.Select(y => y.TargetTotalValue).Sum());
                chartData.GraphLabel.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key));
            }
            return chartData;
        }
        static NutritionChartDisplayData GetNutritionWeeklyChartData(IGrouping<string?, NutritionRawGraphData> ListOfData)
        {
            var chartData = new NutritionChartDisplayData();
            chartData.Name = ListOfData.Key;
            var queryData = ListOfData.GroupBy(x => x.Week).Where(g => g.Count() > 1).Select(y => y).ToList();
            foreach (var item in queryData)
            {
                chartData.GraphDataActual.Add(item.Select(y => y.TotalValue).Sum());
                chartData.GraphDataTarget.Add(item.Select(y => y.TargetTotalValue).Sum());
                chartData.GraphLabel.Add("Week " + item.Key);
            }
            return chartData;
        }
    }
}
