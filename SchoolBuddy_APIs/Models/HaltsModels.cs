namespace SchoolBuddy_APIs.Models.Halts
{
    public class RouteDto
    {
        public int RouteId { get; set; }
        public string RouteName { get; set; } = "";
    }

    public class TelemetryPoint
    {
        public DateTime GpsTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
    }

    public class RouteTelemetryResponse
    {
        public int RouteId { get; set; }
        public string RouteName { get; set; } = "";
        public List<TelemetryPoint> Points { get; set; } = new();
    }

    public class MultiDayTelemetryResponse
    {
        public int DayIndex { get; set; }
        public DateTime Date { get; set; }
        public List<TelemetryPoint> Points { get; set; } = new();
    }
}