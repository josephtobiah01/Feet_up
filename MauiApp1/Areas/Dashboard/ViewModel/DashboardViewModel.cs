using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Dashboard.ViewModel
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private bool _showAllDashboard;
        public bool ShowAllDashboard
        {
            get
            {
                return _showAllDashboard;
            }
            set
            {
                _showAllDashboard = value;
                OnPropertyChanged();
            }
        }

        private bool _showNutrientsDashboard;
        public bool ShowNutrientsDashboard
        {
            get
            {
                return _showNutrientsDashboard;
            }
            set
            {
                _showNutrientsDashboard = value;
                OnPropertyChanged();
            }
        }

        private bool _showExerciseDashboard;
        public bool ShowExerciseDashboard
        {
            get
            {
                return _showExerciseDashboard;
            }
            set
            {
                _showExerciseDashboard = value;
                OnPropertyChanged();
            }
        }

        private bool _showSleepDashboard;
        public bool ShowSleepDashboard
        {
            get
            {
                return _showSleepDashboard;
            }
            set
            {
                _showSleepDashboard = value;
                OnPropertyChanged();
            }
        }

        private bool _showMindfulnessDashboard;
        public bool ShowMindfulnessDashboard
        {
            get
            {
                return _showMindfulnessDashboard;
            }
            set
            {
                _showMindfulnessDashboard = value;
                OnPropertyChanged();
            }
        }

        private bool _showHabitDashboard;
        public bool ShowHabitDashboard
        {
            get
            {
                return _showHabitDashboard;
            }
            set
            {
                _showHabitDashboard = value;
                OnPropertyChanged();
            }
        }

        private bool _showSupplementDashboard;
        public bool ShowSupplementDashboard
        {
            get
            {
                return _showSupplementDashboard;
            }
            set
            {
                _showSupplementDashboard = value;
                OnPropertyChanged();
            }
        }

        private bool _showFastingDashboard;
        public bool ShowFastingDashboard
        {
            get
            {
                return _showFastingDashboard;
            }
            set
            {
                _showFastingDashboard = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
