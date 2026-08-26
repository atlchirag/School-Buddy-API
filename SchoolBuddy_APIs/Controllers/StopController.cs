using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolBuddy_APIs.Database_methods;
using SchoolBuddy_APIs.Models.Master.Route;
using SchoolBuddy_APIs.Models.Master.Students;
using System.Data;

namespace SchoolBuddy_APIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StopController : ControllerBase
    {
        private readonly Idatabase_access _sql_qury_execution;
        public StopController(Idatabase_access sql_qury_execution)
        {
            _sql_qury_execution = sql_qury_execution;
        }

        [HttpPost]
        ///<summary>
        /// stopSectionView actionmethod gets details of students  with stops.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult stopSectionView([FromForm] string stopid)
        {
            try
            {
                
                string json = "";
                if (stopid != null)
                {
                    string query = $@"select bsmb.student_name,bsmb.father_name,
                                      bsmb.admission_no,bsmb.rf_id,bsmb.mobile_no1 
                                      from bs_student_master_backup bsmb 
                                      inner join bs_route_students brs 
                                      on 
                                      brs.student_id = bsmb.id 
                                      where brs.stop_id = {stopid}";
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
                            return Content("-1");
                        }//datatable has no rows
                    }//datatable is not null

                    return Content("0");

                }//user is not equal to null.
                else
                {
                    return Content("0");
                }//user id null.
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return Content("0");

            }//catch block ends.

        }


        [HttpPost]
        ///<summary>
        /// addStop actionmethod add stops.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public bool addStop(Stop stop)
        {
            string json = "";
            try
            {
                #region ADDSTOP QUERY
                string query = $"Insert into bs_stop_master(user_stop_name,route_id,RFId,msg,stop_order,sys_user_id,latitude,longitude) values ('{stop.Stop_Name}',{stop.route_id},' ',10,(select isnull(max(stop_order),0) +1 from bs_stop_master where route_id={stop.route_id}),{stop.uid},{stop.Latitude},{stop.Longitude});";
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
        /// getAllStops actionmethod gets all stops.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult getAllStops(getroutebyid RouteId)
        {
            string json = "";
            try
            {
                #region GETALLSTOPS QUERY
                string query = $"select id,latitude,longitude,user_stop_name,stop_order, status from bs_stop_master where route_id = {RouteId.route_id} order by stop_order";
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
                        return Content("-1");
                    }//datatable has no rows
                }//datatable is not null

                return Content("0");
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return Content("0");

            }//catch block ends.
        }


        [HttpPost]
        public IActionResult getStopById(stopid model)
        {
            string json = "";
            try
            {
                string query = $"SELECT id, user_stop_name, route_id, RFId, msg, stop_order, sys_user_id, latitude, longitude FROM bs_stop_master WHERE id = {model.id}";

                DataTable datatable = _sql_qury_execution.DML_Select(query);
                if (datatable != null && datatable.Rows.Count > 0)
                {
                    json = JsonConvert.SerializeObject(datatable, Formatting.Indented);
                    return Content(json, "application/json");
                }
                else
                {
                    return Content("-1");
                }
            }
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return Content("0");
            }
        }


        [HttpPost]
        public IActionResult editStop(Stop stop)
        {
            try
            {
                string query = $@"UPDATE bs_stop_master
                          SET 
                              user_stop_name = '{stop.Stop_Name}',
                             
                              latitude = {stop.Latitude},
                              longitude = {stop.Longitude}
                          WHERE id = {stop.id}";

                int result = _sql_qury_execution.DML_Insert_Update_Delete(query);
                if (result > 0)
                {
                    return Content("1");
                }
                else
                {
                    return Content("-1");
                }
            }
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return Content("0");
            }
        }



        [HttpPost]
        ///<summary>
        /// getAssignedStudentCount actionmethod count how many students are assigned to that stop.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult getAssignedStudentCount(stopid stop)
        {
            string json = "";
            try
            {
                #region GETALLSTOPS QUERY
                string query = $"select count(*) from bs_route_students where stop_id = {stop.id}";
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
                        return Content("-1");
                    }//datatable has no rows
                }//datatable is not null

                return Content("0");
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return Content("0");

            }//catch block ends.
        }


        [HttpPost]
        ///<summary>
        /// DeleteStopById actionmethod delete stops by id.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public bool DeleteStopById(stop_id stop)
        {
            string json = "";
            try
            {
                #region DELETE QUERY
                string query = $"delete from bs_stop_master where id = '{stop.id}' and route_id = '{stop.rid}'";

                #endregion

                int rowaffected = _sql_qury_execution.DML_Insert_Update_Delete(query);
                if (rowaffected>0)
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
        /// getAllAssignedStudents actionmethod gets all students assign to the perticular stops.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult getAllAssignedStudents(stopid stop)
        {
            string json = "";
            try
            {
                #region GETALLSTOPS QUERY
                string query = $@"select bsm.id,bsmb.student_name,bsmb.rf_id from bs_route_students  brs
                                    inner join 
                                    bs_stop_master bsm
                                    on bsm.id= brs.stop_id
                                    inner join 
                                    bs_student_master_backup bsmb
                                    on bsmb.id = brs.student_id where bsm.id = {stop.id}";
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
                        return Content("-1");
                    }//datatable has no rows
                }//datatable is not null

                return Content("0");
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return Content("0");
            }//catch block ends.
        }
    }
}
