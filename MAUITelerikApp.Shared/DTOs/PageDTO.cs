using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MAUIDevExpressApp.Shared.DTOs
{
    public class PageDTO
    {
        [Display(AutoGenerateField = false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Module name is required")]
        public string Name { get; set; }
        public int ModuleId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        [Display(AutoGenerateField = false)]
        public DateTime CreatedAt { get; set; }
        [Display(AutoGenerateField = false)]
        public DateTime? UpdatedAt { get; set; }

        [JsonIgnore]
        public string Module { get; set; }
        [JsonIgnore]
        public ModuleDTO ModuleObj { get; set; }

        // Navigation properties
        [Display(AutoGenerateField = false)]

        // Display properties
        public string StatusDisplay => IsActive ? "Active" : "Inactive";
        public string CreatedAtDisplay => CreatedAt.ToString("MM/dd/yyyy HH:mm");
        public string UpdatedAtDisplay => UpdatedAt?.ToString("MM/dd/yyyy HH:mm") ?? "Not updated";
    }

}
