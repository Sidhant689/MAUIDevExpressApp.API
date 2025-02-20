namespace MAUIDevExpressApp.Shared.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Username { get; set; }
        public DateTime Expiration { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Permissions { get; set; }
    }
}
