namespace SchoolBuddy_APIs.Models.Master.Holidays_And_Events
{
    public interface IHoliday
    {
        Task<bool> AddHoliday(string start_date,string end_date,string eventname);
        Task<bool> UpdateHoliday(int id, string start_date, string end_date, string schoolid, string eventname, string status);


    }
}
