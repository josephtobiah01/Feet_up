namespace FitappAdminWeb.Net7.Classes.DTO
{
    public class ActualDish_DTO
    {
        public long Id { get; set; }

        public long FkDishTypeId { get; set; }

        public long? FkDishTranscriptionTypeId { get; set; }

        public bool IsComplete { get; set; }

        public bool IsError { get; set; }

        public bool IsFavorite { get; set; }

        public bool IsFrequent { get; set; }

        public DateTime CreationTimestamp { get; set; }

        public DateTime? ErrorTimestamp { get; set; }

        public long? FkErrorCodeId { get; set; }
        public string Name { get; set; }

        public string Remarks { get; set; }

        public string TranscriberRemarks { get; set; }

        public double ShareOfDishConsumed { get; set; }

        public double NumberOfServingsConsumed { get; set; }

        public long? FkReuseReferenceId { get; set; }

        public string UploadPhotoReference { get; set; }

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

    public class ActualDay_DTO
    {
        public long Id { get; set; }

        public long FkUserId { get; set; }

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

        public double SaturatedFatGramsTarget { get; set; }

        public List<ActualMeal_DTO> Meals { get; set; } = new List<ActualMeal_DTO>();

        //custom
        public int DaysToExtrapolate { get; set; }
    }

    public class ActualMeal_DTO
    {
        public long Id { get; set; }

        public long MealTypeId { get; set; }

        public bool HasTarget { get; set; }

        public double MealCalorieTarget { get; set; }

        public double MealCalorieMin { get; set; }

        public double MealCalorieMax { get; set; }

        public double ProteinGramsTarget { get; set; }

        public double CrabsGramsTarget { get; set; }

        public double SugarGramsTarget { get; set; }

        public double FatGramsTarget { get; set; }

        public double UnsaturatedFatGramsTarget { get; set; }

        public double SaturatedFatGramsTarget { get; set; }

        public double FiberGramsTarget { get; set; }

        public double AlcoholGramsTarget { get; set; }

        public bool IsComplete { get; set; }

        public bool IsOngoing { get; set; }

        public DateTime? ScheduledTime
        {
            get
            {
                if (Date.HasValue && ScheduledTime_TimeSpan.HasValue)
                {
                    return Date.Value.Add(ScheduledTime_TimeSpan.Value);
                }
                return null;
            }
        }

        //DTO Specific Items
        public DateTime? Date { get; set; }

        public TimeSpan? ScheduledTime_TimeSpan { get; set; }
    }
}
