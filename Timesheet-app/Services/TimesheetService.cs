using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using System.Globalization;
using Timesheet_app.Helper;
using Timesheet_app.Models;
using Timesheet_app.Models.DAO;
using Timesheet_app.Models.DTOs;
using Timesheet_app.Repositories;

namespace Timesheet_app.Services
{
    public class TimesheetService : ITimesheetService
    {
        private readonly ITimesheetRepo _timesheetRepo;
        private readonly IHollidayService _holidayService;
        private readonly IUserService _userService;
        private readonly IUserProjectService _userProjectService;


        public TimesheetService(ITimesheetRepo timesheetRepo, IUserService userService, IHollidayService holidayService, IUserProjectService userProjectService)
        {
            _timesheetRepo = timesheetRepo;
            _holidayService = holidayService;
            _userService = userService;
            _userProjectService = userProjectService;
        }

        public async Task<TimesheetUserDTO> GetTimesheetByMonthAsync(string userId, int? month, int year)
        {
            var timesheet = await _timesheetRepo.GetTimesheetByMonth(userId, month, year);
            var user = timesheet[1].User;
            var timesheetDto = TimesheetHelper.ConvertToDTOs(timesheet);

            return new TimesheetUserDTO
            {
                Timesheet = timesheetDto,
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    VendorName = user.VendorName,
                    NoSpk = user.NoSpk

                }
            };
        }

