using ParentMiddleWare.Models;
using System.Drawing;

namespace FeedApi.Net7.Models
{

    public enum FeedItemStatus
    {
        Scheduled = 1,
        Skipped = 2,
        Ongoing = 3,
        Snoozed = 4,
        Completed = 5,
        Error = 6,
        Partly_Skipped = 7,
    }

    public enum FeedItemType
    {
        TrainingSessionFeedItem,
        NutrientsFeedItem,
        MedicationFeedItem,
        HabitsFeedItem,
        SupplementItem

    }

    public enum TextCategory
    {
        Description = 1,
        Estimated_Time = 2,
        Target_Muscles = 3,
        Supplement_Count = 4,
        Nutrient_Calories = 5,
        Nutrient_Note = 6,
        Supplement_Instructions = 7,
    }



    public class TextPair
    {
        public TextPair()
        {

        }
        public TextPair(TextCategory category, string text)
        {
            this.Text = text;
            this.TextCategory = category;
        }
        public TextCategory TextCategory { get; set; }
        public string Text { get; set; }
    }

    public class FeedItem
    {

        public FeedItemType ItemType { get; set; }
        public FeedItemStatus Status { get; set; }
        // The Date when it's starting. Clients calculate the remaining minutes 
        public DateTime Date { get; set; }
        
        // Each card has a title
        public string Title { get; set; }

        public List<TextPair> Text { get; set; }

        public TrainingSessionFeedItem TrainingSessionFeedItem { get; set; }
        public SupplementFeedItem SupplementFeedItem { get; set; }
        public NutrientsFeedItem NutrientsFeedItem { get; set; }
        public MedicationFeedItem MedicationFeedItem { get; set; }
        public HabitsFeedItem HabitsFeedItem { get; set; }
    }

    public class TrainingSessionFeedItem
    {
        public EmTrainingSession TraningSession { get; set; }
    
    }

    public class SupplementFeedItem
    {
        public long SupplementScheduleID { get; set; }
        public List<SupplementEntry> SupplementEntries;
    }

    public class SupplementEntry
    {
        //  public NdSupplementPlanSupplement SupplementPlanSupplement { get; set; }

        public long SupplementId { get; set; }
        public long DoseId { get; set; }
        public string Supplementname { get; set; }
        public DateTime ScheduledTime { get; set; }
        public float UnitCount { get; set; }

        // for displaying warning / restriction when user changes dose
        public float DoseWarningLimit { get; set; }
        public float DoseHardLimit { get; set; }


        // unit specific
        public string UnitName { get; set; } // i.e "Tablet"
        public bool is_Weight { get; set; }
        public bool is_Volume { get; set; }
        public bool is_Count { get; set; }


        // instructions
        public string Instructions { get; set; }
        public bool Requires_source_of_fat { get; set; }
        public bool Take_after_meal { get; set; }
        public bool Take_before_sleep { get; set; }
        public bool Take_on_empty_stomach { get; set; }

        public bool isComplete { get; set; }
        public bool isSnoozed { get; set; }
        public int  SnoozedTimeMinutes { get; set; }
    }

    public class NutrientsFeedItem 
    {
       public NutrientMeal Meal { get; set; }

    }
    public class MedicationFeedItem 
    {

    }

    public class HabitsFeedItem 
    {

    }
}
