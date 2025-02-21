using System.ComponentModel.DataAnnotations;

namespace MAUIDevExpressApp.Shared.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        // We don't include PasswordHash in DTO for security
        public bool IsActive { get; set; }

        // Navigation properties
        public List<UserRoleDTO> UserRoles { get; set; } = new();

        // Display properties
        public string StatusDisplay => IsActive ? "Active" : "Inactive";
        public List<string> RoleNames => UserRoles?.Select(ur => ur.Role?.Name ?? "Unknown")?.ToList() ?? new();
        public string RolesDisplay => string.Join(", ", RoleNames);
    }

}
