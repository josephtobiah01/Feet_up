﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DAOLayer.Net7.Supplement;

public partial class NdsUnitMetric
{
    public long Id { get; set; }

    public string Name { get; set; }

    public bool IsWeight { get; set; }

    public bool IsCount { get; set; }

    public bool IsVolume { get; set; }

    public virtual ICollection<NdsSupplementReference> NdsSupplementReference { get; set; } = new List<NdsSupplementReference>();
}