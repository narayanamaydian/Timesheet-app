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

        public TimesheetService(ITimesheetRepo timesheetRepo, IUserService userService, IHollidayService holidayService)
        {
            _timesheetRepo = timesheetRepo;
            _holidayService = holidayService;
            _userService = userService;
        }

        public async Task<TimesheetUserDTO> GetTimesheetByMonthAsync(string userId, int? month, int year)
        {
            var timesheet = await _timesheetRepo.GetTimesheetByMonth(userId, month, year);
            var user = timesheet[1].User;
            var timesheetDto = ConvertToDTOs(timesheet);

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
            var timesheetDto = ConvertToDTOs(timesheet);

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
            
            var timesheetDAO = ConvertToDAO(timesheet, user);
            await _timesheetRepo.AddTimesheet(timesheetDAO);
            return timesheet;

        }

        public async Task<List<TimesheetDTO>> GenerateTimesheetForMonthAsync(string userId, int month, int year)
        {
            var timesheets = await GenerateTimesheetsForMonth(userId, month, year);

            if (timesheets.Count == 0)
            {
                throw new InvalidOperationException($"Timesheets for user {userId} in {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)}, {year} already exist");
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

            foreach (var timesheet in timesheets)
            {
                var getDate = timesheet.Date;
                var holiday = _holidayService.GetHolidayByDate(getDate).Result;
            }
            var user = timesheets[1].User;
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Timesheet");

            var timesheetDto = ConvertToDTOs(timesheets);


            var newArrayColumnName = new string[] { "User ID", "Name", "Vendor Name", "No SPK", "Date", "Clock In", "Clock Out", "Accumulated Time", "WFO", "Working", "Holiday Description" };

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
                table.Rows.Add(user.Id, user.Name, user.VendorName, user.NoSpk, timesheet.Date, timesheet.ClockIn, timesheet.ClockOut, timesheet.AccumulatedTime, status, timesheet.Working, holidayDescription);
            }

            return table;
        }

        private async Task<List<TimesheetDTO>> GenerateTimesheetsForMonth(string userId, int month, int year)
        {
            var timesheets = new List<TimesheetDTO>();

            var daysInMonth = DateTime.DaysInMonth(year, month);
            var clockIn = new TimeOnly(9, 0, 0);
            var clockOut = new TimeOnly(17, 0, 0);
            var totalTime = clockOut.ToTimeSpan() - clockIn.ToTimeSpan();

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



        private static List<TimesheetDTO> ConvertToDTOs(List<TimesheetModel> models)
        {
            return models.Select(m => new TimesheetDTO
            {
                Id = m.Id,
                Date = m.Date,
                ClockIn = m.ClockIn,
                ClockOut = m.ClockOut,
                AccumulatedTime = m.AccumulatedTime,
                Working = m.Working,
                WFO = (TimesheetDTO.WorkStatus)(WorkStatus)m.WFO,
            }).ToList();
        }
        private static TimesheetModel ConvertToDAO(TimesheetDTO dto, UserDto user)
        {
            
            return new TimesheetModel
            {
                Id = dto.Id,
                Date = dto.Date,
                ClockIn = dto.ClockIn,
                ClockOut = dto.ClockOut,
                AccumulatedTime = dto.AccumulatedTime,
                Working = dto.Working,
                WFO = (WorkStatus)dto.WFO,
                UserID = user.Id
            };
        }

        
    }
}