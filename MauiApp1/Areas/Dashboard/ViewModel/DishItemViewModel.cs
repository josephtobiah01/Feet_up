using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Dashboard.ViewModel
{
    public class DishItemViewModel : INotifyPropertyChanged
    {

        public string ImageUrl { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public double Carb { get; set; }

        public double Sugar { get; set; }

        public double Fibre { get; set; }

        public double Protein { get; set; }

        public double Fat { get; set; }

        public double Calories { get; set; }

        public double StarturatedFat { get; set; }

        public double UnsaturatedFat { get; set; }

        public double Servings { get; set; }

        public double AminoAcid { get; set; }

        public double ProteinPercentageIntake { get; set; }

        public double CarbsPercentageIntake { get; set; }

        public double FatPercentageIntake { get; set; }

        private bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                OnPropertyChanged();
            }
        }

        public string Status { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
