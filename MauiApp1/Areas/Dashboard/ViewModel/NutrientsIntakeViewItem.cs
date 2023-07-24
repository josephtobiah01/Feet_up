using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Dashboard.ViewModel
{
    public class NutrientsIntakeViewItem
    {
        public double CaloriesProtein { get; set; }
        public double CaloriesCarbs { get; set; }
        public double CaloriesFat { get; set; }
        public double TotalCalories { get; set; }
        public double CarbPercentage { get; set; }
        public double ProteinPercentage { get; set; }
        public double FatPercentage { get; set; }

        public double ProteinToDisplay { get; set; }
        public double CarbohydratesToDisplay { get; set; }
        public double FatToDisplay { get; set; }

        public int TranscribedCalories { get; set; }
        public int TargetCalories { get; set; }
    }

    
}
