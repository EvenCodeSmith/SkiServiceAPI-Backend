using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SkiServiceAPI.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.Extensions.Logging;

namespace SkiServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SkiServiceDbContext _dbContext;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, SkiServiceDbContext dbContext, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Registration failed due to invalid input.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Register request received for username: {Username}", user.Username);

            if (await _dbContext.Users.AnyAsync(u => u.Username.ToLower() == user.Username.ToLower()))
            {
                _logger.LogWarning("Registration failed: Username {Username} is already taken.", user.Username);
                return Conflict(new { message = "Username already exists." });
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("User {Username} registered successfully.", user.Username);
            return Ok("User registered successfully.");
        }



        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            _logger.LogInformation("Login request received for username: {Username}", userLogin.Username);

            var user = _dbContext.Users
                .FirstOrDefault(u => u.Username.ToLower() == userLogin.Username.ToLower());

            if (user == null)
            {
                _logger.LogWarning("Login failed for username: {Username} - User not found.", userLogin.Username);
                return Unauthorized(new { message = "Invalid credentials" });
            }

            _logger.LogInformation("User {Username} found in database. Verifying password...", user.Username);

            if (!BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
            {
                _logger.LogWarning("Login failed for username: {Username} - Incorrect password.", user.Username);
                return Unauthorized(new { message = "Invalid credentials" });
            }

            _logger.LogInformation("Password verification successful for user {Username}. Generating JWT token...", user.Username);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Username) }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _logger.LogInformation("JWT Token generated successfully for user {Username}.", user.Username);

            return Ok(new { Token = tokenString, Username = user.Username, message = "Login successful" });
        }

        // GET: api/auth/users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            _logger.LogInformation("Fetching all registered users.");

            var users = await _dbContext.Users
                .Select(u => new { u.Id, u.Username }) // Passwort nicht mitgeben! - offensichtlich weshalb 
                .ToListAsync();

            _logger.LogInformation("Fetched {Count} users.", users.Count);

            return Ok(users);
        }



        private string GenerateJwtToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: new[] { new Claim(ClaimTypes.Name, username) },
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            _logger.LogInformation("Generated JWT token for username: {Username}", username);
            return tokenString;
        }
    }
}
