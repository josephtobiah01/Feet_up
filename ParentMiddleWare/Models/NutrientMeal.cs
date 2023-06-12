using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.Models
{
    public class NutrientMeal
    {
        // 
        public long DayId { get; set; }


        public long MealId { get; set; }
        public long MealType { get; set; } // maybe 1 - breakfast, 2 - lunch, 3 - dinner, 4 - brunch, 5 - snack, 6 - uncategorized meal?
        public double TargetKiloCalories { get; set; }
        public bool IsCustom { get; set; } = false;
        public List<NutrientDish> DishesEaten = new List<NutrientDish>();
    }
}
