using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Dashboard.ViewModel
{
    public  class DailyNutrientDetailViewModel : INotifyPropertyChanged
    {
        private NutrientsIntakeViewItem _nutrientsIntakeViewItem { get; set; }

        // to draw first graph
        public NutrientsIntakeViewItem NutrientsIntakeViewItem
        {
            get
            {
                return _nutrientsIntakeViewItem;
            }
            set
            {
                _nutrientsIntakeViewItem = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<MealDetailViewModel> _mealDetails { get; set; }

        // draw the breakfeast, lunch diner bar
        // draw 2nd graph
        public ObservableCollection<MealDetailViewModel> MealDetails
        {
            get
            {
                return _mealDetails;
            }
            set
            {
                _mealDetails = value;
                OnPropertyChanged();
            }
        }

        private DateTime _selectedDate;

        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
