namespace SchoolBuddy_APIs.Models.Report
{
    public class StopVoilation
    {
        public string schoolid { get; set; }
        public int[] route { get; set; }

        public string date { get; set; }
    }
    public class routedata
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public double speed { get; set; }
        public DateTime date { get; set; }
        public double angle { get; set; }
        public double odometer { get; set; }
    }
}
