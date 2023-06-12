using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FitappAdminWeb.Net7.Models
{
    public class MuhdoRegisterModel
    {
        public long Id { get; set; }

        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string KitId { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public int Height { get; set; }
        public string HeightUnit { get; set; } = "cm"; //hardcode to metric for now
        [Required]
        public int Weight { get; set; }
        public string WeightUnit { get; set; } = "kg"; //hardcode to metric for now
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Birthday { get; set; }

        public List<SelectListItem> CountryList { get; set; } = new List<SelectListItem>();
    }
}
