using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolBuddy_APIs.Database_methods;
using SchoolBuddy_APIs.Models.Master.Holidays_And_Events;
using System.Data;

namespace SchoolBuddy_APIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HolidayController : ControllerBase
    {
        private readonly Idatabase_access _sql_qury_execution;
        private readonly IHoliday _holiday;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HolidayController(Idatabase_access sql_qury_execution, IHoliday holiday, IHttpContextAccessor httpContextAccessor)
        {
            _sql_qury_execution = sql_qury_execution;
            _holiday = holiday;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> AddHoliday([FromBody] holiday_properties holidayData)
        {
            try
            {
                // Log the received data for debugging
                Console.WriteLine($" API Received Data: {JsonConvert.SerializeObject(holidayData)}");

                // Validate the input data
                if (holidayData == null || string.IsNullOrEmpty(holidayData.start) ||
                    string.IsNullOrEmpty(holidayData.end) || string.IsNullOrEmpty(holidayData.eventname) ||
                    string.IsNullOrEmpty(holidayData.status) || string.IsNullOrEmpty(holidayData.schoolid))
                {
                    Console.WriteLine("Invalid input data.");
                    return BadRequest(new { message = "Invalid input data." });
                }

                // Check the date format
                if (!DateTime.TryParse(holidayData.start, out var startDate) || !DateTime.TryParse(holidayData.end, out var endDate))
                {
                    Console.WriteLine("Invalid date format.");
                    return BadRequest(new { message = "Invalid date format." });
                }

                // Log the query for debugging
                string query_newtrack = $@"INSERT INTO bs_holidays 
        (sys_user_id, from_date, to_date, description, added_on, type)
        VALUES 
        ('{holidayData.schoolid}', '{startDate.ToString("yyyy-MM-dd HH:mm:ss")}', '{endDate.ToString("yyyy-MM-dd HH:mm:ss")}', '{holidayData.eventname}', GETDATE(), '{holidayData.status}')";

                Console.WriteLine($"Executing SQL: {query_newtrack}");

                // Execute the SQL query
                int row_affected = _sql_qury_execution.DML_Insert_Update_Delete(query_newtrack);

                // Log the result of the query
                if (row_affected > 0)
                {
                    return Ok(new { message = "Holiday added successfully!" });
                }
                else
                {
                    Console.WriteLine("Database insertion failed.");
                    return BadRequest(new { message = "Database insertion failed." });
                }
            }
            catch (Exception e)
            {
                // Log the exception details for debugging
                Console.WriteLine($" Exception in AddHoliday: {e.Message}");
                return StatusCode(500, new { message = "Internal Server Error", error = e.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateHoliday([FromBody] holiday_properties holidayData)
            {
            try
            {

                Console.WriteLine($" Received Data: {JsonConvert.SerializeObject(holidayData)}");

                if (holidayData == null || string.IsNullOrEmpty(holidayData.start) ||
                    string.IsNullOrEmpty(holidayData.end) || string.IsNullOrEmpty(holidayData.eventname) ||
                    string.IsNullOrEmpty(holidayData.status))
                {
                    return BadRequest(new { message = "Invalid input data. Please check the form values." });
                }

                //  Correctly Get User ID from Session
                string uid = _httpContextAccessor.HttpContext?.Session.GetString("uid");
                uid = holidayData.schoolid;
                if (string.IsNullOrEmpty(uid))
                {
                    Console.WriteLine(" Session UID is NULL");
                    return Unauthorized(new { message = "User not logged in" });
                }

                Console.WriteLine($" Retrieved UID from session: {uid}");

                var response = await _holiday.UpdateHoliday(holidayData.id, holidayData.start, holidayData.end, uid, holidayData.eventname, holidayData.status);

                if (response)
                {
                    Console.WriteLine($" Holiday Updated Successfully! Returning TRUE");
                    return Ok(true);

                }
                else
                {
                    return BadRequest(new { message = "Failed to update holiday." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Exception in UpdateHoliday: " + ex.Message);
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteHoliday([FromBody] DeleteHolidayModel holidayData)
        {
            try
            {
                if (holidayData == null || holidayData.id <= 0 || string.IsNullOrEmpty(holidayData.schoolid))
                {
                    return BadRequest(new { message = "Invalid request data." });
                }

                // ✅ Query fix, ab `sys_user_id` bhi use ho raha hai
                string query = $@"
        DELETE FROM bs_holidays 
        WHERE id = {holidayData.id} AND sys_user_id = '{holidayData.schoolid}'";

                int rowsAffected = _sql_qury_execution.DML_Insert_Update_Delete(query);

                if (rowsAffected > 0)
                {
                    return Ok(true);
                }
                else
                {
                    return BadRequest(new { message = "Failed to delete holiday. Record not found." });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "Internal Server Error", error = e.Message });
            }
        }


        [HttpPost]
        public IActionResult GetHolidays([FromBody] school school)
        {
            try
            {
                if (!String.IsNullOrEmpty(school.id))
                {
                    string getholidays = $@"SELECT * FROM bs_holidays WHERE sys_user_id = '{school.id}'";
                    DataTable dataTable = _sql_qury_execution.DML_Select(getholidays);

                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        string json = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
                        return Content(json, "application/json");
                    }
                    return NoContent(); // Return HTTP 204 if no holidays found
                }
                return BadRequest(new { message = "Invalid School ID" });
            }
            catch (Exception e)
            {
                Console.WriteLine(" Exception in GetHolidays: " + e.Message);
                return StatusCode(500, new { message = "Internal Server Error", error = e.Message });
            }
        }
    }
}
