﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DAOLayer.Net7.Chat;

public partial class ChatContext : DbContext
{
    public ChatContext()
    {
    }

    public ChatContext(DbContextOptions<ChatContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Country> Country { get; set; }

    public virtual DbSet<Image> Image { get; set; }

    public virtual DbSet<MsgBroadcast> MsgBroadcast { get; set; }

    public virtual DbSet<MsgMessage> MsgMessage { get; set; }

    public virtual DbSet<MsgRoom> MsgRoom { get; set; }

    public virtual DbSet<User> User { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("country");

            entity.Property(e => e.AreaCode).HasColumnName("area_code");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.TimeOffset).HasColumnName("time_offset");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.Property(e => e.RealImageUrl)
                .IsRequired()
                .HasMaxLength(320)
                .IsUnicode(false)
                .HasColumnName("real_image_url");
            entity.Property(e => e.ThumbnailImageUrl)
                .HasMaxLength(320)
                .IsUnicode(false)
                .HasColumnName("thumbnail_image_url");
        });

        modelBuilder.Entity<MsgBroadcast>(entity =>
        {
            entity.ToTable("msg_broadcast");

            entity.Property(e => e.FkUserId).HasColumnName("fk_user_id");
            entity.Property(e => e.Icon)
                .HasMaxLength(50)
                .HasColumnName("icon");
            entity.Property(e => e.Imageurl)
                .HasMaxLength(256)
                .HasColumnName("imageurl");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Message)
                .HasMaxLength(1024)
                .HasColumnName("message");
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
            entity.Property(e => e.Title)
                .HasMaxLength(512)
                .HasColumnName("title");
            entity.Property(e => e.Url)
                .HasMaxLength(256)
                .HasColumnName("url");
        });

        modelBuilder.Entity<MsgMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_msg_message_1");

            entity.ToTable("msg_message");

            entity.Property(e => e.FkImageId).HasColumnName("fk_image_id");
            entity.Property(e => e.FkRoomId).HasColumnName("fk_room_id");
            entity.Property(e => e.FkUserSender).HasColumnName("fk_user_sender");
            entity.Property(e => e.MessageContent).HasColumnName("message_content");
            entity.Property(e => e.NotSeenByUserNumber).HasColumnName("not_seen_by_user_number");
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");

            entity.HasOne(d => d.FkImage).WithMany(p => p.MsgMessage)
                .HasForeignKey(d => d.FkImageId)
                .HasConstraintName("FK_msg_message_Image");

            entity.HasOne(d => d.FkRoom).WithMany(p => p.MsgMessage)
                .HasForeignKey(d => d.FkRoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_msg_message_msg_room");

            entity.HasOne(d => d.FkUserSenderNavigation).WithMany(p => p.MsgMessage)
                .HasForeignKey(d => d.FkUserSender)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_msg_message_User");
        });

        modelBuilder.Entity<MsgRoom>(entity =>
        {
            entity.ToTable("msg_room");

            entity.Property(e => e.FkUserId).HasColumnName("fk_user_id");
            entity.Property(e => e.HasConcern).HasColumnName("has_concern");
            entity.Property(e => e.RoomName)
                .IsRequired()
                .HasMaxLength(55)
                .IsFixedLength()
                .HasColumnName("room_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.ApiKey).HasColumnName("api_key");
            entity.Property(e => e.BarcodeString)
                .HasMaxLength(50)
                .HasColumnName("barcode_string");
            entity.Property(e => e.Country)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("country");
            entity.Property(e => e.Dob)
                .HasColumnType("date")
                .HasColumnName("dob");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.FkFederatedUser)
                .IsRequired()
                .HasMaxLength(450)
                .HasColumnName("fk_federated_user");
            entity.Property(e => e.FkGender).HasColumnName("fk_gender");
            entity.Property(e => e.FkInternalNotesId).HasColumnName("fk_internal_notes_id");
            entity.Property(e => e.FkShippingAddress).HasColumnName("fk_shipping_address");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.Height).HasColumnName("height");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("((1))")
                .HasColumnName("is_Active");
            entity.Property(e => e.IsNewBarcode).HasColumnName("is_new_barcode");
            entity.Property(e => e.LastKnownTimeOffset).HasColumnName("last_known_time_offset");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .HasColumnName("middle_name");
            entity.Property(e => e.Mobile)
                .HasMaxLength(50)
                .HasColumnName("mobile");
            entity.Property(e => e.MobileCountryCode)
                .HasMaxLength(10)
                .HasColumnName("mobile_country_code");
            entity.Property(e => e.SetTimeOffset).HasColumnName("set_time_offset");
            entity.Property(e => e.Signupstatus).HasColumnName("signupstatus");
            entity.Property(e => e.UserLevel).HasColumnName("user_level");
            entity.Property(e => e.Weight).HasColumnName("weight");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}