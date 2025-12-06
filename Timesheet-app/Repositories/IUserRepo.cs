using Timesheet_app.Models;

namespace Timesheet_app.Repositories
{
    public interface IUserRepo
    {
        public Task<UsersModel> GetUser(Object userId); 
    }
}
