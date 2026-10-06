using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolBuddy_APIs.Database_methods;
using SchoolBuddy_APIs.Models.App;
using SchoolBuddy_APIs.Services;
using System.Data;
using System.Globalization;

namespace SchoolBuddy_APIs.Controllers
{
    ///<summary>
    /// School panel side of parent leave requests (parents apply through app/LeaveRequest).
    /// bs_leave_master.is_approved : 0 = Applied, 1 = Approved, 2 = Rejected, 3 = Cancelled.
    /// Needs Database_scripts/bs_leave_master_status.sql to be run once.
    ///</summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly Idatabase_access _sql_qury_execution;
        private readonly ILogger<LeaveController> _logger;

        public LeaveController(Idatabase_access sql_qury_execution,  ILogger<LeaveController> logger)
        {
            _sql_qury_execution = sql_qury_execution;
            _logger = logger;
        }

        [HttpPost]
        ///<summary>
        /// All leave requests of a school for one month (by leave date).
        /// month format : yyyy-MM, defaults to the current month.
        ///</summary>
        public IActionResult GetLeaveRequests(string user_id, string? month)
        {
            try
            {
                if (!int.TryParse(user_id, out int schoolId))
                {
                    return Content(JsonConvert.SerializeObject(new { status = "0", msg = "user_id is required" }), "application/json");
                }

                if (!DateTime.TryParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime monthStart))
                {
                    monthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                }

                string query = @"
SELECT
    l.id,
    l.student_id,
    s.student_name,
    s.admission_no,
    ISNULL(c.class_name, ISNULL(s.class, '')) AS class_name,
    ISNULL(s.section, '') AS section,
    ISNULL(s.father_name, '') AS parent_name,
    ISNULL(s.mobile_no1, '') AS mobile_no,
    l.reason,
    l.applied_date AS leave_date,
    l.applied_on,
    l.is_approved AS status,
    ISNULL(l.remark, '') AS remark,
    l.action_on
FROM bs_leave_master l
INNER JOIN bs_student_master_backup s
    ON s.id = l.student_id
LEFT JOIN bs_class_master c
    ON c.id = TRY_CONVERT(int, s.class)
WHERE s.sys_user_id = @SchoolId
  AND l.applied_date >= @MonthStart
  AND l.applied_date < @MonthEnd
ORDER BY l.applied_date DESC, l.applied_on DESC;";

                DataTable? dt = _sql_qury_execution.DML_Select(query, new Dictionary<string, object>
                {
                    { "@SchoolId", schoolId },
                    { "@MonthStart", monthStart },
                    { "@MonthEnd", monthStart.AddMonths(1) }
                });

                if (dt == null)
                {
                    return Content(JsonConvert.SerializeObject(new { status = "0", msg = "fail" }), "application/json");
                }

                var settings = new JsonSerializerSettings { DateFormatString = "yyyy-MM-dd HH:mm:ss" };
                return Content(JsonConvert.SerializeObject(new
                {
                    status = "1",
                    msg = "success",
                    month = monthStart.ToString("yyyy-MM"),
                    data = dt
                }, settings), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLeaveRequests failed");
                return Content(JsonConvert.SerializeObject(new { status = "0", msg = ex.Message }), "application/json");
            }
        }

        public class UpdateLeaveStatusRequest
        {
            public string? user_id { get; set; }
            public int leave_id { get; set; }
            public int status { get; set; }
            public string? remark { get; set; }
        }

        [HttpPost]
        ///<summary>
        /// Approve / reject / cancel a leave request and notify the parent
        /// (FCM push + bs_notification_parent row, same as PIS_Engine).
        ///</summary>
        public IActionResult UpdateLeaveStatus([FromBody] UpdateLeaveStatusRequest request)
        {
            try
            {
                if (request == null || !int.TryParse(request.user_id, out int schoolId) || request.leave_id <= 0)
                {
                    return Content(JsonConvert.SerializeObject(new { status = "0", msg = "Invalid input" }), "application/json");
                }

                if (!LeaveStatus.IsActionable(request.status))
                {
                    return Content(JsonConvert.SerializeObject(new { status = "0", msg = "Invalid status" }), "application/json");
                }

                string remark = (request.remark ?? "").Trim();
                if (remark.Length > 500)
                {
                    remark = remark.Substring(0, 500);
                }

                // Fetch first: confirms the leave belongs to this school and gives what the notification needs.
                string detailQuery = @"
SELECT
    l.id,
    ISNULL(l.is_approved, 0) AS status,
    l.applied_date,
    s.id AS student_id,
    s.student_name,
    ISNULL(TRY_CONVERT(int, s.bs_user_id), TRY_CONVERT(int, s.parent_id)) AS parent_id
FROM bs_leave_master l
INNER JOIN bs_student_master_backup s
    ON s.id = l.student_id
WHERE l.id = @LeaveId
  AND s.sys_user_id = @SchoolId;";

                DataTable? detail = _sql_qury_execution.DML_Select(detailQuery, new Dictionary<string, object>
                {
                    { "@LeaveId", request.leave_id },
                    { "@SchoolId", schoolId }
                });

                if (detail == null || detail.Rows.Count == 0)
                {
                    return Content(JsonConvert.SerializeObject(new { status = "-1", msg = "Leave request not found" }), "application/json");
                }

                DataRow row = detail.Rows[0];
                if (Convert.ToInt32(row["status"]) == request.status)
                {
                    return Content(JsonConvert.SerializeObject(new { status = "-1", msg = $"Leave is already {LeaveStatus.Text(request.status)}" }), "application/json");
                }

                string updateQuery = $@"
UPDATE bs_leave_master
SET is_approved = {request.status},
    is_pending = 1,
    remark = {(remark == "" ? "NULL" : $"'{remark.Replace("'", "''")}'")},
    action_by = {schoolId},
    action_on = GETDATE()
WHERE id = {request.leave_id};";

                int rowsAffected = _sql_qury_execution.DML_Insert_Update_Delete(updateQuery);
                if (rowsAffected <= 0)
                {
                    return Content(JsonConvert.SerializeObject(new { status = "0", msg = "fail" }), "application/json");
                }

               // NotifyParent(row, request.status, remark);

                return Content(JsonConvert.SerializeObject(new { status = "1", msg = $"Leave {LeaveStatus.Text(request.status)}" }), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateLeaveStatus failed");
                return Content(JsonConvert.SerializeObject(new { status = "0", msg = ex.Message }), "application/json");
            }
        }

        
    }
}
