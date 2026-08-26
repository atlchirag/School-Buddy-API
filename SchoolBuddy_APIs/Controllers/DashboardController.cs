using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolBuddy_APIs.Database_methods;
using SchoolBuddy_APIs.Models.App;
using SchoolBuddy_APIs.Models.Dashboard;
using SchoolBuddy_APIs.Models.Login;
using SchoolBuddy_APIs.Models.Master.Holidays_And_Events;
using SchoolBuddy_APIs.Models.Master.Route;
using SchoolBuddy_APIs.Models.Master.Students;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using static SchoolBuddy_APIs.Models.Driver.Driver;

namespace SchoolBuddy_APIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly Idatabase_access _sql_qury_execution;
        public DashboardController(Idatabase_access sql_qury_execution)
        {
            _sql_qury_execution = sql_qury_execution;
        }

        [HttpPost]

        ///<summary>
        /// GetTotalStudentCount actionmethod calculate count of students.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public int GetTotalStudentCount(dashboard getallstudents)
        {
            try
            {
                string user_id = getallstudents.user_id;
                string json = "";
                if (user_id != null)
                {
                    string query = $"select count(*) from bs_student_master_backup where sys_user_id = '{user_id}'";
                    DataTable datatable = _sql_qury_execution.DML_Select(query);
                    if (datatable != null)
                    {
                        if (datatable.Rows.Count > 0)
                        {
                            return Convert.ToInt32(datatable.Rows[0][0]);
                        }//datatable has rows
                        else
                        {
                            return -1;
                        }//datatable has no rows
                    }//datatable is not null

                    return 0;

                }//user is not equal to null.
                else
                {
                    return 0;
                }//user id null.
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return 0;

            }//catch block ends.

        }


        [HttpPost]
        public int GetTotalassignstudentCount(dashboard getallstudents)
        {
            try
            {
                string query = $@"
SELECT COUNT(DISTINCT bsmb.id) AS total_students
FROM bs_student_master_backup bsmb

INNER JOIN bs_route_students brs
    ON brs.student_id = bsmb.id

INNER JOIN bs_route_master brm
    ON brm.id = brs.route_id

INNER JOIN bs_class_master bcm
    ON bcm.id = bsmb.class

WHERE bsmb.sys_user_id = {getallstudents.user_id}";
                DataTable datatable = _sql_qury_execution.DML_Select(query);
                if (datatable != null)
                {
                    if (datatable.Rows.Count > 0)
                    {
                        return Convert.ToInt32(datatable.Rows[0][0]);
                    }//datatable has rows
                    else
                    {
                        return -1;
                    }//datatable has no rows
                }//datatable is not null

                return 0;

            }
            catch
            {
                return 0;
            }
        }

        [HttpPost]



        ///<summary>
        /// GetTotalRouteCount actionmethod calculate count of total route.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public int GetTotalRouteCount(dashboard getallstudents)
        {
            try
            {
                string user_id = getallstudents.user_id;
                string json = "";
                if (user_id != null)
                {
                    string query = $"select count(*) from bs_route_master where sys_user_id = {user_id}";
                    DataTable datatable = _sql_qury_execution.DML_Select(query);
                    if (datatable != null)
                    {
                        if (datatable.Rows.Count > 0)
                        {
                            return Convert.ToInt32(datatable.Rows[0][0]);
                        }//datatable has rows
                        else
                        {
                            return -1;
                        }//datatable has no rows
                    }//datatable is not null

                    return 0;

                }//user is not equal to null.
                else
                {
                    return 0;
                }//user id null.
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return 0;

            }//catch block ends.

        }

        [HttpPost]

        public int GetTotalBuses(dashboard dashboard)
        {
            try
            {
                string user_id = dashboard.user_id;

                if (string.IsNullOrEmpty(user_id))
                {
                    // If the user ID is null or empty, return 0 immediately.
                    return 0;
                }

                string query = $@"
            SELECT COUNT(*) 
            FROM [atltracking].[dbo].tbl_services s
            INNER JOIN [atltracking].[dbo].tbl_devices d ON d.id = s.sys_device_id
            WHERE s.sys_user_id = {user_id}";

            //    string query1 = $@"
            //SELECT COUNT(*) 
            //FROM services s
            //INNER JOIN devices d ON d.id = s.sys_device_id
            //WHERE s.sys_user_id = {user_id}";

                // Execute the queries
                DataTable datatable = _sql_qury_execution.DML_Select(query);
                //DataTable datatable1 = _sql_qury_execution.DML_Select(query1);

                // Initialize count variables
                int count1 = datatable != null && datatable.Rows.Count > 0
                    ? Convert.ToInt32(datatable.Rows[0][0])
                    : 0;

                //int count2 = datatable1 != null && datatable1.Rows.Count > 0
                //    ? Convert.ToInt32(datatable1.Rows[0][0])
                //    : 0;

                // Combine the counts
                //int totalCount = count1 + count2;
                int totalCount = count1;

                // Return the total count
                return totalCount;
            }
            catch (Exception ex)
            {
                // Log the error for debugging purposes
                string err_msg = ex.Message;
                // Return 0 on exception
                return 0;
            }
        }


        ///<summary>
        /// GetTotalBuses actionmethod calculate count of Buses.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        //public int GetTotalBuses(dashboard dashboard)
        //{
        //    try
        //    {
        //        string user_id = dashboard.user_id;
        //        string json = "";
        //        string query = "";
        //        if (user_id != null)
        //        {

        //            //if(dashboard.database == "atltracking")
        //            //{
        //            //    query = $"select * from [atltracking].[dbo].tbl_services where sys_user_id = {user_id}";
        //            //}
        //            //else 
        //            //{
        //            //    query = $"select * from services where sys_user_id = {user_id}";
        //            //}

        //            if (dashboard.database == "atltracking")
        //            {
        //                query = $@"select count(*) from [atltracking].[dbo].tbl_services s
        //                                inner join [atltracking].[dbo].tbl_devices d
        //                                on
        //                                d.id = s.sys_device_id
        //                                inner join bs_route_master brm
        //                                on brm.sys_service_id = s.id
        //                                where s.sys_user_id = {user_id}";
        //            }
        //            else
        //            {
        //                query = $@"select count(*) from services s
        //                                inner join devices d
        //                                on
        //                                d.id = s.sys_device_id
        //                                inner join bs_route_master brm
        //                                on brm.sys_service_id = s.id
        //                                where s.sys_user_id = {user_id}";
        //            }



        //            DataTable datatable = _sql_qury_execution.DML_Select(query);

        //            if (datatable != null)
        //            {
        //                if (datatable.Rows.Count > 0)
        //                {
        //                    return Convert.ToInt32(datatable.Rows[0][0]);
        //                }//datatable has rows
        //                else
        //                {
        //                    return -1;
        //                }//datatable has no rows
        //            }//datatable is not null

        //            return 0;

        //        }//user is not equal to null.
        //        else
        //        {
        //            return 0;
        //        }//user id null.
        //    }//try block ends.
        //    catch (Exception ex)
        //    {
        //        string err_msg = ex.Message;
        //        return 0;

        //    }//catch block ends.

        //}

        [HttpPost]

        ///<summary>
        /// GetTotalNotifications actionmethod calculate count of total notifications.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public int GetTotalNotifications(dashboard dashboard)
        {
            try
            {
                string user_id = dashboard.user_id;
                string json = "";
                if (user_id != null)
                {
                    string query = $"select count(*) from bs_notification_parent where parent_id in (select id from bs_user_master where sys_user_id = {user_id})";
                    DataTable datatable = _sql_qury_execution.DML_Select(query);
                    if (datatable != null)
                    {
                        if (datatable.Rows.Count > 0)
                        {
                            return Convert.ToInt32(datatable.Rows[0][0]);
                        }//datatable has rows
                        else
                        {
                            return -1;
                        }//datatable has no rows
                    }//datatable is not null

                    return 0;

                }//user is not equal to null.
                else
                {
                    return 0;
                }//user id null.
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return 0;

            }//catch block ends.

        }
        [HttpPost]


        ///<summary>
        /// GetTodayNotifications actionmethod calculate count of today's total notifications.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public int GetTodayNotifications(dashboard dashboard)
        {
            try
            {
                string datetime = DateTime.Now.ToString("yyyy-MM-dd");
                string user_id = dashboard.user_id;
                string json = "";
                if (user_id != null)
                {
                    //string query = $@"select count(*) from bs_notification_parent where parent_id in 
                    //                (select id from bs_user_master where sys_user_id = {user_id})
                    //                and date_time >= CONVERT(DATETIME, '{datetime}', 120)";

                    //string query = $@"select count(*) from bs_notification_parent where parent_id in 
                    //                (select id from bs_user_master where sys_user_id = {user_id})
                    //                and cast(date_time as Date) = '{datetime}'";



                    //string query = $@"SELECT 
                    //                   count(*)
                    //                FROM 
                    //                    bs_student_master_backup bsmb
                    //                INNER JOIN 
                    //                    bs_route_students brs ON brs.student_id = bsmb.id
                    //                INNER JOIN 
                    //                    bs_route_master ON bs_route_master.id = brs.route_id
                    //                INNER JOIN 
                    //                    bs_user_master ON bs_user_master.bs_user_name = bsmb.mobile_no1
                    //                INNER JOIN 
                    //                    bs_notification_parent bnp ON bnp.parent_id = bs_user_master.id
                    //                WHERE 
                    //                    bsmb.sys_user_id = {user_id}
                    //                    AND bnp.date_time > CONVERT(DATETIME, '{DateTime.Now.AddDays(-1).ToString("MMM d yyyy h:mmtt")}', 100);";

                    string query = $@"SELECT 
                                        count(*)
                                    FROM 
                                        bs_student_master_backup bsmb
                                    INNER JOIN 
                                        bs_user_master ON bs_user_master.bs_user_name = bsmb.mobile_no1
                                    INNER JOIN 
                                        bs_notification_parent bnp ON bnp.parent_id = bs_user_master.id
                                    WHERE 
                                        bsmb.sys_user_id = '{user_id}'
                                        AND bnp.date_time >= CONVERT(DATETIME, CAST(GETDATE() AS DATE), 101) 
                                        AND bnp.date_time <= GETDATE(); ";
                    DataTable datatable = _sql_qury_execution.DML_Select(query);
                    if (datatable != null)
                    {
                        if (datatable.Rows.Count > 0)
                        {
                            return Convert.ToInt32(datatable.Rows[0][0]);
                        }//datatable has rows
                        else
                        {
                            return -1;
                        }//datatable has no rows
                    }//datatable is not null

                    return 0;

                }//user is not equal to null.
                else
                {
                    return 0;
                }//user id null.
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return 0;

            }//catch block ends.

        }

        [HttpPost]


        ///<summary>
        /// Getliveroute actionmethod calculate count of live routes.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public int Getliveroute(dashboard dashboard)
        {
            try
            {
                string user_id = dashboard.user_id;
                string database = dashboard.database?.ToLower();
                string json = "";
                if (user_id != null)
                {
                    string query = "";
                    //string query = $"select count(*) from bs_student_master_backup where sys_user_id = '{user_id}'";
                    //query = $@"select count(*)from latest_telemetry lt
                    //                   inner join 
                    //                   services 
                    //                   on services.id = lt.sys_service_id
                    //                   where services.sys_user_id = {user_id}
                    //                   and lt.gps_speed >0";
                    if (database == "newtrack")
                    {
                        // newtrack database uses 'services' and 'devices' directly
                        query = $@"select count(*)from latest_telemetry lt
                                        inner join 
                                        services 
                                        on services.id = lt.sys_service_id
                                        where services.sys_user_id = {user_id}
                                        and lt.gps_speed >0";
                    }
                    else if (database == "atltracking")
                    {
                        // atltracking database uses 'tbl_services' and 'tbl_devices', but references bs_route_master from newtrack
                        query = $@" select count(*)from atltracking.dbo.tbl_latest_telemetry lt
                                         inner join 
                                         atltracking.dbo.tbl_services s
                                         on s.id = lt.sys_service_id
                                         where s.sys_user_id = {user_id}
                                         and lt.gps_speed >0";
                    }
                    else
                    {
                        return 0;
                    }

                    DataTable datatable = _sql_qury_execution.DML_Select(query);
                    if (datatable != null)
                    {
                        if (datatable.Rows.Count > 0)
                        {
                            return Convert.ToInt32(datatable.Rows[0][0]);
                        }//datatable has rows
                        else
                        {
                            return -1;
                        }//datatable has no rows
                    }//datatable is not null

                    return 0;

                }//user is not equal to null.
                else
                {
                    return 0;
                }//user id null.
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return 0;

            }//catch block ends.

        }

        [HttpPost]

        ///<summary>
        /// TotalNotification_summ actionmethod fetches all data regarding notifications.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult TotalNotification_summ(dashboard dashboard)
        {
            string json = "";
            try
            {
                string user_id = dashboard.user_id;

                if (user_id != null)
                {
                    int? page = dashboard.page;
                    int? pageSize = dashboard.pageSize;
                    int? offset = (page - 1) * pageSize;
                    string query = $@"select bsmb.student_name,bsmb.admission_no,bsmb.mobile_no1
                                    ,bs_route_master.route_name,bnp.message,bnp.date_time from bs_student_master_backup bsmb
                                    inner join 
                                    bs_route_students brs
                                    on brs.student_id = bsmb.id
                                    inner join 
                                    bs_route_master 
                                    on bs_route_master.id = brs.route_id
                                    inner join bs_user_master
                                    on bs_user_master.bs_user_name =bsmb.mobile_no1
                                    inner join 
                                    bs_notification_parent bnp
                                    on bnp.parent_id  = bs_user_master.id
                                    where bsmb.sys_user_id = {user_id}
 ORDER BY bnp.date_time DESC
OFFSET {offset} ROWS
FETCH NEXT {pageSize} ROWS ONLY";
                    DataTable datatable = _sql_qury_execution.DML_Select(query);
                    if (datatable != null)
                    {
                        if (datatable.Rows.Count > 0)
                        {
                            json = JsonConvert.SerializeObject(datatable, Formatting.Indented);
                            return Content(json, "application/json");
                        }//datatable has rows
                        else
                        {
                            return Content(json, "application/json");
                        }//datatable has no rows
                    }//datatable is not null

                    return Content(json, "application/json");

                }//user is not equal to null.
                else
                {
                    return Content(json, "application/json");
                }//user id null.
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return Content(json, "application/json");

            }//catch block ends.

        }

        [HttpPost]


        ///<summary>
        /// TotalStudent_summ actionmethod fetches all data regarding total student.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult TotalStudent_summ(dashboard dashboard)
        {
            string json = "0";
            try
            {
                string user_id = dashboard.user_id;

                if (user_id != null)
                {
                    //string query = $@"select bsmb.student_name,bsmb.father_name,bcm.class_name,
                    //                    bsmb.mobile_no1,bsmb.rf_id,bsmb.admission_no,
                    //                    brm.route_name from bs_student_master_backup bsmb
                    //                    inner join bs_route_students brs
                    //                    on brs.student_id = bsmb.id
                    //                    inner join bs_route_master brm
                    //                    on brm.id=brs.route_id
                    //                    inner join 
                    //                    bs_class_master bcm
                    //                    on bcm.id = bsmb.class
                    //                    where bsmb.sys_user_id = {user_id}";



                    string query = $@"SELECT
                                        bsmb.student_name,
                                        bsmb.father_name,
                                        bcm.class_name,
                                        bsmb.mobile_no1,
                                        bsmb.rf_id,
                                        bsmb.admission_no,
                                        STRING_AGG(brm.route_name, ', ') WITHIN GROUP (ORDER BY brm.route_name) AS route_names
                                    FROM bs_student_master_backup bsmb
                                    INNER JOIN bs_route_students brs
                                        ON brs.student_id = bsmb.id
                                    INNER JOIN bs_route_master brm
                                        ON brm.id = brs.route_id
                                    INNER JOIN bs_class_master bcm
                                        ON bcm.id = bsmb.class
                                    WHERE bsmb.sys_user_id = {user_id}
                                    GROUP BY
                                        bsmb.student_name,
                                        bsmb.father_name,
                                        bcm.class_name,
                                        bsmb.mobile_no1,
                                        bsmb.rf_id,
                                        bsmb.admission_no;
                                    ";
                    DataTable datatable = _sql_qury_execution.DML_Select(query);
                    if (datatable != null)
                    {
                        if (datatable.Rows.Count > 0)
                        {
                            json = JsonConvert.SerializeObject(datatable, Formatting.Indented);
                            return Content(json, "application/json");
                        }//datatable has rows
                        else
                        {
                            return Content(json, "application/json");
                        }//datatable has no rows
                    }//datatable is not null

                    return Content(json, "application/json");

                }//user is not equal to null.
                else
                {
                    return Content(json, "application/json");
                }//user id null.
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return Content(json, "application/json");

            }//catch block ends.

        }

        //[HttpPost]


        ///<summary>
        /// TotalBus_summ actionmethod fetches all data regarding buses.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        //public IActionResult TotalBus_summ(dashboard dashboard)
        //{
        //    string json = "";
        //    try
        //    {
        //        string user_id = dashboard.user_id;
        //        string database = dashboard.database;

        //        if (user_id != null)
        //        { 
        //            string query = $@"select s.veh_reg,d.imei, brm.route_name from services s
        //                                inner join devices d
        //                                on
        //                                d.id = s.sys_device_id
        //                                inner join bs_route_master brm
        //                                on brm.sys_service_id = s.id
        //                                where s.sys_user_id = {user_id}
        //                                ";
        //            string query1 = $@"select s.veh_reg,d.imei, brm.route_name from tbl_services s
        //                                inner join tbl_devices d
        //                                on
        //                                d.id = s.sys_device_id
        //                                inner join newtrack.dbo.bs_route_master brm
        //                                on brm.sys_service_id = s.id
        //                                where s.sys_user_id = {user_id}";
        //            if (database == "newtrack")
        //            {
        //                DataTable datatable = _sql_qury_execution.DML_Select(query);

        //                if (datatable != null)
        //                {
        //                    if (datatable.Rows.Count > 0)
        //                    {
        //                        json = JsonConvert.SerializeObject(datatable, Formatting.Indented);
        //                        return Content(json, "application/json");
        //                    }//datatable has rows
        //                }//datatable is not null
        //            }
        //            else
        //            {
        //                DataTable datatable = _sql_qury_execution.DML_Select(query1);

        //                if (datatable != null)
        //                {
        //                    if (datatable.Rows.Count > 0)
        //                    {
        //                        json = JsonConvert.SerializeObject(datatable, Formatting.Indented);
        //                        return Content(json, "application/json");
        //                    }//datatable has no rows
        //                }//datatable is not null
        //            }

        //            return Content(json, "application/json");

        //        }//user is not equal to null.
        //        else
        //        {
        //            return Content(json, "application/json");
        //        }//user id null.
        //    }//try block ends.
        //    catch (Exception ex)
        //    {
        //        string err_msg = ex.Message;
        //        return Content(json, "application/json");

        //    }//catch block ends.

        //}

        [HttpPost]
        public IActionResult TotalBus_summ(dashboard dashboard)
        {
            string json = "";
            try
            {
                string user_id = dashboard.user_id;
                string database = dashboard.database?.ToLower(); // Normalize for comparison

                if (!string.IsNullOrEmpty(user_id))
                {
                    string query = "";

                    if (database == "newtrack")
                    {
                        // newtrack database uses 'services' and 'devices' directly
                        query = $@"
              SELECT
    s.veh_reg,
    d.imei,

    CASE
        WHEN brm.sys_service_id IS NULL
            THEN 'Not Assigned'
        ELSE brm.route_name
    END AS route_name,

    CASE
        WHEN lt.sys_service_id IS NULL
            THEN 'Offline'

        WHEN ISNULL(lt.gps_speed, 0) > 0
            THEN 'Live'

        WHEN ISNULL(lt.gps_speed, 0) = 0
             AND ISNULL(lt.i2, 0) = 1
            THEN 'Idle'

        WHEN ISNULL(lt.gps_speed, 0) = 0
             AND ISNULL(lt.i2, 0) = 0
            THEN 'Stopped'

        ELSE 'Offline'
    END AS vehicle_status

FROM services s

INNER JOIN devices d
    ON d.id = s.sys_device_id

LEFT JOIN bs_route_master brm
    ON brm.sys_service_id = s.id

LEFT JOIN latest_telemetry lt
    ON lt.sys_service_id = s.id

WHERE s.sys_user_id = {user_id}

ORDER BY
    CASE
        WHEN lt.sys_service_id IS NULL THEN 4
        WHEN ISNULL(lt.gps_speed, 0) > 0 THEN 1
        WHEN ISNULL(lt.gps_speed, 0) = 0
             AND ISNULL(lt.i2, 0) = 1 THEN 2
        WHEN ISNULL(lt.gps_speed, 0) = 0
             AND ISNULL(lt.i2, 0) = 0 THEN 3
        ELSE 4
    END,
    s.veh_reg;";
                    }
                    else if (database == "atltracking")
                    {
                        // atltracking database uses 'tbl_services' and 'tbl_devices', but references bs_route_master from newtrack
                        query = $@"
                    SELECT
    s.id AS sys_service_id,
    s.veh_reg,
    d.imei,
    ISNULL(brm.route_name, '-') AS route_name,

    CASE
        WHEN brm.sys_service_id IS NULL
            THEN 'Not Assigned'

        WHEN lt.sys_service_id IS NULL
            THEN 'Offline'

        WHEN ISNULL(lt.gps_speed, 0) > 0
            THEN 'Live'

        WHEN ISNULL(lt.gps_speed, 0) = 0
             AND ISNULL(lt.i2, 0) = 1
            THEN 'Idle'

        WHEN ISNULL(lt.gps_speed, 0) = 0
             AND ISNULL(lt.i2, 0) = 0
            THEN 'Stopped'

        ELSE 'Offline'
    END AS vehicle_status

FROM atltracking.dbo.tbl_services s

INNER JOIN atltracking.dbo.tbl_devices d
    ON d.id = s.sys_device_id

LEFT JOIN bs_route_master brm
    ON brm.sys_service_id = s.id

LEFT JOIN atltracking.dbo.tbl_latest_telemetry lt
    ON lt.sys_service_id = s.id

WHERE s.sys_user_id = {user_id}

ORDER BY s.veh_reg;";
                    }
                    else
                    {
                        return Content("{\"error\":\"Unsupported database type\"}", "application/json");
                    }

                    DataTable datatable = _sql_qury_execution.DML_Select(query);

                    if (datatable != null && datatable.Rows.Count > 0)
                    {
                        json = JsonConvert.SerializeObject(datatable, Formatting.Indented);
                    }

                    return Content(json, "application/json");
                }
                else
                {
                    return Content("{\"error\":\"Missing user_id\"}", "application/json");
                }
            }
            catch (Exception ex)
            {
                return Content($"{{\"error\":\"{ex.Message}\"}}", "application/json");
            }
        }




        [HttpPost]

        ///<summary>
        /// TotalLiveRoute_summ actionmethod fetches all data regarding live routes.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult TotalLiveRoute_summ(dashboard dashboard)
        {
            string json = "";
            try
            {
                string user_id = dashboard.user_id;
                string database = dashboard.database?.ToLower();

                if (user_id != null)
                {
                    string query = "";
                    //string query = $@"select lt.sys_proc_time,lt.gps_time,lt.gps_latitude,lt.gps_longitude,
                    //                    lt.gps_speed,lt.firmware_version,lt.i2,lt.battery_voltage,
                    //                    lt.signal_strength from latest_telemetry lt
                    //                    inner join 
                    //                    services 
                    //                    on services.id = lt.sys_service_id
                    //                    where services.sys_user_id = {user_id}
                    //                    and lt.gps_speed >0";

                    //           query = $@"select d.imei,services.veh_reg,lt.gps_time ,lt.gps_latitude,lt.gps_longitude,
                    //                              lt.gps_speed,lt.firmware_version,lt.i2,lt.battery_voltage,
                    //                              lt.signal_strength from latest_telemetry lt
                    //                              inner join 
                    //                              services 
                    //                              on services.id = lt.sys_service_id
                    //inner join
                    //devices d
                    //on services.sys_device_id = d.id
                    //                              where services.sys_user_id = {user_id}
                    //                              and lt.gps_speed >0";
                    if (database == "newtrack")
                    {
                        // newtrack database uses 'services' and 'devices' directly
                        query = $@"select d.imei,services.veh_reg,lt.gps_time ,lt.gps_latitude,lt.gps_longitude,
                                        lt.gps_speed,lt.firmware_version,lt.i2,lt.battery_voltage,
                                        lt.signal_strength from latest_telemetry lt
                                        inner join 
                                        services 
                                        on services.id = lt.sys_service_id
										inner join
										devices d
										on services.sys_device_id = d.id
                                        where services.sys_user_id = {user_id}
                                        and lt.gps_speed >0";
                    }
                    else if (database == "atltracking")
                    {
                        // atltracking database uses 'tbl_services' and 'tbl_devices', but references bs_route_master from newtrack
                        query = $@"select d.imei,s.veh_reg,lt.gps_time ,lt.gps_latitude,lt.gps_longitude,
                                        lt.gps_speed,lt.firmware_version,lt.i2,lt.battery_voltage,
                                        lt.signal_strength from atltracking.dbo.tbl_latest_telemetry lt
                                        left join 
                                        atltracking.dbo.tbl_services s
                                        on s.id = lt.sys_service_id
										left join
										atltracking.dbo.tbl_devices d
										on s.sys_device_id = d.id
                                        where s.sys_user_id = {user_id}
                                        and lt.gps_speed >0";
                    }
                    else
                    {
                        return Content("{\"error\":\"Unsupported database type\"}", "application/json");
                    }

                    DataTable datatable = _sql_qury_execution.DML_Select(query);
                    if (datatable != null)
                    {
                        if (datatable.Rows.Count > 0)
                        {
                            json = JsonConvert.SerializeObject(datatable, Formatting.Indented);
                            return Content(json, "application/json");
                        }//datatable has rows
                        else
                        {
                            return Content(json, "application/json");
                        }//datatable has no rows
                    }//datatable is not null

                    return Content(json, "application/json");

                }//user is not equal to null.
                else
                {
                    return Content(json, "application/json");
                }//user id null.
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return Content(json, "application/json");

            }//catch block ends.

        }


        [HttpPost]


        ///<summary>
        /// TotalStopRoute_summ actionmethod fetches all data regarding routes.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public int TotalStopRoute_summ(dashboard dashboard)
        {
            string json = "";
            try
            {
                string user_id = dashboard.user_id;

                if (user_id != null)
                {
                    string query = $@"select count(*) from latest_telemetry lt
                                        inner join 
                                        services 
                                        on services.id = lt.sys_service_id
                                        where services.sys_user_id = {user_id}
                                        and lt.gps_speed =0 and lt.i2 = 0";
                    DataTable datatable = _sql_qury_execution.DML_Select(query);
                    if (datatable != null)
                    {
                        if (datatable.Rows.Count > 0)
                        {
                            return Convert.ToInt32(datatable.Rows[0][0]);
                        }//datatable has rows
                        else
                        {
                            return 0;
                        }//datatable has no rows
                    }//datatable is not null

                    return 0;

                }//user is not equal to null.
                else
                {
                    return 0;
                }//user id null.
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return 0;

            }//catch block ends.

        }

        [HttpPost]


        ///<summary>
        /// TotalStopRoute_summ actionmethod fetches all data regarding routes which is idle.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public int TotalIdleRoute_summ(dashboard dashboard)
        {
            string json = "";
            try
            {
                string user_id = dashboard.user_id;

                if (user_id != null)
                {
                    string query = $@"select count(*) from latest_telemetry lt
                                        inner join 
                                        services 
                                        on services.id = lt.sys_service_id
                                        where services.sys_user_id = {user_id}
                                        and lt.gps_speed =0 and lt.i2 = 1";
                    DataTable datatable = _sql_qury_execution.DML_Select(query);
                    if (datatable != null)
                    {
                        if (datatable.Rows.Count > 0)
                        {


                            return Convert.ToInt32(datatable.Rows[0][0]);
                        }//datatable has rows
                        else
                        {
                            return 0;
                        }//datatable has no rows
                    }//datatable is not null

                    return 0;

                }//user is not equal to null.
                else
                {
                    return 0;
                }//user id null.
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return 0;

            }//catch block ends.

        }


        [HttpPost]

        ///<summary>
        /// TodayNotification_summ actionmethod fetches all data regarding routes all notifications.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult TodayNotification_summ(dashboard dashboard)
        {
            string json = "";
            try
            {
                string user_id = dashboard.user_id;

                if (user_id != null)
                {
                    //string query = $@"SELECT 
                    //                    bsmb.student_name,
                    //                    bsmb.admission_no,
                    //                    bsmb.mobile_no1,
                    //                    bs_route_master.route_name,
                    //                    bnp.message,
                    //                    bnp.date_time
                    //                FROM 
                    //                    bs_student_master_backup bsmb
                    //                INNER JOIN 
                    //                    bs_route_students brs ON brs.student_id = bsmb.id
                    //                INNER JOIN 
                    //                    bs_route_master ON bs_route_master.id = brs.route_id
                    //                INNER JOIN 
                    //                    bs_user_master ON bs_user_master.bs_user_name = bsmb.mobile_no1
                    //                INNER JOIN 
                    //                    bs_notification_parent bnp ON bnp.parent_id = bs_user_master.id
                    //                WHERE 
                    //                    bsmb.sys_user_id = {user_id}
                    //                    AND bnp.date_time > CONVERT(DATETIME, '{DateTime.Now.AddDays(-1).ToString("MMM d yyyy h:mmtt")}', 100);";

                    //string query = $@"SELECT 
                    //                bsmb.student_name,
                    //                bsmb.admission_no,
                    //                bsmb.mobile_no1,
                    //                bnp.message,
                    //                bnp.date_time
                    //                FROM 
                    //                bs_student_master_backup bsmb
                    //                INNER JOIN 
                    //                bs_user_master ON bs_user_master.bs_user_name = bsmb.mobile_no1
                    //                INNER JOIN 
                    //                bs_notification_parent bnp ON bnp.parent_id = bs_user_master.id
                    //                WHERE 
                    //                bsmb.sys_user_id = {user_id}
                    //                AND bnp.date_time >= CONVERT(DATETIME, CAST(GETDATE() AS DATE), 101) 
                    //                AND bnp.date_time <= GETDATE(); ";
                    string query = $@"SELECT 
                                        bsmb.student_name, 
                                        bsmb.admission_no, 
                                        bsmb.mobile_no1, 
                                        bnp.message, 
                                        bnp.date_time 
                                    FROM 
                                        bs_student_master_backup bsmb 
                                    INNER JOIN 
                                        bs_user_master bum ON bum.bs_user_name = bsmb.mobile_no1 
                                    INNER JOIN 
                                        bs_notification_parent bnp ON bnp.parent_id = bum.id AND bnp.student_id = bsmb.id
                                    WHERE 
                                        bsmb.sys_user_id = {user_id} 
                                        AND bnp.date_time >= CONVERT(DATETIME, CAST(GETDATE() AS DATE), 101) 
                                        AND bnp.date_time <= GETDATE();";
                    DataTable datatable = _sql_qury_execution.DML_Select(query);



                    if (datatable != null)
                    {
                        if (datatable.Rows.Count > 0)
                        {
                            json = JsonConvert.SerializeObject(datatable, Formatting.Indented);
                            return Content(json, "application/json");
                        }//datatable has rows
                        else
                        {
                            return Content(json, "application/json");
                        }//datatable has no rows
                    }//datatable is not null

                    return Content(json, "application/json");

                }//user is not equal to null.
                else
                {
                    return Content(json, "application/json");
                }//user id null.
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return Content(json, "application/json");

            }//catch block ends.

        }


        [HttpPost]

        ///<summary>
        /// GetUpcomingHolidays actionmethod fetches all upcoming holidays.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult GetUpcomingHolidays(dashboard dashboard)
        {
            try
            {
                if (!String.IsNullOrEmpty(dashboard.user_id))
                {
                    string getholidays = $@"SELECT * 
                                            FROM bs_holidays 
                                            WHERE type = 0 
                                            AND from_date > CAST(GETDATE() AS DATE) and sys_user_id = {dashboard.user_id};";
                    DataTable dataTable = _sql_qury_execution.DML_Select(getholidays);
                    if (dataTable != null)
                    {
                        if (dataTable.Rows.Count >= 0)
                        {
                            string json = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
                            return Content(json, "application/json");
                        }//datatable has rows
                        else
                        {
                            return Content("-1");
                        }//datatable has no rows
                    }

                    return Content("0");

                }
                return Content("0");
            }
            catch (Exception e)
            {
                return Content("0");
            }
        }


        [HttpPost]

        ///<summary>
        /// GetUpcomingHolidays actionmethod fetches all upcoming events.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult GetUpcomingEvents(dashboard dashboard)
        {
            try
            {
                if (!String.IsNullOrEmpty(dashboard.user_id))
                {
                    string getholidays = $@"SELECT * 
                                            FROM bs_holidays 
                                            WHERE type = 1
                                            AND from_date > CAST(GETDATE() AS DATE) and sys_user_id = {dashboard.user_id};";
                    DataTable dataTable = _sql_qury_execution.DML_Select(getholidays);
                    if (dataTable != null)
                    {
                        if (dataTable.Rows.Count >= 0)
                        {
                            string json = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
                            return Content(json, "application/json");
                        }//datatable has rows
                        else
                        {
                            return Content("-1");
                        }//datatable has no rows
                    }

                    return Content("0");

                }
                return Content("0");
            }
            catch (Exception e)
            {
                return Content("0");
            }
        }

        //        [HttpPost]
        //        public string StopWiseVoilation([FromBody] bus_user_id bus)
        //        {
        //            string tablename = $"telemetry_{DateTime.Now.ToString("MMMyy")}";
        //            string user_id = bus.user_id;
        //            List<object> routeViolationJson = new List<object>();
        //            HashSet<string> violatedRoutes = new HashSet<string>();

        //            int routeViolationCount = 0;
        //            List<object> violationJson = new List<object>();
        //            int stopvoilationcount = 0;


        //            string stop_master = $@"select bsm.id as id, brm.id as route_id, brm.route_name, brm.start_time_up, brm.end_time_up, s.veh_reg from bs_stop_master bsm inner join bs_route_master brm on bsm.route_id = brm.id left join services s on brm.sys_service_id = s.id where bsm.sys_user_id={user_id} order by bsm.id";


        //            DataTable stop_data = _sql_qury_execution.DML_Select(stop_master);

        //            if (stop_data != null && stop_data.Rows.Count > 0)
        //            {
        //                foreach (DataRow row in stop_data.Rows)
        //                {
        //                    List<int> Haversiondistances = new List<int>();

        //                    double stopid = Convert.ToDouble(row["id"]);
        //                    if (row["start_time_up"] == DBNull.Value ||
        //        row["end_time_up"] == DBNull.Value)
        //                    {
        //                        continue;
        //                    }
        //                    DateTime todayStartUp = DateTime.Today.Add((TimeSpan)row["start_time_up"]);
        //                    DateTime todayStartDown = DateTime.Today.Add((TimeSpan)row["end_time_up"]);
        //                    string stopdata = $@"
        //SELECT
        //    brm.id AS route_id,
        //    brm.route_name,
        //    bsm.Id AS stop_id,
        //    bsm.user_stop_name,
        //    bsm.latitude AS stop_latitude,
        //    bsm.longitude AS stop_longitude,
        //    bsm.stop_order,
        //    tel.gps_latitude AS bus_latitude,
        //    tel.gps_longitude AS bus_longitude,
        //    tel.gps_time
        //FROM bs_route_master brm
        //INNER JOIN bs_stop_master bsm
        //    ON bsm.route_id = brm.id
        //inner JOIN {tablename} tel
        //    ON tel.sys_service_id =brm.sys_service_id
        //WHERE brm.sys_user_id = {user_id}
        //  AND bsm.id = {stopid}
        //AND tel.gps_time >= '{todayStartUp:yyyy-MM-dd HH:mm:ss}'
        //    AND tel.gps_time <= '{todayStartDown:yyyy-MM-dd HH:mm:ss}'
        //ORDER BY tel.gps_time, bsm.stop_order;";

        //                    object stopDetails = null;
        //                    DataTable dataTable1 = _sql_qury_execution.DML_Select(stopdata);

        //                    if (dataTable1 != null)
        //                    {
        //                        if (dataTable1.Rows.Count > 0)
        //                        {
        //                            foreach (DataRow row1 in dataTable1.Rows)
        //                            {
        //                                stopDetails = new
        //                                {
        //                                    stop_id = row1["stop_id"],
        //                                    stop_order = row1["stop_order"],
        //                                    stop_name = row1["user_stop_name"],
        //                                    stop_latitude = row1["stop_latitude"],
        //                                    stop_longitude = row1["stop_longitude"]
        //                                };

        //                                int temp = 0;
        //                                if (row1["bus_latitude"] == DBNull.Value ||
        //        row1["bus_longitude"] == DBNull.Value)
        //                                {
        //                                    continue;
        //                                }
        //                                double lat1 = Convert.ToDouble(row1["stop_latitude"]);
        //                                double lon1 = Convert.ToDouble(row1["stop_longitude"]);

        //                                double lat2 = Convert.ToDouble(row1["bus_latitude"]);
        //                                double lon2 = Convert.ToDouble(row1["bus_longitude"]);

        //                                const double R = 6371.0;

        //                                double lat1Rad = lat1 * Math.PI / 180.0;
        //                                double lon1Rad = lon1 * Math.PI / 180.0;
        //                                double lat2Rad = lat2 * Math.PI / 180.0;
        //                                double lon2Rad = lon2 * Math.PI / 180.0;

        //                                double dLat = lat2Rad - lat1Rad;
        //                                double dLon = lon2Rad - lon1Rad;

        //                                double a =
        //                                    Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
        //                                    Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
        //                                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        //                                double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        //                                double distance = R * c;

        //                                if (distance > 0.2)
        //                                {
        //                                    temp = 1;
        //                                }

        //                                Haversiondistances.Add(temp);
        //                            }

        //                            bool exists = Haversiondistances.Contains(0);

        //                            if (!exists)
        //                            {
        //                                stopvoilationcount++;

        //                                violationJson.Add(stopDetails);
        //                                string routeId = row["route_id"].ToString();

        //                                if (!violatedRoutes.Contains(routeId))
        //                                {
        //                                    violatedRoutes.Add(routeId);

        //                                    routeViolationCount++;

        //                                    routeViolationJson.Add(new
        //                                    {
        //                                        route_id = row["route_id"],
        //                                        route_name = row["route_name"],
        //                                        vech_reg = row["veh_reg"]
        //                                    });
        //                                }
        //                            }

        //                        }
        //                    }
        //                }
        //            }
        //            return JsonConvert.SerializeObject(new
        //            {
        //                stopViolationCount = stopvoilationcount,
        //                violations = violationJson,
        //                routeViolationCount = routeViolationCount,
        //                routeViolations = routeViolationJson
        //            });
        //        }
        [HttpPost]
        public IActionResult RFIDPUNCHED([FromBody] boarding dashboard)
        {
            try
            {
                if (dashboard == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Request data is required."
                    });
                }

                if (string.IsNullOrWhiteSpace(dashboard.user_id))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "User ID is required."
                    });
                }

                if (!int.TryParse(dashboard.user_id, out int userId))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid User ID."
                    });
                }

                if (!DateTime.TryParse(dashboard.from, out DateTime fromDateTime))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid from date."
                    });
                }

                if (!DateTime.TryParse(dashboard.to, out DateTime toDateTime))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid to date."
                    });
                }

                /*
                 * Database-compatible service table:
                 *
                 * newtrack:
                 *     services
                 *
                 * other databases:
                 *     atltracking.dbo.tbl_services
                 */
                string servicesTable;

                if (string.Equals(
                    dashboard.database,
                    "newtrack",
                    StringComparison.OrdinalIgnoreCase))
                {
                    servicesTable = "services";
                }
                else
                {
                    servicesTable = "atltracking.dbo.tbl_services";
                }

                string from = fromDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                string to = toDateTime.ToString("yyyy-MM-dd HH:mm:ss");

                string query = $@"
