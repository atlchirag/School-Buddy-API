using SchoolBuddy_APIs.Database_methods;

namespace SchoolBuddy_APIs.Models.Master.Holidays_And_Events
{
    public class HolidayRepository : IHoliday
    {
        private readonly Idatabase_access _sql_qury_execution;

        public HolidayRepository(Idatabase_access sql_qury_execution)
        {
            _sql_qury_execution = sql_qury_execution ?? throw new ArgumentNullException(nameof(sql_qury_execution));
        }

        public Task<bool> AddHoliday(string start_date, string end_date, string eventname)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateHoliday(int id, string start_date, string end_date, string schoolid, string eventname, string status)
        {
            try
            {
                string query = $@"
                    UPDATE bs_holidays 
                    SET from_date = '{start_date}', 
                        to_date = '{end_date}', 
                        description = '{eventname}', 
                        type = '{status}' 
                    WHERE id = {id} AND sys_user_id = '{schoolid}'";

                int rowsAffected = _sql_qury_execution.DML_Insert_Update_Delete(query);

                Console.WriteLine($"🔥 Rows Affected: {rowsAffected}");

                // ✅ FIX: अगर Row Update हो गई तो TRUE Return करो
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"✅ Holiday Updated Successfully! Returning TRUE");
                    return true;
                }
                else
                {
                    Console.WriteLine($"❌ No Rows Affected! Returning FALSE");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Exception in UpdateHoliday: " + ex.Message);
                return false;
            }
        }

    }
}
