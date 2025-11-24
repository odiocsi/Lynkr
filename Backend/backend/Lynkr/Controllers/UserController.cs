using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization; 
using Lynkr.Models;

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

            if (result.Succeeded) return Ok("Profile updated successfully");

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