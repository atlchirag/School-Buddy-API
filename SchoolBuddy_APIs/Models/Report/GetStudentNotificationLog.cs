namespace SchoolBuddy_APIs.Models.Report
{
    public class GetStudentNotificationLog
    {
        
        public string datefrom { get; set; }
        public string dateto { get; set; }
        public string student_id { get; set; }
        public string user_id { get; set;}
    }
}
