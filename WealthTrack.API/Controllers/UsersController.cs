using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WealthTrack.API.DTOs;
using WealthTrack.API.Services;

namespace WealthTrack.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : BaseController
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var emailTaken = await _userService.EmailExistsAsync(dto.Email);

            if (emailTaken)
            {
                return Conflict(new { message = "A user with this email address already exists." });
            }

            var user = await _userService.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            var success = await _userService.SetUserActiveStatusAsync(id, true);

            if (!success)
            {
                return NotFound(new { message = "User not found." });
            }

            return NoContent();
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var success = await _userService.SetUserActiveStatusAsync(id, false);

            if (!success)
            {
                return NotFound(new { message = "User not found." });
            }

            return NoContent();
        }

        [HttpPost("assign-advisor")]
        public async Task<IActionResult> AssignAdvisor([FromBody] AssignAdvisorDto dto)
        {
            var success = await _userService.AssignAdvisorToClientAsync(dto);

            if (!success)
            {
                return BadRequest(new { message = "Advisor or client not found." });
            }

            return NoContent();
        }
    }
}
