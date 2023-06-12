using DAOLayer.Net7.Chat;
using DAOLayer.Net7.Exercise;
using DAOLayer.Net7.Supplement;
using DAOLayer.Net7.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FitappAdminWeb.Net7.Models
{
    public class UserListViewModel
    {
        public List<DAOLayer.Net7.User.User> Users { get; set; } = new List<DAOLayer.Net7.User.User>();
        public List<IdentityUser> Id_Users { get; set; } = new List<IdentityUser>();
        public List<MsgRoom> Rooms { get; set; } = new List<MsgRoom>();
        public bool IsTest { get; set; }
    }

    public class UserEditModel
    {
        public long Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Mobile { get; set; }

        [Required]
        public string Email { get; set; }

        public bool? IsNewBarcode { get; set; }

        public string? BarcodeString { get; set; }

        [Required]
        public DateTime? Dob { get; set; }

        [Required]
        public int? Height { get; set; }

        [Required]
        public int? Weight { get; set; }

        [Required]
        public string? Gender { get; set; } //"male":"female"

        [Required]
        public string Country { get; set; } //in ISO country string. eg. Philippines = "PH"

        public List<SelectListItem> CountryList { get; set; } = new List<SelectListItem>();
    }

    public class UserDetailViewModel
    {
        public DAOLayer.Net7.User.User? CurrentUser { get; set; } = new DAOLayer.Net7.User.User();
        public IdentityUser? UserIdentity { get; set; }
        public long UserRoomId { get; set; }
        public List<DAOLayer.Net7.User.UInternalNotes> InternalNotes { get; set; } = new List<DAOLayer.Net7.User.UInternalNotes>();
        public Eds12weekPlan? CurrentTrainingPlan { get; set; }
        public NdsSupplementPlanWeekly? CurrentSupplementPlan { get; set; }
        public MsgRoom? Room { get; set; }

        public AddInternalNoteModel AddNote { get; set; } = new AddInternalNoteModel();
    }

    public class AddInternalNoteModel
    {
        [Required]
        public long ForUserId { get; set; }
        [Required]
        [StringLength(512)]
        public string Note { get; set; } = String.Empty;
    }
}
