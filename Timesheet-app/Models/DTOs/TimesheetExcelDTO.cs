using System.ComponentModel.DataAnnotations;

namespace Timesheet_app.Models.DTOs
{
    public class TimesheetExcelDTO
    {
        [Display(Name = "Nama")]
        public string Nama { get; set; }

        [Display(Name = "Vendor")]
        public string Vendor { get; set; }

        [Display(Name = "SPK")]
        public string SPK { get; set; }

        [Display(Name = "Tanggal")]
        public string Tanggal { get; set; }

        [Display(Name = "Flexy Hour Start")]
        public string FlexyHourStart { get; set; }

        [Display(Name = "Flexy Hour End")]
        public string FlexyHourEnd { get; set; }

        [Display(Name = "Hour")]
        public string Hour { get; set; }

        [Display(Name = "Project ID - Project Name")]
        public string ProjectIdProjectName { get; set; }

        [Display(Name = "Activity")]
        public string Activity { get; set; }

        [Display(Name = "WFO/WFH")]
        public string WfoWfh { get; set; }
    }
}
