namespace SchoolBuddy_APIs.Models.Driver
{
    public class Driver
    {
        public class DriverPerformanceModel
        {
            public int DriverId { get; set; }

            public string DriverName { get; set; }

            public string BusName { get; set; }

            public string DriverStatus { get; set; }

            public string BusStatus { get; set; }
        }
    }
}
