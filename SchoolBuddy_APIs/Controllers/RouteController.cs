using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using SchoolBuddy_APIs.Database_methods;
using SchoolBuddy_APIs.Models.Master.Route;
using SchoolBuddy_APIs.Models.Master.Students;
using System;
using System.Data;

namespace SchoolBuddy_APIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly Idatabase_access _sql_qury_execution;

        public RouteController(Idatabase_access sql_qury_execution)
        {
            _sql_qury_execution = sql_qury_execution;
        }
        [HttpPost]


        ///<summary>
        /// GetBuses actionmethod fetches bus records.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult GetBuses(bus_user_id bus)
        {
            try
            {
                string user_id = bus.user_id;
                string json = "";
                if (user_id != null && bus.database =="newtrack")
                {
                    string query = $"select id, veh_reg from services where sys_user_id = '{user_id}'";
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
                            return Content("Data Not Found", "application/json");
                        }//datatable has no rows
                    }//datatable is not null

                    return Content("Something Went Wrong", "application/json");

                }
                //user is not equal to null.

                else if (user_id != null && bus.database == "atltracking")
                {
                    string query = $"select id, veh_reg from atltracking.dbo.tbl_services where sys_user_id = '{user_id}'";
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
                            return Content("Data Not Found", "application/json");
                        }//datatable has no rows
                    }//datatable is not null

                    return Content("Something Went Wrong", "application/json");
                }
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
        /// GetAllRoutes actionmethod fetches all records.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult GetAllRoutes(bus_user_id bus)
        {
            try
            {
                string user_id = bus.user_id;
                string json = "";
                if (user_id != null && bus.database == "newtrack")
                {
                    string query = $"select brm.id,brm.route_name,brm.start_time_up,brm.end_time_up,";
                    query += $"s.veh_reg,brm.rowcreated from bs_route_master brm inner join services s on brm.sys_service_id = s.id";
                    query += $" where brm.sys_user_id = '{user_id}'";
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
                            return Content("Data Not Found", "application/json");
                        }//datatable has no rows
                    }//datatable is not null

                    return Content("Something Went Wrong", "application/json");

                }//user is not equal to null.

                else if (user_id != null && bus.database == "atltracking")
                {
                    string query = $"select brm.id,brm.route_name,brm.start_time_up,brm.end_time_up,";
                    query += $"s.veh_reg,brm.rowcreated from bs_route_master brm inner join atltracking.dbo.tbl_services s on brm.sys_service_id = s.id";
                    query += $" where brm.sys_user_id = '{user_id}'";
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
                            return Content("Data Not Found", "application/json");
                        }//datatable has no rows
                    }//datatable is not null

                    return Content("Something Went Wrong", "application/json");
                }
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
        /// GetRoutes actionmethod fetches all routes.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult GetRoutes(bus_user_id bus)
        {
            try
            {
                string user_id = bus.user_id;
                string json = "";
                if (user_id != null)
                {
                    string query = $"select brm.id,brm.route_name,brm.sys_service_id from bs_route_master brm";
                    query += $" where brm.sys_user_id = '{user_id}'";
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
        /// AddRoute actionmethod routes.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public bool AddRoute(Models.Master.Route.Route route)
        {
            try
            {
                if (route == null ||
                    string.IsNullOrWhiteSpace(route.route_name) ||
                    string.IsNullOrWhiteSpace(route.service_id) ||
                    string.IsNullOrWhiteSpace(route.start_time_up) ||
                    string.IsNullOrWhiteSpace(route.end_time_up) ||
                    string.IsNullOrWhiteSpace(route.user_id))
                {
                    return false;
                }

                string checkDuplicateQuery = $@"
            SELECT COUNT(1) AS total
            FROM bs_route_master
            WHERE sys_user_id = '{route.user_id}'
              AND LTRIM(RTRIM(LOWER(route_name))) = LTRIM(RTRIM(LOWER('{route.route_name}')))";

                DataTable checkDt = _sql_qury_execution.DML_Select(checkDuplicateQuery);

                if (checkDt != null && checkDt.Rows.Count > 0)
                {
                    int total = Convert.ToInt32(checkDt.Rows[0]["total"]);
                    if (total > 0)
                    {
                        return false;
                    }
                }

                string query = $"insert into bs_route_master (route_name,sys_service_id,start_time_up,";
                query += $"end_time_up,sys_user_id,rowcreated,is_Pis_Enabled,is_active) ";
                query += $"values ";
                query += $"('{route.route_name}','{route.service_id}','{route.start_time_up}',";
                query += $"'{route.end_time_up}','{route.user_id}',getdate(),1,1);";

                int result = _sql_qury_execution.DML_Insert_Update_Delete(query);
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in API AddRoute: " + ex.Message);
                return false;
            }
        }

        [HttpPost]


        ///<summary>
        /// EditRoute actionmethod edits records of route.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public bool EditRoute(Models.Master.Route.ERoute route)
        {
            string json = "";
            try
            {
                #region EDIT QUERY
                //string query = $"update bs_route_master set route_name = '{route.route_name}',";
                //query += $"start_time_up ='{route.start_time_up}',end_time_up = '{route.end_time_up}',updated_date='{route.rowupdated}' ";
                //query += $"where id = {route.id}";

                string query = $@"
            UPDATE bs_route_master
               SET route_name    = '{route.route_name}',
                   start_time_up = '{route.start_time_up}',
                   end_time_up   = '{route.end_time_up}',
                   sys_service_id= '{route.service_id}',     
                   updated_date  = '{route.rowupdated}'
             WHERE id = {route.id}";
                #endregion

                int result = _sql_qury_execution.DML_Insert_Update_Delete(query);
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return false;

            }//catch block ends.

        }


        [HttpPost]

        ///<summary>
        /// DeleteRoute actionmethod deletes record of route.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>

        public bool DeleteRoute(getroutebyid routeid)
        {
            string json = "";
            try
            {
                #region DELETE Assigned-students QUERY
                string query_assigned_students = $"delete from bs_route_students where route_id = '{routeid.route_id}'";
                #endregion
                #region DELETE stops QUERY
                string query_stops = $"delete from bs_stop_master where route_id = '{routeid.route_id}'";
                #endregion
                #region DELETE ROUTE QUERY
                string query_route = $"delete from bs_route_master where id = '{routeid.route_id}'";
                #endregion

                int result = _sql_qury_execution.DML_Insert_Update_Delete_with_Transaction(query_assigned_students, query_stops, query_route);
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return false;

            }//catch block ends.

        }


        [HttpPost]

        ///<summary>
        /// GetRouteById actionmethod gets route records by id.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult GetRouteById(getroutebyid routeid)
        {
            string json = "";
            try
            {
                #region GET_ROUTE_ID QUERY
                //string query = $"select brm.id,brm.route_name,brm.start_time_up,brm.end_time_up,brm.sys_service_id,brm.sys_user_id,s.sys_device_id from bs_route_master  brm inner join services s on s.id = brm.sys_service_id where brm.id = '{routeid.route_id}'";

                string query = $"select id, route_name, start_time_up, end_time_up, sys_service_id, sys_user_id from bs_route_master where id = '{routeid.route_id}'";
                #endregion

                DataTable dataTable = _sql_qury_execution.DML_Select(query);
                if (dataTable != null)
                {
                    if (dataTable.Rows.Count > 0)
                    {

                        json = JsonConvert.SerializeObject(new Models.Master.Route.Route
                        {
                            id = dataTable.Rows[0]["id"].ToString(),
                            route_name = dataTable.Rows[0]["route_name"].ToString(),
                            start_time_up = dataTable.Rows[0]["start_time_up"].ToString(),
                            end_time_up = dataTable.Rows[0]["end_time_up"].ToString(),
                            service_id = dataTable.Rows[0]["sys_service_id"].ToString(),
                            user_id = dataTable.Rows[0]["sys_user_id"].ToString(),
                            //device_id = dataTable.Rows[0]["sys_device_id"].ToString()
                            
                        });
                        return Content(json, "application/json");
                    }//datatable has rows
                    else
                    {
                        return Content("Data Not Found", "application/json");
                    }//datatable has no rows
                }

                return Content("Data Not Found", "application/json");
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return Content("Something went wrong", "application/json");

            }//catch block ends.

        }

        [HttpPost]
        public IActionResult GetStudentsByRouteId(getroutebyid routeid)
        {
            string json = "";
            try
            {
                #region GET STUDENTS BY ROUTE ID QUERY
                string query = $@"
            SELECT 
                bsmb.id,
                brs.route_id AS routeid, 
                bsmb.student_name,
                bsmb.father_name,
                bsmb.mobile_no1,
                bcm.class_name,
                bsmb.admission_no,
                bsmb.section,
                bsmb.rf_id,
	            bsm.user_stop_name
                FROM bs_student_master_backup bsmb
                JOIN bs_class_master bcm ON bsmb.class = bcm.id
                JOIN bs_route_students brs ON bsmb.id = brs.student_id
                JOIN bs_stop_master bsm on bsm.id = brs.stop_id
                WHERE brs.route_id = '{routeid.route_id}'
 ORDER BY bsmb.student_name";
                #endregion

                DataTable dataTable = _sql_qury_execution.DML_Select(query);

                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    var studentList = new List<Models.Master.Route.StudentDetails>();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        studentList.Add(new Models.Master.Route.StudentDetails
                        {
                            id = row["id"].ToString(),
                            routeid = row["routeid"].ToString(),
                            student_name = row["student_name"].ToString(),
                            father_name = row["father_name"].ToString(),
                            mobile_no = row["mobile_no1"].ToString(),
                            class_name = row["class_name"].ToString(),
                            admission_no = row["admission_no"].ToString(),
                            section = row["section"].ToString(),
                            rf_id = row["rf_id"].ToString(),
                            stop_name = row["user_stop_name"].ToString()
                        });
                    }

                    json = JsonConvert.SerializeObject(studentList);
                    return Content(json, "application/json");
                }
                else
                {
                    return Content("Data Not Found", "application/json");
                }
            }
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return Content("Something went wrong", "application/json");
            }
        }


        [HttpPost]
        public IActionResult GetStudentWise([FromBody] getsysuserid model)
        {
            string json = "";
            try
            {
                string sys_user_id = model.sys_user_id;  // ✔️ Extract value from object

                //                string query = $@"
                //                ;WITH StudentDetails AS (
                //                    SELECT
                //                        bsmb.id AS student_id,
                //                        bsmb.sys_user_id,
                //                        bsmb.student_name,
                //                        bsmb.father_name,
                //                        bsmb.mobile_no1,
                //                        bsmb.admission_no,
                //                        bsmb.rf_id,
                //                        bsmb.section,
                //                        bcm.class_name
                //                    FROM bs_student_master_backup bsmb
                //                    JOIN bs_class_master bcm ON bsmb.class = bcm.id
                //                    JOIN bs_route_students brs ON bsmb.id = brs.student_id
                //                    WHERE bsmb.sys_user_id = '{sys_user_id}'
                //                    GROUP BY
                //                        bsmb.id,
                //                        bsmb.sys_user_id,
                //                        bsmb.student_name,
                //                        bsmb.father_name,
                //                        bsmb.mobile_no1,
                //                        bsmb.admission_no,
                //                        bsmb.rf_id,
                //                        bsmb.section,
                //                        bcm.class_name
                //                ),
                //                StudentRoutes AS (
                //                    SELECT
                //                        brs.student_id,
                //                        brs.route_id,
                //                        brm.route_name,
                //                        bsm.id AS stop_id,
                //                        bsm.latitude,
                //                        bsm.longitude,
                //                        bsm.user_stop_name,
                //                        CASE
                //                            WHEN LOWER(brm.route_name) LIKE '%pick%' THEN 'pick'
                //                            WHEN LOWER(brm.route_name) LIKE '%drop%' THEN 'drop'
                //                        END AS route_type,
                //                        ROW_NUMBER() OVER (
                //                            PARTITION BY brs.student_id,
                //                                CASE
                //                                    WHEN LOWER(brm.route_name) LIKE '%pick%' THEN 'pick'
                //                                    WHEN LOWER(brm.route_name) LIKE '%drop%' THEN 'drop'
                //                                END
                //                            ORDER BY brs.id
                //                        ) AS rn
                //                    FROM bs_route_students brs
                //                    JOIN bs_route_master brm ON brs.route_id = brm.id
                //                    JOIN bs_stop_master bsm ON bsm.id = brs.stop_id
                //                    WHERE LOWER(brm.route_name) LIKE '%pick%'
                //                       OR LOWER(brm.route_name) LIKE '%drop%'
                //                )
                //                SELECT
                //                    COALESCE(p.stop_id, d.stop_id) AS stop_id,
                //                    p.stop_id AS pick_stop_id,
                //                    d.stop_id AS drop_stop_id,
                //                    p.latitude AS pick_latitude,
                //                    p.longitude AS pick_longitude,
                //                    d.latitude AS drop_latitude,
                //                    d.longitude AS drop_longitude,
                //                    COALESCE(p.user_stop_name, d.user_stop_name) AS stop_name,
                //                    p.user_stop_name AS pick_stop_name,
                //                    d.user_stop_name AS drop_stop_name,
                //                    sd.student_id,
                //                    sd.sys_user_id,
                //                    sd.student_name,
                //                    sd.father_name,
                //                    sd.mobile_no1,
                //                    sd.admission_no,
                //                    sd.rf_id,
                //                    sd.section,
                //                    sd.class_name,
                //                    p.route_id AS pick_route_id,
                //                    p.route_name AS pick_route_name,
                //                    d.route_id AS drop_route_id,
                //                    d.route_name AS drop_route_name
                //                FROM StudentDetails sd
                //                LEFT JOIN StudentRoutes p
                //                    ON sd.student_id = p.student_id
                //                   AND p.route_type = 'pick'
                //                   AND p.rn = 1
                //                LEFT JOIN StudentRoutes d
                //                    ON sd.student_id = d.student_id

                //                   AND d.rn = 2

                //";
                string query = $@"
;WITH StudentDetails AS
(
    SELECT
        bsmb.id AS student_id,
        bsmb.sys_user_id,
        bsmb.student_name,
        bsmb.father_name,
        bsmb.mobile_no1,
        bsmb.admission_no,
        bsmb.rf_id,
        bsmb.section,
        bcm.class_name
    FROM bs_student_master_backup bsmb
    JOIN bs_class_master bcm
        ON bsmb.class = bcm.id
    WHERE bsmb.sys_user_id = '{sys_user_id}'
),
StudentRoutes AS
(
    SELECT
        brs.student_id,
        brs.route_id,
        brm.route_name,
        bsm.id AS stop_id,
        bsm.latitude,
        bsm.longitude,
        bsm.user_stop_name,

        CASE
            WHEN brm.route_name LIKE '%pick%' THEN 'pick'
            WHEN brm.route_name LIKE '%drop%' THEN 'drop'
        END AS route_type,

        ROW_NUMBER() OVER
        (
            PARTITION BY
                brs.student_id,
                CASE
                    WHEN brm.route_name LIKE '%pick%' THEN 'pick'
                    WHEN brm.route_name LIKE '%drop%' THEN 'drop'
                END
            ORDER BY brs.id
        ) AS rn

    FROM bs_route_students brs

    JOIN StudentDetails sd
        ON sd.student_id = brs.student_id

    JOIN bs_route_master brm
        ON brs.route_id = brm.id

    JOIN bs_stop_master bsm
        ON bsm.id = brs.stop_id

    WHERE brm.route_name LIKE '%pick%'
       OR brm.route_name LIKE '%drop%'
)

SELECT
    COALESCE(p.stop_id, d.stop_id) AS stop_id,

    p.stop_id AS pick_stop_id,
    d.stop_id AS drop_stop_id,

    p.latitude AS pick_latitude,
    p.longitude AS pick_longitude,

    d.latitude AS drop_latitude,
    d.longitude AS drop_longitude,

    COALESCE(p.user_stop_name, d.user_stop_name) AS stop_name,

    p.user_stop_name AS pick_stop_name,
    d.user_stop_name AS drop_stop_name,

    sd.student_id,
    sd.sys_user_id,
    sd.student_name,
    sd.father_name,
    sd.mobile_no1,
    sd.admission_no,
    sd.rf_id,
    sd.section,
    sd.class_name,

    p.route_id AS pick_route_id,
    p.route_name AS pick_route_name,

    d.route_id AS drop_route_id,
    d.route_name AS drop_route_name

FROM StudentDetails sd

LEFT JOIN StudentRoutes p
    ON sd.student_id = p.student_id
    AND p.route_type = 'pick'
    AND p.rn = 1

LEFT JOIN StudentRoutes d
    ON sd.student_id = d.student_id
    AND d.route_type = 'drop'
    AND d.rn = 1;";

                var dt = _sql_qury_execution.DML_Select(query);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var list = new List<StudentWiseModel>();
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(new StudentWiseModel
                        {
                            stop_id = row["stop_id"].ToString(),
                            pick_stop_id = row["pick_stop_id"].ToString(),
                            drop_stop_id = row["drop_stop_id"].ToString(),
                            student_id = row["student_id"].ToString(),
                            student_name = row["student_name"].ToString(),
                            father_name = row["father_name"].ToString(),
                            mobile_no1 = row["mobile_no1"].ToString(),
                            admission_no = row["admission_no"].ToString(),
                            rf_id = row["rf_id"].ToString(),
                            section = row["section"].ToString(),
                            class_name = row["class_name"].ToString(),
                            pick_route_id = row["pick_route_id"].ToString(),
                            pick_route_name = row["pick_route_name"].ToString(),
                            pick_stop_name = row["pick_stop_name"].ToString(),
                            drop_route_id = row["drop_route_id"].ToString(),
                            drop_route_name = row["drop_route_name"].ToString(),
                            drop_stop_name = row["drop_stop_name"].ToString(),
                            sys_user_id = row["sys_user_id"].ToString(),
                            stop_name = row["stop_name"].ToString(),
                            pick_latitude = row["pick_latitude"].ToString(),
                            pick_longitude = row["pick_longitude"].ToString(),
                            drop_latitude = row["drop_latitude"].ToString(),
                            drop_longitude = row["drop_longitude"].ToString(),
                        });
                    }
                    return Ok(list);
                    json = JsonConvert.SerializeObject(list);
                    return Content(json, "application/json");
                }

                return Content("[]", "application/json");
            }
            catch
            {
                return Content("[]", "application/json");
            }
        }





        [HttpPost]
        ///<summary>
        /// deleteAllRoutes actionmethod delete all routes
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult deleteAllRoutes(bus_user_id id)
        {
            string json = "";
            try
            {
                #region GETSTUDENTBYID QUERY
                string query = $"delete  from bs_route_master where id = '{id.user_id}'";
                #endregion

                DataTable dataTable = _sql_qury_execution.DML_Select(query);
                if (dataTable != null)
                {
                    if (dataTable.Rows.Count > 0)
                    {

                        json = JsonConvert.SerializeObject(new getstudent
                        {
                            id = Convert.ToInt32(dataTable.Rows[0]["id"]),
                            student_name = dataTable.Rows[0]["name"].ToString(),
                            class_ = dataTable.Rows[0]["class"].ToString(),
                            admission_no = dataTable.Rows[0]["ad_no"].ToString(),
                            gender = dataTable.Rows[0]["gender"].ToString(),
                            birth = dataTable.Rows[0]["birth"].ToString(),
                            division = dataTable.Rows[0]["division"].ToString(),
                            parent_name = dataTable.Rows[0]["parent_name"].ToString(),
                            mobile_no1 = dataTable.Rows[0]["mobile"].ToString(),
                            password = dataTable.Rows[0]["pwd"].ToString(),
                            street = dataTable.Rows[0]["address"].ToString(),
                            rf_id = dataTable.Rows[0]["rfid"].ToString(),
                            email = dataTable.Rows[0]["email"].ToString(),
                            relation_with_std = dataTable.Rows[0]["relation_with_std"].ToString(),
                            blood_group = dataTable.Rows[0]["blood_group"].ToString(),
                            qr_code = dataTable.Rows[0]["qrcode"].ToString()
                        });
                        return Content(json, "application/json");
                    }//datatable has rows
                    else
                    {
                        return Content("Data Not Found", "application/json");
                    }//datatable has no rows
                }

                return Content("Data Not Found", "application/json");
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return Content("Something went wrong", "application/json");

            }//catch block ends.

        }


        [HttpPost]
        ///<summary>
        /// addPlayBack actionmethod delete all routes
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public bool addPlayBack(playback playback)
        {
            string json = "";
            try
            {
                #region INSERT QUERY

                string query = $"insert into bs_route_vias (Route_id,";
                query += $"latitude,longitude,via_order) ";
                query += $"values ";
                query += $"('{playback.route_id}','{playback.lat}','{playback.lng}',";
                query += $"'{playback.via_order}');";
                #endregion

                int result = _sql_qury_execution.DML_Insert_Update_Delete(query);
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return false;

            }//catch block ends.

        }

        [HttpPost]

        ///<summary>
        /// addPlayBack actionmethod delete all routes
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult GetPlayBack(playback_atl_newtrack playback)
        {
            string json = "";
            
            try
            {
                string tablename = "";
                if (playback.database=="newtrack")
                {
                    tablename = $"telemetry{DateTime.Now.ToString("MMMyy")}";
                }
                else
                {
                    tablename = $"tbl_telemetry{DateTime.Now.ToString("MMMyy")}";
                }
                string start_time_ = Convert.ToDateTime(playback.start_date).ToString("yyyy-MM-dd HH:mm:tt");
                string end_time_ = Convert.ToDateTime(playback.end_date).ToString("yyyy-MM-dd HH:mm:tt");
                #region GETSTUDENTBYID QUERY
                string query = $@"select gps_latitude,gps_longitude from {tablename} where 
                                  sys_proc_time >= '{playback.start_date}' and sys_proc_time<='{start_time_}'
                                  sys_service_id = '{end_time_}'";
                #endregion

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
                        return Content("Data Not Found", "application/json");
                    }//datatable has no rows
                }//datatable is not null

                return Content("Something Went Wrong", "application/json");
                    
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return Content("Something went wrong", "application/json");

            }//catch block ends.

        }


        [HttpPost]

        ///<summary>
        /// GetIMEI actionmethod gets imei of devices.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult GetIMEI(device device)
        {
            try
            {
                if (device != null)
                {
                    string imei = null;

                    if (device.database.Equals("newtrack", StringComparison.OrdinalIgnoreCase))
                    {
                        string query = $"select imei from devices d join services s on s.sys_device_id = d.id where s.id = '{device.id}'";//this is service id
                        var datatable = _sql_qury_execution.DML_Select(query);
                        if (datatable != null && datatable.Rows.Count > 0)
                            imei = datatable.Rows[0]["imei"].ToString();
                    }
                    else if (device.database.Equals("atltracking", StringComparison.OrdinalIgnoreCase))
                    {
                        string query2 = $"select * from atltracking.dbo.tbl_devices d inner join atltracking.dbo.tbl_services s on s.sys_device_id=d.id where s.id = '{device.id}'";
                        var datatable = _sql_qury_execution.DML_Select(query2);
                        if (datatable != null && datatable.Rows.Count > 0)
                            imei = datatable.Rows[0]["imei"].ToString();
                    }

                    if (!string.IsNullOrEmpty(imei))
                    {
                        var response = new[]
                        {
                    new {
                        imei = imei,
                        database = device.database
                    }
                };
                        return Content(JsonConvert.SerializeObject(response, Formatting.Indented), "application/json");
                    }

                    return Content("Data Not Found", "application/json");
                }
                else
                {
                    return Content("Login Again..", "application/json");
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message, "application/json");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadAssignedStudentsExcel(string routeId)
        {
            try
            {
                if (string.IsNullOrEmpty(routeId))
                {
                    return BadRequest("Route ID is required.");
                }

                string query = $@"
            SELECT 
                bsmb.admission_no,
                bsmb.student_name,
                bsmb.father_name,
                bsmb.mobile_no1,
                bcm.class_name,
                bsmb.section,
                bsmb.rf_id,
                bsm.user_stop_name AS stop_name
            FROM bs_student_master_backup bsmb
            JOIN bs_class_master bcm ON TRY_CONVERT(int, bsmb.class) = bcm.id
            JOIN bs_route_students brs ON bsmb.id = brs.student_id
            JOIN bs_stop_master bsm ON bsm.id = brs.stop_id
            WHERE brs.route_id = '{routeId}'
            ORDER BY bsmb.student_name";

                DataTable dt = _sql_qury_execution.DML_Select(query);

                if (dt == null || dt.Rows.Count == 0)
                {
                    return NotFound("No assigned student data found.");
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add("Assigned Students");

                    ws.Cells[1, 1].Value = "admission_no";
                    ws.Cells[1, 2].Value = "student_name";
                    ws.Cells[1, 3].Value = "father_name";
                    ws.Cells[1, 4].Value = "mobile_no1";
                    ws.Cells[1, 5].Value = "class_name";
                    ws.Cells[1, 6].Value = "section";
                    ws.Cells[1, 7].Value = "rf_id";
                    ws.Cells[1, 8].Value = "stop_name";

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ws.Cells[i + 2, 1].Value = dt.Rows[i]["admission_no"]?.ToString();
                        ws.Cells[i + 2, 2].Value = dt.Rows[i]["student_name"]?.ToString();
                        ws.Cells[i + 2, 3].Value = dt.Rows[i]["father_name"]?.ToString();
                        ws.Cells[i + 2, 4].Value = dt.Rows[i]["mobile_no1"]?.ToString();
                        ws.Cells[i + 2, 5].Value = dt.Rows[i]["class_name"]?.ToString();
                        ws.Cells[i + 2, 6].Value = dt.Rows[i]["section"]?.ToString();
                        ws.Cells[i + 2, 7].Value = dt.Rows[i]["rf_id"]?.ToString();
                        ws.Cells[i + 2, 8].Value = dt.Rows[i]["stop_name"]?.ToString();
                    }

                    if (ws.Dimension != null)
                    {
                        ws.Cells[ws.Dimension.Address].AutoFitColumns();
                    }

                    var stream = new MemoryStream();
                    await package.SaveAsAsync(stream);
                    stream.Position = 0;

                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"AssignedStudents_Route_{routeId}.xlsx"
                    );
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error while generating excel: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult UpdateStudentWise([FromBody] UpdateStudentModel model)
        {
            try
            {
                static string NormalizeId(string value) => value?.Trim() ?? string.Empty;

                string changeType = NormalizeId(model.change_type).ToLowerInvariant();
                bool pickChanged = NormalizeId(model.pick_route_id) != NormalizeId(model.prev_pick_route_id) ||
                                   NormalizeId(model.pick_stop_id) != NormalizeId(model.prev_pick_stop_id);
                bool dropChanged = NormalizeId(model.drop_route_id) != NormalizeId(model.prev_drop_route_id) ||
                                   NormalizeId(model.drop_stop_id) != NormalizeId(model.prev_drop_stop_id);

                if (!pickChanged && !dropChanged)
                {
                    return Content("No changes detected.", "application/json");
                }

                // --- VALIDATE CHANGE TYPE vs PAYLOAD ---
                if (changeType == "pick" && (!pickChanged || dropChanged))
                {
                    return Content("The requested transport change does not match change_type.", "application/json");
                }
                if (changeType == "drop" && (!dropChanged || pickChanged))
                {
                    return Content("The requested transport change does not match change_type.", "application/json");
                }
                if (changeType == "both" && (!pickChanged || !dropChanged))
                {
                    return Content("Both Pick and Drop changes are required when change_type is 'both'.", "application/json");
                }
                if (changeType != "pick" && changeType != "drop" && changeType != "both")
                {
                    return Content("Invalid transport change type.", "application/json");
                }

                if (!int.TryParse(model.student_id, out int studentId))
                {
                    return Content("Invalid student id.", "application/json");
                }

                // --- UPDATE PICK IF REQUESTED ---
                if (changeType == "pick" || changeType == "both")
                {
                    if (!int.TryParse(model.pick_route_id, out int currentRouteId) ||
                        !int.TryParse(model.pick_stop_id, out int currentStopId))
                    {
                        return Content("Please select a valid Pick Route and Pick Stop.", "application/json");
                    }

                    bool previousRouteMissing = string.IsNullOrEmpty(NormalizeId(model.prev_pick_route_id));
                    bool previousStopMissing = string.IsNullOrEmpty(NormalizeId(model.prev_pick_stop_id));
                    bool isInitialAssignment = previousRouteMissing && previousStopMissing;

                    if (previousRouteMissing != previousStopMissing)
                    {
                        return Content("The previous pick route and stop assignment is incomplete.", "application/json");
                    }

                    string targetValidationQuery = $@"SELECT COUNT(1) FROM bs_stop_master bsm JOIN bs_route_master brm ON brm.id = bsm.route_id WHERE bsm.id = {currentStopId} AND bsm.route_id = {currentRouteId} AND LOWER(brm.route_name) LIKE '%pick%';";

                    DataTable targetValidation = _sql_qury_execution.DML_Select(targetValidationQuery);
                    if (targetValidation == null || targetValidation.Rows.Count == 0 ||
                        Convert.ToInt32(targetValidation.Rows[0][0]) != 1)
                    {
                        return Content("The selected stop does not belong to a Pick route.", "application/json");
                    }

                    if (isInitialAssignment)
                    {
                        string insertQuery = $@"INSERT INTO bs_route_students (route_id, stop_id, student_id) SELECT {currentRouteId}, {currentStopId}, {studentId} WHERE NOT EXISTS ( SELECT 1 FROM bs_route_students brs JOIN bs_route_master brm ON brm.id = brs.route_id WHERE brs.student_id = {studentId} AND LOWER(brm.route_name) LIKE '%pick%');";

                        int inserted = _sql_qury_execution.DML_Insert_Update_Delete(insertQuery);
                        if (inserted <= 0)
                        {
                            return Content("A Pick assignment already exists.", "application/json");
                        }
                    }
                    else
                    {
                        if (!int.TryParse(model.prev_pick_route_id, out int previousRouteId) ||
                            !int.TryParse(model.prev_pick_stop_id, out int previousStopId))
                        {
                            return Content("The previous pick student assignment is invalid.", "application/json");
                        }

                        string validationQuery = $@"SELECT (SELECT COUNT(1) FROM bs_route_students brs JOIN bs_route_master brm ON brm.id = brs.route_id WHERE brs.student_id = {studentId} AND brs.route_id = {previousRouteId} AND brs.stop_id = {previousStopId}) AS previous_assignment_count;";

                        DataTable validation = _sql_qury_execution.DML_Select(validationQuery);
                        if (validation == null || validation.Rows.Count == 0 ||
                            Convert.ToInt32(validation.Rows[0]["previous_assignment_count"]) != 1)
                        {
                            return Content("The previous pick student assignment no longer matches the database.", "application/json");
                        }

                        string query = $@"UPDATE bs_route_students SET route_id = {currentRouteId}, stop_id = {currentStopId} WHERE student_id = {studentId} AND route_id = {previousRouteId} AND stop_id = {previousStopId};";

                        int response = _sql_qury_execution.DML_Insert_Update_Delete(query);
                        if (response <= 0)
                        {
                            return Content("Pick route data Not Updated use Different Combination", "application/json");
                        }
                    }
                }

                // --- UPDATE DROP IF REQUESTED ---
                if (changeType == "drop" || changeType == "both")
                {
                    if (!int.TryParse(model.drop_route_id, out int currentRouteId) ||
                        !int.TryParse(model.drop_stop_id, out int currentStopId))
                    {
                        return Content("Please select a valid Drop Route and Drop Stop.", "application/json");
                    }

                    bool previousRouteMissing = string.IsNullOrEmpty(NormalizeId(model.prev_drop_route_id));
                    bool previousStopMissing = string.IsNullOrEmpty(NormalizeId(model.prev_drop_stop_id));
                    bool isInitialAssignment = previousRouteMissing && previousStopMissing;

                    if (previousRouteMissing != previousStopMissing)
                    {
                        return Content("The previous drop route and stop assignment is incomplete.", "application/json");
                    }

                    string targetValidationQuery = $@"SELECT COUNT(1) FROM bs_stop_master bsm JOIN bs_route_master brm ON brm.id = bsm.route_id WHERE bsm.id = {currentStopId} AND bsm.route_id = {currentRouteId} AND LOWER(brm.route_name) LIKE '%drop%';";

                    DataTable targetValidation = _sql_qury_execution.DML_Select(targetValidationQuery);
                    if (targetValidation == null || targetValidation.Rows.Count == 0 ||
                        Convert.ToInt32(targetValidation.Rows[0][0]) != 1)
                    {
                        return Content("The selected stop does not belong to a Drop route.", "application/json");
                    }

                    if (isInitialAssignment)
                    {
                        string insertQuery = $@"INSERT INTO bs_route_students (route_id, stop_id, student_id) SELECT {currentRouteId}, {currentStopId}, {studentId} WHERE NOT EXISTS ( SELECT 1 FROM bs_route_students brs JOIN bs_route_master brm ON brm.id = brs.route_id WHERE brs.student_id = {studentId} AND LOWER(brm.route_name) LIKE '%drop%');";

                        int inserted = _sql_qury_execution.DML_Insert_Update_Delete(insertQuery);
                        if (inserted <= 0)
                        {
                            return Content("A Drop assignment already exists.", "application/json");
                        }
                    }
                    else
                    {
                        if (!int.TryParse(model.prev_drop_route_id, out int previousRouteId) ||
                            !int.TryParse(model.prev_drop_stop_id, out int previousStopId))
                        {
                            return Content("The previous drop student assignment is invalid.", "application/json");
                        }

                        string validationQuery = $@"SELECT (SELECT COUNT(1) FROM bs_route_students brs JOIN bs_route_master brm ON brm.id = brs.route_id WHERE brs.student_id = {studentId} AND brs.route_id = {previousRouteId} AND brs.stop_id = {previousStopId}) AS previous_assignment_count;";

                        DataTable validation = _sql_qury_execution.DML_Select(validationQuery);
                        if (validation == null || validation.Rows.Count == 0 ||
                            Convert.ToInt32(validation.Rows[0]["previous_assignment_count"]) != 1)
                        {
                            return Content("The previous drop student assignment no longer matches the database.", "application/json");
                        }

                        string query = $@"UPDATE bs_route_students SET route_id = {currentRouteId}, stop_id = {currentStopId} WHERE student_id = {studentId} AND route_id = {previousRouteId} AND stop_id = {previousStopId};";

                        int response = _sql_qury_execution.DML_Insert_Update_Delete(query);
                        if (response <= 0)
                        {
                            return Content("Drop route data Not Updated use Different Combination", "application/json");
                        }
                    }
                }

                return Content("Data Updated Successfully", "application/json");
            }
            catch (Exception)
            {
                return Content("Data Not Updated", "application/json");
            }
        }

        //        [HttpPost]
        //        public IActionResult UpdateStudentWise([FromBody] UpdateStudentModel model)
        //        {
        //            try
        //            {
        //                static string NormalizeId(string value) => value?.Trim() ?? string.Empty;

        //                string changeType = NormalizeId(model.change_type).ToLowerInvariant();
        //                bool pickChanged = NormalizeId(model.pick_route_id) != NormalizeId(model.prev_pick_route_id) ||
        //                                   NormalizeId(model.pick_stop_id) != NormalizeId(model.prev_pick_stop_id);
        //                bool dropChanged = NormalizeId(model.drop_route_id) != NormalizeId(model.prev_drop_route_id) ||
        //                                   NormalizeId(model.drop_stop_id) != NormalizeId(model.prev_drop_stop_id);

        //                if (changeType != "pick" && changeType != "drop")
        //                {
        //                    return Content("Invalid transport change type.", "application/json");
        //                }

        //                if (pickChanged && dropChanged)
        //                {
        //                    return Content(
        //                        "Please update either Pick Route/Stop or Drop Route/Stop at one time.",
        //                        "application/json"
        //                    );
        //                }

        //                if (!pickChanged && !dropChanged)
        //                {
        //                    return Content("No changes detected.", "application/json");
        //                }

        //                if ((changeType == "pick" && (!pickChanged || dropChanged)) ||
        //                    (changeType == "drop" && (!dropChanged || pickChanged)))
        //                {
        //                    return Content("The requested transport change does not match change_type.", "application/json");
        //                }

        //                if (!int.TryParse(model.student_id, out int studentId))
        //                {
        //                    return Content("Invalid student id.", "application/json");
        //                }

        //                string currentRouteValue = changeType == "pick" ? model.pick_route_id : model.drop_route_id;
        //                string currentStopValue = changeType == "pick" ? model.pick_stop_id : model.drop_stop_id;
        //                string previousRouteValue = changeType == "pick" ? model.prev_pick_route_id : model.prev_drop_route_id;
        //                string previousStopValue = changeType == "pick" ? model.prev_pick_stop_id : model.prev_drop_stop_id;

        //                if (!int.TryParse(currentRouteValue, out int currentRouteId) ||
        //                    !int.TryParse(currentStopValue, out int currentStopId))
        //                {
        //                    string side = changeType == "pick" ? "Pick" : "Drop";
        //                    return Content($"Please select a valid {side} Route and {side} Stop.", "application/json");
        //                }

        //                bool previousRouteMissing = string.IsNullOrEmpty(NormalizeId(previousRouteValue));
        //                bool previousStopMissing = string.IsNullOrEmpty(NormalizeId(previousStopValue));
        //                bool isInitialDropAssignment = changeType == "drop" &&
        //                                               previousRouteMissing &&
        //                                               previousStopMissing;

        //                if (previousRouteMissing != previousStopMissing)
        //                {
        //                    return Content("The previous route and stop assignment is incomplete.", "application/json");
        //                }

        //                string expectedRouteType = changeType == "pick" ? "pick" : "drop";
        //                string targetValidationQuery = $@"
        //SELECT COUNT(1)
        //FROM bs_stop_master bsm
        //JOIN bs_route_master brm ON brm.id = bsm.route_id
        //WHERE bsm.id = {currentStopId}
        //  AND bsm.route_id = {currentRouteId}
        //  AND LOWER(brm.route_name) LIKE '%{expectedRouteType}%';";

        //                DataTable targetValidation = _sql_qury_execution.DML_Select(targetValidationQuery);
        //                if (targetValidation == null || targetValidation.Rows.Count == 0 ||
        //                    Convert.ToInt32(targetValidation.Rows[0][0]) != 1)
        //                {
        //                    string side = changeType == "pick" ? "Pick" : "Drop";
        //                    return Content($"The selected stop does not belong to a {side} route.", "application/json");
        //                }

        //                if (isInitialDropAssignment)
        //                {
        //                    string insertQuery = $@"
        //INSERT INTO bs_route_students (route_id, stop_id, student_id)
        //SELECT {currentRouteId}, {currentStopId}, {studentId}
        //WHERE NOT EXISTS (
        //    SELECT 1
        //    FROM bs_route_students brs
        //    JOIN bs_route_master brm ON brm.id = brs.route_id
        //    WHERE brs.student_id = {studentId}
        //      AND LOWER(brm.route_name) LIKE '%drop%'
        //);";

        //                    int inserted = _sql_qury_execution.DML_Insert_Update_Delete(insertQuery);
        //                    return inserted > 0
        //                        ? Content("Data Updated Successfully", "application/json")
        //                        : Content("A Drop assignment already exists. Refresh and migrate the existing Drop assignment.", "application/json");
        //                }

        //                if (!int.TryParse(previousRouteValue, out int previousRouteId) ||
        //                    !int.TryParse(previousStopValue, out int previousStopId))
        //                {
        //                    return Content("The previous student assignment is invalid.", "application/json");
        //                }

        //                string validationQuery = $@"
        //SELECT
        //    (SELECT COUNT(1)
        //       FROM bs_route_students brs
        //       JOIN bs_route_master brm ON brm.id = brs.route_id
        //      WHERE brs.student_id = {studentId}
        //        AND brs.route_id = {previousRouteId}
        //        AND brs.stop_id = {previousStopId}
        //        ) AS previous_assignment_count;";

        //                DataTable validation = _sql_qury_execution.DML_Select(validationQuery);
        //                if (validation == null || validation.Rows.Count == 0 ||
        //                    Convert.ToInt32(validation.Rows[0]["previous_assignment_count"]) != 1)
        //                {
        //                    return Content("The previous student assignment no longer matches the database.", "application/json");
        //                }

        //                string query = $@"
        //UPDATE bs_route_students
        //SET route_id = {currentRouteId},
        //    stop_id = {currentStopId}
        //WHERE student_id = {studentId}
        //  AND route_id = {previousRouteId}
        //  AND stop_id = {previousStopId};";

        //                int response = _sql_qury_execution.DML_Insert_Update_Delete(query);
        //                return response > 0
        //                    ? Content("Data Updated Successfully", "application/json")
        //                    : Content("Data Not Updated use Different Combination", "application/json");
        //            }
        //            catch (Exception)
        //            {
        //                return Content("Data Not Updated", "application/json");
        //            }
        //        }

        [HttpPost]
        public async Task<IActionResult> DeleteStudent( int id)
        {
            try
            {
                string query = $@"
DELETE FROM bs_route_students 
WHERE student_id = {id}";
                var response = _sql_qury_execution.DML_Insert_Update_Delete(query);

                if (response > 0)
                {
                    return Content("Student Removed Successfully", "application/json");
                }
                return Content("Student not Removed", "application/json");
            }
            catch(Exception ex)
            {
                return Content("Student not Removed", "application/json");
            }
        }
    }
}
