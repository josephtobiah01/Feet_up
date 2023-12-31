﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DAOLayer.Net7.Nutrition;

public partial class FnsNutritionActualDay
{
    public long Id { get; set; }

    public long FkUserId { get; set; }

    public DateTime Date { get; set; }

    public double DayCalorieTarget { get; set; }

    public double DayCalorieTargetMin { get; set; }

    public double DayCalorieTargetMax { get; set; }

    public double ProteinGramsTarget { get; set; }

    public double CrabsGramsTarget { get; set; }

    public double SugarGramsTarget { get; set; }

    public double FatGramsTarget { get; set; }

    public double UnsaturatedFatGramsTarget { get; set; }

    public double FiberGramsTarget { get; set; }

    public double AlcoholGramsTarget { get; set; }

    public double SaturatedFatGramsTarget { get; set; }

    public virtual User FkUser { get; set; }

    public virtual ICollection<FnsNutritionActualMeal> FnsNutritionActualMeal { get; set; } = new List<FnsNutritionActualMeal>();
}