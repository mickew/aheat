using AHeat.Web.API.Models;
using AHeat.Web.Shared.Authorization;
using AHeat.Web.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AHeat.Web.API.Data;

namespace AHeat.Web.API.Controllers.Admin;

[ApiController]
[Route("api/Admin/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserManager<User> userManager, ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    // GET: api/Admin/Users
    [HttpGet]
    [Authorize(Permissions.ViewUsers)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        return await _userManager.Users
            .OrderBy(r => r.UserName)
            .Select(u => new UserDto(u.Id, u.UserName ?? string.Empty, u.Email ?? string.Empty, u.FirstName ?? string.Empty, u.LastName ?? string.Empty))
            .ToListAsync();
    }

    // GET: api/Admin/Users/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Permissions.ViewUsers)]
    public async Task<ActionResult<UserDto>> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            _logger.LogError($"User with id {id} not found");
            return NotFound();
        }

        var dto = new UserDto(user.Id, user.UserName ?? string.Empty, user.Email ?? string.Empty, user.FirstName ?? string.Empty, user.LastName ?? string.Empty);

        var roles = await _userManager.GetRolesAsync(user);

        dto.Roles.AddRange(roles);

        return dto;
    }

    // PUT: api/Admin/Users/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Permissions.ManageUsers)]
    public async Task<IActionResult> PutUser(string id, UserDto updatedUser)
    {
        if (id != updatedUser.Id)
        {
            _logger.LogError($"User id {id} dont mach {updatedUser.Id}");
            return BadRequest();
        }

        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            _logger.LogError($"User with id {id} not found");
            return NotFound();
        }

        user.UserName = updatedUser.UserName;
        user.Email = updatedUser.Email;
        user.FirstName = updatedUser.FirstName;
        user.LastName = updatedUser.LastName;

        await _userManager.UpdateAsync(user);

        var currentRoles = await _userManager.GetRolesAsync(user);
        var addedRoles = updatedUser.Roles.Except(currentRoles);
        var removedRoles = currentRoles.Except(updatedUser.Roles);

        if (addedRoles.Any())
        {
            await _userManager.AddToRolesAsync(user, addedRoles);
        }

        if (removedRoles.Any())
        {
            await _userManager.RemoveFromRolesAsync(user, removedRoles);
        }

        return NoContent();
    }

    // post: api/Admin/Users
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Permissions.ManageUsers)]
    public async Task<ActionResult<UserDto>> PostUser(UserDto newUser)
    {
        var user = new User
        {
            UserName = newUser.UserName,
            Email = newUser.Email,
            FirstName = newUser.FirstName,
            LastName = newUser.LastName
        };
        await _userManager.CreateAsync(user, user.UserName);

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // DELETE: api/Admin/Users/5
    [HttpDelete("{id}")]
    [Authorize(Permissions.ManageUsers)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Permissions.ManageUsers)]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            _logger.LogError($"User with id {id} not found");
            return NotFound();
        }

        if (user.UserName == DbInitializer.DefaultAdminUserName)
        {
            _logger.LogError($"Cannot delete default administrator {user.UserName} user");
            return BadRequest("Cannot delete default admin user");
        }

        await _userManager.DeleteAsync(user);

        return NoContent();
    }

    // PUT: api/Admin/Users/5/ResetPassword
    [HttpPut("{id}/resetpassword")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Permissions.ManageUsers)]
    public async Task<IActionResult> ResetPassword(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            _logger.LogError($"User with id {id} not found");
            return NotFound();
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, $"!1{char.ToUpper(user.UserName![0])}{user.UserName.Substring(1)}1!");

        if (!result.Succeeded)
        {
            _logger.LogError($"Error resetting password for user {user.UserName}");
            foreach (var error in result.Errors)
            {
                _logger.LogError(error.Description);
            }
            return BadRequest(result.Errors);
        }

        return NoContent();
    }
}