DECLARE @UserId INT = {userId};
DECLARE @FromDateTime DATETIME = '{from}';
DECLARE @ToDateTime DATETIME = '{to}';

;WITH RouteCandidates AS
(
    SELECT
        rf.rfid,
        sm.id AS student_id,
        rf.sys_service_id,
        rf.sys_proc_time,
        rf.punch_gps_time,

        ISNULL(sm.student_name, 'N/A') AS student_name,
        ISNULL(cm.class_name, 'N/A') AS class_name,
        s.veh_reg,

        rm.id AS route_id,
        rm.route_name,
        rm.start_time_up,
        rm.end_time_up,

        rs.stop_id,
        st.user_stop_name,

        CASE
            WHEN UPPER(ISNULL(rm.route_name, '')) LIKE '%DROP%'
                THEN 'Drop'

            WHEN UPPER(ISNULL(rm.route_name, '')) LIKE '%PICK%'
                THEN 'Pick'

            ELSE 'Unknown'
        END AS movement_type,

        CASE
            WHEN TRY_CONVERT(TIME, rf.sys_proc_time)
                 BETWEEN TRY_CONVERT(TIME, rm.start_time_up)
                     AND TRY_CONVERT(TIME, rm.end_time_up)
                THEN 0
            ELSE 1
        END AS outside_time_range,

        CASE
            WHEN TRY_CONVERT(TIME, rm.start_time_up) IS NULL
              OR TRY_CONVERT(TIME, rm.end_time_up) IS NULL
                THEN 999999

            WHEN TRY_CONVERT(TIME, rf.sys_proc_time)
                 BETWEEN TRY_CONVERT(TIME, rm.start_time_up)
                     AND TRY_CONVERT(TIME, rm.end_time_up)
                THEN 0

            WHEN TRY_CONVERT(TIME, rf.sys_proc_time)
                 < TRY_CONVERT(TIME, rm.start_time_up)
                THEN ABS
                (
                    DATEDIFF
                    (
                        MINUTE,
                        TRY_CONVERT(TIME, rf.sys_proc_time),
                        TRY_CONVERT(TIME, rm.start_time_up)
                    )
                )

            ELSE ABS
            (
                DATEDIFF
                (
                    MINUTE,
                    TRY_CONVERT(TIME, rm.end_time_up),
                    TRY_CONVERT(TIME, rf.sys_proc_time)
                )
            )
        END AS route_time_difference

    FROM rf_punch_history rf

    INNER JOIN {servicesTable} s
        ON s.id = rf.sys_service_id
       AND s.sys_user_id = @UserId

    LEFT JOIN bs_student_master_backup sm
        ON sm.rf_id = rf.rfid
       AND sm.sys_user_id = @UserId

    LEFT JOIN bs_class_master cm
        ON cm.id = sm.class

    LEFT JOIN bs_route_master rm
        ON rm.sys_service_id = rf.sys_service_id
       AND rm.sys_user_id = @UserId

    LEFT JOIN bs_route_students rs
        ON rs.route_id = rm.id
       AND rs.student_id = sm.id

    LEFT JOIN bs_stop_master st
        ON st.id = rs.stop_id

    WHERE rf.sys_proc_time >= @FromDateTime
      AND rf.sys_proc_time < @ToDateTime
),

