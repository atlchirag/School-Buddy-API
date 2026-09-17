using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolBuddy_APIs.Database_methods;
using System.Data;
using System.Globalization;
using static SchoolBuddy_APIs.Models.Attendance.Attendance;

namespace SchoolBuddy_APIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class AttendanceController:ControllerBase
    {
        private readonly Idatabase_access _sql_qury_execution;
        public AttendanceController(Idatabase_access sql_query_execution)
        {
            _sql_qury_execution = sql_query_execution;

        }
        
        [HttpPost]
        public IActionResult DaywiseAttendence([FromBody] DayWise notfy)
        {

            try
            {
                string user_id = notfy.user_id;
                string json = "";
                DateTime fromDateTime = DateTime.ParseExact(
                    notfy.date.Trim(),
                    "MM/dd/yyyy",
                    CultureInfo.InvariantCulture
                );

                string from = fromDateTime.ToString("yyyy-MM-dd"); string db = notfy.database;
                //string servicesTable;

                //if (string.Equals(
                //    db,
                //    "newtrack",
                //    StringComparison.OrdinalIgnoreCase))
                //{
                //    servicesTable = "newtrack.dbo.services";
                //}
                //else
                //{
                   string servicesTable = "atltracking.dbo.tbl_services";
                //}
                if (user_id != null)
                {
                    string query = @$"
DECLARE @AttendanceDate DATE = '{from}';
DECLARE @UserId INT = {user_id};

;WITH Students AS
(
    SELECT
        bs.id AS StudentId,
        ISNULL(bs.admission_no, '') AS AdmissionNo,
        ISNULL(bs.student_name, '') AS StudentName,

        -- Keep students even when RFID is missing
        NULLIF(
            LTRIM(RTRIM(CAST(bs.rf_id AS VARCHAR(100)))),
            ''
        ) AS Rfid,

        ISNULL(bs.class, '') AS ClassId,
        ISNULL(bs.section, '') AS SectionName,
        ISNULL(bs.father_name, '') AS FatherName,
        ISNULL(bs.mobile_no1, '') AS MobileNo,
        CASE LTRIM(RTRIM(ISNULL(bs.class, '')))
    WHEN '1' THEN 'K.G./BAL VATIKA 1'
    WHEN '2' THEN 'I'
    WHEN '3' THEN 'II'
    WHEN '4' THEN 'III'
    WHEN '5' THEN 'IV'
    WHEN '6' THEN 'V'
    WHEN '7' THEN 'VI'
    WHEN '8' THEN 'VII'
    WHEN '9' THEN 'VIII'
    WHEN '10' THEN 'IX'
    WHEN '11' THEN 'PRE PRIMARY'
    WHEN '12' THEN 'PRE SCHOOL'
    WHEN '13' THEN 'XI'
    WHEN '14' THEN 'XII'
    WHEN '15' THEN 'X'
    WHEN '16' THEN 'UKG/BAL VATIKA 3'
    WHEN '17' THEN 'NURSERY'
    WHEN '18' THEN 'PRE NURSERY'
    ELSE ISNULL(bs.class, '')
END AS ClassName

    FROM bs_student_master_backup bs

    WHERE bs.sys_user_id = @UserId
),

AttendancePunches AS
(
    SELECT
        LTRIM(RTRIM(CAST(r.rfid AS VARCHAR(100)))) AS Rfid,
        r.sys_service_id AS ServiceId,
        ISNULL(s.veh_reg, '') AS VehicleNo,
        r.punch_gps_time,

        ROW_NUMBER() OVER
        (
            PARTITION BY
                LTRIM(RTRIM(CAST(r.rfid AS VARCHAR(100))))
            ORDER BY r.punch_gps_time DESC
        ) AS RowNo

    FROM rf_punch_history r

    INNER JOIN {servicesTable} s
        ON s.id = r.sys_service_id

    WHERE s.sys_user_id = @UserId
      AND r.rfid IS NOT NULL
      AND LTRIM(RTRIM(CAST(r.rfid AS VARCHAR(100)))) <> ''
      AND r.punch_gps_time >= @AttendanceDate
      AND r.punch_gps_time < DATEADD(DAY, 1, @AttendanceDate)
)

SELECT
st.ClassName,
    st.StudentId,
    st.AdmissionNo,
    st.StudentName,
    ISNULL(st.Rfid, '') AS Rfid,
    st.ClassId,
    st.SectionName,
    st.FatherName,
    st.MobileNo,
    

    CONVERT(VARCHAR(10), @AttendanceDate, 23) AS AttendanceDate,

    ISNULL(
        CONVERT(VARCHAR(8), ap.punch_gps_time, 108),
        ''
    ) AS AttendanceTime,

    ISNULL(ap.ServiceId, 0) AS ServiceId,
    ISNULL(ap.VehicleNo, '') AS VehicleNo,

    CASE
        WHEN st.Rfid IS NULL THEN 'Absent'
        WHEN ap.Rfid IS NOT NULL THEN 'Present'
        ELSE 'Absent'
    END AS AttendanceStatus,

    CASE
        WHEN st.Rfid IS NULL THEN 'RFID Not Assigned'
        WHEN ap.Rfid IS NULL THEN 'No Punch Found'
        ELSE 'Punch Found'
    END AS AttendanceRemark

FROM Students st

LEFT JOIN AttendancePunches ap
    ON ap.Rfid = st.Rfid
   AND ap.RowNo = 1

ORDER BY
    CASE
        WHEN ap.Rfid IS NOT NULL THEN 1
        ELSE 2
    END,
    st.ClassId,
    st.SectionName,
    st.StudentName;

";

                    DataTable datatable = _sql_qury_execution.DML_Select(query.ToString());
                    if (datatable != null)
                    {
                        if (datatable.Rows.Count > 0)
                        {
                            var settings = new JsonSerializerSettings
                            {
                                DateFormatString = "yyyy-MM-dd HH:mm:ss"
                            };
                            json = JsonConvert.SerializeObject(datatable, Formatting.Indented, settings); return Content(json, "application/json");
                        }//datatable has rows
                        else
                        {
                            return Content("Data Not Found", "application/json");
                        }//datatable has no rows
                    }//datatable is not null

                    return Content("Something Went Wrong", "application/json");

                }//user is not equal to null.
                else
                {
                    return Content("Login Again..", "application/json");
                }//user id null.
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return Content(err_msg, "application/json");

            }//catch block ends.
        }


        [HttpPost]
        public IActionResult ClassWiseAttendence([FromBody] ClassWiseAttendanceRequest notfy)
        {

            try
            {
                string user_id = notfy.user_id;
                string json = "";
                DateTime fromDateTime = DateTime.ParseExact(
    notfy.from_date.Trim(),
    "yyyy-MM-dd",
    CultureInfo.InvariantCulture
);

                string from = fromDateTime.ToString("yyyy-MM-dd");
                string db = notfy.database;
                //string servicesTable;

                //if (string.Equals(
                //    db,
                //    "newtrack",
                //    StringComparison.OrdinalIgnoreCase))
                //{
                //    servicesTable = "services";
                //}
                //else
                //{
                   string servicesTable = "atltracking.dbo.tbl_services";
                //}
                if (user_id != null)
                {
                    string query = $@"
DECLARE @AttendanceDate DATE = '{from}';
DECLARE @UserId INT = {user_id};
DECLARE @ClassId VARCHAR(100) = '{notfy.class_id}';

;WITH ClassStudents AS
(
    SELECT
        bs.id AS StudentId,

        ISNULL(bs.admission_no, '') AS AdmissionNo,

        ISNULL(bs.student_name, '') AS StudentName,

        NULLIF(
            LTRIM(RTRIM(CAST(bs.rf_id AS VARCHAR(100)))),
            ''
        ) AS Rfid,

        CASE LTRIM(RTRIM(ISNULL(bs.class, '')))
            WHEN '1'  THEN 'K.G./BAL VATIKA 1'
            WHEN '2'  THEN 'I'
            WHEN '3'  THEN 'II'
            WHEN '4'  THEN 'III'
            WHEN '5'  THEN 'IV'
            WHEN '6'  THEN 'V'
            WHEN '7'  THEN 'VI'
            WHEN '8'  THEN 'VII'
            WHEN '9'  THEN 'VIII'
            WHEN '10' THEN 'IX'
            WHEN '11' THEN 'PRE PRIMARY'
            WHEN '12' THEN 'PRE SCHOOL'
            WHEN '13' THEN 'XI'
            WHEN '14' THEN 'XII'
            WHEN '15' THEN 'X'
            WHEN '16' THEN 'UKG/BAL VATIKA 3'
            WHEN '17' THEN 'NURSERY'
            WHEN '18' THEN 'PRE NURSERY'
            ELSE ISNULL(bs.class, '')
        END AS ClassName,

        ISNULL(bs.section, '') AS SectionName,

        ISNULL(bs.father_name, '') AS FatherName,

        ISNULL(bs.mobile_no1, '') AS MobileNo

    FROM bs_student_master_backup bs

    WHERE bs.sys_user_id = @UserId
      AND LTRIM(RTRIM(ISNULL(bs.class, ''))) =
          LTRIM(RTRIM(@ClassId))
),

LastStudentAttendance AS
(
    SELECT
        LTRIM(RTRIM(CAST(r.rfid AS VARCHAR(100)))) AS Rfid,
        r.sys_service_id,
        r.punch_gps_time,
        ISNULL(s.veh_reg, '') AS VehicleNo,

        ROW_NUMBER() OVER
        (
            PARTITION BY
                LTRIM(RTRIM(CAST(r.rfid AS VARCHAR(100))))
            ORDER BY r.punch_gps_time DESC
        ) AS RowNo

    FROM rf_punch_history r

    INNER JOIN {servicesTable} s
        ON s.id = r.sys_service_id

    WHERE s.sys_user_id = @UserId
      AND r.rfid IS NOT NULL
      AND LTRIM(RTRIM(CAST(r.rfid AS VARCHAR(100)))) <> ''
      AND r.sys_service_id IS NOT NULL
      AND r.punch_gps_time >= @AttendanceDate
      AND r.punch_gps_time < DATEADD(DAY, 1, @AttendanceDate)
)

SELECT
    cs.StudentId,
    cs.AdmissionNo,
    cs.StudentName,
    cs.ClassName,
    cs.SectionName,
    cs.FatherName,
    cs.MobileNo,

    ISNULL(cs.Rfid, '') AS Rfid,

    ISNULL(
        la.sys_service_id,
        0
    ) AS ServiceId,

    ISNULL(
        la.VehicleNo,
        ''
    ) AS VehicleNo,

    CONVERT(
        VARCHAR(10),
        @AttendanceDate,
        23
    ) AS AttendanceDate,

    CASE
        WHEN la.punch_gps_time IS NULL
            THEN ''
        ELSE CONVERT(
            VARCHAR(8),
            la.punch_gps_time,
            108
        )
    END AS AttendanceTime,

    CASE
        WHEN la.Rfid IS NOT NULL
            THEN 'Present'
        ELSE 'Absent'
    END AS AttendanceStatus,

    CASE
        WHEN cs.Rfid IS NULL
            THEN 'RFID Not Assigned'
        WHEN la.Rfid IS NULL
            THEN 'No Punch Found'
        ELSE 'Punch Found'
    END AS AttendanceRemark

FROM ClassStudents cs

LEFT JOIN LastStudentAttendance la
    ON la.Rfid = cs.Rfid
   AND la.RowNo = 1

ORDER BY
    CASE
        WHEN la.Rfid IS NOT NULL THEN 1
        ELSE 2
    END,
    cs.SectionName,
    cs.StudentName;
";

                    DataTable datatable = _sql_qury_execution.DML_Select(query.ToString());
                    if (datatable != null)
                    {
                        if (datatable.Rows.Count > 0)
                        {
                            var settings = new JsonSerializerSettings
                            {
                                DateFormatString = "yyyy-MM-dd HH:mm:ss"
                            };
                            json = JsonConvert.SerializeObject(datatable, Formatting.Indented, settings);
                            return Content(json, "application/json");
                        }//datatable has rows
                        else
                        {
                            return Content("Data Not Found", "application/json");
                        }//datatable has no rows
                    }//datatable is not null

                    return Content("Something Went Wrong", "application/json");

                }//user is not equal to null.
                else
                {
                    return Content("Login Again..", "application/json");
                }//user id null.
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return Content(err_msg, "application/json");

            }//catch block ends.
        }
    }
}
