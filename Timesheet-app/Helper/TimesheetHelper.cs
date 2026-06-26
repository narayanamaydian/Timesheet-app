using Timesheet_app.Models;
using Timesheet_app.Models.DAO;
using Timesheet_app.Models.DTOs;

namespace Timesheet_app.Helper
{
    public class TimesheetHelper
    {
        /// <summary>
        /// Generates a timesheet ID based on userId and current date.
        /// Format: {userId}-{yyyyMMdd}
        /// </summary>
        public static string GetIdByClockAndUserId(string userId, out DateOnly dateOnly, out TimeOnly timeOnly)
        {
            var dateNow = DateTimeHelper.NowJakarta();
            dateOnly = DateOnly.FromDateTime(dateNow);
            timeOnly = TimeOnly.FromDateTime(dateNow);
            return userId + "-" + dateOnly.ToString("yyyyMMdd");
        }

        /// <summary>
        /// Converts a list of TimesheetModel (DAO) to TimesheetDTO.
        /// </summary>
        public static List<TimesheetDTO> ConvertToDTOs(List<TimesheetModel> models)
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
                TaskDetail = m.TaskDetail,
            }).ToList();
        }

        /// <summary>
        /// Converts TimesheetDTO to TimesheetModel (DAO) with user information.
        /// </summary>
        public static TimesheetModel ConvertToDAO(TimesheetDTO dto, UserDto user)
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
                UserID = user.Id,
                TaskDetail = dto.TaskDetail,
            };
        }

        /// <summary>
        /// Converts a single TimesheetModel (DAO) to TimesheetDTO.
        /// </summary>
        public static TimesheetDTO ConvertToDTO(TimesheetModel model)
        {
            return new TimesheetDTO
            {
                Id = model.Id,
                Date = model.Date,
                ClockIn = model.ClockIn,
                ClockOut = model.ClockOut,
                AccumulatedTime = model.AccumulatedTime,
                WFO = (TimesheetDTO.WorkStatus)(WorkStatus)model.WFO,
                Working = model.Working,
                TaskDetail = model.TaskDetail,
            };
        }
    }
}
