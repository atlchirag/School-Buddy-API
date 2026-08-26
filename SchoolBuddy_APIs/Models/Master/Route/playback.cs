namespace SchoolBuddy_APIs.Models.Master.Route
{
    public class playback
    {

        public string lat { get; set; }
        public string lng { get; set; }
        public int via_order { get; set; }
        public string route_id { get; set; }
    }

    public class playback_atl_newtrack
    {
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string service_id { get; set; }
        public string database { get; set; }

    }
}
