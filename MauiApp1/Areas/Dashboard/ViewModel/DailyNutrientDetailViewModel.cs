using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Dashboard.ViewModel
{
    public  class DailyNutrientDetailViewModel
    {
        // to draw first graph
        public NutrientsIntakeViewItem nutrientsIntakeViewItem { get; set; }

        // draw the breakfeast, lunch diner bar
        // draw 2nd graph
       public List<MealDetailViewModel> MealDetails { get; set; }
    }
}
