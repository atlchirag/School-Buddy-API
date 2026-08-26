namespace SchoolBuddy_APIs.Models.Indoor
{
    public class Staff
    {
        public string emp_code { get; set; }
        public string rf_id { get; set; }
        public string mobile_no { get; set; }
        public string address { get; set; }
        public DateTime in_time { get; set; }
        public DateTime out_time { get; set; }
        public int sys_user_id { get; set; }
        public string name { get; set; }
    }
}