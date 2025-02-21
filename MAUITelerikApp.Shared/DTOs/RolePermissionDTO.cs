namespace MAUIDevExpressApp.Shared.DTOs
{
    public class RolePermissionDTO
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public DateTime AssignedAt { get; set; }

        // Navigation properties
        public RoleDTO Role { get; set; }
        public PermissionDTO Permission { get; set; }

        // Display properties
        public string AssignedAtDisplay => AssignedAt.ToString("MM/dd/yyyy HH:mm");
    }

}
