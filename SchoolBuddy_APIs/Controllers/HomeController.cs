using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SchoolBuddy_APIs.Database_methods;
using SchoolBuddy_APIs.Models;
using SchoolBuddy_APIs.Models.Dashboard;
using SchoolBuddy_APIs.Models.Login;
using SchoolBuddy_APIs.Models.Master.Holidays_And_Events;
using SchoolBuddy_APIs.Models.Master.Students;
using System.Data;

namespace SchoolBuddy_APIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly Idatabase_access _sql_qury_execution;

        public HomeController(Idatabase_access sql_qury_execution)
        {
            _sql_qury_execution = sql_qury_execution;
        }

        [HttpPost]
        public IActionResult Login(Login login)
        {
            string traceId = HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();

            try
            {
                if (login == null)
                {
                    Log("LOGIN FAILED", traceId, "Login model null");
                    return Content("-1");
                }

                var username = login.username?.Trim();
                var password = login.password;

                Log("LOGIN START", traceId, $"Username={username}");

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    Log("LOGIN FAILED", traceId, "Username or password empty");
                    return Content("-1");
                }

                // ================= NEWTRACK =================
                string query_newtrack = $@"select id,sys_username,sys_password from newtrack.dbo.users where sys_username COLLATE SQL_Latin1_General_CP1_CS_AS = @username and sys_password COLLATE SQL_Latin1_General_CP1_CS_AS=@password";
                var parameters = new Dictionary<string, object>
{
    { "@username", username },
    { "@password", password }
};
                // ================= ATLTRACKING =================
                Log("LOGIN FALLBACK", traceId, "Checking atltracking");

                string query_atltracking =
                    $"select id,sys_username,sys_password from tbl_users where sys_username COLLATE SQL_Latin1_General_CP1_CS_AS = @username and sys_password COLLATE SQL_Latin1_General_CP1_CS_AS=@password";

                DataTable datatable = _sql_qury_execution.DML_Select(query_atltracking, parameters);

                if (datatable == null)
                {
                    Log("LOGIN ERROR", traceId, "DB returned null (newtrack)");
                    return Content("0"); // DB error
                }

                if (datatable.Rows.Count > 0)
                {
                    string userId = datatable.Rows[0]["id"]?.ToString() ?? "";

                    if (userId == "3094" || userId == "26467")
                    {
                        Log("LOGIN BLOCKED", traceId, $"UserId={userId} (atltracking)");
                        return Content("-1");
                    }

                    login_result login_Result = new login_result
                    {
                        user_id = userId,
                        database = "atltracking",
                        schoolname = datatable.Rows[0]["sys_username"]?.ToString()
                    };

                    Log("LOGIN SUCCESS", traceId, $"DB=atltracking UserId={userId}");

                    return Content(JsonConvert.SerializeObject(login_Result), "application/json");
                }

                
//                var parameters1 = new Dictionary<string, object>
//{
//    { "@username", username },
//    { "@password", password }
//};
                DataTable dt1 = _sql_qury_execution.DML_Select(query_newtrack,parameters);

                if (dt1 == null)
                {
                    Log("LOGIN ERROR", traceId, "DB returned null (newtrack)");
                    return Content("0"); // DB error
                }

                if (dt1.Rows.Count > 0)
                {
                    string userId = dt1.Rows[0]["id"]?.ToString() ?? "";

                    if (userId == "3094" || userId == "26467")
                    {
                        Log("LOGIN BLOCKED", traceId, $"UserId={userId} (newtrack)");
                        return Content("-1");
                    }

                    login_result login_Result = new login_result
                    {
                        user_id = userId,
                        database = "newtrack",
                        schoolname = dt1.Rows[0]["sys_username"]?.ToString()
                    };

                    Log("LOGIN SUCCESS", traceId, $"DB=newtrack UserId={userId}");

                    return Content(JsonConvert.SerializeObject(login_Result), "application/json");
                }

                // ================= FINAL FAIL =================
                Log("LOGIN FAILED", traceId, "User not found in both DBs");
                return Content("-1");
            }
            catch (Exception ex)
            {
                Log("LOGIN EXCEPTION", traceId, ex.ToString());
                return Content("0");
            }

        }

        // ================= LOGGER =================
        private void Log(string type, string traceId, string message)
        {
            try
            {
                string logFolder = @"D:\LoginDebugLogs";

                if (!Directory.Exists(logFolder))
                {
                    Directory.CreateDirectory(logFolder);
                }

                string logFile = Path.Combine(logFolder, $"login_{DateTime.Now:yyyyMMdd}.txt");

                System.IO.File.AppendAllText(
                    logFile,
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {type} | TraceId={traceId} | {message}{Environment.NewLine}"
                );
            }
            catch
            {
                // avoid crash due to logging failure
            }
        }
    }
}












































