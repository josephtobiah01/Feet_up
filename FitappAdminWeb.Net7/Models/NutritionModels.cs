using DAOLayer.Net7.Nutrition;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using X.PagedList;

namespace FitappAdminWeb.Net7.Models
{
    public class DailyMealEditModel
    {
        public long Id { get; set; }
        public DateTime? Date { get; set; } = DateTime.Now;
        public int? NumberOfDaysToCopy { get; set; }
        public bool IsCopy { get; set; }

        public User CurrentUser { get; set; }

        public List<SelectListItem> MealTypeList { get; set; } = new List<SelectListItem>();
    }

    public class MealQueueViewModel
    {
        public List<FnsNutritionActualDish> DishesForTranscription { get; set; } = new List<FnsNutritionActualDish>();
        public MealQueueAddDishModel DishToAdd { get; set; } = new MealQueueAddDishModel();
        public bool IncludeTestAccounts { get; set; } = false;

        public List<SelectListItem> MealTypeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> UserList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> DishTypeList { get; set; } = new List<SelectListItem>();
        public PagedList<FnsNutritionActualDish>? PagingDishesForTranscription { get; set; }
    }

    public class MealQueueAddDishModel
    {
        [Required]
        public DateTime Day { get; set; } = DateTime.Now.Date;

        public TimeSpan Time { get; set; } = DateTime.Now.TimeOfDay;

        [Required]
        public long? MealTypeId { get; set; }

        [Required]
        public long? DishTypeId { get; set; }

        [Required]
        public string? ImageUrl { get; set; } = "/img/sampleimg.png";

        [Required]
        public long? UserId { get; set; }
    }

    public class MealEditDishModel
    {
        public FnsNutritionActualDish? NutritionActualDish { get; set; }
        public List<SelectListItem> DishTypeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> TranscriptionTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ErrorCode { get; set; } = new List<SelectListItem>();

        public DishEditFormModel Data { get; set; } = new DishEditFormModel();
        public User SubmittedBy { get; set; }
        public DateTime SubmittedOn { get; set; }
        public string UserRemarks { get; set; }
        public long MealId { get; set; }
        public string MealType { get; set; }

        public MealEditDishModel(FnsNutritionActualDish? nutritionActualDish)
        {
            NutritionActualDish = nutritionActualDish;
        }
    }

    public class DishEditFormModel
    {
        public long Id { get; set; }

        [Required]
        public long FkDishTypeId { get; set; }

        [Required]
        public long? FkDishTranscriptionTypeId { get; set; }

        public bool IsComplete { get; set; }

        public bool IsError { get; set; }

        public DateTime? ErrorTimestamp { get; set; }

        public long? FkErrorCodeId { get; set; }

        public DateTime? CompletionTimestamp { get; set; }

        [Required]
        public string Name { get; set; }

        //public string? Remarks { get; set; } = String.Empty;

        public string? TranscriberRemarks { get; set; } = String.Empty;

        public double ShareOfDishConsumed { get; set; }

        [Required]
        public int ShareOfDishConsumed_PP { get; set; }

        [Required]
        public double NumberOfServingsConsumed { get; set; }

        //public long? FkReuseReferenceId { get; set; }

        //public string UploadPhotoReference { get; set; }

        public long? FkTranscriberId { get; set; }

        public long? FkUserRecipeId { get; set; }

        public double? CalorieActual { get; set; }

        public double? ProteinActual { get; set; }

        public double? CrabsActual { get; set; }

        public double? SugarActual { get; set; }

        public double? FatActual { get; set; }

        public double? UnsaturatedFatActual { get; set; }

        public double? FiberGramsActual { get; set; }

        public double? SaturatedFatGramsActual { get; set; }

        public double? AlcoholGramsActual { get; set; }
    }

    public class NutritionDailyPlanViewModel
    {
        public FnsNutritionActualDay? CurrentDailyPlan { get; set; }

        public List<FnsNutritionActualDay> DailyPlans { get; set; } = new List<FnsNutritionActualDay>();
        public List<DayActualNutritionValueCalculationModel> TotalValues { get; set; } = new List<DayActualNutritionValueCalculationModel>();
        public DateTime? LatestDayWithPlan { get; set; }

        //search criteria
        public DateTime Day { get; set; } = DateTime.Now.Date;

        public DateTime? EndDay { get; set; }

