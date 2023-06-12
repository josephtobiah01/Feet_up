using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.Models
{
    public class NutrientRecipeModel
    {
        public long RecipeID { get; set; }
        public string DisplayImageUrl { get; set; } //does not have to be Image filetype, but has to be some way to send/receive a photo to display.
        public string RecipeName { get; set; } // name
        public bool IsFavorite { get; set; }


        public RecipeNutrientInformation NutrientInformation { get; set; }

        public class RecipeNutrientInformation
        {
            public double? Carbohydrates { get; set; } //in milligrams
            public double? Fiber { get; set; } //in milligrams
            public double? Protein { get; set; } //in milligrams
            public double? Fat { get; set; } //in milligrams
            public double? Calories { get; set; } //in milligrams
        }

    }


    public class NutrientrecipesForMeal
    {
        public List<NutrientRecipeModel> Favorite { get; set; }
        public List<NutrientRecipeModel> History { get; set; }

        public NutrientrecipesForMeal()
        {
            Favorite = new List<NutrientRecipeModel>();
            History = new List<NutrientRecipeModel>();
        }
    }
}
