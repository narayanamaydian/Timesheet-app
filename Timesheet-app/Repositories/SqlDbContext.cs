using Microsoft.EntityFrameworkCore;

namespace Timesheet_app.Repositories
{
    public class SqlDbContext : DbContext
    {
        public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)
        {

        }
    }
}
