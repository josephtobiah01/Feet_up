﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DAOLayer.Net7.Chat;

public partial class MsgRoom
{
    public long Id { get; set; }

    public string RoomName { get; set; }

    public bool HasConcern { get; set; }

    public long FkUserId { get; set; }

    public virtual ICollection<MsgMessage> MsgMessage { get; set; } = new List<MsgMessage>();
}