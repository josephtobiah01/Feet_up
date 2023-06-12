
using DAOLayer.Net7.Supplement;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitappAdminWeb.Net7.Models
{
    public class SupplementWeeklyPlanListViewModel
    {
        public User? CurrentUser { get; set; }
        public IdentityUser? UserIdentity { get; set; }
        public List<NdsSupplementPlanWeekly> SupplementWeeklyPlanList { get; set; } = new List<NdsSupplementPlanWeekly>();
        public List<NdsSupplementReference> SupplementReference { get; set; } = new List<NdsSupplementReference>();

        public List<SelectListItem> UserList { get; set; } = new List<SelectListItem>();
    }

    public class SupplementWeeklyPlanEditViewModel
    {
        public User? CurrentUser { get; set; }
        public SupplementWeeklyPlanEditFormModel Data { get; set; } = new SupplementWeeklyPlanEditFormModel();
        public bool IsCopy { get; set; } = false;

        public List<NdsSupplementReference> SupplementReference { get; set; } = new List<NdsSupplementReference>();
        public List<SelectListItem> TemplateList { get; set; }
        public List<SelectListItem> SupplementList { get; set; }
        public List<SelectListItem> UnitMetricList { get; set; }
    }

    public class SupplementWeeklyPlanEditFormModel
    {
        public long Id { get; set; }
        public long FkCustomerId { get; set; }
        public bool IsActive { get; set; }
        public bool IsTemplate { get; set; }
        public bool TemplateId { get; set; }
        public bool ForceScheduleSync { get; set; }
        public string Remark { get; set; }
    }
}