BestRoute AS
(
    SELECT
        *,

        ROW_NUMBER() OVER
        (
            PARTITION BY
                rfid,
                sys_service_id,
                sys_proc_time

            ORDER BY
                outside_time_range ASC,
                route_time_difference ASC,
                CASE
                    WHEN rs_student_assigned IS NOT NULL THEN 0
                    ELSE 1
                END,
                route_id ASC
        ) AS route_rn

    FROM
    (
        SELECT
            *,
            CASE
                WHEN stop_id IS NOT NULL THEN 1
                ELSE NULL
            END AS rs_student_assigned
        FROM RouteCandidates
    ) x
),

OneRecordPerRFIDMovement AS
(
    SELECT
        *,

        ROW_NUMBER() OVER
        (
            PARTITION BY
                rfid,
                movement_type

            ORDER BY
                outside_time_range ASC,
                route_time_difference ASC,
                sys_proc_time ASC,
                route_id ASC
        ) AS movement_rn

    FROM BestRoute

    WHERE route_rn = 1
      AND movement_type IN ('Pick', 'Drop')
)

SELECT
    rfid,
    student_id,
    student_name,
    class_name,
    route_id,

    ISNULL(
        route_name,
        'Route Not Available'
    ) AS route_name,

    ISNULL(
        user_stop_name,
        'Stop Not Available'
    ) AS user_stop_name,

    ISNULL(veh_reg, '') AS veh_reg,

    sys_proc_time,
    punch_gps_time,
    movement_type,

    CASE
        WHEN student_id IS NULL
            THEN 'RFID Not Mapped'
        ELSE 'RFID Punched'
    END AS status

