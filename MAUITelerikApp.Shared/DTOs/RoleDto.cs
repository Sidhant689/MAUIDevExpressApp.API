using MAUIDevExpressApp.Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace MAUIDevExpressApp.Shared.DTOs
{
    public class RoleDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        public string Name { get; set; }

        public string Description { get; set; }
        public bool IsSystem { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public List<PermissionDTO> Permissions { get; set; } = new();
        public List<RolePermissionDTO> RolePermissions { get; set; } = new();
        public List<UserRoleDTO> UserRoles { get; set; } = new();

        // Display properties
        public string StatusDisplay => IsActive ? "Active" : "Inactive";
        public string TypeDisplay => IsSystem ? "System Role" : "Custom Role";
        public int AssignedUsersCount => UserRoles?.Count ?? 0;
      
    }

}
