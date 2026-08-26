namespace SchoolBuddy_APIs.Models.Master.Students
{
    public class Assign_bulk_student
    {
        public IFormFile file { get; set; }
        public string r_id { get; set; }
    }

    public class assign_student
    {
        public string student_name { get; set; }
        public string Admission_no { get; set; }
        public string stopid { get; set; }
        public string rfid { get; set; }
        public string schoolid { get; set; }
    }
}
