using System.ComponentModel.DataAnnotations;

namespace MAUIDevExpressApp.Shared.DTOs
{
    public class PermissionDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Permission name is required")]
        public string Name { get; set; }

        public string Description { get; set; }
        public int PageId { get; set; }
        public string Action { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string Page { get; set; }

        // Navigation properties
        public PageDTO PageObj { get; set; }

        // For permission management UI
        public bool IsSelected { get; set; }

        // Display properties
        public string StatusDisplay => IsActive ? "Active" : "Inactive";
        public string FullName => $"{PageObj?.Name} - {Name}";
        public string ActionDisplay => $"{Action} ({Description})";
    }
}
