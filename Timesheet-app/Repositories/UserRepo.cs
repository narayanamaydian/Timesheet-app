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

        public async Task<UserModel> GetUserById(string userId)
        {
            var user =  await _dbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null) {
                throw new Exception($"User with ID {userId} not found.");
            }
            _dbContext.Entry(user).State = EntityState.Detached; // Detach the entity to prevent tracking issues
            return user;
        }

        public async Task<bool> UserExists(string userId)
        {
            return await _dbContext.Users.AnyAsync(u => u.Id == userId);
        }


    }
}
