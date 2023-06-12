﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DAOLayer.Net7.Supplement;

public partial class SupplementContext : DbContext
{
    public SupplementContext()
    {
    }

    public SupplementContext(DbContextOptions<SupplementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Country> Country { get; set; }

    public virtual DbSet<NdsSupplementCustomerInventory> NdsSupplementCustomerInventory { get; set; }

    public virtual DbSet<NdsSupplementInstruction> NdsSupplementInstruction { get; set; }

    public virtual DbSet<NdsSupplementLegalStatus> NdsSupplementLegalStatus { get; set; }

    public virtual DbSet<NdsSupplementLegalStatusTypes> NdsSupplementLegalStatusTypes { get; set; }

    public virtual DbSet<NdsSupplementPlanDaily> NdsSupplementPlanDaily { get; set; }

    public virtual DbSet<NdsSupplementPlanDose> NdsSupplementPlanDose { get; set; }

    public virtual DbSet<NdsSupplementPlanSupplement> NdsSupplementPlanSupplement { get; set; }

    public virtual DbSet<NdsSupplementPlanWeekly> NdsSupplementPlanWeekly { get; set; }

    public virtual DbSet<NdsSupplementReference> NdsSupplementReference { get; set; }

    public virtual DbSet<NdsSupplementSchedule> NdsSupplementSchedule { get; set; }

    public virtual DbSet<NdsSupplementScheduleDose> NdsSupplementScheduleDose { get; set; }

    public virtual DbSet<NdsSupplementSchedulePerDate> NdsSupplementSchedulePerDate { get; set; }

    public virtual DbSet<NdsSupplementSkipReason> NdsSupplementSkipReason { get; set; }

    public virtual DbSet<NdsTemplateSupplementPlanDaily> NdsTemplateSupplementPlanDaily { get; set; }

    public virtual DbSet<NdsTemplateSupplementPlanDose> NdsTemplateSupplementPlanDose { get; set; }

    public virtual DbSet<NdsTemplateSupplementPlanSupplement> NdsTemplateSupplementPlanSupplement { get; set; }

    public virtual DbSet<NdsTemplateSupplementPlanWeekly> NdsTemplateSupplementPlanWeekly { get; set; }

    public virtual DbSet<NdsUnitMetric> NdsUnitMetric { get; set; }

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

        modelBuilder.Entity<NdsSupplementCustomerInventory>(entity =>
        {
            entity.ToTable("nds_supplement_customer_inventory");

            entity.Property(e => e.FkCustomerId).HasColumnName("fk_customer_id");
            entity.Property(e => e.FkSupplementReference).HasColumnName("fk_supplement_reference");
            entity.Property(e => e.UnitsLeft).HasColumnName("units_left");

            entity.HasOne(d => d.FkCustomer).WithMany(p => p.NdsSupplementCustomerInventory)
                .HasForeignKey(d => d.FkCustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nds_supplement_customer_inventory_country");

            entity.HasOne(d => d.FkSupplementReferenceNavigation).WithMany(p => p.NdsSupplementCustomerInventory)
                .HasForeignKey(d => d.FkSupplementReference)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nds_supplement_customer_inventory_nds_supplement_reference");
        });

        modelBuilder.Entity<NdsSupplementInstruction>(entity =>
        {
            entity.ToTable("nds_supplement_instruction");

            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.RequiresSourceOfFat).HasColumnName("requires_source_of_fat");
            entity.Property(e => e.TakeAfterMeal).HasColumnName("take_after_meal");
            entity.Property(e => e.TakeBeforeSleep).HasColumnName("take_before_sleep");
            entity.Property(e => e.TakeOnEmptyStomach).HasColumnName("take_on_empty_stomach");
        });

        modelBuilder.Entity<NdsSupplementLegalStatus>(entity =>
        {
            entity.ToTable("nds_supplement_legal_status");

            entity.Property(e => e.FkCountryId).HasColumnName("fk_country_id");
            entity.Property(e => e.FkSupplementLegalStatusTypes).HasColumnName("fk_supplement_legal_status_types");
            entity.Property(e => e.FkSupplementReference).HasColumnName("fk_supplement_reference");
            entity.Property(e => e.LegalReferenceUrl)
                .HasMaxLength(250)
                .HasColumnName("legal_reference_url");
            entity.Property(e => e.Remarks)
                .HasMaxLength(250)
                .HasColumnName("remarks");
            entity.Property(e => e.SpecialDisclaimer)
                .HasMaxLength(250)
                .HasColumnName("special_disclaimer");

            entity.HasOne(d => d.FkCountry).WithMany(p => p.NdsSupplementLegalStatus)
                .HasForeignKey(d => d.FkCountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nds_supplement_legal_status_country");

            entity.HasOne(d => d.FkSupplementLegalStatusTypesNavigation).WithMany(p => p.NdsSupplementLegalStatus)
                .HasForeignKey(d => d.FkSupplementLegalStatusTypes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nds_supplement_legal_status_nds_supplement_legal_status_types");

            entity.HasOne(d => d.FkSupplementReferenceNavigation).WithMany(p => p.NdsSupplementLegalStatus)
                .HasForeignKey(d => d.FkSupplementReference)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nds_supplement_legal_status_nds_supplement_reference");
        });

        modelBuilder.Entity<NdsSupplementLegalStatusTypes>(entity =>
        {
            entity.ToTable("nds_supplement_legal_status_types");

            entity.Property(e => e.HelpText)
                .HasMaxLength(250)
                .HasColumnName("help_text");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<NdsSupplementPlanDaily>(entity =>
        {
            entity.ToTable("nds_supplement_plan_daily");

            entity.Property(e => e.DayOfWeek)
                .HasComment("start at 0 = Monday ")
                .HasColumnName("day_of_week");
            entity.Property(e => e.FkSupplementPlanWeekly).HasColumnName("fk_supplement_plan_weekly");

            entity.HasOne(d => d.FkSupplementPlanWeeklyNavigation).WithMany(p => p.NdsSupplementPlanDaily)
                .HasForeignKey(d => d.FkSupplementPlanWeekly)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nds_supplement_plan_daily_nds_supplement_plan_weekly");
        });

        modelBuilder.Entity<NdsSupplementPlanDose>(entity =>
        {
            entity.ToTable("nds_supplement_plan_dose");

            entity.Property(e => e.DoseHardCeilingLimit).HasColumnName("dose_hard_ceiling_limit");
            entity.Property(e => e.DoseWarningLimit).HasColumnName("dose_warning_limit");
            entity.Property(e => e.FkSupplementPlanSupplement).HasColumnName("fk_supplement_plan_supplement");
            entity.Property(e => e.Remark)
                .HasMaxLength(250)
                .HasColumnName("remark");
            entity.Property(e => e.ScheduledTime).HasColumnName("scheduled_time");
            entity.Property(e => e.UnitCount).HasColumnName("unit_count");

            entity.HasOne(d => d.FkSupplementPlanSupplementNavigation).WithMany(p => p.NdsSupplementPlanDose)
                .HasForeignKey(d => d.FkSupplementPlanSupplement)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nds_supplement_plan_dose_nds_supplement_plan_supplement");
        });

        modelBuilder.Entity<NdsSupplementPlanSupplement>(entity =>
        {
            entity.ToTable("nds_supplement_plan_supplement");

            entity.Property(e => e.FkFreeEntryUnitMetric).HasColumnName("fk_free_entry_unit_metric");
            entity.Property(e => e.FkReferenceToTrainer).HasColumnName("fk_reference_to_trainer");
            entity.Property(e => e.FkSupplementPlanDaily).HasColumnName("fk_supplement_plan_daily");
            entity.Property(e => e.FkSupplementReference).HasColumnName("fk_supplement_reference");
            entity.Property(e => e.FreeEntryName)
                .HasMaxLength(50)
                .HasColumnName("free_entry_name");
            entity.Property(e => e.IsCustomerCreatedEntry).HasColumnName("is_customer_created_entry");
            entity.Property(e => e.IsFreeEntry).HasColumnName("is_free_entry");
            entity.Property(e => e.Remark)
                .HasMaxLength(250)
                .HasColumnName("remark");

            entity.HasOne(d => d.FkSupplementPlanDailyNavigation).WithMany(p => p.NdsSupplementPlanSupplement)
                .HasForeignKey(d => d.FkSupplementPlanDaily)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nds_supplement_plan_supplement_nds_supplement_plan_daily");

            entity.HasOne(d => d.FkSupplementReferenceNavigation).WithMany(p => p.NdsSupplementPlanSupplement)
                .HasForeignKey(d => d.FkSupplementReference)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nds_supplement_plan_supplement_nds_supplement_reference");
        });

        modelBuilder.Entity<NdsSupplementPlanWeekly>(entity =>
        {
            entity.ToTable("nds_supplement_plan_weekly");

            entity.Property(e => e.ActiveSince)
                .HasColumnType("datetime")
                .HasColumnName("active_since");
            entity.Property(e => e.FkCustomerId).HasColumnName("fk_Customer_id");
            entity.Property(e => e.InactiveSince)
                .HasColumnType("datetime")
                .HasColumnName("inactive_since");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Remark)
                .HasMaxLength(250)
                .HasColumnName("remark");

            entity.HasOne(d => d.FkCustomer).WithMany(p => p.NdsSupplementPlanWeekly)
                .HasForeignKey(d => d.FkCustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nds_supplement_plan_weekly_User");
        });

        modelBuilder.Entity<NdsSupplementReference>(entity =>
        {
            entity.ToTable("nds_supplement_reference");

            entity.Property(e => e.FkSupplementInstruction).HasColumnName("fk_supplement_instruction");
            entity.Property(e => e.FkUnitMetric).HasColumnName("fk_unit_metric");
            entity.Property(e => e.InstructionText)
                .HasMaxLength(250)
                .HasColumnName("instruction_text");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.FkSupplementInstructionNavigation).WithMany(p => p.NdsSupplementReference)
                .HasForeignKey(d => d.FkSupplementInstruction)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nds_supplement_reference_nds_supplement_instruction");

            entity.HasOne(d => d.FkUnitMetricNavigation).WithMany(p => p.NdsSupplementReference)
                .HasForeignKey(d => d.FkUnitMetric)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nds_supplement_reference_nds_unit_metric");
        });

        modelBuilder.Entity<NdsSupplementSchedule>(entity =>
        {
            entity.ToTable("nds_supplement_schedule");

            entity.Property(e => e.FkFreeEntryUnitMetric).HasColumnName("fk_free_entry_unit_metric");
            entity.Property(e => e.FkSupplementReference).HasColumnName("fk_supplement_reference");
            entity.Property(e => e.FkSupplementSchedulePerDate).HasColumnName("fk_supplement_schedule_per_date");
            entity.Property(e => e.FreeEntryInstructions)
                .HasMaxLength(250)
                .HasColumnName("free_entry_instructions");
            entity.Property(e => e.FreeEntryName)
                .HasMaxLength(50)
                .HasColumnName("free_entry_name");
            entity.Property(e => e.IsCustomerCreatedEntry).HasColumnName("is_customer_created_entry");
            entity.Property(e => e.IsFreeEntry).HasColumnName("is_free_entry");
            entity.Property(e => e.IsSnoozed).HasColumnName("is_snoozed");
            entity.Property(e => e.SnoozedTime)
                .HasColumnType("datetime")
                .HasColumnName("snoozed_time");

            entity.HasOne(d => d.FkSupplementReferenceNavigation).WithMany(p => p.NdsSupplementSchedule)
                .HasForeignKey(d => d.FkSupplementReference)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nds_supplement_schedule_nds_supplement_reference");

            entity.HasOne(d => d.FkSupplementSchedulePerDateNavigation).WithMany(p => p.NdsSupplementSchedule)
                .HasForeignKey(d => d.FkSupplementSchedulePerDate)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nds_supplement_schedule_nds_Supplement_schedule_per_date");
        });

        modelBuilder.Entity<NdsSupplementScheduleDose>(entity =>
        {
            entity.ToTable("nds_supplement_schedule_dose");

            entity.Property(e => e.CompletionTime)
                .HasColumnType("datetime")
                .HasColumnName("completion_time");
            entity.Property(e => e.CompletionTimeOffset).HasColumnName("completion_time_offset");
            entity.Property(e => e.FkSupplementSchedule).HasColumnName("fk_supplement_schedule");
            entity.Property(e => e.IsComplete).HasColumnName("is_complete");
            entity.Property(e => e.IsSkipped).HasColumnName("is_skipped");
            entity.Property(e => e.IsSnoozed).HasColumnName("is_snoozed");
            entity.Property(e => e.Remarks)
                .HasMaxLength(250)
                .HasColumnName("remarks");
            entity.Property(e => e.ScheduledTime)
                .HasColumnType("datetime")
                .HasColumnName("scheduled_time");
            entity.Property(e => e.SnoozedTime)
                .HasColumnType("datetime")
                .HasColumnName("snoozed_time");
            entity.Property(e => e.SupplementSkipOtherReason)
                .HasMaxLength(50)
                .HasColumnName("supplement_skip_other_reason");
            entity.Property(e => e.SupplementSkipReason).HasColumnName("supplement_skip_reason");
            entity.Property(e => e.UnitCountActual).HasColumnName("unit_count_actual");
            entity.Property(e => e.UnitCountTarget).HasColumnName("unit_count_target");

            entity.HasOne(d => d.FkSupplementScheduleNavigation).WithMany(p => p.NdsSupplementScheduleDose)
                .HasForeignKey(d => d.FkSupplementSchedule)
                .HasConstraintName("FK_nds_supplement_schedule_dose_nds_supplement_schedule");

            entity.HasOne(d => d.SupplementSkipReasonNavigation).WithMany(p => p.NdsSupplementScheduleDose)
                .HasForeignKey(d => d.SupplementSkipReason)
                .HasConstraintName("FK_nds_supplement_schedule_dose_nds_supplement_skip_reason");
        });

        modelBuilder.Entity<NdsSupplementSchedulePerDate>(entity =>
        {
            entity.ToTable("nds_Supplement_schedule_per_date");

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");

            entity.HasOne(d => d.Customer).WithMany(p => p.NdsSupplementSchedulePerDate)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nds_Supplement_schedule_per_date_User");
        });

        modelBuilder.Entity<NdsSupplementSkipReason>(entity =>
        {
            entity.ToTable("nds_supplement_skip_reason");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<NdsTemplateSupplementPlanDaily>(entity =>
        {
            entity.ToTable("nds_template_supplement_plan_daily");

            entity.Property(e => e.DayOfWeek)
                .HasComment("start at 0 = monday")
                .HasColumnName("day_of_week");
            entity.Property(e => e.FkTemplateSupplementPlanWeeklyId).HasColumnName("fk_template_supplement_plan_weekly_id");

            entity.HasOne(d => d.FkTemplateSupplementPlanWeekly).WithMany(p => p.NdsTemplateSupplementPlanDaily)
                .HasForeignKey(d => d.FkTemplateSupplementPlanWeeklyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nds_template_supplement_plan_daily_nds_template_supplement_plan_weekly");
        });

        modelBuilder.Entity<NdsTemplateSupplementPlanDose>(entity =>
        {
            entity.ToTable("nds_template_supplement_plan_dose");

            entity.Property(e => e.FkTemplateSupplementPlanSupplement).HasColumnName("fk_template_supplement_plan_supplement");
            entity.Property(e => e.SceduledTime)
                .HasColumnType("datetime")
                .HasColumnName("sceduled_time");
            entity.Property(e => e.UnitCount).HasColumnName("unit_count");

            entity.HasOne(d => d.FkTemplateSupplementPlanSupplementNavigation).WithMany(p => p.NdsTemplateSupplementPlanDose)
                .HasForeignKey(d => d.FkTemplateSupplementPlanSupplement)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nds_template_supplement_plan_dose_nds_template_supplement_plan_supplement");
        });

        modelBuilder.Entity<NdsTemplateSupplementPlanSupplement>(entity =>
        {
            entity.ToTable("nds_template_supplement_plan_supplement");

            entity.Property(e => e.FkFreeEntryUnitMetric).HasColumnName("fk_free_entry_unit_metric");
            entity.Property(e => e.FkSupplementReference).HasColumnName("fk_supplement_reference");
            entity.Property(e => e.FkTemplateSupplementPlanDaily).HasColumnName("fk_template_supplement_plan_daily");
            entity.Property(e => e.FreeEntryName)
                .HasMaxLength(50)
                .HasColumnName("free_entry_name");
            entity.Property(e => e.IsCustomerCreatedEntry).HasColumnName("is_customer_created_entry");
            entity.Property(e => e.IsFreeEntry).HasColumnName("is_free_entry");

            entity.HasOne(d => d.FkTemplateSupplementPlanDailyNavigation).WithMany(p => p.NdsTemplateSupplementPlanSupplement)
                .HasForeignKey(d => d.FkTemplateSupplementPlanDaily)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nds_template_supplement_plan_supplement_nds_template_supplement_plan_daily");
        });

        modelBuilder.Entity<NdsTemplateSupplementPlanWeekly>(entity =>
        {
            entity.ToTable("nds_template_supplement_plan_weekly");

            entity.Property(e => e.ActiveSince)
                .HasColumnType("datetime")
                .HasColumnName("active_since");
            entity.Property(e => e.DeletedSince)
                .HasColumnType("datetime")
                .HasColumnName("deleted_since");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<NdsUnitMetric>(entity =>
        {
            entity.ToTable("nds_unit_metric");

            entity.Property(e => e.IsCount).HasColumnName("is_count");
            entity.Property(e => e.IsVolume).HasColumnName("is_volume");
            entity.Property(e => e.IsWeight).HasColumnName("is_weight");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("name");
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
            entity.Property(e => e.Mobile)
                .HasMaxLength(50)
                .HasColumnName("mobile");
            entity.Property(e => e.SetTimeOffset).HasColumnName("set_time_offset");
            entity.Property(e => e.UserLevel).HasColumnName("user_level");
            entity.Property(e => e.Weight).HasColumnName("weight");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}