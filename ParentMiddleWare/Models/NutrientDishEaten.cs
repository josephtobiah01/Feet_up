using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.Models
{
    public class NutrientDish
    {
        public long NumberOfServings { get; set; }
        public double PercentageEaten { get; set; }
        public string Notes { get; set; }
        public bool? isFavorite { get; set; }
        public NutrientRecipeModel Recipe { get; set; }

    }
}
