namespace SchoolBuddy_APIs.Models.Master.Students
{
    public class getstudent
    {
   

        public int id { get; set; }
        public string ? user_id { get; set; }
        public string? admission_no { get; set; }
        public string? student_name { get; set; }
        public string? gender { get; set; }
        public string? birth { get; set; }

        public string? class_ { get; set; }
        public string? division { get; set; }
        public string? parent_name { get; set; }
        public string? mobile_no1 { get; set; }
        public string? password { get; set; }
        public string?street { get; set; }
        public string? created_date { get; set; }
        public string? rf_id { get; set; }
        public string? email { get; set; }
        public string? relation_with_std { get; set; }
        public string? blood_group { get; set; }
        public string? qr_code { get; set; }
    }

    public class assign_singal_student
    {
        public string student_name { get; set; }
        public string schoolid { get; set; }
        public string Admission_no { get; set; }
        //public string rfid { get; set; }
       public string stopid { get; set; }

       public string route_id { get; set; }
        public string? rfid { get; set; }


        //public string status { get; set; }
    }
}
