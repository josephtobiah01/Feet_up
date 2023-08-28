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
    public class MealDetailViewModel : INotifyPropertyChanged
    {
        private string mealName { get; set; }

        public string MealName
        {
            get { return mealName; }
            set
            {
                mealName = value;
                OnPropertyChanged();
            }
        }

        //for stacked graph on top
        private bool isClickable { get; set; }

        public bool IsClickable
        {
            get { return isClickable; }
            set
            {
                isClickable = value;
                OnPropertyChanged();
            }
        }

        private bool _isClicked { get; set; }

        public bool IsClicked
        {
            get { return _isClicked; }
            set
            {
                _isClicked = value;
                OnPropertyChanged();
            }
        }

        // for meal specific
        public ObservableCollection<DishItemViewModel> MealSpecificNutrientOverView { get; set; }

        // to draw the 2nd graph (the meals)
        private long targetCalories { get; set; }

        public long TargetCalories
        {
            get { return targetCalories; }
            set
            {
                targetCalories = value;
                OnPropertyChanged();
            }
        }

        private long currentCalories { get; set; }

        public long CurrentCalories
        {
            get { return currentCalories; }
            set
            {
                currentCalories = value;
                OnPropertyChanged();
            }
        }

        private long proteinGram { get; set; }

        public long ProteinGram
        {
            get { return proteinGram; }
            set
            {
                proteinGram = value;
                OnPropertyChanged();
            }
        }

        private long carbsGram { get; set; }

        public long CarbsGram
        {
            get { return carbsGram; }
            set
            {
                carbsGram = value;
                OnPropertyChanged();
            }
        }

        private long fatGram { get; set; }

        public long FatGram
        {
            get { return fatGram; }
            set
            {
                fatGram = value;
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
