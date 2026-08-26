namespace SchoolBuddy_APIs.Models.Master.Students
{
    public class error
    {
        public string student_name { get; set; }
        public string admission_no { get; set; }
        public string Stopid { get; set;}
        public string rfid { get; set; }
        public string reason_for_not_linked { get; set; }


    }

    public class error_student_upload
    {
        public string student_name { get; set; }
        public string admission_no { get; set; }
        public string mobile_no1 { get; set; }
        public string class_ { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string birth { get; set; }
        public string division { get; set; }
        public string parent_name { get; set; }
        public string gender { get; set; }
        public string reason_for_not_upload { get; set; }


    }
}
