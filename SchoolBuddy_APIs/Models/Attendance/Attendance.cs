namespace SchoolBuddy_APIs.Models.Attendance
{
    public class Attendance
    {
        public class DayWise
        {
            public string user_id { get; set; }
            public string? date { get; set; }
            public string? database { get; set; }

        }
        public class ClassWiseAttendanceRequest
        {
            public string user_id { get; set; }

            public string class_id { get; set; }

            public string from_date { get; set; } = string.Empty;

            public string database { get; set; }
        }
    }
}
