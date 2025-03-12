namespace MAUIDevExpressApp.Shared.DTOs
{
    public class PermissionRoleRequest
    {
        public int RoleId { get; set; }
        public List<int> PermissionIds { get; set; } = new List<int>();
    }

}
