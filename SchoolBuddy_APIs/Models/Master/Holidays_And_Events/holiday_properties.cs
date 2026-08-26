using System.ComponentModel.DataAnnotations;

namespace SchoolBuddy_APIs.Models.Master.Holidays_And_Events
{
    public class holiday_properties
    {
        public int id { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string eventname { get; set; }
        public string schoolid { get; set; }

        [Required(ErrorMessage = "The status field is required.")]
        public string status { get; set; }  // ✅ Ensure this is included
    }

    public class DeleteHolidayModel
    {
        public int id { get; set; }
        public string schoolid { get; set; }
    }


    public class school
    {
        public string  id { get; set; }
    }
}
