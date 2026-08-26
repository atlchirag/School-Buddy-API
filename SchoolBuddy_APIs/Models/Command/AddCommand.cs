namespace SchoolBuddy_APIs.Models.Command
{
    public class AddCommand
    {
        public string sys_user_id { get; set; }
        public string route_id { get; set; }
        public string message { get; set; }
        public string reason { get; set; }

    }
}
