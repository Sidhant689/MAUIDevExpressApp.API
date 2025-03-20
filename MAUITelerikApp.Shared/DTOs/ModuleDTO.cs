using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MAUIDevExpressApp.Shared.DTOs
{
    public class ModuleDTO
    {
        [Display(AutoGenerateField = false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Module name is required")]
        public string Name { get; set; }

        public string Description { get; set; }
        public bool IsActive { get; set; }

        [Display(AutoGenerateField = false)]
        public DateTime CreatedAt { get; set; }

        [Display(AutoGenerateField = false)]
        public DateTime? UpdatedAt { get; set; }

        // ✅ Add Pages Collection
        public ObservableCollection<PageDTO> Pages { get; set; } = new();

        // Display properties
        public string StatusDisplay => IsActive ? "Active" : "Inactive";
        public string CreatedAtDisplay => CreatedAt.ToString("MM/dd/yyyy HH:mm");
        public string UpdatedAtDisplay => UpdatedAt?.ToString("MM/dd/yyyy HH:mm") ?? "Not updated";
    }
}
