﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DataEntryTest.Models;

public partial class EdsSet
{
    public long Id { get; set; }

    public long? FkExerciseId { get; set; }

    public short SetSequenceNumber { get; set; }

    public bool IsComplete { get; set; }

    public bool IsSkipped { get; set; }

    public bool IsCustomerAddedSet { get; set; }

    public DateTime? EndTimeStamp { get; set; }

    public virtual ICollection<EdsSetMetrics> EdsSetMetrics { get; } = new List<EdsSetMetrics>();

    public virtual EdsExercise FkExercise { get; set; }
}