using Timesheet_app.Models;

namespace Timesheet_app.Repositories
{
    public interface IUserRepo
    {
        public Task AddUser(UserModel user);
        public Task<UserModel> GetUserById(string userId);

        public Task<bool> UserExists(string userId);
    }
}
