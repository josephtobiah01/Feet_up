﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DataEntryTest.Models;

public partial class EdsAvaiableSetMetrics
{
    public long Id { get; set; }

    public long? FkExerciseTypeId { get; set; }

    public long? FkSetMetricsTypesId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual EdsExerciseType FkExerciseType { get; set; }

    public virtual EdsSetMetricTypes FkSetMetricsTypes { get; set; }
}