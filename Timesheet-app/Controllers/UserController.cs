using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Timesheet_app.Models;
using Timesheet_app.Models.DTOs;
using Timesheet_app.Repositories;

namespace Timesheet_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;

        public UserController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> AddUser([FromBody] UserDto userDto)
        {
            string dateString = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string userId = GenerateId(userDto.Name, userDto.VendorName, userDto.NoSpk);
            userId = dateString + userId.ToString();
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
                // Map DTO to Entity
                var user = new UserModel
                {
                    Id = userId,
                    Name = userDto.Name,
                    VendorName = userDto.VendorName,
                    NoSpk = userDto.NoSpk
                    // Timesheets is null by default (optional property)
                };

                await _userRepo.AddUser(user);
                return CreatedAtAction(nameof(AddUser), new { id = userId }, userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        public static string GenerateId(string name, string vendorName, string spkNumber)
        {
            string input = $"{name}|{vendorName}|{spkNumber}";
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hash).Replace("-", "").ToLower(); ;
            }
        }

    }
}
