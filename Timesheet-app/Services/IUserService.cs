using Timesheet_app.Models.DTOs;

namespace Timesheet_app.Services
{
    public interface IUserService
    {
        public Task<UserDto> GetUserByIdAsync(string userId);
        public Task<UserDto> AddUserAsync(UserDto userDto);
        public Task<bool> UserExistsAsync(string userId);

    }
}