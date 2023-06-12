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
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public long SetId { get; set; }
        public long ExerciseId { get; set; }
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
    }

}
