using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.NutrientModels
{
    public class NutrientsDataResponse
    {

      //  public List<DateTime> Days { get; set; }

        // The Target Values.  i.e how much he should eat
        public double TargetProtein { get; set; }
        public double TargetCarbohydrates { get; set; }
        public double TargetFat { get; set; }
        
        // the current Calories. i.e how much he ACTUALLY has eatend
        public double AvgCurrentCalories { get; set; }

        // The Target Calories. i.e How much he is SUPPOSED to eat (that the target)
        public double AvgTargetCalories { get; set; }

        // how much of these he ACTUALLY ate
        public double AverageProteinIntake { get; set; }
        public double AverageCarbsIntake { get; set; }
        public double AverageSugarIntake { get; set; }
        public double AverageFatIntake { get; set; }


        public List<Protein> ProteinModel { get; set; }
        public List<Carbohydrates> CarbohydratesModel { get; set; }
        public List<Fat> FatModel { get; set; }
        public List<TotalNutrients> TotalNutrientsModel { get; set; }

    }

    public class TotalNutrients
    {
        // not sure why these are double (from parentclass)
        // thse would eb the total.. i.e how much he ACTUALLY ate
        public double TotalProtein { get; set; }
        public double TotalCarbs { get; set; }
        public double TotalFat { get; set; }
        public double AverageSugarIntake { get; set; }

        // these 2 are the same.. how much he ACTUALLY ate
        public int CaloriesTranscribedTotal { get; set; }
        public double CurrentCalories { get; set; }
    }

    public class Protein
    {
        public double ProteinIntakeCount { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class Carbohydrates
    {
        // missing fiber for page 2
        public double CarbsIntakeCount { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class Fat
    {
        // missing saturated, polysaturated and monosaturated here
        public double FatIntakeCount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
