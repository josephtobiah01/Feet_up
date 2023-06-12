using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Supplement.ViewModels
{
    public class AddSupplementPageViewModel : INotifyPropertyChanged
    {

        public ObservableCollection<TimeOnly> SupplementTimes { get; set; }

        private bool _isSunday;
        public bool IsSunday
        {
            get
            {
                return _isSunday;
            }
            set
            {
                _isSunday = value;
                OnPropertyChanged();
            }
        }

        private bool _isMonday;
        public bool IsMonday
        {
            get
            {
                return _isMonday;
            }
            set
            {
                _isMonday = value;
                OnPropertyChanged();
            }
        }

        private bool _isTuesday;
        public bool IsTuesday
        {
            get
            {
                return _isTuesday;
            }
            set
            {
                _isTuesday = value;
                OnPropertyChanged();
            }
        }

        private bool _isWednesday;
        public bool IsWednesday
        {
            get
            {
                return _isWednesday;
            }
            set
            {
                _isWednesday = value;
                OnPropertyChanged();
            }
        }

        private bool _isThursday;
        public bool IsThursday
        {
            get
            {
                return _isThursday;
            }
            set
            {
                _isThursday = value;
                OnPropertyChanged();
            }
        }

        private bool _isFriday;
        public bool IsFriday
        {
            get
            {
                return _isFriday;
            }
            set
            {
                _isFriday = value;
                OnPropertyChanged();
            }
        }

        private bool _isSaturday;
        public bool IsSaturday
        {
            get
            {
                return _isSaturday;
            }
            set
            {
                _isSaturday = value;
                OnPropertyChanged();
            }
        }

        private bool _isBeforeMeal;
        public bool IsBeforeMeal
        {
            get
            {
                return _isBeforeMeal;
            }
            set
            {
                _isBeforeMeal = value;
                OnPropertyChanged();
            }
        }

        private bool _isAfterMeal;
        public bool IsAfterMeal
        {
            get { return _isAfterMeal; }
            set
            {
                _isAfterMeal = value;
                OnPropertyChanged();
            }
        }

        private bool _isEmptyStomach;
        public bool IsEmptyStomach
        {
            get { return _isEmptyStomach; }
            set
            {
                _isEmptyStomach = value;
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
