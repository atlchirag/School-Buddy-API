namespace SchoolBuddy_APIs.Models.Analysis
{
    public class CheckIgnition
    {
        public string service_id { get; set; }
        public string start_date { get; set;}
        public string end_date { get; set; }
        public string database { get; set; }
    }

    public class CheckInActive
    {
        public string service_id { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string database { get; set; }
    }
    public class CheckRoute
    {
        public string schoolid { get; set; }
        
    }

    public class Checkvias
    {
        public string route_id { get; set; }

    }

    public class Checketa
    {
        public int route_id
        {
            get; set;
        }
    }




}