FROM OneRecordPerRFIDMovement

WHERE movement_rn = 1

ORDER BY
    student_name DESC,
    movement_type ASC;
";

                DataTable dataTable =
                    _sql_qury_execution.DML_Select(query);

                if (dataTable == null)
                {
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "Unable to retrieve RFID punch data."
                    });
                }

                /*
                 * Rows.Count >= 0 is unnecessary because it is always true.
                 * An empty DataTable will serialize as [].
                 */
                string json = JsonConvert.SerializeObject(
                    dataTable,
                    Formatting.Indented
                );

                return Content(
                    json,
                    "application/json"
                );
            }
            catch (Exception ex)
            {
               

                return StatusCode(500, new
                {
                    success = false,
                    message = "Something went wrong while loading RFID punch data.",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult DriverPerformance(Login login)
        {
            try
            {
                var username = login.username?.Trim();
                var password = login.password;

                if (string.IsNullOrWhiteSpace(username) ||
                    string.IsNullOrWhiteSpace(password))
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = "Username and password are required",
                        data = new List<object>()
                    });
                }

                var driverList = new List<DriverPerformanceModel>();

                string query = $@"
           SELECT
    dm.name AS DriverName,
    s.veh_reg AS BusName,

    CASE
        WHEN dml.driver_id IS NOT NULL THEN 'Active'
        ELSE 'Inactive'
    END AS DriverStatus,

    CASE
        WHEN ISNULL(lt.gps_speed, 0) > 0 THEN 'Running'
        WHEN ISNULL(lt.gps_speed, 0) = 0 AND lt.i2 = 1 THEN 'Idle'
        WHEN ISNULL(lt.gps_speed, 0) = 0 AND lt.i2 = 0 THEN 'Stopped'
        ELSE 'Unknown'
    END AS BusStatus

FROM [atltracking].[dbo].[tbl_driver_master] dm

INNER JOIN [atltracking].[dbo].[tbl_users] u
    ON u.id = dm.created_by

OUTER APPLY
(
    SELECT TOP (1) *
    FROM [atltracking].[dbo].[tbl_driver_logs] dl
    WHERE dl.driver_id = dm.id
    ORDER BY dl.assigned_date DESC
) dml

LEFT JOIN [atltracking].[dbo].[tbl_services] s
    ON s.id = dml.service_id

LEFT JOIN [atltracking].[dbo].[tbl_latest_telemetry] lt
    ON lt.sys_service_id = s.id

WHERE u.sys_username = '{username}'
  AND u.sys_password = '{password}';";

                DataTable dataTable =
                    _sql_qury_execution.DML_Select(query);

                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        driverList.Add(new DriverPerformanceModel
                        {
                            DriverName =
                                row["DriverName"] == DBNull.Value
                                    ? string.Empty
                                    : row["DriverName"].ToString(),

                            BusName =
                                row["BusName"] == DBNull.Value
                                    ? string.Empty
                                    : row["BusName"].ToString(),

                            DriverStatus =
                                row["DriverStatus"] == DBNull.Value
                                    ? "Inactive"
                                    : row["DriverStatus"].ToString(),

                            BusStatus =
                                row["BusStatus"] == DBNull.Value
                                    ? "Unknown"
                                    : row["BusStatus"].ToString()
                        });
                    }
                }

                return Ok(new
                {
                    status = true,
                    message = driverList.Count > 0
                        ? "Driver performance fetched successfully"
                        : "No driver records found",
                    data = driverList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = "Something went wrong",
                    error = ex.Message,
                    data = new List<object>()
                });
            }
        }

        [HttpPost]
        public IActionResult RoutePerformance([FromBody] boarding dashboard)
        {
            try
            {
                if (dashboard == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Request data is required."
                    });
                }

                if (string.IsNullOrWhiteSpace(dashboard.user_id))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "User ID is required."
                    });
                }

                if (!int.TryParse(dashboard.user_id, out int userId))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid User ID."
                    });
                }

                /*
                 * Only these table names are changed according to the database.
                 * Do not directly use dashboard.database as a table name.
                 */
                string servicesTable;
                string latestTelemetryTable;

                if (string.Equals(
                    dashboard.database,
                    "newtrack",
                    StringComparison.OrdinalIgnoreCase))
                {
                    servicesTable = "services";
                    latestTelemetryTable = "latest_telemetry";
                }
                else
                {
                  
                    servicesTable = "atltracking.dbo.tbl_services";
                    latestTelemetryTable = "atltracking.dbo.tbl_latest_telemetry";
                }

                string query = $@"
