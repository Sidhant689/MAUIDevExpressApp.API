namespace MAUIDevExpressApp.Shared.DTOs
{
    // Request/Response DTOs for specific operations
    public class AssignRoleRequestDTO
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

}
