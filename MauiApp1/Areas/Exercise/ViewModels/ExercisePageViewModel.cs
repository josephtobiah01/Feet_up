using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp1.Areas.Exercise.ViewModels
{
    public class ExercisePageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<SetPageViewModel> SetviewModel { get; set; }
        public long ExerciseId { get; set; }

        public string ExerciseName { get; set; }

        public long TraningSessionId { get; set; }

        public string MetricsName1 { get; set; }
        public string MetricsName2 { get; set; }

        //public string VideoUrl { get; set; }
        public string VideoUrl { 
            get {
                return "https://aiqtoolstoragedev.blob.core.windows.net/music/Sultans.mp4";
                //return "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"; 
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
                OnPropertyChanged();
            }
        }


        private bool isSummaryTabContentVisible;
        public bool IsSummaryTabContentVisible
        {
            get { return isSummaryTabContentVisible; }
            set
            {
                isSummaryTabContentVisible = value;
                OnPropertyChanged();
            }
        }

        private bool isHistoryTabContentVisible;
        public bool IsHistoryTabContentVisible
        {
            get { return isHistoryTabContentVisible; }
            set
            {
                isHistoryTabContentVisible = value;
                OnPropertyChanged();
            }
        }

        private bool isSetContentVisible;
        public bool IsSetContentVisible
        {
            get { return isSetContentVisible; }
            set
            {
                isSetContentVisible = value;
                OnPropertyChanged();
            }
        }

       

        public bool IsCustomerAddedExercise { get; internal set; }
        public bool IsExerciseComplete { get; internal set; }
        public bool IsExerciseSkipped { get; internal set; }
        public DateTime? ExerciseEndTimeStamp { get; internal set; }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }

}
