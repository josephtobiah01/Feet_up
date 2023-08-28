using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Exercise.ViewModels
{
    public class SetPageViewModel : INotifyPropertyChanged
    {
        private bool _weightHasValue = false;
        private ColumnDefinition _weightColumnDefinition;


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public long SetId { get; set; }
        public long ExerciseId { get; set; }
        public long Exercise_type_Id { get; set; }
        public string SetName { get; set; }
        public string SetSequenceNumber { get; set; }
        public bool IsSkipped { get; internal set; }
        public bool IsCustomerAddedSet { get; internal set; }
        public DateTimeOffset? TimeOffset { get; internal set; }
        public bool IsComplete { get; internal set; }
        public DateTime? EndTimeStamp { get; set; }


        public int SetRestTimeSecs { get; set; }
        public bool IsSetonBreakInv { get; set; }


        public long MetricId1 { get; set; }
        public long MetricId2 { get; set; }
        public string MetricsName1 { get; set; }
        public string MetricsName2 { get; set; }

        public string MetricsValue1 { get; set; }
        public string MetricsValue2 { get; set; }

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
    }

}