DECLARE @CurrentTime TIME = CAST(GETDATE() AS TIME);

;WITH RoutePerformanceData AS
(
    SELECT
        brm.route_name AS Route,

        ISNULL(
            s.veh_reg,
            'Not Assigned'
        ) AS Vehicle,

        ISNULL(
            StudentData.Students,
            0
        ) AS Students,

        CONVERT(
            VARCHAR(5),
            TRY_CONVERT(
                TIME,
                brm.start_time_up
            ),
            108
        ) AS StartTime,

        CONVERT(
            VARCHAR(5),
            TRY_CONVERT(
                TIME,
                brm.end_time_up
            ),
            108
        ) AS EndTime,

        ISNULL(
            NearestStop.user_stop_name,
            'N/A'
        ) AS StopName,

        CASE
            WHEN @CurrentTime >
                 TRY_CONVERT(
                    TIME,
                    brm.end_time_up
                 )
            THEN '0'

            WHEN NearestStop.eta IS NULL
              OR TRY_CONVERT(
                    TIME,
                    NearestStop.eta
                 ) IS NULL
              OR TRY_CONVERT(
                    TIME,
                    NearestStop.eta
                 ) = '00:00:00'
            THEN 'N/A'

            ELSE CONVERT(
                VARCHAR(5),
                TRY_CONVERT(
                    TIME,
                    NearestStop.eta
                ),
                108
            )
        END AS ETA,

        CASE
            WHEN @CurrentTime >
                 TRY_CONVERT(
                    TIME,
                    brm.end_time_up
                 )
            THEN 0

            WHEN NearestStop.eta IS NULL
              OR TRY_CONVERT(
                    TIME,
                    NearestStop.eta
                 ) IS NULL
              OR TRY_CONVERT(
                    TIME,
                    NearestStop.eta
                 ) = '00:00:00'
            THEN NULL

            ELSE DATEDIFF(
                MINUTE,
                TRY_CONVERT(
                    TIME,
                    NearestStop.eta
                ),
                @CurrentTime
            )
        END AS EtaDifferenceMinutes,

        CASE
            WHEN TRY_CONVERT(
                    TIME,
                    brm.start_time_up
                 ) IS NULL
              OR TRY_CONVERT(
                    TIME,
                    brm.end_time_up
                 ) IS NULL
            THEN 'Schedule Not Available'

            WHEN @CurrentTime <
                 TRY_CONVERT(
                    TIME,
                    brm.start_time_up
                 )
            THEN 'Not Started'

            WHEN @CurrentTime >
                 TRY_CONVERT(
                    TIME,
                    brm.end_time_up
                 )
            THEN 'Route Time Completed'

           

            WHEN @CurrentTime >
                 TRY_CONVERT(
                    TIME,
                    NearestStop.eta
                 )
            THEN 'Delayed'

            ELSE 'On Time'
        END AS RouteStatus,

        CASE
            WHEN LatestTelemetry.gps_time IS NULL
            THEN 'Offline'

            WHEN DATEDIFF(
                MINUTE,
                LatestTelemetry.gps_time,
                GETDATE()
            ) > 15
            THEN 'Offline'

            WHEN ISNULL(
                    LatestTelemetry.gps_speed,
                    0
                 ) > 0
            THEN 'Running'

            WHEN ISNULL(
                    LatestTelemetry.gps_speed,
                    0
                 ) = 0
             AND ISNULL(
                    LatestTelemetry.I2,
                    0
                 ) = 1
            THEN 'Idle'

            WHEN ISNULL(
                    LatestTelemetry.gps_speed,
                    0
                 ) = 0
             AND ISNULL(
                    LatestTelemetry.I2,
                    0
                 ) = 0
            THEN 'Stopped'

            ELSE 'Unknown'
        END AS VehicleLiveStatus,

        ISNULL(
            LatestTelemetry.gps_speed,
            0
        ) AS CurrentSpeed,

        ISNULL(
            LatestTelemetry.I2,
            0
        ) AS I2,

        LatestTelemetry.gps_time AS LastTelemetryTime

    FROM bs_route_master brm

    LEFT JOIN {servicesTable} s
        ON s.id = brm.sys_service_id

    OUTER APPLY
    (
        SELECT
            COUNT(
                DISTINCT brs.student_id
            ) AS Students
        FROM bs_route_students brs
        WHERE brs.route_id = brm.id
    ) StudentData

    OUTER APPLY
    (
        SELECT TOP 1
            sm.id AS StopId,
            sm.user_stop_name,
            sm.stop_order,
            sm.eta

        FROM bs_stop_master sm

        WHERE sm.route_id = brm.id
          AND sm.eta IS NOT NULL
          AND TRY_CONVERT(
                TIME,
                sm.eta
              ) IS NOT NULL
          AND TRY_CONVERT(
                TIME,
                sm.eta
              ) <> '00:00:00'

        ORDER BY
            ABS(
                DATEDIFF(
                    SECOND,
                    TRY_CONVERT(
                        TIME,
                        sm.eta
                    ),
                    @CurrentTime
                )
            ),
            sm.stop_order
    ) NearestStop

    OUTER APPLY
    (
        SELECT TOP 1
            lt.sys_proc_time AS gps_time,
            lt.gps_speed,
            lt.I2

        FROM {latestTelemetryTable} lt

        WHERE lt.sys_service_id =
              brm.sys_service_id

        ORDER BY
            lt.sys_proc_time DESC,
            lt.id DESC
    ) LatestTelemetry

    WHERE brm.sys_user_id = {userId}
)

