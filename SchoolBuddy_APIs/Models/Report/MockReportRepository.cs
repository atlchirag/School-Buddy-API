namespace SchoolBuddy_APIs.Models.Report
{
    public class MockReportRepository : IReportRepository
    {
        public Task<string> GetNotification(string id)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetStudentNotificationLog(string id)
        {
            throw new NotImplementedException();
        }

        public Task<string> LoginReport()
        {
            throw new NotImplementedException();
        }

        public Task<string> NoGps()
        {
            throw new NotImplementedException();
        }

        public Task<string> RfidReport()
        {
            throw new NotImplementedException();
        }

        public Task<string> StopViolation()
        {
            throw new NotImplementedException();
        }

        public Task<string> Uhfreport()
        {
            throw new NotImplementedException();
        }
    }
}
