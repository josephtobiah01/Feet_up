﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DAOLayer.Net7.Exercise;

public partial class EdsMechanicsType
{
    public long Id { get; set; }

    public string Name { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<EdsExerciseType> EdsExerciseType { get; set; } = new List<EdsExerciseType>();
}