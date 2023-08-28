using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.NutrientModels
{
    public class DailyNutrientDetails
    {
        // to draw first graph
        public NutrientOverview DailyNutrientOverview { get; set; }

        // draw the breakfeast, lunch diner bar
        // draw 2nd graph
        public List<MealDetails> MealDetails { get; set; }
    }

    public class MealDetails
    {
        public string MealName { get; set; }

        //for stacked graph on top
        public bool IsClickable { get; set; }

        // for meal specific
        public List<DishDetails> MealSpecificNutrientOverview { get; set; }

        // to draw the 2nd graph (the meals)
        public long TargetCalories { get; set; }
        public long CurrentCalories { get; set; }
        public long ProteinGram { get; set; }
        public long CarbsGram { get; set; }
        public long FatGram { get; set; }
    }

    public class NutrientOverview
    {
        public long TargetCalories { get; set; }
        public long CurrentCalories { get; set; } // TOTAL
        public long ProteinGram { get; set; }
        public long CrabsGram { get; set; }
        public long FatGram { get; set; }
    }

    public class DishDetails
    {
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; } // if false - in progress
        public double Carb { get; set; }
        public double Sugar { get; set; }
        public double Fibre { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Calories { get; set; }
        public double SaturatedFat { get; set; }
        public double UnsaturatedFat { get; set; }
        public double Servings { get; set; }
        /// enter intake for percent 50% hardcoded
    }
}
