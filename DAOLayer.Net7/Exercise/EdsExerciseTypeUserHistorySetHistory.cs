﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DAOLayer.Net7.Exercise;

public partial class EdsExerciseTypeUserHistorySetHistory
{
    public long Id { get; set; }

    public long FkSetId { get; set; }

    public short SetNumber { get; set; }

    public string HistoryString { get; set; }

    public long FkExerciseType { get; set; }

    public virtual EdsExerciseTypeUserHistory FkExerciseTypeNavigation { get; set; }
}