        public User CurrentUser { get; set; }
    }

    public class NutritionDailyPlanEditModel
    {
        public long Id { get; set; }
        public User FkUser { get; set; }
        public DateTime Date { get; set; }
        public double DayCalorieTarget { get; set; }

        public double DayCalorieTargetMin { get; set; }

        public double DayCalorieTargetMax { get; set; }

        public double ProteinGramsTarget { get; set; }

        public double CrabsGramsTarget { get; set; }

        public double SugarGramsTarget { get; set; }

        public double FatGramsTarget { get; set; }

        public double UnsaturatedFatGramsTarget { get; set; }

        public double FiberGramsTarget { get; set; }

        public double AlcoholGramsTarget { get; set; }
    }

    public class DayActualNutritionValueCalculationModel
    {
        public long DayId { get; set; }
        public double TotalCalories { get; set; }
        public double TotalProtein { get; set; }
        public double TotalCarbs { get; set; }
        public double TotalSugars { get; set; }
        public double TotalFat { get; set; }
        public double TotalUnsaturatedFat { get; set; }
        public double TotalSaturatedFat { get; set; }
        public double TotalFiber { get; set; }
        public double TotalAlcohol { get; set; }

        public List<MealActualNutritionalValueCalculationModel> MealTotals { get; set; } = new List<MealActualNutritionalValueCalculationModel>();
    }

    public class MealActualNutritionalValueCalculationModel
    {
        public long MealId { get; set; }
        public double TotalCalories { get; set; }
        public double TotalProtein { get; set; }
        public double TotalCarbs { get; set; }
        public double TotalSugars { get; set; }
        public double TotalFat { get; set; }
        public double TotalUnsaturatedFat { get; set; }
        public double TotalSaturatedFat { get; set; }
        public double TotalFiber { get; set; }
        public double TotalAlcohol { get; set; }

        public List<DishActualNutritionalValueCalculationModel> DishTotal { get; set; } = new List<DishActualNutritionalValueCalculationModel>();
    }

    public class DishActualNutritionalValueCalculationModel
    {
        public long DishId { get; set; }
        public double TotalCalories { get; set; }
        public double TotalProtein { get; set; }
        public double TotalCarbs { get; set; }
        public double TotalSugars { get; set; }
        public double TotalFat { get; set; }
        public double TotalUnsaturatedFat { get; set; }
        public double TotalSaturatedFat { get; set; }
        public double TotalFiber { get; set; }
        public double TotalAlcohol { get; set; }
    }
    public class NutritionDailyPlanGraphViewModel
    {
        public List<FnsNutritionActualDay> DailyPlans { get; set; } = new List<FnsNutritionActualDay>();

        public List<DayActualNutritionValueCalculationModel> TotalValues { get; set; } = new List<DayActualNutritionValueCalculationModel>();

        //ranged criteria
        public DateTime Day { get; set; } = DateTime.Now.Date;

        public DateTime? EndDay { get; set; }

        public long ChartDisplayId { get; set; }

        public User? CurrentUser { get; set; }

        public List<SelectListItem> ChartDisplay { get; set; } = new List<SelectListItem>()
        {
            new SelectListItem {Text="Daily",Value="0",Selected=true },
            new SelectListItem {Text="Weekly",Value="1"},
            new SelectListItem {Text="Monthly",Value="2"}
        };

        public List<NutritionRawGraphData> NutritionListRawGraphData { get; set; } = new List<NutritionRawGraphData>();

        public List<NutritionChartDisplayData> NutritionListChartDisplayData { get; set; } = new List<NutritionChartDisplayData>();
    }

    public enum Nutrients
    {
        Calories,
        Protein,
        Carbohydrates,
        Sugars,
        Fat,
        SaturatedFat,
        UnsaturatedFat,
        Fiber,
        Alcohol
    }

    public class NutritionRawGraphData
    {
        public string? Name { get; set; }
        public double TotalValue { get; set; }
        public double TargetTotalValue { get; set; }
        public DateTime Date { get; set; }
        public int Month { get; set; }
        public int Week { get; set; }
    }

    public class NutritionChartDisplayData
    {
        public string? Name { get; set; }
        public List<double> GraphDataActual { get; set; } = new List<double>();
        public List<double> GraphDataTarget { get; set; } = new List<double>();
        public List<string> GraphLabel { get; set; } = new List<string>();
    }
}
