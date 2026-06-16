using Timesheet_app.Models;

namespace Timesheet_app.Repositories
{
    public interface IUserRepo
    {
        public Task<UserModel> GetUser(Object userId);

        public Task AddUser(UserModel user);
        public Task<UserModel> GetUserById(string userId);
    }
}
