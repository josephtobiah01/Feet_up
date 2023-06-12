using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Pages.Nutrient
{
    public class NutritionUploadModel
    {
        public long MealId { get; set; }
        public long RecipeId { get; set; } = 0;
        public double NumberOfServings { get; set; }
        public double NutrientPortion { get; set; }

        public string NutrientNotes { get; set; }
        public string NutrientDishName { get; set; }

        public string FoodImage64String { get; set; }
        public string FoodImageType { get; set; }
        public long SelectedDishId { get; set; }
        public bool IsFavorite { get; set; } = false;

        public NutritionUploadModel_Type UploadType { get; set; }
    }


    public enum NutritionUploadModel_Type
    {
        PhotoUpload = 0,
        ByRecipe = 1
    }
}
