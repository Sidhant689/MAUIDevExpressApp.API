using MAUIDevExpressApp.API.Data;
using MAUIDevExpressApp.API.InterfaceServices;
using MAUIDevExpressApp.Shared.DTOs;
using MAUIDevExpressApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MAUIDevExpressApp.API.Controllers
{
    /// <summary>
    /// Controller for handling authentication-related actions.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IPermissionManagementService _permissionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="configuration">The configuration settings.</param>
        public AuthController(AppDbContext context, IConfiguration configuration, IPermissionManagementService permissionManagementService)
        {
            _context = context;
            _configuration = configuration;
            _permissionService = permissionManagementService;
        }

        /// <summary>
        /// Authenticates a user and generates JWT and refresh tokens.
        /// </summary>
        /// <param name="request">The login request containing username and password.</param>
        /// <returns>An <see cref="IActionResult"/> containing the authentication result.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                return Unauthorized("Invalid username or password");

            // Get user permissions
            var permissions = await _permissionService.GetUserPermissionsAsync(user.Id);
            var roles = await _permissionService.GetUserRolesAsync(user.Id);

            var (accessToken, refreshToken) = GenerateTokens(user, roles, permissions);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new LoginResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                Username = user.Username,
                Roles = roles.Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsSystem = r.IsSystem,
                    Permissions = r.RolePermissions.Select(rp => new PermissionDto
                    {
                        Module = rp.Permission.Module.Name,
                        Action = rp.Permission.Action
                    }).ToList()
                }).ToList(),
                Permissions = permissions.Select(p => new PermissionDto
                {
                    Module = p.Module.Name,
                    Action = p.Action
                }).ToList(),
                Expiration = DateTime.UtcNow.AddMinutes(
                    double.Parse(_configuration["Jwt:ExpireMinutes"] ?? "60"))
            });
        }

        /// <summary>
        /// Generates a new JWT and refresh token using a valid refresh token.
        /// </summary>
        /// <param name="request">The refresh token request containing the expired token and refresh token.</param>
        /// <returns>An <see cref="IActionResult"/> containing the new tokens.</returns>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequest request)
        {
            var principal = GetPrincipalFromExpiredToken(request.Token);
            if (principal == null)
                return BadRequest("Invalid access token");

            string username = principal.Identity.Name;
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null ||
                user.RefreshToken != request.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return BadRequest("Invalid refresh token or token expired");
            // Get user permissions
            var permissions = await _permissionService.GetUserPermissionsAsync(user.Id);
            var roles = await _permissionService.GetUserRolesAsync(user.Id);


            var (accessToken, refreshToken) = GenerateTokens(user,roles, permissions);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new RefreshTokenResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                Username = user.Username,
                Roles = roles.Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsSystem = r.IsSystem,
                    Permissions = r.RolePermissions.Select(rp => new PermissionDto
                    {
                        Module = rp.Permission.Module.Name,
                        Action = rp.Permission.Action
                    }).ToList()
                }).ToList(),
                Permissions = permissions.Select(p => new PermissionDto
                {
                    Module = p.Module.Name,
                    Action = p.Action
                }).ToList(),
                Expiration = DateTime.UtcNow.AddMinutes(
                    double.Parse(_configuration["Jwt:ExpireMinutes"] ?? "60"))
            });
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">The registration request containing username, email, and password.</param>
        /// <returns>An <see cref="IActionResult"/> containing the registration result.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest("Username already exists");

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("Email already exists");

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password)
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully" });
        }

        /// <summary>
        /// Generates a pair of JWT and refresh tokens.
        /// </summary>
        /// <param name="user">The user for whom the tokens are generated.</param>
        /// <returns>A tuple containing the access token and refresh token.</returns>
        private (string accessToken, string refreshToken) GenerateTokens(User user, List<Role> roles, List<Permission> permissions)
        {
            var accessToken = GenerateJwtToken(user, roles, permissions);
            var refreshToken = GenerateRefreshToken();
            return (accessToken, refreshToken);
        }

        /// <summary>
        /// Generates a secure random refresh token.
        /// </summary>
        /// <returns>A refresh token as a Base64 string.</returns>
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        /// <summary>
        /// Generates a JWT for the authenticated user.
        /// </summary>
        /// <param name="user">The user for whom the JWT is generated.</param>
        /// <returns>A JWT as a string.</returns>
        private string GenerateJwtToken(User user, List<Role> roles, List<Permission> permissions)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add roles
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }
            // Add permissions as claims
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("Permission", permission.Name));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var expires = DateTime.UtcNow.AddMinutes(
                double.Parse(_configuration["Jwt:ExpireMinutes"] ?? "60"));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Extracts the user identity from an expired JWT.
        /// </summary>
        /// <param name="token">The expired JWT.</param>
        /// <returns>The claims principal extracted from the token.</returns>
        /// <exception cref="SecurityTokenException">Thrown if the token is invalid.</exception>
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token,
                tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512Signature,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        /// <summary>
        /// Hashes a plain text password using BCrypt.
        /// </summary>
        /// <param name="password">The plain text password.</param>
        /// <returns>The hashed password.</returns>
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verifies a plain text password against a stored hash.
        /// </summary>
        /// <param name="password">The plain text password.</param>
        /// <param name="storedHash">The stored password hash.</param>
        /// <returns><c>true</c> if the password matches the hash; otherwise, <c>false</c>.</returns>
        private bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}
