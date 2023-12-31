﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace FitAppData.Net7;

public partial class EdsExerciseType
{
    public long Id { get; set; }

    public string Name { get; set; }

    public long FkExerciseClassId { get; set; }

    public long FkMainMuscleWorkedId { get; set; }

    public long FkOtherMuscleWorkedId { get; set; }

    public long FkEquipmentId { get; set; }

    public long FkMechanicsTypeId { get; set; }

    public long FkLevelId { get; set; }

    public long FkSportId { get; set; }

    public long FkForceId { get; set; }

    public string ExplainerVideoFr { get; set; }

    public string ExplainerTextFr { get; set; }

    public bool HasSetDefaultTemplate { get; set; }

    public bool IsSetCollapsed { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<EdsAvaiableSetMetrics> EdsAvaiableSetMetrics { get; } = new List<EdsAvaiableSetMetrics>();

    public virtual ICollection<EdsExercise> EdsExercise { get; } = new List<EdsExercise>();

    public virtual ICollection<EdsSetDefaults> EdsSetDefaults { get; } = new List<EdsSetDefaults>();

    public virtual EdsEquipment FkEquipment { get; set; }

    public virtual EdsExerciseClass FkExerciseClass { get; set; }

    public virtual EdsForce FkForce { get; set; }

    public virtual EdsLevel FkLevel { get; set; }

    public virtual EdsMainMuscleWorked FkMainMuscleWorked { get; set; }

    public virtual EdsMechanicsType FkMechanicsType { get; set; }

    public virtual EdsOtherMuscleWorked FkOtherMuscleWorked { get; set; }

    public virtual EdsSport FkSport { get; set; }
}