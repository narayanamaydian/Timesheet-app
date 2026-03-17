using Timesheet_app.Models;
using Timesheet_app.Models.DTOs;
using Timesheet_app.Repositories;

namespace Timesheet_app.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;

        public UserService(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<UserDto> AddUserAsync(UserDto userDto)
        {
            // Map DTO to Entity
            var user = new UserModel
            {
                Id = userDto.Id,
                Name = userDto.Name,
                VendorName = userDto.VendorName,
                NoSpk = userDto.NoSpk
            };

            await _userRepo.AddUser(user);

            return userDto;
        }

        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userRepo.GetUserById(userId);
            
            if (user == null) return null;

            // Map Entity to DTO
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                VendorName = user.VendorName,
                NoSpk = user.NoSpk
            };
        }
    }
}