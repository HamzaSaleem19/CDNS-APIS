using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CDNSBlazorApp.Models;

namespace CDNSBlazorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new
                {
                    id = user.Id,
                    userName = user.UserName,
                    email = user.Email,
                    fullName = user.FullName,
                    isActive = user.IsActive,
                    createdAt = user.CreatedAt,
                    roles
                });
            }

            return Ok(userDtos);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                id = user.Id,
                userName = user.UserName,
                email = user.Email,
                fullName = user.FullName,
                isActive = user.IsActive,
                createdAt = user.CreatedAt,
                roles
            });
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            if (!string.IsNullOrEmpty(model.Role))
                await _userManager.AddToRoleAsync(user, model.Role);

            return Ok(new { message = "User created successfully", userId = user.Id });
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserModel model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.IsActive = model.IsActive;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            return Ok(new { message = "User updated successfully" });
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            return Ok(new { message = "User deleted successfully" });
        }

        [HttpPost("users/{id}/roles")]
        public async Task<IActionResult> AssignRole(string id, [FromBody] AssignRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (!await _roleManager.RoleExistsAsync(model.Role))
                return BadRequest(new { message = "Role does not exist" });

            await _userManager.AddToRoleAsync(user, model.Role);

            return Ok(new { message = $"Role {model.Role} assigned successfully" });
        }

        [HttpDelete("users/{id}/roles/{role}")]
        public async Task<IActionResult> RemoveRole(string id, string role)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            await _userManager.RemoveFromRoleAsync(user, role);

            return Ok(new { message = $"Role {role} removed successfully" });
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Ok(roles);
        }

        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = new IdentityRole(model.RoleName);
            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            return Ok(new { message = "Role created successfully" });
        }
    }

    public class CreateUserModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Role { get; set; }
    }

    public class UpdateUserModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class AssignRoleModel
    {
        public string Role { get; set; } = string.Empty;
    }

    public class CreateRoleModel
    {
        public string RoleName { get; set; } = string.Empty;
    }
}
