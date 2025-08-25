using insuranceclaimproject.Dtos;
using insuranceclaimproject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Claim = System.Security.Claims.Claim;

namespace insuranceclaimproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("Register")]
       
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto, string role)
        {
            var userExist = await _userManager.FindByNameAsync(registerUserDto.Username);
            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response { Status = "Error", Message = "User with this username already exists!" });
            }

            AppUser user = new()
            {
                Email = registerUserDto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUserDto.Username,
                Role = (UserRole)Enum.Parse(typeof(UserRole), role, true)
            };

            var result = await _userManager.CreateAsync(user, registerUserDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "User creation failed: " + string.Join(", ", errors) });
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
            await _userManager.AddToRoleAsync(user, role);

            return StatusCode(StatusCodes.Status200OK,
                new Response { Status = "Success", Message = $"User '{user.UserName}' created successfully and assigned to role '{role}'." });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    username = user.UserName,
                    email = user.Email,
                    id=user.Id,
                    roles = userRoles

                });
            }

            return Unauthorized(new Response { Status = "Error", Message = "Invalid username or password." });
        }

        // New API endpoint to get all policyholders
        [HttpGet("policyholders")]
        [Authorize(Roles = "ADMIN, AGENT")] // Restrict access to administrators and agents
        public async Task<IActionResult> GetPolicyholders()
        {
            var policyholderRole = await _roleManager.FindByNameAsync("POLICYHOLDER");
            if (policyholderRole == null)
            {
                return NotFound(new { message = "POLICYHOLDER role not found." });
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync("POLICYHOLDER");

            var policyholders = usersInRole.Select(u => new
            {
                Id = u.Id,
                Username = u.UserName
            }).ToList();

            if (!policyholders.Any())
            {
                return NotFound(new { message = "No policyholders found." });
            }

            return Ok(policyholders);
        }

        private JwtSecurityToken GetToken(List<System.Security.Claims.Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? string.Empty));

            if (string.IsNullOrEmpty(_configuration["JWT:Secret"]) || Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]).Length < 16)
            {
                throw new InvalidOperationException("JWT Secret is not configured or is too short. It must be at least 16 characters long.");
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
        // Replace your existing GetUsers method with this corrected version

        [HttpGet("users")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetUsers([FromQuery] string? role)
        {
            var usersQuery = _userManager.Users;

            // If a role is specified, parse it to an enum and then filter
            if (!string.IsNullOrWhiteSpace(role))
            {
                // Use Enum.TryParse for safe conversion from string to your UserRole enum.
                // The 'true' argument makes the parsing case-insensitive (so "admin" works too).
                if (Enum.TryParse<UserRole>(role, true, out var roleEnum))
                {
                    // This is a direct enum comparison that EF Core CAN translate to SQL.
                    usersQuery = usersQuery.Where(u => u.Role == roleEnum);
                }
                else
                {
                    // If the role string is invalid (e.g., ?role=SomeInvalidRole),
                    // return an empty list instead of erroring out.
                    return Ok(new List<UserProfileDto>());
                }
            }

            var userDtos = await usersQuery.Select(u => new UserProfileDto
            {
                UserId = u.Id,
                Username = u.UserName,
                Email = u.Email,
                Role = u.Role
            }).ToListAsync();

            if (!userDtos.Any())
            {
                return NotFound(new Response { Status = "Not Found", Message = "No users found matching the criteria." });
            }

            return Ok(userDtos);
        }
    }

    public class Response
    {
        public string Status { get; set; } = default!;
        public string Message { get; set; } = default!;
    }
}