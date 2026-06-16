using Microsoft.EntityFrameworkCore;
using Timesheet_app.Models;
using Timesheet_app.Repositories;

namespace Timesheet_app.Data
{
    public class SQLServerDBConnection : DbContext
    {
        public SQLServerDBConnection(DbContextOptions<SQLServerDBConnection> options ) : base(options) 
        { 
        }
        public DbSet<TimesheetModel> Timesheets { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<HollidayModel> Holidays { get; set; }
        public DbSet<ProjectModel> Projects { get; set; }
        public DbSet<ProjectDetailModel> ProjectDetails { get; set; }
        public DbSet<UserProjectModel> UserProjects { get; set; }

    }
}
