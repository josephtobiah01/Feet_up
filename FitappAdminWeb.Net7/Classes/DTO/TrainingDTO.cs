using DAOLayer.Net7.Exercise;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace FitappAdminWeb.Net7.Classes.DTO
{
    [Serializable]
    public class TrainingSessionDTO
    {
        public long UserId { get; set; }
        public long Eds12WeekPlanId { get; set; }
        public long TrainingSessionID { get; set; }
        public string? Name { get; set; }
        public DateTime StartDateTime { get; set; }       
        public TimeSpan StartDateTime_TimeSpan { get; set; }
        public DateTime EndDateTime { get; set; }
        public TimeSpan EndDateTime_TimeSpan { get; set; }
        public string Description { get; set; }
        public bool IsTemplate { get; set; }

        //NOTE: Only add these properties if they are subject to change via update/insert
        //public bool hasBeenMoved { get; set; }
        //public bool IsSkipped { get; set; }
        //public DateTime? EndTimeStamp { get; set; }
        //public bool IsCustomerAddedTrainingSession { get; set; }
        //public int? ReasonForReschedule { get; set; }
        //public int? ReasonForSkipping { get; set; }
        //public float? ExerciseRating { get; set; }
        //public string? ExerciseMessage { get; set; }


        public IEnumerable<ExerciseDTO>? Exercises { get; set; } = new List<ExerciseDTO>();
    }

    public class ExerciseDTO
    {
        public long ExerciseID { get; set; }     
        public long ExerciseTypeID { get; set; }

        //NOTE: Only add these properties if they are subject to change via update/insert
        //public long TrainingID { get; set; }
        public bool IsSkipped { get; set; }
        public bool IsComplete { get; set; }
        //public DateTime? EndTimeStamp { get; set; }
        //public bool IsCustomerAddedExercise { get; set; }

        public IEnumerable<SetDTO>? Sets { get; set; } = new List<SetDTO>();
    }

    public class SetDTO
    {
        public long SetID { get; set; }
        public int SetSequenceNumber { get; set; }

        //NOTE: Only add these properties if they are subject to change via update/insert
        public bool IsComplete { get; set; }
        public bool IsSkipped { get; set; }
        //public DateTime? EndTimeStamp { get; set; }
        //public bool IsCustomerAddedSet { get; set; }
        //public long ExerciseID { get; set; }

        public IEnumerable<SetMetricDTO> SetMetrics { get; set; } = new List<SetMetricDTO>();
    }

    public class SetMetricDTO
    {
        public long SetMetricID { get; set; }
        public long SetMetricTypeID { get; set; }       
        public string? TargetCustomMetric { get; set; }

        //NOTE: Only add these properties if they are subject to change via update/insert
        public string? ActualCustomMetric { get; set; }
        //public long SetID { get; set; }
    }

    public class ProgramDTO
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public bool IsTemplate { get; set; }

        public bool IsCurrent { get; set; }

        public short DurationWeeks { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public long UserId { get; set; }

        public long TemplateId { get; set; }

        public List<WeeklyPlanDTO> WeeklyPlans { get; set; } = new List<WeeklyPlanDTO>();
    }

    public class WeeklyPlanDTO
    {
        public long Id { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<DailyPlanDTO> DailyPlans { get; set; } = new List<DailyPlanDTO>();
    }
    
    public class DailyPlanDTO
    {
        public long Id { get; set; }

        public DateTime? StartDay { get; set; }

        public bool IsComplete { get; set; }
    }

    public class CopyWeekResult_DTO
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public long? WeekId { get; set; }
    }
}
