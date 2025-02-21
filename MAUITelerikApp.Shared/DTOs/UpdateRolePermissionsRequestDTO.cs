namespace MAUIDevExpressApp.Shared.DTOs
{
    public class UpdateRolePermissionsRequestDTO
    {
        public int RoleId { get; set; }
        public List<int> PermissionIds { get; set; }
    }

}
