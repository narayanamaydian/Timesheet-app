using Timesheet_app.Models.DTOs;

namespace Timesheet_app.Services
{
    public interface IUserService
    {
        Task<UserDto> AddUserAsync(UserDto userDto);
        Task<UserDto> GetUserByIdAsync(string userId);
    }
}