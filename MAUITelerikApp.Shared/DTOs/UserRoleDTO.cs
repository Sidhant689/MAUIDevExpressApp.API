namespace MAUIDevExpressApp.Shared.DTOs
{
    public class UserRoleDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }

        // Navigation properties
        public UserDTO User { get; set; }
        public RoleDTO Role { get; set; }

        // Display properties
        public string AssignedAtDisplay => AssignedAt.ToString("MM/dd/yyyy HH:mm");
        public string ExpiresAtDisplay => ExpiresAt?.ToString("MM/dd/yyyy HH:mm") ?? "Never";
        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
        public string StatusDisplay => IsExpired ? "Expired" : "Active";
    }

}
