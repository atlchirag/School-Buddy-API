using Microsoft.AspNetCore.Mvc;
using SchoolBuddy_APIs.Database_methods;
using SchoolBuddy_APIs.Models.Dashboard;
using System.Data;

namespace SchoolBuddy_APIs.Controllers
{
    /// <summary>
    /// Video Telematics (MDVR camera) support for the School Buddy portal.
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VideoTelematicsController : ControllerBase
    {
        private readonly Idatabase_access _sql_qury_execution;

        public VideoTelematicsController(Idatabase_access sql_qury_execution)
        {
            _sql_qury_execution = sql_qury_execution;
        }

        ///<summary>
        /// IsMdvrEnabled checks whether the school has any vehicle with an MDVR camera.
        /// Enabled when at least one of the school's services has is_mdvr = 1.
        /// if database is newtrack, tables used : services, devices.
        /// otherwise, tables used : atltracking.dbo.tbl_services, atltracking.dbo.tbl_devices.
        /// Request : { "user_id": "123", "database": "atltracking" }
        /// Response: { status, message, isMdvrEnabled, mdvrVehicleCount, vehicles: [ { service_id, veh_reg, imei } ] }
        ///</summary>
        [HttpPost]
        public IActionResult IsMdvrEnabled(dashboard dashboard)
        {
            if (dashboard == null || !int.TryParse(dashboard.user_id, out int userId) || userId <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Invalid user id.",
                    isMdvrEnabled = false,
                    mdvrVehicleCount = 0
                });
            }

            bool isNewtrack = string.Equals(dashboard.database, "newtrack", StringComparison.OrdinalIgnoreCase);
            string servicesTable = isNewtrack ? "services" : "atltracking.dbo.tbl_services";
            string devicesTable = isNewtrack ? "devices" : "atltracking.dbo.tbl_devices";

            string query = $@"
SELECT
    s.id AS service_id,
    s.veh_reg,
    d.imei
FROM {servicesTable} s
LEFT JOIN {devicesTable} d ON d.id = s.sys_device_id
WHERE s.sys_user_id = @UserId
  AND s.is_mdvr = 1
  AND s.sys_renewal_due >= GETDATE()
ORDER BY s.veh_reg";

            DataTable? datatable = _sql_qury_execution.DML_Select(query, new Dictionary<string, object>
            {
                { "@UserId", userId }
            });

            if (datatable == null)
            {
                // DML_Select logs the SQL error and returns null (e.g. is_mdvr missing in this database).
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    status = false,
                    message = "Unable to check MDVR status.",
                    isMdvrEnabled = false,
                    mdvrVehicleCount = 0
                });
            }

            var vehicles = datatable.AsEnumerable()
                .Select(row => new
                {
                    service_id = row["service_id"] == DBNull.Value ? 0 : Convert.ToInt32(row["service_id"]),
                    veh_reg = row["veh_reg"]?.ToString() ?? string.Empty,
                    imei = row["imei"]?.ToString() ?? string.Empty
                })
                .ToList();

            return Ok(new
            {
                status = true,
                message = vehicles.Count > 0
                    ? "MDVR is enabled"
                    : "No MDVR vehicles are linked to this school.",
                isMdvrEnabled = vehicles.Count > 0,
                mdvrVehicleCount = vehicles.Count,
                vehicles
            });
        }
    }
}