SELECT
    Route,
    Vehicle,
    Students,
    StartTime,
    EndTime,
    StopName,
    ETA,
    EtaDifferenceMinutes,
    RouteStatus,
    VehicleLiveStatus,
    CurrentSpeed,
    I2,
    LastTelemetryTime

FROM RoutePerformanceData

ORDER BY
    CASE RouteStatus
        WHEN 'Delayed' THEN 1
        WHEN 'On Time' THEN 2
        WHEN 'Not Started' THEN 3
        WHEN 'ETA Not Available' THEN 4
        WHEN 'Schedule Not Available' THEN 5
        WHEN 'Completed' THEN 6
        ELSE 7
    END,
    Route ASC;";

                DataTable dataTable =
                    _sql_qury_execution.DML_Select(query);

                if (dataTable == null)
                {
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "Unable to retrieve route performance."
                    });
                }

                /*
                 * This also returns [] when the DataTable contains no rows.
                 */
                string json = JsonConvert.SerializeObject(
                    dataTable,
                    Formatting.Indented
                );

                return Content(
                    json,
                    "application/json"
                );
            }
            catch (Exception ex)
            {
                

                return StatusCode(500, new
                {
                    success = false,
                    message = "Something went wrong while loading route performance.",
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult VehicleInServices([FromBody] boarding dashboard)
        {
            try
            {
                if (dashboard == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Request data is required."
                    });
                }

                if (string.IsNullOrWhiteSpace(dashboard.user_id))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "User ID is required."
                    });
                }

                if (!int.TryParse(dashboard.user_id, out int userId))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid User ID."
                    });
                }

                /*
                 * Only these table names are changed according to the database.
                 * Do not directly use dashboard.database as a table name.
                 */
                string servicesTable;
                string latestTelemetryTable;

                if (string.Equals(
                    dashboard.database,
                    "newtrack",
                    StringComparison.OrdinalIgnoreCase))
                {
                    servicesTable = "services";
                    string telemetry_MMMyy = $"telemetry_{DateTime.Now:MMMyy}".ToLower();
                    latestTelemetryTable = telemetry_MMMyy;
                }
                else
                {

                    servicesTable = "atltracking.dbo.tbl_services";
                    string telemetry_MMMyy = $"atltracking.dbo.tbl_telemetry_{DateTime.Now:MMMyy}".ToLower();
                    latestTelemetryTable = telemetry_MMMyy;
                }

                string query = $@"
