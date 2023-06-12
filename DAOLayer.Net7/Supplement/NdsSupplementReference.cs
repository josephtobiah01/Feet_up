﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DAOLayer.Net7.Supplement;

public partial class NdsSupplementReference
{
    public long Id { get; set; }

    public string Name { get; set; }

    public long FkUnitMetric { get; set; }

    public long FkSupplementInstruction { get; set; }

    public string InstructionText { get; set; }

    public bool IsDeleted { get; set; }

    public virtual NdsSupplementInstruction FkSupplementInstructionNavigation { get; set; }

    public virtual NdsUnitMetric FkUnitMetricNavigation { get; set; }

    public virtual ICollection<NdsSupplementCustomerInventory> NdsSupplementCustomerInventory { get; set; } = new List<NdsSupplementCustomerInventory>();

    public virtual ICollection<NdsSupplementLegalStatus> NdsSupplementLegalStatus { get; set; } = new List<NdsSupplementLegalStatus>();

    public virtual ICollection<NdsSupplementPlanSupplement> NdsSupplementPlanSupplement { get; set; } = new List<NdsSupplementPlanSupplement>();

    public virtual ICollection<NdsSupplementSchedule> NdsSupplementSchedule { get; set; } = new List<NdsSupplementSchedule>();
}