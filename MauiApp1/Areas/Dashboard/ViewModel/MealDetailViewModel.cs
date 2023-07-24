using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Dashboard.ViewModel
{
    public class MealDetailViewModel
    {
        public string MealName { get; set; }

        //for stacked graph on top
        public bool IsClickAble { get; set; }

        // for meal specific
        public List<DishItemViewModel> MealSpecificNutrientOverView { get; set; }

        // to draw the 2nd graph (the meals)
        public long TargetCalories { get; set; }
        public long CurrentCalories { get; set; }
        public long ProteinGram { get; set; }
        public long CarbsGram { get; set; }
        public long FatGram { get; set; }
    }
}
