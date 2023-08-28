using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp1.Areas.Exercise.ViewModels
{
    public class ExercisePageViewModel : INotifyPropertyChanged
    {
        private bool _weightHasValue = false;
        private ColumnDefinition _weightColumnDefinition;

        #region Interface

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        #endregion Interface
        public ObservableCollection<SetPageViewModel> SetviewModel { get; set; }

        public long ExerciseId { get; set; }

        public long Exercise_type_id { get; set; }
        public string ExerciseName { get; set; }

        public long TraningSessionId { get; set; }

        public string MetricsName1 { get; set; }
        public string MetricsName2 { get; set; }

        //public string VideoUrl { get; set; }
        //public string VideoUrl { 
        //    get {
        //      //  return "https://aiqtoolstoragedev.blob.core.windows.net/music/Sultans.mp4";
        //        //return "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"; 
        //    }
        // }

        public bool WeightHasValue
        {
            get { return _weightHasValue; }
            set
            {
                _weightHasValue = value;
                OnPropertyChanged(nameof(WeightHasValue));
            }
        }

        public ColumnDefinition WeightColumnDefinition
        {
            get { return _weightColumnDefinition; }
            set
            {
                _weightColumnDefinition = value;
                OnPropertyChanged(nameof(WeightColumnDefinition));
            }
        }

        public string ExplainerText { get; set; }

        private bool isRecordExerciseTabContentVisible;
        public bool IsRecordExerciseTabContentVisible
        {
            get { return isRecordExerciseTabContentVisible; }
            set
            {
                isRecordExerciseTabContentVisible = value;
                OnPropertyChanged(nameof(IsRecordExerciseTabContentVisible));
            }
        }


        private bool isSummaryTabContentVisible;
        public bool IsSummaryTabContentVisible
        {
            get { return isSummaryTabContentVisible; }
            set
            {
                isSummaryTabContentVisible = value;
                OnPropertyChanged(nameof(IsSummaryTabContentVisible));
            }
        }

        private bool isHistoryTabContentVisible;
        public bool IsHistoryTabContentVisible
        {
            get { return isHistoryTabContentVisible; }
            set
            {
                isHistoryTabContentVisible = value;
                OnPropertyChanged(nameof(IsHistoryTabContentVisible));
            }
        }

        private bool isSetContentVisible;
        public bool IsSetContentVisible
        {
            get { return isSetContentVisible; }
            set
            {
                isSetContentVisible = value;
                OnPropertyChanged(nameof(IsSetContentVisible));
            }
        }

       

        public bool IsCustomerAddedExercise { get; internal set; }
        public bool IsExerciseComplete { get; internal set; }
        public bool IsExerciseSkipped { get; internal set; }
        public DateTime? ExerciseEndTimeStamp { get; internal set; }


    }

}