DECLARE @SysUserId INT = {dashboard.user_id};

SELECT
    s.id AS sys_service_id,
    rm.route_name,
    s.veh_reg,
    MAX(t.gps_speed) AS MaximumSpeedToday,
    MAX(t.sys_proc_time) AS LastRunningTime
FROM {latestTelemetryTable} t

INNER JOIN {servicesTable} s
    ON s.id = t.sys_service_id
inner join bs_route_master rm on rm.sys_service_id=s.id
WHERE s.sys_user_id = @SysUserId
  AND t.sys_proc_time >= CONVERT(DATE, GETDATE())
  AND t.sys_proc_time < DATEADD(DAY, 1, CONVERT(DATE, GETDATE()))
  AND ISNULL(t.gps_speed, 0) > 0

GROUP BY
    s.id,
    s.veh_reg,
rm.route_name

ORDER BY
    LastRunningTime DESC;";

                DataTable dataTable =
                    _sql_qury_execution.DML_Select(query);

                if (dataTable == null)
                {
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "Unable to retrieve route performance."
                    });
                }

                /*
                 * This also returns [] when the DataTable contains no rows.
                 */
                string json = JsonConvert.SerializeObject(
                    dataTable,
                    Formatting.Indented
                );

                return Content(
                    json,
                    "application/json"
                );
            }
            catch (Exception ex)
            {


                return StatusCode(500, new
                {
                    success = false,
                    message = "Something went wrong while loading route performance.",
                    error = ex.Message
                });
            }
        }
    }
      
    
}
      

  
