using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Dashboard.ViewModel
{
    public class ExerciseFeedItemDashboardViewModel : INotifyPropertyChanged
    {

        public string Title { get; set; }

        public string Status { get; set; }
        public string TimeCompleted { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
