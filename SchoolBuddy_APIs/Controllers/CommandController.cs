using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolBuddy_APIs.Database_methods;
using SchoolBuddy_APIs.Models.Command;
using SchoolBuddy_APIs.Models.Master.Holidays_And_Events;
using System.Data;

namespace SchoolBuddy_APIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        private readonly Idatabase_access _sql_qury_execution;
        public CommandController(Idatabase_access sql_qury_execution)
        {
            _sql_qury_execution = sql_qury_execution;
        }

        [HttpPost]

        ///<summary>
        /// GetCommandList actionmethod fetches all command sent to the devices.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult GetCommandList(school school)
        {
            try
            {
                if (!String.IsNullOrEmpty(school.id))
                {
                    string getcommand = $@"select bm.id,r.route_name,bm.message 
                                            from bs_broadcast_msg bm
                                            inner join bs_route_master r
                                            on 
                                            r.id=bm.route_id where bm.sys_user_id = {school.id}";
                    DataTable dataTable = _sql_qury_execution.DML_Select(getcommand);
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
        /// GetCommandHistory actionmethod fetches all command 's history .
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult GetCommandHistory(school school)
        {
            try
            {
                if (!String.IsNullOrEmpty(school.id))
                {
                    string getcommand = $@"select id,sent_date,reason,is_pending from bs_broadcast_msg where id = {school.id}";
                    DataTable dataTable = _sql_qury_execution.DML_Select(getcommand);
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
        /// AddBroadcastMessage actionmethod add broadcast messages to the server .
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public bool AddBroadcastMessage(AddCommand add)
        {
            try
            {
             
            string query_newtrack = $@"insert into bs_broadcast_msg (sys_user_id, route_id, sent_date, message,reason,is_pending)
                                    values ('{add.sys_user_id}','{add.route_id}',getdate(),
                                    '{add.message}','{add.reason}',1)";

                int row_affected = _sql_qury_execution.DML_Insert_Update_Delete(query_newtrack);
                if (row_affected > 0)
                {
                    return true;

                }
                return false;


            }
            catch (Exception e)
            {
                return false;
            }
        }


    }
}
