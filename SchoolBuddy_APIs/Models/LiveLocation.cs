namespace SchoolBuddy_APIs.Models
{
    public class LiveLocation
    {
        public string service_id { get; set; } = "";

        public string gps_latitude { get; set; } = "";

        public string gps_longitude { get; set; } = "";

        public string gps_speed { get; set; } = "";

        public string gps_time { get; set; } = "";

        public string server_time { get; set; } = "";

        public string battery_voltage { get; set; } = "";
    }
}
