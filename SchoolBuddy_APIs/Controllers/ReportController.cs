using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using SchoolBuddy_APIs.Database_methods;
using SchoolBuddy_APIs.Models.Dashboard;
using SchoolBuddy_APIs.Models.Indoor;
using SchoolBuddy_APIs.Models.Master.Students;
using SchoolBuddy_APIs.Models.Report;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace SchoolBuddy_APIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly Idatabase_access _sql_qury_execution;

        public ReportController(Idatabase_access sql_qury_execution) 
        {
            _sql_qury_execution = sql_qury_execution;
        }

        [HttpPost]


        ///<summary>
        /// GetNotification actionmethod fetches notifications.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult GetNotification(GetNotification notfy)
        {
            try
            {
                string user_id = notfy.user_id;
                string date = notfy.date;
                string json = "";
                StringBuilder query = new StringBuilder();
                if (user_id != null)
                {
                    DateTime inputDate = DateTime.ParseExact(date, new[] {   "yyyy-MM-dd",
        "MM-dd-yyyy",
        "dd-MM-yyyy",
        "MM/dd/yyyy",
        "dd/MM/yyyy",
        "yyyy/MM/dd" }, CultureInfo.InvariantCulture);
                    DateTime compareDate = new DateTime(2026, 8, 6);

                    if (inputDate < compareDate)
                    {
                        if (notfy.database == "newtrack")
                        {
                            query.Append($"SELECT student.student_name,student.admission_no,brm.route_name, ");
                            query.Append($"brm.id ,bum.bs_user_name as mobile_no,noti.message ,'Sent' AS status");
                            query.Append($"FROM bs_notification_parent noti left join bs_user_master bum ");
                            query.Append($"on noti.parent_id = bum.id left join bs_route_master brm ");
                            query.Append($"on noti.Route_id = brm.id left  join bs_student_master_backup student ");
                            query.Append($"on bum.id = student.bs_user_id left join users u ");
                            query.Append($"on student.sys_user_id = u.id where cast(noti.date_time as Date) = '{date}' ");
                            query.Append($"and u.Id ='{user_id}' order by student.student_name;");
                            //query.Append($"SELECT student.student_name,student.admission_no,brm.route_name, brm.id ,bum.bs_user_name as mobile_no,noti.message,    'Sent' AS status from bs_notification_parent noti inner join bs_user_master bum on noti.parent_id = bum.id inner join bs_route_master brm on noti.Route_id = brm.id inner join bs_student_master_backup student on noti.student_id = student.id inner join users u on student.sys_user_id = u.id where cast(noti.date_time as Date) ='{date}' and u.Id ='{user_id}' order by student.student_name;");
                        }
                        else
                        {
                            query.Append($"SELECT student.student_name,student.admission_no,brm.route_name, ");
                            query.Append($"brm.id ,bum.bs_user_name as mobile_no,noti.message ,'Sent' AS status");
                            query.Append($"FROM bs_notification_parent noti left join bs_user_master bum ");
                            query.Append($"on noti.parent_id = bum.id left join bs_route_master brm ");
                            query.Append($"on noti.Route_id = brm.id left  join bs_student_master_backup student ");
                            query.Append($"on bum.id = student.bs_user_id left join [atltracking].[dbo].tbl_users u ");
                            query.Append($"on student.sys_user_id = u.id where cast(noti.date_time as Date) = '{date}' ");
                            query.Append($"and u.Id ='{user_id}' order by student.student_name;");
                            //query.Append($"SELECT student.student_name,student.admission_no,brm.route_name, brm.id ,bum.bs_user_name as mobile_no,noti.message ,'Sent' AS status from bs_notification_parent noti inner join bs_user_master bum on noti.parent_id = bum.id inner join bs_route_master brm on noti.Route_id = brm.id inner join bs_student_master_backup student on noti.student_id = student.id inner join atltracking.[dbo].tbl_users u on student.sys_user_id = u.id where cast(noti.date_time as Date) ='{date}' and u.Id ='{user_id}' order by student.student_name;");
                        }
                    }
                    else
                    {
                        if (notfy.database == "newtrack")
                        {
                            //query.Append($"SELECT student.student_name,student.admission_no,brm.route_name, ");
                            //query.Append($"brm.id ,bum.bs_user_name as mobile_no,noti.message ");
                            //query.Append($"FROM bs_notification_parent noti left join bs_user_master bum ");
                            //query.Append($"on noti.parent_id = bum.id left join bs_route_master brm ");
                            //query.Append($"on noti.Route_id = brm.id left  join bs_student_master_backup student ");
                            //query.Append($"on bum.id = student.bs_user_id left join users u ");
                            //query.Append($"on student.sys_user_id = u.id where cast(noti.date_time as Date) = '{date}' ");
                            //query.Append($"and u.Id ='{user_id}' order by student.student_name;");
                            query.Append($"SELECT student.student_name,student.admission_no,brm.route_name, brm.id ,bum.bs_user_name as mobile_no,noti.message,    'Sent' AS status from bs_notification_parent noti inner join bs_user_master bum on noti.parent_id = bum.id inner join bs_route_master brm on noti.Route_id = brm.id inner join bs_student_master_backup student on noti.student_id = student.id inner join users u on student.sys_user_id = u.id where cast(noti.date_time as Date) ='{date}' and u.Id ='{user_id}' order by student.student_name;");
                        }
                        else
                        {
                            //query.Append($"SELECT student.student_name,student.admission_no,brm.route_name, ");
                            //query.Append($"brm.id ,bum.bs_user_name as mobile_no,noti.message ");
                            //query.Append($"FROM bs_notification_parent noti left join bs_user_master bum ");
                            //query.Append($"on noti.parent_id = bum.id left join bs_route_master brm ");
                            //query.Append($"on noti.Route_id = brm.id left  join bs_student_master_backup student ");
                            //query.Append($"on bum.id = student.bs_user_id left join atltracking.[dbo].tbl_users u ");
                            //query.Append($"on student.sys_user_id = u.id where cast(noti.date_time as Date) = '{date}' ");
                            //query.Append($"and u.Id ='{user_id}' order by student.student_name;");
                            query.Append($"SELECT student.student_name,student.admission_no,brm.route_name, brm.id ,bum.bs_user_name as mobile_no,noti.message ,'Sent' AS status from bs_notification_parent noti inner join bs_user_master bum on noti.parent_id = bum.id inner join bs_route_master brm on noti.Route_id = brm.id inner join bs_student_master_backup student on noti.student_id = student.id inner join atltracking.[dbo].tbl_users u on student.sys_user_id = u.id where cast(noti.date_time as Date) ='{date}' and u.Id ='{user_id}' order by student.student_name;");
                        }
                    }
                
                    

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
                            return Content("[]", "application/json");
                        }//datatable has no rows
                    }//datatable is not null

                    return Content("[]", "application/json");

                }//user is not equal to null.
                else
                {
                    return Content("[]", "application/json");
                }//user id null.
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return Content("[]", "application/json");

            }//catch block ends.

        }

        [HttpPost]



        ///<summary>
        /// GetStudentNotificationLog actionmethod fetches notifications.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult GetStudentNotificationLog(GetStudentNotificationLog notfy)
        {
            try
            {
                string user_id = notfy.user_id;
                string json = "";
                if (user_id != null)
                {
                    StringBuilder query = new StringBuilder();
                    query.Append($"SELECT student.id as student_id, student.student_name,");
                    query.Append($"student.admission_no,brm.route_name, brm.id,");
                    query.Append($"bum.bs_user_name AS mobile_no,noti.message,noti.date_time,");
                    query.Append($"noti.parent_id FROM bs_notification_parent noti ");
                    query.Append($"LEFT JOIN bs_user_master bum ON noti.parent_id = bum.id ");
                    query.Append($"LEFT JOIN bs_route_master brm ON noti.Route_id = brm.id ");
                    query.Append($"LEFT  JOIN bs_student_master_backup student ON bum.id = student.bs_user_id ");
                    query.Append($"LEFT JOIN tbl_users u ON student.sys_user_id = u.id ");
                    query.Append($"WHERE cast(noti.date_time AS Date)  between '{notfy.datefrom}' and '{notfy.dateto}' and u.Id = '{notfy.user_id}' and student.id in ({notfy.student_id}) order by student_id;");

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
        /// NoGps actionmethod fetches details of devices where no gps data is coming.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult NoGps(NoGps notfy)
        {
            try
            {
                string user_id = notfy.user_id;
                string json = "";
                if (user_id != null)
                {
             

                    StringBuilder query = new StringBuilder();
                    query.Append($"SELECT s.veh_reg as Vehicle, dateadd(minute,-330,t.gps_time) as last_updated ");
                    query.Append($"FROM tbl_latest_telemetry t, services s ");
                    query.Append($"WHERE datediff(day,t.gps_time, dateadd(minute,-330,GETDATE()))>=1 ");
                    query.Append($"and t.sys_service_id in (select id from services where sys_user_id='{user_id}') and t.sys_service_id=s.id");
                    

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
        /// LoginReport actionmethod fetches login reports of users.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult LoginReport(LoginReport notfy)
        {
            try
            {
                string user_id = notfy.user_id;
                string json = "";
                string db = notfy.database;
                string servicesTable;

                if (string.Equals(
                    db,
                    "newtrack",
                    StringComparison.OrdinalIgnoreCase))
                {
                    servicesTable = "services";
                }
                else
                {
                    servicesTable = "atltracking.dbo.tbl_services";
                }

                if (user_id != null)
                {


                    StringBuilder query = new StringBuilder();
                    query.Append($"select s.bs_user_id, s.student_name,s.admission_no,c.class_name ,s.section as class,");
                    query.Append($"p.login_time,p.source from bs_student_master_backup s inner join ");
                    query.Append($"bs_parent_login_log p on p.bs_user_id = s.bs_user_id inner join bs_class_master c on c.id = s.class ");
                    query.Append($"inner join bs_user_master u on u.id = p.bs_user_id ");
                    query.Append($"where p.login_time between  '{Convert.ToDateTime(notfy.date).ToString("yyyy-MM-dd")}'");
                    query.Append($"and '{Convert.ToDateTime(notfy.date).AddDays(1.0).ToString("yyyy-MM-dd")}' and u.sys_user_id={user_id} order by s.bs_user_id");






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
        /// RfidReport actionmethod fetches students who puches RF-card.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult RfidReport(LoginReport notfy)
        {
            try
            {
                string user_id = notfy.user_id;
                string json = "";
                if (user_id != null)
                {
                    StringBuilder query = new StringBuilder();
                    //query.Append($"select tbl1.route_name, tbl1.total,isnull(tbl2.punched, 0) as punched,");
                    //query.Append($"(tbl1.total - isnull(tbl2.punched, 0)) as not_punched ,tbl3.ids, tbl4.punched_ids,");
                    //query.Append($"(select count(distinct r.rfid) from rf_punch_history r where r.sys_service_id in");
                    //query.Append($"(select distinct sys_service_id from bs_route_master where rtrim(ltrim(REPLACE(REPLACE(route_name, 'pick', ''), 'drop', '')))");
                    //query.Append($"= rtrim(ltrim(REPLACE(REPLACE(tbl1.route_name, 'pick', ''), 'drop', '')))");
                    //query.Append($"and sys_user_id = \" + uid + @\") and(student_id not  in ");
                    //query.Append($"(select student_id from bs_route_students where route_id in");
                    //query.Append($"(select  id from bs_route_master where rtrim");
                    //query.Append($"(ltrim(REPLACE(REPLACE(route_name, 'pick', ''), 'drop', '')))");
                    //query.Append($"= rtrim(ltrim(REPLACE(REPLACE(tbl1.route_name, 'pick', ''),");
                    //query.Append($" 'drop', '')))  and sys_user_id = \" + uid + @\"))");
                    //query.Append($"and r.student_id not in (select student_id from bs_route_students ");
                    //query.Append($"where route_id in (select id from bs_route_master where sys_service_id in");
                    //query.Append($"((select distinct sys_service_id from bs_route_master where ");
                    //query.Append($"rtrim(ltrim(REPLACE(REPLACE(route_name, 'pick', ''), 'drop', '')))");
                    //query.Append($"= rtrim(ltrim(REPLACE(REPLACE(tbl1.route_name, 'pick', ''), 'drop', '')))");
                    //query.Append($"and sys_user_id = \" + uid + @\"))))  or student_id is null) ");
                    //query.Append($"and punch_gps_time between  '\" + fdate + \"' and '\" + Convert.ToDateTime(fdate).AddDays(1.0).ToString(\"yyyy-MM-dd\") + @\"') as other from");
                    //query.Append($"(select distinct  rtrim(ltrim(REPLACE(REPLACE(route_name, 'pick', ''), 'drop', ''))) as route_name,");
                    //query.Append($"count(distinct rs.student_id) as total");
                    //query.Append($"from bs_route_master rm inner join bs_route_students rs on rs.route_id = rm.id");
                    //query.Append($"where rm.sys_user_id = \" + uid + @\"");
                    //query.Append($"group by rtrim(ltrim(REPLACE(REPLACE(route_name, 'pick', ''), 'drop', '')))) as tbl1");
                    //query.Append($" left join(select rtrim(ltrim(REPLACE(REPLACE(rm.route_name,'pick',''),'drop','')))");
                    //query.Append($" as route_name, count(distinct rf.student_id) as punched\r\n");
                    //query.Append($"from rf_punch_history rf right join bs_route_master rm on rm.sys_service_id = rf.sys_service_id inner");
                    //query.Append($" join bs_student_master_backup s on s.id = rf.student_id inner");
                    //query.Append($"join bs_route_students rs on rs.route_id = rm.Id and s.id = rs.student_id");
                    //query.Append($"where rm.sys_user_id = \" + uid + @\" and punch_gps_time ");
                    //query.Append($"between '\" + fdate + \"' and '\" + Convert.ToDateTime(fdate).AddDays(1.0).ToString(\"yyyy-MM-dd\") + @\"'");
                    //query.Append($"group by  rtrim(ltrim(REPLACE(REPLACE(rm.route_name, 'pick', ''), 'drop', '')))) tbl2");
                    //query.Append($"on rtrim(ltrim(tbl2.route_name)) = rtrim(ltrim(tbl1.route_name))");
                    //query.Append($"left join(select distinct  rtrim(ltrim(REPLACE(REPLACE(route_name,'pick',''),'drop',''))) as route_name,");
                    //query.Append($"(select distinct CAST(student_id as varchar) +',' as 'data()'  from bs_route_students");
                    //query.Append($"where route_id in (select id from bs_route_master where");
                    //query.Append($"rtrim(ltrim(REPLACE(REPLACE(route_name, 'pick', ''), 'drop', ''))) = rtrim(ltrim(REPLACE(REPLACE(rm.route_name, 'pick', ''), 'drop', '')))");
                    //query.Append($"and sys_user_id = \" + uid + @\")for xml path('')) as ids from bs_route_master rm");
                    //query.Append($"where rm.sys_user_id = \" + uid + @\") tbl3");
                    //query.Append($"on rtrim(ltrim(tbl3.route_name)) = rtrim(ltrim(tbl1.route_name))");
                    //query.Append($"inner join(select distinct rtrim(ltrim(REPLACE(REPLACE(rm.route_name,'pick',''),'drop',''))) as route_name,");
                    //query.Append($"(select distinct CAST(p.student_id as varchar) +',' as 'data()'  from rf_punch_history p");
                    //query.Append($"left join bs_student_master_backup s on s.id = p.student_id");
                    //query.Append($"inner  join bs_route_students rs on rs.student_id = s.Id");
                    //query.Append($"inner join bs_route_master r on r.Id = rs.route_id and p.sys_service_id = r.sys_service_id");
                    //query.Append($" where r.sys_user_id = \" + uid + @\"");
                    //query.Append($"and punch_gps_time between '\" + fdate + \"' and ");
                    //query.Append($"'\" + Convert.ToDateTime(fdate).AddDays(1.0).ToString(\"yyyy-MM-dd\") + @\"'");
                    //query.Append($"and rtrim(ltrim(REPLACE(REPLACE(r.route_name,'pick',''),'drop',''))) = ");
                    //query.Append($"rtrim(ltrim(REPLACE(REPLACE(rm.route_name, 'pick', ''), 'drop', '')))");
                    //query.Append($"for xml path(''))as punched_ids  from rf_punch_history rf right");
                    //query.Append($"join bs_route_master rm on rm.sys_service_id = rf.sys_service_id");
                    //query.Append($"where rm.sys_user_id = \" + uid + @\") tbl4");
                    //query.Append($" on rtrim(ltrim(tbl4.route_name)) = rtrim(ltrim(tbl1.route_name))");

                    //query.Append(@"select tbl1.route_name, tbl1.total,isnull(tbl2.punched,0) as punched,(tbl1.total - isnull(tbl2.punched,0)) as not_punched ,tbl3.ids, tbl4.punched_ids,
                    //    (select count(distinct r.rfid) from rf_punch_history r where r.sys_service_id in
                    //    (select distinct sys_service_id from bs_route_master where  rtrim(ltrim(REPLACE(REPLACE(route_name,'pick',''),'drop','')))
                    //    = rtrim(ltrim(REPLACE(REPLACE(tbl1.route_name,'pick',''),'drop','')))
                    //    and sys_user_id=" + user_id + @") and (student_id not  in (select student_id from bs_route_students where route_id in
                    //    (select  id from bs_route_master where  rtrim(ltrim(REPLACE(REPLACE(route_name,'pick',''),'drop',''))) 
                    //    = rtrim(ltrim(REPLACE(REPLACE(tbl1.route_name,'pick',''),'drop','')))  and sys_user_id=" + user_id + @"))
                    //    and r.student_id not in (select student_id from bs_route_students where route_id in (select id from bs_route_master where sys_service_id in
                    //    ((select distinct sys_service_id from bs_route_master where  
                    //    rtrim(ltrim(REPLACE(REPLACE(route_name,'pick',''),'drop',''))) 
                    //    = rtrim(ltrim(REPLACE(REPLACE(tbl1.route_name,'pick',''),'drop','')))
                    //    and sys_user_id = " + user_id + @"))))  or student_id is null) and punch_gps_time between  '" + notfy.date + "' and '" + Convert.ToDateTime(notfy.date).AddDays(1.0).ToString("yyyy-MM-dd") + @"') as other from 
                    //    (select distinct  rtrim(ltrim(REPLACE(REPLACE(route_name,'pick',''),'drop',''))) as route_name,
                    //    count(distinct rs.student_id) as total
                    //    from bs_route_master rm 
                    //    inner join bs_route_students rs on rs.route_id=rm.id
                    //    where rm.sys_user_id=" + user_id + @"
                    //    group by  rtrim(ltrim(REPLACE(REPLACE(route_name,'pick',''),'drop','')))) as tbl1
                    //    left join (select rtrim(ltrim(REPLACE(REPLACE(rm.route_name,'pick',''),'drop',''))) as route_name, count(distinct rf.student_id) as punched
                    //    from rf_punch_history rf right join 
                    //    bs_route_master rm on rm.sys_service_id = rf.sys_service_id
                    //    inner join bs_student_master_backup s on s.id= rf.student_id
                    //    inner join bs_route_students rs on rs.route_id=rm.Id and s.id= rs.student_id
                    //    where rm.sys_user_id=" + user_id + @" and punch_gps_time between '" + notfy.date + "' and '" + Convert.ToDateTime(notfy.date).AddDays(1.0).ToString("yyyy-MM-dd") + @"'
                    //    group by  rtrim(ltrim(REPLACE(REPLACE(rm.route_name,'pick',''),'drop','')))) tbl2
                    //    on rtrim(ltrim(tbl2.route_name)) = rtrim(ltrim(tbl1.route_name))
                    //    left join (select distinct  rtrim(ltrim(REPLACE(REPLACE(route_name,'pick',''),'drop',''))) as route_name,
                    //    (select distinct CAST(student_id as varchar) +',' as 'data()'  from bs_route_students 
                    //    where route_id in (select id from bs_route_master where 
                    //    rtrim(ltrim(REPLACE(REPLACE(route_name,'pick',''),'drop',''))) = rtrim(ltrim(REPLACE(REPLACE(rm.route_name,'pick',''),'drop',''))) 
                    //    and sys_user_id=" + user_id + @") 
                    //    for xml path('')) as ids
                    //    from bs_route_master rm 
                    //    where rm.sys_user_id=" + user_id + @") tbl3
                    //    on rtrim(ltrim(tbl3.route_name))=rtrim(ltrim(tbl1.route_name))
                    //    inner join (select distinct rtrim(ltrim(REPLACE(REPLACE(rm.route_name,'pick',''),'drop',''))) as route_name,
                    //    (select distinct CAST(p.student_id as varchar) +',' as 'data()'  from rf_punch_history p
                    //    left join bs_student_master_backup s on s.id=p.student_id
                    //    inner join bs_route_students rs on rs.student_id=s.Id
                    //    inner join bs_route_master r on r.Id=rs.route_id and p.sys_service_id=r.sys_service_id
                    //    where r.sys_user_id=" + user_id + @"
                    //    and punch_gps_time between '" + notfy.date + "' and '" + Convert.ToDateTime(notfy.date).AddDays(1.0).ToString("yyyy-MM-dd") + @"'
                    //    and rtrim(ltrim(REPLACE(REPLACE(r.route_name,'pick',''),'drop',''))) = rtrim(ltrim(REPLACE(REPLACE(rm.route_name,'pick',''),'drop','')))
                    //    for xml path(''))as punched_ids
                    //    from rf_punch_history rf right join 
                    //    bs_route_master rm on rm.sys_service_id = rf.sys_service_id
                    //    where rm.sys_user_id=" + user_id + @") tbl4 
                    //    on rtrim(ltrim(tbl4.route_name))=rtrim(ltrim(tbl1.route_name))");

                    string db = notfy.database;
                    string servicesTable;

                    if (string.Equals(
                        db,
                        "newtrack",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        servicesTable = "services";
                    }
                    else
                    {
                        servicesTable = "atltracking.dbo.tbl_services";
                    }
                    query.Append($@"
DECLARE @UserId INT = {user_id};
DECLARE @PunchDate DATE = '{Convert.ToDateTime(notfy.date):yyyy-MM-dd}';

DECLARE @FromDate DATETIME = CAST(@PunchDate AS DATETIME);
DECLARE @ToDate DATETIME = DATEADD(DAY, 1, @FromDate);

;WITH RouteMaster AS
(
    SELECT
        rm.id AS route_id,
        rm.sys_service_id,

        LTRIM
        (
            RTRIM
            (
                REPLACE
                (
                    REPLACE
                    (
                        UPPER(ISNULL(rm.route_name, '')),
                        'PICK',
                        ''
                    ),
                    'DROP',
                    ''
                )
            )
        ) AS route_name,

        rm.route_name AS original_route_name,
        rm.start_time_up,
        rm.end_time_up,

        CASE
            WHEN UPPER(ISNULL(rm.route_name, '')) LIKE '%PICK%'
                THEN 'Pick'

            WHEN UPPER(ISNULL(rm.route_name, '')) LIKE '%DROP%'
                THEN 'Drop'

            ELSE 'Unknown'
        END AS movement_type

    FROM bs_route_master rm

    WHERE rm.sys_user_id = @UserId
),

RouteStudents AS
(
    SELECT DISTINCT
        rm.route_name,
        rm.route_id,
        rm.sys_service_id,
        rm.movement_type,
        rs.student_id

    FROM RouteMaster rm

    INNER JOIN bs_route_students rs
        ON rs.route_id = rm.route_id
),

RouteTotal AS
(
    SELECT
        route_name,
        COUNT(DISTINCT student_id) AS total

    FROM RouteStudents

    GROUP BY route_name
),

DatePunches AS
(
    SELECT
        rf.id AS punch_id,
        rf.rfid,

        COALESCE
        (
            rf.student_id,
            sm.id
        ) AS student_id,

        rf.sys_service_id,
        rf.sys_proc_time,
        rf.punch_gps_time

    FROM rf_punch_history rf

    INNER JOIN {servicesTable} s
        ON s.id = rf.sys_service_id
       AND s.sys_user_id = @UserId

    LEFT JOIN bs_student_master_backup sm
        ON sm.rf_id = rf.rfid
       AND sm.sys_user_id = @UserId

    WHERE rf.punch_gps_time >= @FromDate
      AND rf.punch_gps_time < @ToDate
),

RouteCandidates AS
(
    SELECT
        dp.punch_id,
        dp.rfid,
        dp.student_id,
        dp.sys_service_id,
        dp.sys_proc_time,
        dp.punch_gps_time,

        rm.route_id,
        rm.route_name,
        rm.original_route_name,
        rm.movement_type,
        rm.start_time_up,
        rm.end_time_up,

        CASE
            WHEN rs.student_id IS NOT NULL
                THEN 0
            ELSE 1
        END AS student_route_priority,

        CASE
            WHEN TRY_CONVERT(TIME, dp.sys_proc_time)
                 BETWEEN TRY_CONVERT(TIME, rm.start_time_up)
                     AND TRY_CONVERT(TIME, rm.end_time_up)
                THEN 0

            ELSE 1
        END AS outside_time_range,

        CASE
            WHEN TRY_CONVERT(TIME, rm.start_time_up) IS NULL
              OR TRY_CONVERT(TIME, rm.end_time_up) IS NULL
                THEN 999999

            WHEN TRY_CONVERT(TIME, dp.sys_proc_time)
                 BETWEEN TRY_CONVERT(TIME, rm.start_time_up)
                     AND TRY_CONVERT(TIME, rm.end_time_up)
                THEN 0

            WHEN TRY_CONVERT(TIME, dp.sys_proc_time)
                 < TRY_CONVERT(TIME, rm.start_time_up)
                THEN ABS
                (
                    DATEDIFF
                    (
                        MINUTE,
                        TRY_CONVERT(TIME, dp.sys_proc_time),
                        TRY_CONVERT(TIME, rm.start_time_up)
                    )
                )

            ELSE ABS
            (
                DATEDIFF
                (
                    MINUTE,
                    TRY_CONVERT(TIME, rm.end_time_up),
                    TRY_CONVERT(TIME, dp.sys_proc_time)
                )
            )
        END AS route_time_difference

    FROM DatePunches dp

    INNER JOIN RouteMaster rm
        ON rm.sys_service_id = dp.sys_service_id

    LEFT JOIN bs_route_students rs
        ON rs.route_id = rm.route_id
       AND rs.student_id = dp.student_id
),

BestRoute AS
(
    SELECT
        *,

        ROW_NUMBER() OVER
        (
            PARTITION BY punch_id

            ORDER BY
                student_route_priority ASC,
                outside_time_range ASC,
                route_time_difference ASC,
                route_id ASC
        ) AS route_rn

    FROM RouteCandidates
),

AssignedPunches AS
(
    SELECT
        br.punch_id,
        br.route_name,
        br.route_id,
        br.movement_type,
        br.student_id,
        br.rfid,
        br.sys_proc_time,
        br.punch_gps_time,

        CASE
            WHEN br.student_id IS NOT NULL
             AND EXISTS
             (
                 SELECT 1
                 FROM RouteStudents rs
                 WHERE rs.route_id = br.route_id
                   AND rs.student_id = br.student_id
             )
                THEN 1

            ELSE 0
        END AS is_assigned_student

    FROM BestRoute br

    WHERE br.route_rn = 1
),

ValidPunches AS
(
    SELECT DISTINCT
        route_name,
        student_id,
        rfid

    FROM AssignedPunches

    WHERE is_assigned_student = 1
      AND student_id IS NOT NULL
),

PunchedSummary AS
(
    SELECT
        route_name,

        COUNT(DISTINCT student_id) AS punched,

        COUNT(DISTINCT rfid) AS distinct_rfid_punched

    FROM ValidPunches

    GROUP BY route_name
),

OtherPunches AS
(
    SELECT
        route_name,
        COUNT(DISTINCT rfid) AS other

    FROM AssignedPunches

    WHERE is_assigned_student = 0
      AND NULLIF(LTRIM(RTRIM(ISNULL(rfid, ''))), '') IS NOT NULL

    GROUP BY route_name
),

StudentIds AS
(
    SELECT
        r.route_name,

        STUFF
        (
            (
                SELECT DISTINCT
                    ',' + CAST(rs2.student_id AS VARCHAR(20))

                FROM RouteStudents rs2

                WHERE rs2.route_name = r.route_name

                FOR XML PATH(''), TYPE
            ).value('.', 'VARCHAR(MAX)'),
            1,
            1,
            ''
        ) AS ids

    FROM
    (
        SELECT DISTINCT route_name
        FROM RouteStudents
    ) r
),

PunchedStudentIds AS
(
    SELECT
        r.route_name,

        STUFF
        (
            (
                SELECT DISTINCT
                    ',' + CAST(vp2.student_id AS VARCHAR(20))

                FROM ValidPunches vp2

                WHERE vp2.route_name = r.route_name

                FOR XML PATH(''), TYPE
            ).value('.', 'VARCHAR(MAX)'),
            1,
            1,
            ''
        ) AS punched_ids

    FROM
    (
        SELECT DISTINCT route_name
        FROM ValidPunches
    ) r
)

SELECT
    rt.route_name,

    rt.total,

    ISNULL
    (
        ps.punched,
        0
    ) AS punched,

    ISNULL
    (
        ps.distinct_rfid_punched,
        0
    ) AS distinct_rfid_punched,

    CASE
        WHEN rt.total - ISNULL(ps.punched, 0) < 0
            THEN 0
        ELSE rt.total - ISNULL(ps.punched, 0)
    END AS not_punched,

    ISNULL
    (
        si.ids,
        ''
    ) AS ids,

    ISNULL
    (
        psi.punched_ids,
        ''
    ) AS punched_ids,

    ISNULL
    (
        op.other,
        0
    ) AS other

FROM RouteTotal rt

LEFT JOIN PunchedSummary ps
    ON ps.route_name = rt.route_name

LEFT JOIN StudentIds si
    ON si.route_name = rt.route_name

LEFT JOIN PunchedStudentIds psi
    ON psi.route_name = rt.route_name

LEFT JOIN OtherPunches op
    ON op.route_name = rt.route_name

ORDER BY rt.route_name

OPTION(RECOMPILE);
");
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
        /// StopVoilationReport actionmethod fetches voilations record.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult StopVoilationReport(StopVoilation notfy)
        {
            //general gen = new general();
            //DataTable dt = new DataTable();

            //dt.Columns.Add("route_id", typeof(int));
            //dt.Columns.Add("route_name", typeof(string));
            //dt.Columns.Add("stop_name", typeof(string));
            //dt.Columns.Add("count", typeof(int));
            //foreach (int route in notfy.route)
            //{
            //    string query = "select sys_service_id, start_time_up,end_time_up,route_name from bs_route_master where id= " + route +"";
            //    DataTable data = _sql_qury_execution.DML_Select(query);

            //    WebClient wc = new WebClient();
            //    string start_time = "";
            //    string end_time = "";
            //    string service_id = "";
            //    if (data != null)
            //    {
            //        if (data.Rows.Count > 0)
            //        {
            //            DataRow dt_row = dt.NewRow();
            //            dt_row["route_id"] = route;
            //            dt_row["route_name"] = data.Rows[0]["route_name"].ToString();
            //            dt_row["count"] = 0;
            //            start_time = notfy.date + " " + data.Rows[0]["start_time_up"].ToString();
            //            end_time = notfy.date + " " + data.Rows[0]["end_time_up"].ToString();
            //            service_id = data.Rows[0]["sys_service_id"].ToString();
            //            string route_data = wc.DownloadString("http://fasttracksoft.us/api_v2/abctraq/GetplaybackData.php?did=" + service_id + "&sdate=" + start_time + "&edate=" + end_time);

            //            route_data = route_data.Replace("\\\"", "\"").Trim();
            //            try
            //            {
            //                string query1 = "select sys_service_id, start_time_up,end_time_up,route_name from bs_route_master where id= " + route + "";

            //                List<routedata> new_route_data = JsonConvert.DeserializeObject<List<routedata>>(route_data);


            //                DataTable stop_table = _sql_qury_execution.DML_Select(query1);

            //                if (stop_table != null)
            //                {

            //                    if (stop_table.Rows.Count > 0)
            //                    {

            //                        stop_table.Columns.Add("STA", typeof(string));
            //                        string sLat, sLng, elat, eLng;
            //                        routedata[] data_new = new_route_data.ToArray();
            //                        int last = 0;
            //                        double dis;
            //                        for (int i = 0; i < stop_table.Rows.Count; i++)
            //                        {

            //                            sLat = stop_table.Rows[i]["latitude"].ToString();

            //                            sLng = stop_table.Rows[i]["longitude"].ToString();

            //                            if (string.IsNullOrEmpty(sLat) || string.IsNullOrEmpty(sLat))
            //                            {

            //                                continue;

            //                            }

            //                            for (int j = last; j < data_new.Length; j++)
            //                            {

            //                                elat = data_new[j].lat.ToString();
            //                                eLng = data_new[j].lng.ToString();

            //                                dis = gen.GetDistance1(sLat, sLng, elat, eLng);

            //                                if (dis < 150)
            //                                {
            //                                    stop_table.Rows[i]["STA"] = data_new[j].date.TimeOfDay.ToString();
            //                                    last = j;
            //                                    break;
            //                                }


            //                            }

            //                            //  dt.Rows[i]["STA"] = 

            //                        }

            //                        foreach (DataRow stop_row in stop_table.Rows)
            //                        {
            //                            string sta = stop_row["STA"].ToString();
            //                            if (string.IsNullOrEmpty(sta))
            //                            {

            //                                dt_row["stop_name"] += stop_row["user_stop_name"].ToString() + "<>";
            //                                dt_row["count"] = Convert.ToInt32(dt_row["count"]) + 1;

            //                            }


            //                        }



            //                    }


            //                }

            //                dt.Rows.Add(dt_row);
            //            }
            //            catch { }

            //        }
            //    }

            //}

            return Content("", "application/json");

        }


        [HttpPost]

        ///<summary>
        /// UhfReport actionmethod fetches Uhf record.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult UhfReport(UhfReport notfy)
        {

            try
            {
                string user_id = notfy.user_id;
                string json = "";
                if (user_id != null)
                {
                    StringBuilder query = new StringBuilder();
                    query.Append($"select sm.student_name, dateadd(minute,330, punch_time)");
                    query.Append($"as punch_time,in_out from uhf_punch_history uph inner join");
                    query.Append($"bs_student_master_backup sm on");
                    query.Append($"sm.id=uph.student_id where sm.sys_user_id= '{user_id}' ");
                    query.Append($"and punch_time between '{notfy.date}' and ");
                    query.Append($"Convert.ToDateTime({notfy.date}).AddDays(1).ToString(\"yyyy-MM-dd\") + \"'\"))");
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
        public IActionResult Attendence([FromBody]LoginReport notfy)
        {

            try
            {
                string user_id = notfy.user_id;
                string json = "";
                DateTime FromDateTime = Convert.ToDateTime(notfy.from);
                DateTime ToDateTime = Convert.ToDateTime(notfy.to);
                string from = FromDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                string to = ToDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                string db = notfy.database;
                string servicesTable;

                if (string.Equals(
                    db,
                    "newtrack",
                    StringComparison.OrdinalIgnoreCase))
                {
                    servicesTable = "services";
                }
                else
                {
                    servicesTable = "atltracking.dbo.tbl_services";
                }
                if (user_id != null)
                {
                    string query = @$"
;WITH FirstAttendance AS
(
    SELECT
        bsmb.admission_no,
        bsmb.student_name,
        bsmb.class as class_name,
        bsmb.section,
        bsmb.father_name,
        bsmb.mobile_no1,

        r.student_id,
        r.rfid,
        r.sys_service_id,
        r.punch_gps_time,
        s.veh_reg,

        ROW_NUMBER() OVER
        (
            PARTITION BY
                CASE
                    WHEN r.student_id IS NOT NULL
                        THEN 'STUDENT_' + CAST(r.student_id AS VARCHAR(50))
                    ELSE
                        'RFID_' + ISNULL(CAST(r.rfid AS VARCHAR(100)), 'UNKNOWN')
                END
            ORDER BY r.punch_gps_time ASC
        ) AS RowNo

    FROM rf_punch_history r

    INNER JOIN {servicesTable} s
        ON s.id = r.sys_service_id

    LEFT JOIN bs_student_master_backup bsmb
        ON bsmb.id = r.student_id

    WHERE s.sys_user_id = {user_id}
  
  AND r.sys_service_id IS NOT NULL
  AND r.sys_proc_time >= '{from}'
  AND r.sys_proc_time <= '{to}'
)

SELECT
    ISNULL(admission_no, '') AS admission_no,

    CASE
        WHEN student_id IS NULL THEN 'Unknown Student'
        ELSE ISNULL(student_name, '')
    END AS student_name,

    ISNULL(class_name, '') AS class_name,
    ISNULL(section, '') AS section,
    ISNULL(father_name, '') AS father_name,
    ISNULL(mobile_no1, '') AS mobile_no1,

    student_id,
    rfid,
    sys_service_id,

    CONVERT(VARCHAR(10), punch_gps_time, 105) AS AttendanceDate,
    CONVERT(VARCHAR(8), punch_gps_time, 108) AS AttendanceTime,

    veh_reg,

    CASE
        WHEN student_id IS NULL THEN 'Unknown Card'
        WHEN student_name IS NULL THEN 'Student Data Missing'
        ELSE 'Present'
    END AS AttendanceStatus

FROM FirstAttendance
WHERE RowNo = 1
ORDER BY punch_gps_time ASC;
";
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








    }
}
