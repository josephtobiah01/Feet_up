using System.ComponentModel.DataAnnotations;

namespace FitappAdminWeb.Net7.Classes.DTO
{
    #region Training DTOs
    public class ExerciseType_DTO
    {
        public long Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public long? FkExerciseClassId { get; set; }

        public string? ExerciseClassFreeText { get; set; }

        public long? FkMainMuscleWorkedId { get; set; }

        public string? MainMuscleWorkedFreeText { get; set; }

        public long? FkOtherMuscleWorkedId { get; set; }

        public string? OtherMuscleFreeText { get; set; }

        public long? FkEquipmentId { get; set; }

        public string? EquipmentFreeText { get; set; }

        public long? FkMechanicsTypeId { get; set; }

        public string? MechanicsTypeFreeText { get; set; }

        public long? FkLevelId { get; set; }

        public string? LevelFreeText { get; set; }

        public long? FkSportId { get; set; }

        public string? SportFreeText { get; set; }

        public long? FkForceId { get; set; }

        public string? ForceFreeText { get; set; }

        public string? ExplainerVideoFr { get; set; }

        public string? ExplainerTextFr { get; set; }
        public string? ExerciseImage { get; set; }
        
        public IFormFile? Image { get; set; }

        public List<SetDefault_DTO> SetDefaults { get; set; } = new List<SetDefault_DTO>();
    }

    public class SetDefault_DTO
    {
        public long Id { get; set; }

        public long? FkExerciseTypeId { get; set; }

        public short SetSequenceNumber { get; set; }

        public List<SetMetricDefault_DTO> SetMetricDefaults { get; set; } = new List<SetMetricDefault_DTO>();
    }
    
    public class SetMetricDefault_DTO
    {
        public long Id { get; set; }
        public long? FkSetMetricTypeId { get; set; }
        public long? DefaultCustomMetric { get; set; }
    }
    #endregion

    #region Supplement DTOs
    public class SupplementReference_DTO
    {
        public long Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public SupplementUnitMetric_DTO UnitMetric { get; set; } = new SupplementUnitMetric_DTO();

        public SupplementInstruction_DTO SupplementInstruction { get; set; } = new SupplementInstruction_DTO();

        [Required]
        public string? InstructionText { get; set; }
    }

    public class SupplementInstruction_DTO
    {
        public long? Id { get; set; }

        [Required]
        public string? Description { get; set; }

        public bool RequiresSourceOfFat { get; set; }

        public bool TakeAfterMeal { get; set; }

        public bool TakeBeforeSleep { get; set; }

        public bool TakeOnEmptyStomach { get; set; }
    }

    public class SupplementUnitMetric_DTO
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public bool IsWeight { get; set; }

        public bool IsCount { get; set; }

        public bool IsVolume { get; set; }
    }

    public class SupplementLegalStatus_DTO
    {
        public long Id { get; set; }

        public string SpecialDisclaimer { get; set; }

        public string Remarks { get; set; }

        public string LegalReferenceUrl { get; set; }

        public SupplementCountry_DTO Country { get; set; } = new SupplementCountry_DTO();

        public SupplementLegalStatusType_DTO LegalStatusType { get; set; } = new SupplementLegalStatusType_DTO();

    }

    public class SupplementLegalStatusType_DTO
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string HelpText { get; set; }
    }

    public class SupplementCountry_DTO
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public short? AreaCode { get; set; }
    }
    #endregion
}
