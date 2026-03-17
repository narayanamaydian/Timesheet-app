using Microsoft.AspNetCore.Mvc;
using Timesheet_app.Models.DTOs;
using Timesheet_app.Services;

namespace Timesheet_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> AddUser([FromBody] UserDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest("User data is required");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _userService.AddUserAsync(userDto);
                return CreatedAtAction(nameof(AddUser), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDto>> GetUser(string userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                
                if (user == null)
                {
                    return NotFound($"User with ID {userId} not found");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
