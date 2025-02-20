namespace MAUIDevExpressApp.Shared.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        //public ICollection<UserRole> UserRoles { get; set; }
    }

}
