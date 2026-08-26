using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolBuddy_APIs.Database_methods;
using SchoolBuddy_APIs.Models.Analysis;
using SchoolBuddy_APIs.Models.Report;
using System.Data;
using System.Text;

namespace SchoolBuddy_APIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        private readonly Idatabase_access _sql_qury_execution;
        public AnalysisController(Idatabase_access sql_qury_execution)
        {
            _sql_qury_execution = sql_qury_execution;
        }
        [HttpPost]
        ///<summary>
        /// checkIgnition actionmethod checks for device ignition status.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult checkIgnition(CheckIgnition checkIgnition)
        {
            try
            {
                string service_id = checkIgnition.service_id;

                if (string.IsNullOrEmpty(service_id))
                    return Content("Login Again..", "application/json");

                string year = Convert.ToDateTime(checkIgnition.start_date).ToString("MMMyy").ToLower();

                string tableName = checkIgnition.database == "newtrack"
                    ? $"telemetry_{year}"
                    : $"tbl_telemetry_{year}";

                string query = $@"
            SELECT 
                i2,
                CONVERT(VARCHAR(8), DATEADD(MINUTE, 330, gps_time), 108) AS time
            FROM {tableName}
            WHERE sys_service_id = {service_id}
              AND gps_time BETWEEN DATEADD(MINUTE, -330, '{checkIgnition.start_date}')
                               AND DATEADD(MINUTE, -330, '{checkIgnition.end_date}')
            ORDER BY gps_time";

                DataTable datatable = _sql_qury_execution.DML_Select(query);

                if (datatable != null && datatable.Rows.Count > 0)
                {
                    string json = JsonConvert.SerializeObject(datatable, Formatting.Indented);
                    return Content(json, "application/json");
                }

                return Content("Data Not Found", "application/json");
            }
            catch (Exception ex)
            {
                return Content(ex.Message, "application/json");
            }
        }

        [HttpPost]
        ///<summary>
        /// checkInActive actionmethod checks status of device .
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///if currentdate and device data time is greater than 5, status inactive else active.
        ///</summary>
        public IActionResult checkInActive(CheckInActive checkInActive)
        {
            try
            {
                string service_id = checkInActive.service_id;

                if (string.IsNullOrEmpty(service_id))
                    return Content("Login Again..", "application/json");

                string year = Convert.ToDateTime(checkInActive.start_date).ToString("MMMyy").ToLower();

                //string tableName = checkInActive.database == "newtrack"
                //    ? $"telemetry_{year}"
                //    : $"tbl_telemetry_{year}";

                string query = $@"
            SELECT 
                CONVERT(VARCHAR(8), DATEADD(MINUTE, 330, gps_time), 108) AS time,
                CASE 
                    WHEN DATEDIFF(MINUTE, DATEADD(MINUTE, 330, gps_time), sys_proc_time) > 5 
                    THEN 'inactive'
                    ELSE 'active'
                END AS status
            FROM tbl_telemetry_{year}
            WHERE sys_service_id = {service_id}
              AND gps_time BETWEEN DATEADD(MINUTE, -330, '{checkInActive.start_date}')
                               AND DATEADD(MINUTE, -330, '{checkInActive.end_date}')
            ORDER BY gps_time";

                DataTable datatable = _sql_qury_execution.DML_Select(query);

                if (datatable != null && datatable.Rows.Count > 0)
                {
                    string json = JsonConvert.SerializeObject(datatable, Formatting.Indented);
                    return Content(json, "application/json");
                }

                return Content("Data Not Found", "application/json");
            }
            catch (Exception ex)
            {
                return Content(ex.Message, "application/json");
            }
        }



        [HttpPost]
        public IActionResult Route(CheckRoute route)
        {

            return Content("0", "application/json");

        }



        [HttpPost]
        ///<summary>
        /// checkRoute actionmethod fetches route details like id, route name, is_pis_enabled.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult checkRoute(CheckRoute route)
        {
            try
            {
                if (route == null || string.IsNullOrWhiteSpace(route.schoolid))
                {
                    return Content("0", "application/json");
                }

                string select_query = $@"
            SELECT 
                id,
                route_name,
                sys_service_id,
                start_time_up,
                end_time_up,
                is_pis_enabled
            FROM bs_route_master
            WHERE sys_user_id = {route.schoolid}
            ORDER BY route_name";

                DataTable dataTable = _sql_qury_execution.DML_Select(select_query);

                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    string json = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
                    return Content(json, "application/json");
                }

                return Content("0", "application/json");
            }
            catch (Exception ex)
            {
                return Content("0", "application/json");
            }
        }



        [HttpPost]
        ///<summary>
        /// checkUrl actionmethod fetches route details like id, route name, is_pis_enabled.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult checkUrl(CheckInActive checkInActive)
        {

            try
            {
                string service_id = checkInActive.service_id;
                string json = "";
                if (service_id != null)
                {
                    string year = Convert.ToDateTime(checkInActive.start_date).ToString("MMMyy").ToLower();
                    StringBuilder query = new StringBuilder();
                    //if (checkInActive.database == "newtrack")
                    //{
                    //    query.Append($"select convert(varchar(8),dateadd(minute,330,gps_time),108) as time, case when datediff(minute,DATEADD(minute,330,gps_time),sys_proc_time) > 5 then 'inactive' else 'active' end as status from telemetry_{year} where sys_service_id= {service_id} and gps_time between dateadd(minute,-330,'" + checkInActive.start_date + "') and dateadd(minute,-330,'" + checkInActive.end_date + "') order by gps_time");

                    //}
                    //else
                    //{
                        query.Append($"select  convert(varchar(8),dateadd(minute,330,gps_time),108) as time, case when datediff(minute,DATEADD(minute,330,gps_time),sys_proc_time) > 5 then 'inactive' else 'active' end as status from telemetry_{year} where sys_service_id= {service_id} and gps_time between dateadd(minute,-330,'" + checkInActive.start_date + "') and dateadd(minute,-330,'" + checkInActive.end_date + "') order by gps_time");

                   // }
                    DataTable datatable = _sql_qury_execution.DML_Select(query.ToString());
                    if (datatable != null)
                    {
                        if (datatable.Rows.Count > 0)
                        {
                            json = JsonConvert.SerializeObject(datatable, Formatting.Indented);
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



        [HttpPost]
        ///<summary>
        /// getSta actionmethod fetches standard time of arrival.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult getSta(CheckInActive checkInActive)
        {

            try
            {
                string service_id = checkInActive.service_id;
                string json = "";
                if (service_id != null)
                {
                    string year = Convert.ToDateTime(checkInActive.start_date).ToString("MMMyy").ToLower();
                    StringBuilder query = new StringBuilder();
                    //if (checkInActive.database == "newtrack")
                    //{
                    //    query.Append($"select convert(varchar(8),dateadd(minute,330,gps_time),108) as time, case when datediff(minute,DATEADD(minute,330,gps_time),sys_proc_time) > 5 then 'inactive' else 'active' end as status from telemetry_{year} where sys_service_id= {service_id} and gps_time between dateadd(minute,-330,'" + checkInActive.start_date + "') and dateadd(minute,-330,'" + checkInActive.end_date + "') order by gps_time");

                    //}
                    //else
                    //{
                        query.Append($"select  convert(varchar(8),dateadd(minute,330,gps_time),108) as time, case when datediff(minute,DATEADD(minute,330,gps_time),sys_proc_time) > 5 then 'inactive' else 'active' end as status from telemetry_{year} where sys_service_id= {service_id} and gps_time between dateadd(minute,-330,'" + checkInActive.start_date + "') and dateadd(minute,-330,'" + checkInActive.end_date + "') order by gps_time");

                    //}
                    DataTable datatable = _sql_qury_execution.DML_Select(query.ToString());
                    if (datatable != null)
                    {
                        if (datatable.Rows.Count > 0)
                        {
                            json = JsonConvert.SerializeObject(datatable, Formatting.Indented);
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


        [HttpPost]
        public IActionResult checkVias(Checkvias vias)
        {

            try
            {
                #region
                string select_query = $@"select * from bs_route_vias where route_id='{vias.route_id}' order by via_order";
                #endregion

                DataTable dataTable = _sql_qury_execution.DML_Select(select_query);
                if (dataTable != null)
                {

                    string json = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
                    return Content(json, "application/json");

                }
                return Content("0", "application/json");



            }//try block ends.
            catch (Exception ex)
            {

                return Content("0", "application/json");

            }//catch block ends.

        }


        [HttpPost]

        ///<summary>
        /// checkEta actionmethod fetches estimate time of arrival.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult checkEta(Checketa eta)
        {
            try
            {
                if (eta == null || eta.route_id <= 0)
                {
                    return Content("Invalid Route", "application/json");
                }

                #region OLD CODE - Date wise ETA history from bs_test_route_new
                /*
                string select_query = $@"select t.eta,t.counter,t.from_vias,CONVERT(VARCHAR(20),log_date, 108) as [time],s.user_stop_name
                                        from bs_test_route_new t 
                                        inner join bs_stop_master s on s.id = t.stop_id 
                                        where stop_id in ({eta.stop_id}) 
                                        and t.log_date between '{eta.date}' and '{Convert.ToDateTime(eta.date).AddDays(1).ToString("yyyy-MM-dd")}' 
                                        order by t.id";
                */
                #endregion

                #region NEW CODE - Current ETA of all stops from bs_stop_master

                string select_query = $@"
                SELECT 
                    brm.route_name,
                    bsm.user_stop_name,
                
                    ISNULL(CONVERT(VARCHAR(8), bsm.sta, 108), '00:00:00') AS sta,
                
                    CASE 
                        WHEN ISNULL(brm.running, 0) = 0 THEN '00:00:00'
                        WHEN CAST(GETDATE() AS TIME) < brm.start_time_up THEN '00:00:00'
                        WHEN CAST(GETDATE() AS TIME) > brm.end_time_up THEN '00:00:00'
                        ELSE ISNULL(CONVERT(VARCHAR(8), bsm.eta, 108), '00:00:00')
                    END AS eta,
                
                    CASE 
                        WHEN ISNULL(brm.running, 0) = 0 THEN 'Route Completed / Not Running'
                        WHEN CAST(GETDATE() AS TIME) < brm.start_time_up THEN 'Not Started'
                        WHEN CAST(GETDATE() AS TIME) > brm.end_time_up THEN 'Route Completed'
                        ELSE 'Running'
                    END AS route_status
                
                FROM bs_stop_master bsm
                INNER JOIN bs_route_master brm ON brm.id = bsm.route_id
                WHERE bsm.route_id = {eta.route_id}
                ORDER BY bsm.stop_order";

                #endregion

                DataTable dataTable = _sql_qury_execution.DML_Select(select_query);

                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    string json = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
                    return Content(json, "application/json");
                }

                return Content("Data Not Found", "application/json");
            }
            catch (Exception ex)
            {
                return Content(ex.Message, "application/json");
            }
        }


        [HttpPost]
        public IActionResult checkEachRouteViasing(Checkvias vias)
        {
            try
            {
                #region
                string select_query = $@"select user_stop_name ,link_no from bs_stop_master where route_id={vias.route_id} 
                                         order by stop_order";
                #endregion

                DataTable dataTable = _sql_qury_execution.DML_Select(select_query);
                if (dataTable != null)
                {

                    string json = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
                    return Content(json, "application/json");

                }
                return Content("0", "application/json");



            }//try block ends.
            catch (Exception ex)
            {

                return Content("0", "application/json");

            }//catch block ends.
        }





    }
}