        public async Task<TimesheetUserDTO> GetTimesheetByUserAsync(string userId, int? month)
        {
            month ??= DateTime.Now.Month;
            var year = DateTime.Now.Year;

            var timesheet = await _timesheetRepo.GetTimesheetByMonth(userId, month, year);
            var user = timesheet[1].User;
            var timesheetDto = TimesheetHelper.ConvertToDTOs(timesheet);

            return new TimesheetUserDTO
            {
                Timesheet = timesheetDto,
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    VendorName = user.VendorName,
                    NoSpk = user.NoSpk
                }
            };
        }

        public async Task<TimesheetDTO> AddTimesheetAsync(TimesheetDTO timesheet, string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {userId} not found");
            }
            
            var timesheetDAO = TimesheetHelper.ConvertToDAO(timesheet, user);
            await _timesheetRepo.AddTimesheet(timesheetDAO);
            return timesheet;

        }

        public async Task<List<TimesheetDTO>> GenerateTimesheetForMonthAsync(string userId, int month, int year)
        {
            var timesheets = await GenerateTimesheetsForMonth(userId, month, year);

            if (timesheets.Count == 0)
            {
                return timesheets;
            }

            foreach (var timesheet in timesheets)
            {
                await AddTimesheetAsync(timesheet, userId);
            }

            return timesheets;
        }
        public async Task<DataTable> GetTimesheetByMonthForUsersAsync(int month, int year, string userId)
        {
            var timesheets = await _timesheetRepo.GetTimesheetByMonth(userId, month, year);
            var projects = await _userProjectService.GetProjectsByUserIdAsync(userId);
            var projectName = projects.Count > 0 ? projects[0].ProjectName : string.Empty;

            foreach (var timesheet in timesheets)
            {
                var getDate = timesheet.Date;
                var holiday = _holidayService.GetHolidayByDate(getDate).Result;
            }
            var user = timesheets[1].User;
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Timesheet");

            var timesheetDto = TimesheetHelper.ConvertToDTOs(timesheets);


            var newArrayColumnName = new string[] { "User ID", "Name", "Vendor Name", "No SPK", "Date", "Clock In", "Clock Out", "Accumulated Time", "WFO", "Working", "Holiday Description", "Activity", "ProjectName" };

            var table = new DataTable();
            foreach (var columnName in newArrayColumnName)
            {
                table.Columns.Add(columnName);
            }

            foreach (var timesheet in timesheetDto)
            {
                var status = ExtensionHelper.GetEnumDescription(timesheet.WFO);
                var holiday = await _holidayService.GetHolidayByDate(timesheet.Date);
                var holidayDescription = holiday != null ? holiday.Description : string.Empty;
                var clockIn = timesheet.ClockIn.HasValue ? timesheet.ClockIn.Value.ToString("hh:mm:ss tt") : string.Empty;
                var clockOut = timesheet.ClockOut.HasValue ? timesheet.ClockOut.Value.ToString("hh:mm:ss tt") : string.Empty;
                var date = timesheet.Date.ToString("dddd, dd MMMM yyyy");

                table.Rows.Add(user.Id, user.Name, user.VendorName, user.NoSpk, date, clockIn, clockOut, timesheet.AccumulatedTime, status, timesheet.Working, holidayDescription, timesheet.TaskDetail, projectName);
            }

            return table;
        }

        

        public async Task<TimesheetDTO> AddOrUpdateClockIn(string userId, int status)
        {
            var timesheetId = TimesheetHelper.GetIdByClockAndUserId(userId, out DateOnly dateOnly, out TimeOnly timeOnly);
            var existingTimesheet = await _timesheetRepo.GetTimesheetById(timesheetId);
            var workStatus = (TimesheetDTO.WorkStatus)status;

            if (existingTimesheet != null)
            {
                existingTimesheet.ClockIn = timeOnly;
                existingTimesheet.Working = true;
                existingTimesheet.WFO = (WorkStatus)TimesheetDTO.WorkStatus.WFH;
                _ = _timesheetRepo.UpdateTimesheet(existingTimesheet);
                return TimesheetHelper.ConvertToDTO(existingTimesheet);
            }
            else
            {
                var newTimesheet = new TimesheetModel
                {
                    Id = timesheetId,
                    Date = dateOnly,
                    ClockIn = timeOnly,
                    ClockOut = null,
                    AccumulatedTime = TimeSpan.Zero,
                    Working = true,
                    WFO = (WorkStatus)workStatus,
                    UserID = userId
                };

                await _timesheetRepo.AddTimesheet(newTimesheet);
                return TimesheetHelper.ConvertToDTO(newTimesheet);

            }
        }

       

        public async Task<TimesheetDTO> AddOrUpdateClockOut(string userId, string activity)
        {
            DateOnly dateOnly;
            TimeOnly timeOnly;
            var timesheetId = TimesheetHelper.GetIdByClockAndUserId(userId, out dateOnly, out timeOnly);
            var existingTimesheet = await _timesheetRepo.GetTimesheetById(timesheetId);
            if (existingTimesheet == null)
            {
                throw new InvalidOperationException("Please Input Clock In First");
            }
            else
            {
                var clockIn = (TimeOnly)existingTimesheet.ClockIn;
                var AccumulatedTime = timeOnly.ToTimeSpan() - clockIn.ToTimeSpan();
                existingTimesheet.TaskDetail = activity;
                existingTimesheet.ClockOut = timeOnly;
                existingTimesheet.AccumulatedTime = AccumulatedTime;


            }
            await _timesheetRepo.UpdateTimesheet(existingTimesheet);
            return TimesheetHelper.ConvertToDTO(existingTimesheet);

        }

        private async Task<List<TimesheetDTO>> GenerateTimesheetsForMonth(string userId, int month, int year)
        {
            var timesheets = new List<TimesheetDTO>();

            var daysInMonth = DateTime.DaysInMonth(year, month);

            for (int day = 1; day <= daysInMonth; day++)
            {
                var currentDate = new DateOnly(year, month, day);

                var isHoliday = await _holidayService.GetHolidayByDate(currentDate) != null;

                var isWeekend = ExtensionHelper.IsWeekend(currentDate);

                var timesheetId = userId + "-" + currentDate.ToString("yyyyMMdd");

                var isItExist = _timesheetRepo.GetTimesheetById(timesheetId).Result;
                if (isItExist != null)
                {
                    continue;
                }
                TimeOnly? clockIn = new TimeOnly(9, 0, 0);
                TimeOnly? clockOut = new TimeOnly(17, 0, 0);
                var totalTime = clockOut.Value.ToTimeSpan() - clockIn.Value.ToTimeSpan();

                if (isHoliday || isWeekend)
                {
                    clockIn = null;
                    clockOut = null;
                    totalTime = TimeSpan.Zero;
                }

                var newTimesheet = new TimesheetDTO
                {
                    Id = timesheetId,
                    Date = currentDate,
                    ClockIn = clockIn,
                    ClockOut = clockOut,
                    AccumulatedTime = totalTime,
                    Working = !isWeekend && !isHoliday,
                    WFO = (!isWeekend && !isHoliday) ? TimesheetDTO.WorkStatus.WFH : TimesheetDTO.WorkStatus.NotWorking
                };

                timesheets.Add(newTimesheet);
            }

            return timesheets;
        }
    }
}