namespace SchoolBuddy_APIs.Models.Login
{
    public class Login
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    public class login_result
    {
        public string user_id { get; set; }
        public string database { get; set; }
        public string schoolname { get; set; }
    }
}
