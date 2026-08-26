namespace SchoolBuddy_APIs.Models.Report
{
    public interface IReportRepository
    {
        Task<string> GetNotification(string id);
        Task<string> GetStudentNotificationLog(string id);
        Task<string> NoGps();
        Task<string> LoginReport();
        Task<string> RfidReport();
        Task<string> StopViolation();
        Task<string> Uhfreport();

    }
}
