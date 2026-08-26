namespace SchoolBuddy_APIs.Models.Master.Route
{
    public class Stop
    {
        public string Stop_Name { get; set; }
        public string route_id { get; set; }
        public string? id { get; set; }
        public string uid { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string stop_order { get; set; }

       
    }
    public class stopid
    {
        public string id { get; set; }

    }

    public class stop_id
    {
        public string id { get; set; }
        public string rid { get; set; }

    }
}
