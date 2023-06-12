using DAOLayer.Net7.Exercise;
using DAOLayer.Net7.Supplement;
using FitappAdminWeb.Net7.Classes.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FitappAdminWeb.Net7.Models
{
    public class ExerciseTypeListViewModel
    {
        public List<EdsExerciseType> EdsExerciseTypes { get; set; } = new List<EdsExerciseType>();
    }

    public class ExerciseTypeEditViewModel
    {
        public ExerciseType_DTO Data { get; set; }

        public List<SelectListItem> Equipment_List { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Force_List { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ExerciseClass_List { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Level_List { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> MainMuscle_List { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> OtherMuscle_List { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Sport_List { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> MechanicsType_List { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> SetMetricType_List { get; set; } = new List<SelectListItem>();
    }

    public class SupplementReferenceListViewModel
    {
        //search criteria
        public int? Page { get; set; }
        public string? Name { get; set; }
        public long? Id { get; set; }

        public List<NdsSupplementReference> NdsSupplementReferences { get; set; }
    }

    public class SupplementReferenceEditViewModel
    {
        public SupplementReference_DTO Data { get; set; } = new SupplementReference_DTO();

        public List<SelectListItem> UnitMetric_List { get; set; } = new List<SelectListItem>();

    }

    public class TrainingSessionTemplateListViewModel
    {
        public List<EdsTrainingSession> TemplateSessions { get; set; } = new List<EdsTrainingSession>();
    }
}