//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using Newtonsoft.Json;
//using SchoolBuddy_APIs.Database_methods;
//using SchoolBuddy_APIs.Models;
//using SchoolBuddy_APIs.Models.Dashboard;

//using SchoolBuddy_APIs.Models.Login;
//using SchoolBuddy_APIs.Models.Master.Students;
//using System.Data;
//using System.Data.SqlClient;
//using System.Security.Cryptography.Xml;

//namespace SchoolBuddy_APIs.Controllers
//{
//    [Route("api/[controller]/[action]")]
//    [ApiController]
//    public class HomeController : ControllerBase
//    {
//        private readonly Idatabase_access _sql_qury_execution;
//        public HomeController(Idatabase_access sql_qury_execution)
//        {
//            _sql_qury_execution = sql_qury_execution;
//        }


//        [HttpPost]

//        ///<summary>
//        /// Login actionmethod.
//        ///if database is newtrack, table used : telemetry_month.
//        ///if database is alttracking, table used : tbl_telemetry_month.
//        ///</summary>
//        public IActionResult Login(Login login)
//        {
//            try
//            {
//                var username = login.username;
//                var password = login.password;

//                string query_newtrack = $"select id,sys_username,sys_password from users where sys_username = '{username}' and sys_password='{password}'";
//                DataTable datatable = _sql_qury_execution.DML_Select(query_newtrack);

//                if (datatable != null)
//                {
//                    if (datatable.Rows.Count > 0)
//                    {
//                        string userId = datatable.Rows[0]["id"].ToString();

//                        // BLOCKED USERS
//                        if (userId == "3094" || userId == "26467")
//                        {
//                            return Content("-1");
//                        }

//                        login_result login_Result = new login_result
//                        {
//                            user_id = userId,
//                            database = "newtrack",
//                            schoolname = datatable.Rows[0]["sys_username"].ToString()
//                        };

//                        return Content(JsonConvert.SerializeObject(login_Result), "application/json");
//                    }
//                    else
//                    {
//                        string query_atltracking = $"select id,sys_username,sys_password from atltracking.dbo.tbl_users where sys_username = '{username}' and sys_password='{password}'";
//                        DataTable dt1 = _sql_qury_execution.DML_Select(query_atltracking);

//                        if (dt1 != null)
//                        {
//                            if (dt1.Rows.Count > 0)
//                            {
//                                string userId = dt1.Rows[0]["id"].ToString();

//                                // BLOCKED USERS
//                                if (userId == "3094" || userId == "26467")
//                                {
//                                    return Content("-1");
//                                }

//                                login_result login_Result = new login_result
//                                {
//                                    user_id = userId,
//                                    database = "atltracking",
//                                    schoolname = dt1.Rows[0]["sys_username"].ToString()
//                                };

//                                return Content(JsonConvert.SerializeObject(login_Result), "application/json");
//                            }
//                            else
//                            {
//                                return Content("-1");
//                            }
//                        }
//                        else
//                        {
//                            return Content("0");
//                        }
//                    }
//                }
//                else
//                {
//                    return Content("0");
//                }
//            }
//            catch (Exception ex)
//            {
//                return Content("0");
//            }
//        }
//    }
//}
