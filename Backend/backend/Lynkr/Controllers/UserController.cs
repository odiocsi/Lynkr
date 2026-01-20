using Lynkr.Models;
using Microsoft.AspNetCore;
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
        private readonly IWebHostEnvironment _environment;

        private static readonly HashSet<string> AllowedContentTypes = new()
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

    public UserController(UserManager<User> userManager, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _configuration = configuration;
            _environment = environment;
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
        [RequestSizeLimit(5 * 1024 * 1024)] // 5MB
        public async Task<IActionResult> UpdateProfile(
            [FromForm] UserUpdateDto form,
            CancellationToken ct)
        {
            if (form.Username == null && form.File == null)
                return BadRequest(new { message = "Nothing to update." });

            var userId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound("User not found");


            if (!string.IsNullOrWhiteSpace(form.Username))
            {
                user.Name = form.Username.Trim();
            }

            if (form.File != null)
            {
                var file = form.File;

                var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
                if (!allowedTypes.Contains(file.ContentType))
                    return BadRequest(new { message = "Only JPG, PNG, or WEBP images allowed." });

                if (file.Length == 0 || file.Length > 5 * 1024 * 1024)
                    return BadRequest(new { message = "Invalid file size (max 5MB)." });

                var webRoot = _environment.WebRootPath ??
                              Path.Combine(_environment.ContentRootPath, "wwwroot");

                Directory.CreateDirectory(webRoot);

                var uploadDir = Path.Combine(webRoot, "uploads", "profile");
                Directory.CreateDirectory(uploadDir);

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (ext is not ".jpg" and not ".jpeg" and not ".png" and not ".webp")
                    ext = ".img";

                var fileName = $"{Guid.NewGuid():N}{ext}";
                var fullPath = Path.Combine(uploadDir, fileName);

                await using (var fs = System.IO.File.Create(fullPath))
                {
                    await file.CopyToAsync(fs, ct);
                }

                // Delete old profile picture (best effort)
                TryDeleteOldLocalProfilePicture(user.ProfilePictureUrl, webRoot);

                user.ProfilePictureUrl =
                    $"{Request.Scheme}://{Request.Host}/uploads/profile/{fileName}";
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new
            {
                user.Name,
                user.ProfilePictureUrl
            });
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

        private static void TryDeleteOldLocalProfilePicture(string? oldUrl, string webRoot)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(oldUrl)) return;

                var marker = "/uploads/profile/";
                var idx = oldUrl.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (idx< 0) return;

                var relativePart = oldUrl[(idx + 1)..];
                var fullPath = Path.Combine(webRoot, relativePart.Replace('/', Path.DirectorySeparatorChar));

                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }
            catch
            {
                // ignore (best-effort cleanup)
            }
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
        public string? Username { get; set; }
        public IFormFile? File { get; set; }
    }
}