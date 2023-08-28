using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Pages.Nutrient
{
    public class NutritionUploadModel
    {
        public long MealId { get; set; } = 0;
        public long RecipeId { get; set; } = 0;
        public double NumberOfServings { get; set; } = 0;
        public double NutrientPortion { get; set; } = 0;

        public string NutrientNotes { get; set; } = string.Empty;
        public string NutrientDishName { get; set; } = string.Empty;

        public string FoodImage64String { get; set; } = string.Empty;
        public string FoodImageType { get; set; } = string.Empty;
        public long SelectedDishId { get; set; } = 0;
        public bool IsFavorite { get; set; } = false;

        public NutritionUploadModel_Type UploadType { get; set; }
    }


    public enum NutritionUploadModel_Type
    {
        PhotoUpload = 0,
        ByRecipe = 1
    }
}
