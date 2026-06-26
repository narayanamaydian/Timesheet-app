namespace Timesheet_app.Models
{
    public class TimesheetTemplateMappingModel
    {
        public string Id { get; set; }

        public string TemplateId { get; set; } // FK to BaseTemplate ONLY

        public string DataHeader { get; set; } // internal field name (e.g. "UserName", "ClockIn")

        public string ExcelHeader { get; set; } // header from Excel (e.g. "Nama", "Jam Masuk")
    }

}
