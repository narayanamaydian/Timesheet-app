using Microsoft.EntityFrameworkCore;
using Timesheet_app.Data;
using Timesheet_app.Repositories;
using Timesheet_app.Services;

var builder = WebApplication.CreateBuilder(args);

// Register Repositories
builder.Services.AddScoped<ITimesheetRepo, TimesheetRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IHollidayRepo, HollidayRepo>();

// Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITimesheetService, TimesheetService>();
builder.Services.AddScoped<IHollidayService, HollidayService>();
builder.Services.AddScoped<ITimesheetExportService, TimesheetExportService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var sqlConn = builder.Configuration.GetSection("ConnectionStringsSql")["SqlServer"];
if (string.IsNullOrWhiteSpace(sqlConn))
    throw new InvalidOperationException("SQL connection string not found. Set ConnectionStringsSql:SqlServer in appsettings.json.");

builder.Services.AddDbContext<SQLServerDBConnection>(options =>
    options.UseSqlServer(sqlConn));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
