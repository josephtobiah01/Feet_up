using DAOLayer.Net7.Exercise;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace FitappAdminWeb.Net7.Models
{
    public class TrainingProgramViewModel
    {
        public User? CurrentUser { get; set; }
        public IdentityUser? IdentityUser { get; set; }
        public List<Eds12weekPlan> Programs { get; set; } = new List<Eds12weekPlan>();
    }

    public class TrainingSessionViewModel
    {
        public User? CurrentUser { get; set; }
        public IdentityUser? IdentityUser { get; set; }
        public Eds12weekPlan? CurrentProgram { get; set; }
        public DateTime? SelectedDate { get; set; }
        public long SelectedProgramId { get; set; }
        public List<Eds12weekPlan> Programs { get; set; } = new List<Eds12weekPlan>();
        public List<EdsTrainingSession> TrainingSessions { get; } = new List<EdsTrainingSession>();

        public List<SelectListItem> List_Programs { get; set; } = new List<SelectListItem>();
    }

    public class TrainingSessionEditViewModel
    {
        public User? CurrentUser { get; set; }
        public TrainingSessionEditFormModel? Data { get; set; } = new TrainingSessionEditFormModel();
        public bool IsCopy { get; set; }

        public List<SelectListItem> Select_ExerciseTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Select_MetricTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Select_Program { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Select_Template { get; set; } = new List<SelectListItem>();
    }  

    public class TrainingSessionEditFormModel
    {
        public string? Name { get; set; }
        public DateTime? StartDateTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:hh':'mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? StartDateTime_TimeSpan { get; set; }
        public DateTime? EndDateTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:hh':'mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? EndDateTime_TimeSpan { get; set; }
        public string? Description { get; set; }
        public long TrainingSessionId { get; set; }
        public long UserId { get; set; }
        public long Eds12WeekProgramId { get; set; }
        public bool IsTemplate { get; set; }
    }

    public class TrainingProgramEditViewModel
    {
        public User? CurrentUser { get; set; }
        public TrainingProgramEditFormModel Data { get; set; } = new TrainingProgramEditFormModel();

        public List<SelectListItem> List_ProgramTemplates { get; set; }
}

    public class TrainingProgramEditFormModel
    {
        public long UserId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int DurationWeeks { get; set; }
        //[DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? StartDate { get; set; }
        //[DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? EndDate { get; set; }
        public long? LoadTemplateId { get; set; }
        public bool IsTemplate { get; set; }
        public bool IsCurrent { get; set; }
    }

    public class ProgramDetailViewModel
    {
        public Eds12weekPlan? CurrentPlan { get; set; }

        public CopyWeekFormModel CopyWeekModel { get; set; } = new CopyWeekFormModel();

        public float ExerciseComplianceRate { get; set; }
        public float CompletedExercises { get; set; }
        public float SkippedExercises { get; set; }
        public float PendingExercises { get; set; }
        public float TotalExercises { get; set; }
        public string MostCompletedExercise { get; set; }
        public string MostSkippedExercise { get; set; }

        public float SetComplianceRate { get; set; }
        public float CompletedSets { get; set; }
        public float SkippedSets { get; set; }
        public float PendingSets { get; set; }
        public float TotalSets { get; set; }

        public float CompletedExercisesPercentage { get { return CompletedExercises / TotalExercises * 100; } }
        public float SkippedExercisesPercentage { get { return SkippedExercises / TotalExercises * 100; } }
        public float PendingExercisesPercentage { get { return PendingExercises / TotalExercises * 100; } }
        public float CompletedSetPercentage { get { return CompletedSets / TotalSets * 100; } }
        public float SkippedSetPercentage { get { return SkippedSets / TotalSets * 100; } }
        public float PendingSetPercentage { get { return PendingSets / TotalSets * 100; } }
    }

    public class CopyWeekFormModel
    {
        [Required]
        public long? WeekId { get; set; }       
        [Required]
        public DateTime? StartDay { get; set; }
    }

    public class TrainingGraphViewModel
    {
        public User? CurrentUser { get; set; }
        public List<Eds12weekPlan> Programs { get; set; } = new List<Eds12weekPlan>();
        public Eds12weekPlan? CurrentProgram { get; set; }
        public List<SelectListItem> List_Programs { get; set; } = new List<SelectListItem>();
        public List<TrainingChartDisplayData> TrainingListChartDisplayData { get; set; } = new List<TrainingChartDisplayData>();
    }

    public class TrainingChartDisplayData
    {
        public string? Name { get; set; }
        public string? Id { get; set; }
        public List<double> GraphDataTotal { get; set; } = new List<double>();
        public List<double> GraphDataComplete { get; set; } = new List<double>();
        public List<double> GraphDataSkipped { get; set; } = new List<double>();
        public List<double> GraphDataInComplete { get; set; } = new List<double>();
        public List<string> GraphLabel { get; set; } = new List<string>();
    }
}
