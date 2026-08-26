namespace SchoolBuddy_APIs.Models.Dashboard
{
    public class dashboard
    {
        public string user_id { get; set; }
        public string? database { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }


    }
    public class boarding
    {
        public string user_id { get; set; }
        public string? database { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public string? from { get; set; }
        public string? to { get; set; }
    }
}