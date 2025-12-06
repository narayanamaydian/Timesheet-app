using Timesheet_app.Models;

namespace Timesheet_app.Repositories
{
    public class UserRepo : IUserRepo
    {
        public Task<UsersModel> GetUser(object userId)
        {
            throw new NotImplementedException();
        }
    }
}
