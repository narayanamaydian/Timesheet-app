using Microsoft.EntityFrameworkCore;

namespace Timesheet_app.Data
{
    public class SQLServerDBConnection
    {
        public SqlDbContext Context { get; }

        public SQLServerDBConnection(SqlDbContext context)
        {
            Context = context;
        }


    }
}
