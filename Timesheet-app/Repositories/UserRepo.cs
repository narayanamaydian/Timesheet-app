using Microsoft.EntityFrameworkCore;
using Timesheet_app.Data;
using Timesheet_app.Models;

namespace Timesheet_app.Repositories
{
    public class UserRepo : IUserRepo
    {
        private SQLServerDBConnection _dbContext;

        public UserRepo(SQLServerDBConnection dbContext)
        {
            _dbContext = dbContext;
        }

        public Task AddUser(UserModel user)
        {
            _dbContext.Users.Add(user);
            return _dbContext.SaveChangesAsync();

        }

        public Task<UserModel> GetUser(object userId)
        {
            throw new NotImplementedException();
        }

        public async Task<UserModel> GetUserById(string userId)
        {
            return await _dbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        }

       
    }
}
