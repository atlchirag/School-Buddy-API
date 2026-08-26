using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolBuddy_APIs.Database_methods;
using SchoolBuddy_APIs.Models.Indoor;
using System.Data;
using System.Text;

namespace SchoolBuddy_APIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StaffController : Controller
    {
        private readonly Idatabase_access _sql_qury_execution;

        public StaffController(Idatabase_access sql_qury_execution)
        {
            _sql_qury_execution = sql_qury_execution;
        }

        [HttpPost]
        public bool AddStaffDetails([FromBody] Staff staff)
        {
            try
            {
                string query = $@"
        INSERT INTO bs_staff_master
        (
            emp_code,
name,
            rf_id,
            mobile_no,
            address,
            in_time,
            out_time,
            sys_user_id
        )
        VALUES
        (
            '{staff.emp_code}',
            '{staff.name}',
            '{staff.rf_id}',
            '{staff.mobile_no}',
            '{staff.address}',
            '{staff.in_time:yyyy-MM-dd HH:mm:ss}',
            '{staff.out_time:yyyy-MM-dd HH:mm:ss}',
             {staff.sys_user_id}
        )";

                int result =
                    _sql_qury_execution
                    .DML_Insert_Update_Delete(query);
                if (result > 0)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                string err_msg = ex.Message;

                return false;
            }
        }

        [HttpGet]
        public IActionResult GetAllStaffDetails(int uid)
        {
            try
            {
                string query = $@"
                    SELECT
                        emp_code,name,
                        rf_id,
                        mobile_no,
                        address,
                        in_time,
                        out_time,
                        sys_user_id
                    FROM bs_staff_master
                    WHERE sys_user_id = {uid}
                    ORDER BY emp_code";

                DataTable staffTable = _sql_qury_execution.DML_Select(query);
                string json = JsonConvert.SerializeObject(staffTable, Formatting.Indented);

                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddStaffInBulk([FromForm] IFormFile file, [FromForm] string user_id)
        {
            List<string> errors = new List<string>();
            int insertedCount = 0;

            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { status = "error", message = "No file uploaded" });
                }

                if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { status = "error", message = "Only .xlsx files are accepted." });
                }

                if (string.IsNullOrWhiteSpace(user_id))
                {
                    return BadRequest(new { status = "error", message = "User id is required." });
                }

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true
                            }
                        });

                        DataTable table = result.Tables[0];

                        foreach (DataRow row in table.Rows)
                        {
                            try
                            {
                                string empCodeValue = GetCellValue(row, "Employee Code");
                                string mobileNo = EscapeSql(GetCellValue(row, "Mobile Number"));
                                string rfId = EscapeSql(GetCellValue(row, "RFID"));
                                string address = EscapeSql(GetCellValue(row, "Address"));
                                string inTimeValue = GetCellValue(row, "In Time");
                                string outTimeValue = GetCellValue(row, "Out Time");
                                string name = GetCellValue(row, "Name");
                                var empCode = empCodeValue;
                                //if (!int.TryParse(empCodeValue, out int empCode))
                                //{
                                //    errors.Add($"Skipping row: Invalid Employee Code - {JsonConvert.SerializeObject(row.ItemArray)}");
                                //    continue;
                                //}

                                if (string.IsNullOrWhiteSpace(mobileNo) ||
                                    string.IsNullOrWhiteSpace(rfId) ||
                                    string.IsNullOrWhiteSpace(address))
                                {
                                    errors.Add($"Skipping employee code {empCode}: Mobile Number, RFID and Address are required.");
                                    continue;
                                }

                                if (!TryParseExcelDate(row, "In Time", inTimeValue, out DateTime inTime))
                                {
                                    errors.Add($"Skipping employee code {empCode}: Invalid In Time.");
                                    continue;
                                }


                                if (!TryParseExcelDate(row, "Out Time", outTimeValue, out DateTime outTime))
                                {
                                    errors.Add($"Skipping employee code {empCode}: Invalid Out Time.");
                                    continue;
                                }

                                string escapedUserId = EscapeSql(user_id);
                                string duplicateCheckQuery = $@"
                                    SELECT emp_code
                                    FROM bs_staff_master
                                    WHERE emp_code = {empCode}
                                      AND sys_user_id = '{escapedUserId}'";

                                DataTable existingStaff = _sql_qury_execution.DML_Select(duplicateCheckQuery);
                                if (existingStaff != null && existingStaff.Rows.Count > 0)
                                {
                                    errors.Add($"Skipping duplicate employee code: {empCode}");
                                    continue;
                                }
                                string duplicateRFIDCheckQuery = $@"
                                    SELECT rf_id
                                    FROM bs_staff_master
                                    WHERE emp_code = {empCode}
                                      AND sys_user_id = '{escapedUserId}'";

                                DataTable existingrfid = _sql_qury_execution.DML_Select(duplicateRFIDCheckQuery);
                                if (existingrfid != null && existingrfid.Rows.Count > 0)
                                {
                                    errors.Add($"Skipping duplicate RFID code: {empCode}");
                                    continue;
                                }

                                string insertQuery = $@"
                                    INSERT INTO bs_staff_master
                                    (
                                        emp_code,
                                        name,
                                        rf_id,
                                        mobile_no,
                                        address,
                                        in_time,
                                        out_time,
                                        sys_user_id
                                    )
                                    VALUES
                                    (
                                        {empCode},
                                        '{name}',
                                        '{rfId}',
                                        '{mobileNo}',
                                        '{address}',
                                        '{inTime:yyyy-MM-dd HH:mm:ss}',
                                        '{outTime:yyyy-MM-dd HH:mm:ss}',
                                        '{escapedUserId}'
                                    )";

                                int rowsAffected = _sql_qury_execution.DML_Insert_Update_Delete(insertQuery);
                                if (rowsAffected > 0)
                                {
                                    insertedCount++;
                                }
                                else
                                {
                                    errors.Add($"Database insert failed for employee code: {empCode} may RFID is assign to someone Else ");
                                }
                            }
                            catch (Exception rowEx)
                            {
                                errors.Add($"Error inserting staff row: {rowEx.Message}");
                            }
                        }
                    }
                }

                if (insertedCount > 0)
                {
                    return Ok(new { status = "success", message = $"{insertedCount} staff records added successfully!", errors });
                }

                return BadRequest(new { status = "error", message = "No staff records were added.", errors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = $"An error occurred: {ex.Message}", errors });
            }
        }

        private static string GetCellValue(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName)
                ? row[columnName]?.ToString()?.Trim() ?? ""
                : "";
        }

        private static bool TryParseExcelDate(DataRow row, string columnName, string value, out DateTime dateTime)
        {
            dateTime = default;

            if (row.Table.Columns.Contains(columnName) && row[columnName] is DateTime typedDate)
            {
                dateTime = typedDate;
                return true;
            }

            if (double.TryParse(value, out double oaDate))
            {
                dateTime = DateTime.FromOADate(oaDate);
                return true;
            }

            return DateTime.TryParse(value, out dateTime);
        }

        private static string EscapeSql(string value)
        {
            return value.Replace("'", "''").Trim();
        }
    }
}