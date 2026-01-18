using Lynkr.Models;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Lynkr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public UserController(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // GET: api/user/me
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var currentUserId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (user is null) return Unauthorized("No user logged in.");

            return Ok(new
            {
                userId = user.Id,
                name = user.Name,
                profilePictureUrl = user.ProfilePictureUrl
            });
        }

        // GET: api/user/search?query=abc
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string query)
        {
            var currentUserId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(query))
                return Ok(Array.Empty<object>());

            query = query.Trim();

            // Simple "contains" search on Name and Email
            var users = await _userManager.Users
                .AsNoTracking()
                .Where(u =>
                    (u.Name != null && u.Name.Contains(query) && u.Id != currentUserId)
                )
                .OrderBy(u => u.Name)
                .Select(u => new
                {
                    id = u.Id,
                    name = u.Name,
                    email = u.Email,
                    profilePictureUrl = u.ProfilePictureUrl
                })
                .Take(20)
                .ToListAsync();

            return Ok(users);
        }

        // POST: api/user/register
        [HttpPost("register")]
        [AllowAnonymous] 
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
                return BadRequest("User already exists.");

            var user = new User
            {
                Email = registerDto.Email,
                UserName = registerDto.Email,
                Name = registerDto.Name,
                CreatedAt = DateTimeOffset.UtcNow,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { Message = "User created successfully!" });
        }

        // POST: api/user/login
        [HttpPost("login")]
        [AllowAnonymous] 
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                var token = GenerateJwtToken(user);

                return Ok(new
                {
                    Token = token,
                    UserId = user.Id,
                    Name = user.Name,
                    ProfilePic = user.ProfilePictureUrl
                });
            }

            return Unauthorized("Invalid Credentials");
        }

        // PUT: api/user/update
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateDto updateDto)
        {
            var userId = GetCurrentUserId();

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null) return NotFound("User not found");

            user.Name = updateDto.Name;
            user.ProfilePictureUrl = updateDto.ProfilePictureUrl;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok(new { message = "Profile updated successfully!" }); ;

            return BadRequest(result.Errors);
        }

        // DELETE: api/user/delete
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = GetCurrentUserId();

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null) return NotFound("User not found");

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded) return Ok("Account deleted successfully");

            return BadRequest(result.Errors);
        }

        // --- HELPER ---
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            var claims = new List<Claim>
            {

                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

                new Claim(JwtRegisteredClaimNames.Email, user.Email),
        
                new Claim("name", user.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idClaim)) throw new UnauthorizedAccessException("User ID not found in token.");
            return int.Parse(idClaim);
        }
    }
}

// --- DTOs ---

namespace Lynkr.Models
{
    public class RegisterDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserUpdateDto
    {
        public string Name { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}