﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DAOLayer.Net7.Nutrition;

public partial class FnsNutritionActualMeal
{
    public long Id { get; set; }

    public long FkNutritionActualDayId { get; set; }

    public long MealTypeId { get; set; }

    public bool HasTarget { get; set; }

    public double MealCalorieTarget { get; set; }

    public double MealCalorieMin { get; set; }

    public double MealCalorieMax { get; set; }

    public double ProteinGramsTarget { get; set; }

    public double CrabsGramsTarget { get; set; }

    public double SugarGramsTarget { get; set; }

    public double FatGramsTarget { get; set; }

    public double UnsaturatedFatGramsTarget { get; set; }

    public double FiberGramsTarget { get; set; }

    public double AlcoholGramsTarget { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsSkipped { get; set; }

    public bool IsSnoozed { get; set; }

    public DateTime? SnoozedTime { get; set; }

    public bool IsComplete { get; set; }

    public bool IsOngoing { get; set; }

    public double SaturatedFatGramsTarget { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual FnsNutritionActualDay FkNutritionActualDay { get; set; }

    public virtual ICollection<FnsNutritionActualDish> FnsNutritionActualDish { get; set; } = new List<FnsNutritionActualDish>();

    public virtual FnsMealType MealType { get; set; }
}