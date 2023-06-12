namespace FitappAdminWeb.Net7.Classes.DTO
{

    public class SupplementPlanWeekly_DTO
    {
        public long Id { get; set; }
        public List<SupplementPlanDaily_DTO> Days { get; set; } = new List<SupplementPlanDaily_DTO>();
        public bool IsActive { get; set; }
        public bool IsTemplate { get; set; }
        public bool ForceScheduleSync { get; set; }
        public string? Remark { get; set; }
        public long? UserId { get; set; }
    }

    public class SupplementPlanDaily_DTO
    {
        public long Id { get; set; }
        public int DayOfWeek { get; set; }
        public List<SupplementPlanSupplement_DTO> Supplements { get; set; } = new List<SupplementPlanSupplement_DTO>();
    }

    public class SupplementPlanSupplement_DTO
    {
        public long Id { get; set; }
        public long FkSupplementReference { get; set; }
        public bool IsFreeEntry { get; set; }
        public string? FreeEntryName { get; set; }
        public long? FkFreeEntryUnitMetric { get; set; }
        public string? Remark { get; set; }
        public List<SupplementPlanDose_DTO> Doses { get; set; } = new List<SupplementPlanDose_DTO>();
    }

    public class SupplementPlanDose_DTO
    {
        public long Id { get; set; }
        public double UnitCount { get; set; }
        public double? DoseWarningLimit { get; set; }
        public double? DoseHardCeilingLimit { get; set; }

        public TimeSpan? ScheduledTime { get; set; }

        //private DateTime _ScheduledTime;
        //public DateTime ScheduledTime
        //{
        //    get
        //    {
        //        if (this.ScheduledTime_TimeSpan.HasValue)
        //        {
        //            return DateTime.Now.Date.Add(this.ScheduledTime_TimeSpan.Value);
        //        }
        //        else return _ScheduledTime;
        //    }
        //    set
        //    {
        //        _ScheduledTime = value;
        //    }
        //}
        public string? Remark { get; set; }
    }
}
