using FeedApi.Net7.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Supplement.ViewModels
{
    public class SupplementPageViewModel : INotifyPropertyChanged
    {

        public long SupplementId { get; set; }
        public long SupplementDoseId { get; set; }

        public string SupplementName { get; set; }

        /// <summary>
        /// /// Properties: ScheduledTime
        /// Mapping: NdSupplementList.TimeString
        /// </summary>
        public DateTime ScheduledTime { get; set; }

        public string TimeString { get; set; }
        public int SnoozedTimeMinutes { get; set; }

        //public DateTime FromDate { get; set; }
        //public DateTime ToDate { get; set; }


        /// <summary>
        /// /// Properties: UnitCount
        /// Mapping: NdSupplementList.Ammount
        /// </summary>
        public float UnitCount { get; set; }
        public float DoseWarningLimit { get; set; }
        public float DoseHardLimit { get; set; }


        /// <summary>
        /// /// Properties: UnitName
        /// Mapping: NdSupplementList.Type
        /// </summary>
        public string UnitName { get; set; } // i.e "Tablet"
        public bool? is_Weight { get; set; }
        public bool? is_Volume { get; set; }
        public bool? is_Count { get; set; }


        /// <summary>
        /// Properties: Instructions
        /// Mapping: NdSupplementList.FoodRemark
        /// </summary>
        public string Instructions { get; set; }
        public bool? Requires_source_of_fat { get; set; }
        public bool? Take_after_meal { get; set; }
        public bool? Take_before_sleep { get; set; }
        public bool? Take_on_empty_stomach { get; set; }

        /// <summary>
        /// /// Properties: TimeRemark
        /// Mapping: NdSupplementList.TimeRemark
        /// </summary>
        public string TimeRemark { get; set; } //Mapping: NdSupplementList.TimeRemark

        /// <summary>
        /// /// Properties: FrequencyRemark
        /// Mapping: NdSupplementList.Frequency
        /// </summary>
        public string FrequencyRemark { get; set; } //Mapping: NdSupplementList.Frequency

        public bool is_Monday { get; set; }
        public bool is_Tuesday { get; set; }
        public bool is_Wednesday { get; set; }
        public bool is_Thrusday { get; set; }
        public bool is_Friday { get; set; }
        public bool is_Saturday { get; set; }
        public bool is_Sunday { get; set; }

        //public Dictionary<TextCategory, string> TextDictionary { get; set; }

        private bool isSnoozed;
        public bool IsSnoozed
        {
            get { return isSnoozed; }
            set
            {
                isSnoozed = value;
                OnPropertyChanged();
            }
        }

        private bool isSkipped;
        public bool IsSkipped
        {
            get { return isSkipped; }
            set
            {
                isSkipped = value;
                OnPropertyChanged();
            }
        }

        private bool isDone;
        public bool IsDone
        {
            get { return isDone; }
            set
            {
                isDone = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch
            {
                App.alertBottomSheetManager.ShowAlertMessage("Error", "An error occurred.", "OK");
            }
        }
    }

    public enum TextCategory
    {
        Description = 1,
        Before_Meal = 2
    }

}
