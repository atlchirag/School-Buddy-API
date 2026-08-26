using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolBuddy_APIs.Database_methods;
using SchoolBuddy_APIs.Models.App;
using SchoolBuddy_APIs.Models.Master.Route;
using SchoolBuddy_APIs.Models.Report;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace SchoolBuddy_APIs.Controllers
{
    [Route("[controller]/[Action]")]
    [ApiController]
    public class appController : ControllerBase
    {
        private readonly Idatabase_access _sql_qury_execution;

        public appController(Idatabase_access sql_qury_execution)
        {
            _sql_qury_execution = sql_qury_execution;
        }

        [HttpPost]

        ///<summary>
        /// login actionmethod having login functionality from APP.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        ///
        public IActionResult login([FromQuery] string username, [FromQuery] string password, [FromQuery] string? firebaseToken_android, [FromQuery] string? firebaseToken_Ios)
        {
            try
            {
                string database = "";
                string query_to_insert_login_details = "";

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return Content(JsonConvert.SerializeObject(new { status = "0", msg = "fail" }), "application/json");
                }

                // MAIN LOGIN QUERY
                string query = $@"
            select bum.id as id, bum.sys_user_id as schoolId, bum.bs_user_name as contact,
                   bsmb.father_name as fatherName, bsmb.mother_name as motherName,
                   bsmb.street as address, bsmb.email
            from bs_user_master bum
            inner join bs_student_master_backup bsmb 
                on bum.bs_user_name = bsmb.mobile_no1 
            where bum.bs_user_name = '{username}' and bum.bs_password = '{password}'";

                DataTable dt = _sql_qury_execution.DML_Select(query);

                if (dt == null || dt.Rows.Count == 0)
                {
                    return Content(JsonConvert.SerializeObject(new { status = "-1", msg = "fail" }), "application/json");
                }

                // Extracted early for performance
                DataRow row = dt.Rows[0];
                string userId = row["id"].ToString();
                string schoolId = row["schoolId"].ToString();

                // BLOCKED SCHOOL USERS
                if (schoolId == "3094" || schoolId == "26467")
                {
                    return Content(JsonConvert.SerializeObject(new { status = "-1", msg = "fail" }), "application/json");
                }

                // CHECK IN NEWTRACK USERS
                string query_newtrack = $"select id,sys_username from users where id = '{schoolId}'";
                DataTable dt_newtrack = _sql_qury_execution.DML_Select(query_newtrack);

                bool isNewtrack = dt_newtrack != null && dt_newtrack.Rows.Count > 0;

                if (isNewtrack)
                {
                    database = "newtrack";    
                }
                else
                {
                    // CHECK IN ATLTRACKING
                    string query_atltracking1 = $"select id,sys_username from atltracking.dbo.tbl_users where id = '{schoolId}'";
                    DataTable dt_at = _sql_qury_execution.DML_Select(query_atltracking1);

                    if (dt_at == null || dt_at.Rows.Count == 0)
                    {
                        return Content(JsonConvert.SerializeObject(new { status = "-1", msg = "fail" }), "application/json");
                    }

                    database = "atltracking";
                }

                // INSERT LOGIN LOG + UPDATE TOKEN (optimised)
                string fcm = !string.IsNullOrWhiteSpace(firebaseToken_android)
                                ? firebaseToken_android
                                : firebaseToken_Ios;

                if (!string.IsNullOrWhiteSpace(fcm))
                {
                    query_to_insert_login_details =
                        $"insert into bs_parent_login_log (bs_user_id,login_time,source,fcm) " +
                        $"values ('{userId}',getdate(),'APP','{fcm}')";

                    string updateToken =
                        $"update bs_user_master set auid='{firebaseToken_android}', iuid='{firebaseToken_Ios}' where id={userId}";

                    _sql_qury_execution.DML_Insert_Update_Delete(query_to_insert_login_details);
                    _sql_qury_execution.DML_Insert_Update_Delete(updateToken);
                }

                // RESPONSE OBJECT (unchanged)
                login login1 = new login
                {
                    id = userId,
                    schoolId = schoolId,
                    contact = row["contact"].ToString(),
                    fatherName = row["fatherName"].ToString(),
                    motherName = row["motherName"].ToString(),
                    email = row["email"].ToString(),
                    address = row["address"].ToString(),
                    database = database
                };

                api_response_app_variables res = new api_response_app_variables
                {
                    status = "1",
                    msg = "success",
                    response = login1
                };

                return Content(JsonConvert.SerializeObject(res), "application/json");
            }
            catch
            {
                return Content("0");
            }
        }



        //public IActionResult login([FromQuery] string username, [FromQuery] string password, [FromQuery] string? firebaseToken_android, [FromQuery] string? firebaseToken_Ios)
        //{

        //    try
        //    {
        //        string database = "";
        //        string query = $"select bum.id as id,bum.sys_user_id as schoolId,bum.bs_user_name as contact,bsmb.father_name as fatherName,bsmb.mother_name as motherName,bsmb.street as address,bsmb.email  from bs_user_master  bum inner join bs_student_master_backup bsmb on bum.bs_user_name = bsmb.mobile_no1 where bum.bs_user_name = '{username}' and bum.bs_password = '{password}'";
        //        string query_to_insert_login_details = "";
        //        int rowAffected = 0;
        //        DataTable dt = _sql_qury_execution.DML_Select(query);
        //        if (dt != null)
        //        {
        //            if (dt.Rows.Count > 0)
        //            {
        //                string query_newtrack = $"select id,sys_username from users where id = '{dt.Rows[0]["schoolId"]}'";
        //                DataTable dataTable = _sql_qury_execution.DML_Select(query_newtrack);
        //                if (dataTable != null)
        //                {
        //                    if (dataTable.Rows.Count > 0)
        //                    {
        //                       // if (dataTable.Rows[0]["sys_username"].ToString().Contains("queen_valley"))
        //                        {
        //                            database = "newtrack";
        //                            string query_To_update_firebase_Token = $"update bs_user_master set auid='{firebaseToken_android}',iuid='{firebaseToken_Ios}' where id = {dt.Rows[0]["id"]}";
        //                            if (firebaseToken_android != null || firebaseToken_android != "")
        //                            {
        //                                query_to_insert_login_details = $"insert into  bs_parent_login_log (bs_user_id,login_time,source,fcm) values ('{dt.Rows[0]["id"]}',getdate(),'Android','{firebaseToken_android}')";

        //                            }
        //                            else if (firebaseToken_Ios != null || firebaseToken_Ios != "")
        //                            {
        //                                query_to_insert_login_details = $"insert into bs_parent_login_log (bs_user_id,login_time,source,fcm) values ('{dt.Rows[0]["id"]}',getdate(),'IOS','{firebaseToken_Ios}')";
        //                            }
        //                            _sql_qury_execution.DML_Insert_Update_Delete(query_to_insert_login_details);
        //                            _sql_qury_execution.DML_Insert_Update_Delete(query_To_update_firebase_Token);

        //                            login login1 = new login
        //                            {
        //                                id = dt.Rows[0]["id"].ToString(),
        //                                schoolId = dt.Rows[0]["schoolId"].ToString(),
        //                                contact = dt.Rows[0]["contact"].ToString(),
        //                                fatherName = dt.Rows[0]["fatherName"].ToString(),
        //                                motherName = dt.Rows[0]["motherName"].ToString(),
        //                                email = dt.Rows[0]["email"].ToString(),
        //                                address = dt.Rows[0]["address"].ToString(),
        //                                database = database
        //                            };
        //                            api_response_app_variables arav_ = new api_response_app_variables
        //                            {
        //                                status = "1",
        //                                msg = "success",
        //                                response = login1
        //                            };

        //                            return Content(JsonConvert.SerializeObject(arav_), "application/json");
        //                        }
        //                        //else
        //                        //{
        //                        //    string query_atltracking1 = $"select id,sys_username from atltracking.dbo.tbl_users where id = '{dt.Rows[0]["schoolId"]}'";
        //                        //    DataTable dataTable1 = _sql_qury_execution.DML_Select(query_atltracking1);
        //                        //    if (dataTable1 != null)
        //                        //    {
        //                        //        if (dataTable1.Rows.Count > 0)
        //                        //        {
        //                        //            if (dataTable1.Rows[0]["sys_username"].ToString().Contains("queen_valley"))
        //                        //            {
        //                        //                database = "atltracking";
        //                        //                string query_To_update_firebase_Token1 = $"update bs_user_master set auid='{firebaseToken_android}',iuid='{firebaseToken_Ios}' where id = {dt.Rows[0]["id"]}";
        //                        //                if (firebaseToken_android != null || firebaseToken_android != "")
        //                        //                {
        //                        //                    query_to_insert_login_details = $"insert into bs_parent_login_log (bs_user_id,login_time,source) values ('{dt.Rows[0]["id"]}',getdate(),'Android')";

        //                        //                }
        //                        //                else if (firebaseToken_Ios != null || firebaseToken_Ios != "")
        //                        //                {
        //                        //                    query_to_insert_login_details = $"insert into bs_parent_login_log (bs_user_id,login_time,source) values ('{dt.Rows[0]["id"]}',getdate(),'IOS')";
        //                        //                }
        //                        //                _sql_qury_execution.DML_Insert_Update_Delete(query_to_insert_login_details);
        //                        //                _sql_qury_execution.DML_Insert_Update_Delete(query_To_update_firebase_Token1);

        //                        //                login login1 = new login
        //                        //                {
        //                        //                    id = dt.Rows[0]["id"].ToString(),
        //                        //                    schoolId = dt.Rows[0]["schoolId"].ToString(),
        //                        //                    contact = dt.Rows[0]["contact"].ToString(),
        //                        //                    fatherName = dt.Rows[0]["fatherName"].ToString(),
        //                        //                    motherName = dt.Rows[0]["motherName"].ToString(),
        //                        //                    email = dt.Rows[0]["email"].ToString(),
        //                        //                    address = dt.Rows[0]["address"].ToString(),
        //                        //                    database = database
        //                        //                };
        //                        //                api_response_app_variables arav_1 = new api_response_app_variables
        //                        //                {
        //                        //                    status = "1",
        //                        //                    msg = "success",
        //                        //                    response = login1
        //                        //                };

        //                        //                return Content(JsonConvert.SerializeObject(arav_1), "application/json");
        //                        //            }
        //                        //            api_response_app_variables arav_2 = new api_response_app_variables
        //                        //            {
        //                        //                status = "0",
        //                        //                msg = "fail"

        //                        //            };
        //                        //            return Content(JsonConvert.SerializeObject(arav_2), "application/json");
        //                        //        }
        //                        //        else
        //                        //        {
        //                        //            api_response_app_variables arav_2 = new api_response_app_variables
        //                        //            {
        //                        //                status = "-1",
        //                        //                msg = "fail"

        //                        //            };
        //                        //            return Content(JsonConvert.SerializeObject(arav_2), "application/json");
        //                        //        }
        //                        //    }
        //                        //    else
        //                        //    {
        //                        //        api_response_app_variables arav_2 = new api_response_app_variables
        //                        //        {
        //                        //            status = "0",
        //                        //            msg = "fail"

        //                        //        };
        //                        //        return Content(JsonConvert.SerializeObject(arav_2), "application/json");
        //                        //    }

        //                        //}
        //                    }
        //                    else
        //                    {
        //                        string query_atltracking1 = $"select id,sys_username from atltracking.dbo.tbl_users where id = '{dt.Rows[0]["schoolId"]}'";
        //                        DataTable dataTable1 = _sql_qury_execution.DML_Select(query_atltracking1);
        //                        if (dataTable1 != null)
        //                        {
        //                            if (dataTable1.Rows.Count > 0)
        //                            {
        //                               // if (dataTable1.Rows[0]["sys_username"].ToString().Contains("queen_valley"))
        //                                {
        //                                    database = "atltracking";
        //                                    string query_To_update_firebase_Token1 = $"update bs_user_master set auid='{firebaseToken_android}',iuid='{firebaseToken_Ios}' where id = {dt.Rows[0]["id"]}";
        //                                    if (firebaseToken_android != null || firebaseToken_android != "")
        //                                    {
        //                                        query_to_insert_login_details = $"insert into bs_parent_login_log (bs_user_id,login_time,source) values ('{dt.Rows[0]["id"]}',getdate(),'Android')";

        //                                    }
        //                                    else if (firebaseToken_Ios != null || firebaseToken_Ios != "")
        //                                    {
        //                                        query_to_insert_login_details = $"insert into bs_parent_login_log (bs_user_id,login_time,source) values ('{dt.Rows[0]["id"]}',getdate(),'IOS')";
        //                                    }
        //                                    _sql_qury_execution.DML_Insert_Update_Delete(query_to_insert_login_details);
        //                                    _sql_qury_execution.DML_Insert_Update_Delete(query_To_update_firebase_Token1);

        //                                    login login1 = new login
        //                                    {
        //                                        id = dt.Rows[0]["id"].ToString(),
        //                                        schoolId = dt.Rows[0]["schoolId"].ToString(),
        //                                        contact = dt.Rows[0]["contact"].ToString(),
        //                                        fatherName = dt.Rows[0]["fatherName"].ToString(),
        //                                        motherName = dt.Rows[0]["motherName"].ToString(),
        //                                        email = dt.Rows[0]["email"].ToString(),
        //                                        address = dt.Rows[0]["address"].ToString(),
        //                                        database = database
        //                                    };
        //                                    api_response_app_variables arav_1 = new api_response_app_variables
        //                                    {
        //                                        status = "1",
        //                                        msg = "success",
        //                                        response = login1
        //                                    };

        //                                    return Content(JsonConvert.SerializeObject(arav_1), "application/json");
        //                                }
        //                                //api_response_app_variables arav_2 = new api_response_app_variables
        //                                //{
        //                                //    status = "0",
        //                                //    msg = "fail"

        //                                //};
        //                                //return Content(JsonConvert.SerializeObject(arav_2), "application/json");
        //                            }
        //                            else
        //                            {
        //                                api_response_app_variables arav = new api_response_app_variables
        //                                {
        //                                    status = "-1",
        //                                    msg = "fail"

        //                                };
        //                                return Content(JsonConvert.SerializeObject(arav), "application/json");
        //                            }
        //                        }
        //                        else
        //                        {
        //                            api_response_app_variables arav_2 = new api_response_app_variables
        //                            {
        //                                status = "0",
        //                                msg = "fail"

        //                            };
        //                            return Content(JsonConvert.SerializeObject(arav_2), "application/json");
        //                        }

        //                    }
        //                }
        //                else
        //                {
        //                    api_response_app_variables arav_2 = new api_response_app_variables
        //                    {
        //                        status = "0",
        //                        msg = "fail"

        //                    };
        //                    return Content(JsonConvert.SerializeObject(arav_2), "application/json");
        //                }
        //            }
        //            else
        //            {
        //                api_response_app_variables arav = new api_response_app_variables
        //                {
        //                    status = "-1",
        //                    msg = "fail"

        //                };
        //                return Content(JsonConvert.SerializeObject(arav), "application/json");
        //            }


        //        }
        //        else
        //        {
        //            api_response_app_variables arav = new api_response_app_variables
        //            {
        //                status = "0",
        //                msg = "fail"

        //            };
        //            return Content(JsonConvert.SerializeObject(arav), "application/json");
        //        }



        //    }//try block ends.
        //    catch (Exception ex)
        //    {
        //        string err_msg = ex.Message;
        //        return Content("0");

        //    }//catch block ends.
        //}



        [HttpPost]


        ///<summary>
        /// updatepassword actionmethod update password in APP.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult updatepassword([FromQuery] string email, [FromQuery] string newpassword)
        {

            try
            {

                if (email != "" && newpassword != "")
                {
                    string query = $@"update bs_user_master 
                                    set bs_user_master.bs_password = '{newpassword}' from bs_user_master
                                    inner join bs_student_master_backup bsmb
                                    on bsmb.mobile_no1 = bs_user_master.bs_user_name
                                    where bsmb.email = '{email}'";
                    int rowaffected = _sql_qury_execution.DML_Insert_Update_Delete(query);
                    if (rowaffected > 0)
                    {
                        api_response_app_variables arav1 = new api_response_app_variables
                        {
                            status = "1",
                            msg = "success"

                        };
                        return Content(JsonConvert.SerializeObject(arav1), "application/json");
                        
                    }
                    api_response_app_variables arav2 = new api_response_app_variables
                    {
                        status = "-1",
                        msg = "fail"

                    };
                    return Content(JsonConvert.SerializeObject(arav2), "application/json");
                    
                }
                api_response_app_variables arav3 = new api_response_app_variables
                {
                    status = "0",
                    msg = "fail"

                };
                return Content(JsonConvert.SerializeObject(arav3), "application/json");




            }//try block ends.
            catch (Exception ex)
            {
                api_response_app_variables arav4 = new api_response_app_variables
                {
                    status = "0",
                    msg = "fail"

                };
                return Content(JsonConvert.SerializeObject(arav4), "application/json");
               

            }//catch block ends.
        }

        [HttpPost]

        ///<summary>
        /// generateotp actionmethod generate ota on mail.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public async Task<IActionResult> generateotp([FromQuery] string contactormail, string parent_name)
        {

            try
            {
                general gen = new general();
                if (contactormail.Contains('@'))
                {
                    string query_To_check_presence = $"select bsmb.email,bum.contact_no,bum.id  from bs_user_master  bum inner join bs_student_master_backup bsmb on bum.bs_user_name = bsmb.mobile_no1 where bsmb.email='{contactormail}'";
                    DataTable dt = _sql_qury_execution.DML_Select(query_To_check_presence);
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            string otp = gen.SendEmail("ticket@atlantasys.com", contactormail, parent_name);
                            if (otp != "0")
                            {
                                string query_to_update_otp = $"update bs_user_master set otp = {otp} where id = '{dt.Rows[0]["id"]}'";
                                int rowaffected = _sql_qury_execution.DML_Insert_Update_Delete(query_to_update_otp);
                                if (rowaffected > 0)
                                {
                                    api_response_app_variables arav_otp = new api_response_app_variables
                                    {
                                        status = "1",
                                        msg = "success"

                                    };
                                    return Content(JsonConvert.SerializeObject(arav_otp), "application/json");
                                }
                                api_response_app_variables arav_otp1 = new api_response_app_variables
                                {
                                    status = "0",
                                    msg = "fail"

                                };
                                return Content(JsonConvert.SerializeObject(arav_otp1), "application/json");


                            }
                        }
                        else
                        {
                            api_response_app_variables arav_otp2 = new api_response_app_variables
                            {
                                status = "-1",
                                msg = "fail"

                            };
                            return Content(JsonConvert.SerializeObject(arav_otp2), "application/json");
                        }
                    }
                    api_response_app_variables arav_otp3 = new api_response_app_variables
                    {
                        status = "0",
                        msg = "fail"

                    };
                    return Content(JsonConvert.SerializeObject(arav_otp3), "application/json");
                }
                else
                {
                    //string query_To_check_presence = $"select * from bs_user_master where bs_user_name = '{contactormail}'";

                    string query_To_check_presence = $"select bum.bs_user_name,bsm.sender_id from bs_user_master bum inner join bs_sms_master bsm on bum.sys_user_id = bsm.sys_user_id where bum.bs_user_name = '{contactormail}'";


                    DataTable dt = _sql_qury_execution.DML_Select(query_To_check_presence);
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            string otp = await gen.SendSms(parent_name, "9625258231", dt.Rows[0]["sender_id"].ToString());
                            if (otp != "0")
                            {
                                string query_to_update_otp = $"update bs_user_master set otp = {otp} where id = '{dt.Rows[0]["id"]}'";
                                int rowaffected = _sql_qury_execution.DML_Insert_Update_Delete(query_to_update_otp);
                                if (rowaffected > 0)
                                {
                                    api_response_app_variables arav_otp5 = new api_response_app_variables
                                    {
                                        status = "1",
                                        msg = "success"

                                    };
                                    return Content(JsonConvert.SerializeObject(arav_otp5), "application/json");
                                }
                                api_response_app_variables arav_otp6 = new api_response_app_variables
                                {
                                    status = "0",
                                    msg = "fail"

                                };
                                return Content(JsonConvert.SerializeObject(arav_otp6), "application/json");


                            }
                            //gen.SendSms("","","", "BHTSNR", 0,0);
                        }
                        else
                        {
                            api_response_app_variables arav_otp7 = new api_response_app_variables
                            {
                                status = "-1",
                                msg = "fail"

                            };
                            return Content(JsonConvert.SerializeObject(arav_otp7), "application/json");
                            //return Content("-1");
                        }
                    }
                    api_response_app_variables arav_otp8 = new api_response_app_variables
                    {
                        status = "0",
                        msg = "fail"

                    };
                    return Content(JsonConvert.SerializeObject(arav_otp8), "application/json");
                    //return Content("0");
                }




            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                api_response_app_variables arav_otp9 = new api_response_app_variables
                {
                    status = "0",
                    msg = "fail"

                };
                return Content(JsonConvert.SerializeObject(arav_otp9), "application/json");
                //return Content("0");

            }//catch block ends.
        }

        [HttpPost]

        ///<summary>
        /// checkotp actionmethod checks OTP generated on mail and OTP present on databse.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult checkotp([FromQuery] string email, [FromQuery] string otp)
        {

            try
            {

                if (email != "" && otp != "")
                {
                    string query = $@"select bs_user_master.otp from bs_user_master 
                                    inner join 
                                    bs_student_master_backup
                                    on bs_user_master.bs_user_name = bs_student_master_backup.mobile_no1
                                    where bs_student_master_backup.email = '{email.Trim()}'";
                    DataTable dt = _sql_qury_execution.DML_Select(query);
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            if (dt.Rows[0]["otp"].ToString() == otp)
                            {
                                api_response_app_variables check_otp = new api_response_app_variables
                                {
                                    status = "1",
                                    msg = "success"

                                };
                                return Content(JsonConvert.SerializeObject(check_otp), "application/json");
                                //return Content("success");
                            }
                        }
                        else
                        {
                            api_response_app_variables check_otp1 = new api_response_app_variables
                            {
                                status = "-1",
                                msg = "fail"

                            };
                            return Content(JsonConvert.SerializeObject(check_otp1), "application/json");
                            //return Content("fail");
                        }
                    }
                }
                api_response_app_variables check_otp2 = new api_response_app_variables
                {
                    status = "0",
                    msg = "fail"

                };
                return Content(JsonConvert.SerializeObject(check_otp2), "application/json");
                //return Content("0");




            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                api_response_app_variables check_otp3 = new api_response_app_variables
                {
                    status = "0",
                    msg = "fail"

                };
                return Content(JsonConvert.SerializeObject(check_otp3), "application/json");
                //return Content("0");

            }//catch block ends.
        }

        [HttpPost]


        ///<summary>
        /// getstudentdetails actionmethod fetches all student data.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        ///
        public IActionResult getstudentdetails([FromQuery] string parent_id, [FromQuery] string database)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(parent_id))
                {
                    return Content(JsonConvert.SerializeObject(new studentlist { status = "0", msg = "fail" }), "application/json");
                }

                // STEP 1: Get sys_user_id for this parent
                string schoolQuery = $@"
            SELECT TOP 1 sys_user_id 
            FROM bs_student_master_backup 
            WHERE bs_user_id = '{parent_id}' OR parent_id = '{parent_id}'";

                DataTable schDT = _sql_qury_execution.DML_Select(schoolQuery);

                string sys_userid = (schDT != null && schDT.Rows.Count > 0)
                                    ? schDT.Rows[0]["sys_user_id"].ToString()
                                    : "";

                string queryforgettingstudentdetails;

                // SPECIAL CASE — DIRECT NEWTRACK
                if (sys_userid == "5415" || sys_userid == "5456" || sys_userid == "28535" || sys_userid == "3094" || sys_userid == "5383")
                {
                    queryforgettingstudentdetails = @"
                select bsmb.id, brs.route_id as routeid, bsmb.student_name,
                bcm.class_name, bsmb.admission_no, bsmb.section, bsmb.rf_id,
                brm.route_name, brm.sys_service_id, brm.start_time_up, brm.end_time_up,
                services.veh_reg, bd.name as driver_name, bd.mobile as driver_mobileno
                from bs_student_master_backup bsmb
                inner join bs_class_master bcm on bsmb.class = bcm.id
                inner join bs_route_students brs on bsmb.id = brs.student_id
                inner join bs_route_master brm on brm.id = brs.route_id
                inner join services on brm.sys_service_id = services.id
                left join bs_driver bd on services.id = bd.sys_service_id
                where bsmb.bs_user_id = '" + parent_id + @"' 
                   or bsmb.parent_id = '" + parent_id + "'";
                }
                else if (database == "atltracking")
                {
                    queryforgettingstudentdetails = @"
                select bsmb.id, brs.route_id as routeid, bsmb.student_name,
                bcm.class_name, bsmb.admission_no, bsmb.section, bsmb.rf_id,
                brm.route_name, brm.sys_service_id, brm.start_time_up, brm.end_time_up,
                s.veh_reg, bd.name as driver_name, bd.mobile as driver_mobileno
                from bs_student_master_backup bsmb
                left join bs_class_master bcm on bsmb.class = bcm.id
                left join bs_route_students brs on bsmb.id = brs.student_id
                left join bs_route_master brm on brm.id = brs.route_id
                left join atltracking.dbo.tbl_services s on brm.sys_service_id = s.id
                left join bs_driver bd on s.id = bd.sys_service_id
                where bsmb.bs_user_id = '" + parent_id + @"' 
                   or bsmb.parent_id = '" + parent_id + "'";
                }
                else
                {
                    return Content(JsonConvert.SerializeObject(new studentlist { status = "0", msg = "fail" }), "application/json");
                }

                DataTable dt = _sql_qury_execution.DML_Select(queryforgettingstudentdetails);

                if (dt == null)
                {
                    return Content(JsonConvert.SerializeObject(new studentlist { status = "0", msg = "fail" }), "application/json");
                }

                if (dt.Rows.Count == 0)
                {
                    return Content(JsonConvert.SerializeObject(new studentlist { status = "-1", msg = "fail" }), "application/json");
                }

                // BUILD RESPONSE FASTER
                List<student> stu = new List<student>(dt.Rows.Count);

                foreach (DataRow row in dt.Rows)
                {
                    stu.Add(new student
                    {
                        id = row["id"].ToString(),
                        routeid = row["routeid"].ToString(),
                        student_name = row["student_name"].ToString(),
                        class_name = row["class_name"].ToString(),
                        admission_no = row["admission_no"].ToString(),
                        rf_id = row["rf_id"].ToString(),
                        route_name = row["route_name"].ToString(),
                        sys_service_id = row["sys_service_id"].ToString(),
                        start_time_up = row["start_time_up"].ToString(),
                        end_time_up = row["end_time_up"].ToString(),
                        veh_reg = row["veh_reg"].ToString(),
                        driver_name = row["driver_name"].ToString(),
                        driver_mobileno = row["driver_mobileno"].ToString()
                    });
                }

                studentlist response = new studentlist
                {
                    status = "1",
                    msg = "success",
                    response = stu
                };

                return Content(JsonConvert.SerializeObject(response), "application/json");
            }
            catch
            {
                return Content(JsonConvert.SerializeObject(new studentlist { status = "0", msg = "fail" }), "application/json");
            }
        }


        //public IActionResult getstudentdetails([FromQuery] string parent_id, [FromQuery] string database)
        //{

        //    try
        //    {
        //        // STEP 1: Get sys_user_id for this parent
        //        string schoolQuery = $@"
        //        SELECT TOP 1 sys_user_id 
        //        FROM bs_student_master_backup 
        //        WHERE bs_user_id = '{parent_id}' OR parent_id = '{parent_id}'";

        //        DataTable schDT = _sql_qury_execution.DML_Select(schoolQuery);

        //        string sys_userid = "";
        //        if (schDT != null && schDT.Rows.Count > 0)
        //        {
        //            sys_userid = schDT.Rows[0]["sys_user_id"].ToString();
        //        }

        //        string queryforgettingstudentdetails = "";
        //        if (sys_userid == "5415")
        //        {
        //            queryforgettingstudentdetails = @"select bsmb.id,brs.route_id as routeid, bsmb.student_name,
        //                                            bcm.class_name,bsmb.admission_no,bsmb.section,bsmb.rf_id,
        //                                            brm.route_name,brm.sys_service_id,brm.start_time_up,brm.end_time_up 
        //                                            ,services.veh_reg,bd.name as driver_name,bd.mobile as driver_mobileno
        //                                            from bs_student_master_backup bsmb
        //                                            inner join 
        //                                            bs_class_master bcm
        //                                            on
        //                                            bsmb.class=bcm.id
        //                                            inner join
        //                                            bs_route_students brs
        //                                            on
        //                                            bsmb.id=brs.student_id
        //                                            inner join
        //                                            bs_route_master brm
        //                                            on 
        //                                            brm.id=brs.route_id
        //                                            inner join 
        //                                            services
        //                                            on
        //                                            brm.sys_service_id=services.id
        //                                            left join  
        //                                            bs_driver bd
        //                                            on
        //                                            services.id = bd.sys_service_id
        //                                            where 
        //                                            bsmb.bs_user_id = '" + parent_id+ "' or bsmb.parent_id = '"+parent_id+"'";

        //            //Console.WriteLine(queryforgettingstudentdetails);
        //        }
        //        else if (database=="atltracking")
        //        {
        //            queryforgettingstudentdetails = @"
        //                                            select bsmb.id,brs.route_id as routeid, bsmb.student_name,
        //                                            bcm.class_name,bsmb.admission_no,bsmb.section,bsmb.rf_id,
        //                                            brm.route_name,brm.sys_service_id,brm.start_time_up,brm.end_time_up 
        //                                            ,s.veh_reg,bd.name as driver_name,bd.mobile as driver_mobileno
        //                                            from bs_student_master_backup bsmb
        //                                            left join 
        //                                            bs_class_master bcm
        //                                            on
        //                                            bsmb.class=bcm.id
        //                                            left join
        //                                            bs_route_students brs
        //                                            on
        //                                            bsmb.id=brs.student_id
        //                                            left join
        //                                            bs_route_master brm
        //                                            on 
        //                                            brm.id=brs.route_id
        //                                            left join 
        //                                            atltracking.dbo.tbl_services s
        //                                            on
        //                                            brm.sys_service_id=s.id
        //                                            left join  
        //                                            bs_driver bd
        //                                            on
        //                                            s.id = bd.sys_service_id
        //                                            where 
        //                                            bsmb.bs_user_id = '" + parent_id+ "' or bsmb.parent_id = '"+parent_id+"'";
        //        }


        //        DataTable dt = _sql_qury_execution.DML_Select(queryforgettingstudentdetails);
        //        if (dt != null)
        //        {
        //            if (dt.Rows.Count > 0)
        //            {
        //                List<student> stu = new List<student>();
        //                foreach(DataRow row in dt.Rows)
        //                {
        //                    student student = new student
        //                    {
        //                        id = row["id"].ToString(),
        //                        routeid = row["routeid"].ToString(),
        //                        student_name = row["student_name"].ToString(),
        //                        class_name = row["class_name"].ToString(),
        //                        admission_no = row["admission_no"].ToString(),
        //                        rf_id = row["rf_id"].ToString(),
        //                        route_name = row["route_name"].ToString(),
        //                        sys_service_id = row["sys_service_id"].ToString(),
        //                        start_time_up = row["start_time_up"].ToString(),
        //                        end_time_up = row["end_time_up"].ToString(),
        //                        veh_reg = row["veh_reg"].ToString(),
        //                        driver_name = row["driver_name"].ToString(),
        //                        driver_mobileno = row["driver_mobileno"].ToString()

        //                    };
        //                    stu.Add(student);
        //                }
        //                studentlist getstu = new studentlist
        //                {
        //                    status = "1",
        //                    msg = "success",
        //                    response = stu

        //                };
        //                return Content(JsonConvert.SerializeObject(getstu), "application/json");

        //                //string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
        //                //return Content(json);
        //            }
        //            else
        //            {

        //                studentlist getstu3 = new studentlist
        //                {
        //                    status = "-1",
        //                    msg = "fail"

        //                };
        //                return Content(JsonConvert.SerializeObject(getstu3), "application/json");
        //                //return Content("0");
        //            }
        //        }
        //        studentlist getstu4 = new studentlist
        //        {
        //            status = "0",
        //            msg = "fail"

        //        };
        //        return Content(JsonConvert.SerializeObject(getstu4), "application/json");
        //        //return Content("0");
        //    }//try block ends.
        //    catch (Exception ex)
        //    {
        //        string err_msg = ex.Message;
        //        studentlist getstu5 = new studentlist
        //        {
        //            status = "0",
        //            msg = "fail"

        //        };
        //        return Content(JsonConvert.SerializeObject(getstu5), "application/json");
        //        //return Content("0");

        //    }//catch block ends.
        //}

        [HttpPost]

        ///<summary>
        /// getnotifications actionmethod fetches all Notifications.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult getnotifications([FromQuery] string parent_id)
        {

            try
            {
                string query = $"select message,date_time from bs_notification_parent where parent_id = {parent_id} order by id desc";
                DataTable dt = _sql_qury_execution.DML_Select(query);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        List<message_details> list = new List<message_details>();
                        foreach (DataRow row in dt.Rows)
                        {
                            message_details message_Details = new message_details
                            {
                                message = row["message"].ToString(),
                                date_time = row["date_time"].ToString()
                            };
                            list.Add(message_Details);
                        }
                        notificationlist notify = new notificationlist
                        {
                            
                            msg = "success",
                            response = list,
                            status = "1"

                        };
                        return Content(JsonConvert.SerializeObject(notify), "application/json");
                        //string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                        //return Content(json);
                    }
                    notificationlist notify1 = new notificationlist
                    {

                        msg = "fail",
                        //response = list,
                        status = "-1"

                    };
                    return Content(JsonConvert.SerializeObject(notify1), "application/json");
                    //return Content("-1");

                }
                notificationlist notify2 = new notificationlist
                {

                    msg = "fail",
                    //response = list,
                    status = "0"

                };
                return Content(JsonConvert.SerializeObject(notify2), "application/json");
                //return Content("0");

            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                notificationlist notify3 = new notificationlist
                {

                    msg = "fail",
                    //response = list,
                    status = "0"

                };
                return Content(JsonConvert.SerializeObject(notify3), "application/json");
                //return Content("0");

            }//catch block ends.
        }

        [HttpPost]

        ///<summary>
        /// getbroadcastmessages actionmethod fetches broadcast messages.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult getbroadcastmessages([FromQuery] string route_id)
        {

            try
            {
                string query = $"select message,sent_date from bs_broadcast_msg where route_id = {route_id} order by id desc";
                DataTable dt = _sql_qury_execution.DML_Select(query);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        List<message_details> list = new List<message_details>();
                        foreach (DataRow row in dt.Rows)
                        {
                            message_details message_Details = new message_details
                            {
                                message = row["message"].ToString(),
                                date_time = row["sent_date"].ToString()
                            };
                            list.Add(message_Details);
                        }
                        notificationlist notify_ = new notificationlist
                        {

                            msg = "success",
                            response = list,
                            status = "1"

                        };
                        return Content(JsonConvert.SerializeObject(notify_), "application/json");

                        //string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                        //return Content(json);
                    }
                    notificationlist notify_1 = new notificationlist
                    {

                        msg = "fail",
                        //response = list,
                        status = "-1"

                    };
                    return Content(JsonConvert.SerializeObject(notify_1), "application/json");
                    //return Content("-1");

                }
                notificationlist notify_2 = new notificationlist
                {

                    msg = "fail",
                    //response = list,
                    status = "0"

                };
                return Content(JsonConvert.SerializeObject(notify_2), "application/json");
                //return Content("0");

            }//try block ends.
            catch (Exception ex)
            {
                notificationlist notify_3 = new notificationlist
                {

                    msg = "fail",
                    //response = list,
                    status = "0"

                };
                return Content(JsonConvert.SerializeObject(notify_3), "application/json");
                //string err_msg = ex.Message;
                //return Content("0");

            }//catch block ends.
        }

        [HttpPost]

        ///<summary>
        /// livetracking actionmethod fetches device latest latitude and longitude.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        ///
        public IActionResult livetracking([FromQuery] string service_id, [FromQuery] string database)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(service_id))
                {
                    return Content(JsonConvert.SerializeObject(new livetracking
                    {
                        status = "0",
                        msg = "fail"
                    }), "application/json");
                }

                // STEP 1: Get sys_user_id for this service
                string schoolQuery = $@"
            SELECT TOP 1 sys_user_id 
            FROM services 
            WHERE id = '{service_id}'";

                DataTable schDT = _sql_qury_execution.DML_Select(schoolQuery);

                string sys_userid = (schDT != null && schDT.Rows.Count > 0)
                                    ? schDT.Rows[0]["sys_user_id"].ToString()
                                    : "";

                string querytogetlivedata;
                DataTable dt = null;

                // Direct Newtrack school special case
                if (sys_userid == "5415" || sys_userid == "5456" || sys_userid == "28535" || sys_userid == "3094")
                {
                    querytogetlivedata = $@"
                select id, sys_proc_time as server_time, gps_time, gps_latitude,
                gps_longitude, gps_speed, latitude_direction, longitude_direction, battery_voltage
                from latest_telemetry 
                where sys_service_id = '{service_id}'";

                    dt = _sql_qury_execution.DML_Select(querytogetlivedata);
                }
                else if (database == "atltracking")
                {
                    // 1st TRY → ATL
                    querytogetlivedata = $@"
                select id, sys_proc_time as server_time, gps_time, gps_latitude,
                gps_longitude, gps_speed, latitude_direction, longitude_direction, battery_voltage
                from atltracking.dbo.tbl_latest_telemetry 
                where sys_service_id = '{service_id}'";

                    dt = _sql_qury_execution.DML_Select(querytogetlivedata);

                    // FALLBACK to NEWTRACK
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        querytogetlivedata = $@"
                    select id, sys_proc_time as server_time, gps_time, gps_latitude,
                    gps_longitude, gps_speed, latitude_direction, longitude_direction, battery_voltage
                    from newtrack.dbo.latest_telemetry 
                    where sys_service_id = '{service_id}'";

                        dt = _sql_qury_execution.DML_Select(querytogetlivedata);
                    }
                }

                // If still null
                if (dt == null)
                {
                    return Content(JsonConvert.SerializeObject(new livetracking
                    {
                        status = "0",
                        msg = "fail"
                    }), "application/json");
                }

                // No data found
                if (dt.Rows.Count == 0)
                {
                    return Content(JsonConvert.SerializeObject(new livetracking
                    {
                        status = "-1",
                        msg = "fail"
                    }), "application/json");
                }

                // Build live tracking response
                DataRow row = dt.Rows[0];

                livetracking_var live = new livetracking_var
                {
                    id = row["id"].ToString(),
                    server_time = row["server_time"].ToString(),
                    gps_time = row["gps_time"].ToString(),
                    gps_latitude = row["gps_latitude"].ToString(),
                    gps_longitude = row["gps_longitude"].ToString(),
                    gps_speed = row["gps_speed"].ToString(),
                    latitude_direction = row["latitude_direction"].ToString(),
                    longitude_direction = row["longitude_direction"].ToString(),
                    battery_voltage = row["battery_voltage"].ToString()
                };

                return Content(JsonConvert.SerializeObject(new livetracking
                {
                    status = "1",
                    msg = "success",
                    response = live
                }), "application/json");
            }
            catch
            {
                return Content(JsonConvert.SerializeObject(new livetracking
                {
                    status = "0",
                    msg = "fail"
                }), "application/json");
            }
        }



        //public IActionResult livetracking([FromQuery] string service_id, [FromQuery] string database)
        //{

        //    try
        //    {
        //        // STEP 1: Get sys_user_id for this parent
        //        string schoolQuery = $@"
        //        SELECT TOP 1 sys_user_id 
        //        FROM services 
        //        WHERE id = '{service_id}'";

        //        DataTable schDT = _sql_qury_execution.DML_Select(schoolQuery);

        //        string sys_userid = "";
        //        if (schDT != null && schDT.Rows.Count > 0)
        //        {
        //            sys_userid = schDT.Rows[0]["sys_user_id"].ToString();
        //        }

        //        string querytogetlivedata = $"";
        //        DataTable dt = null;

        //        if (sys_userid == "5415")
        //        {
        //            querytogetlivedata = $"select id,sys_proc_time as server_time,gps_time,gps_latitude,gps_longitude,gps_speed,latitude_direction,longitude_direction,battery_voltage from latest_telemetry where sys_service_id = '{service_id}'";

        //             dt = _sql_qury_execution.DML_Select(querytogetlivedata);
        //        }
        //        else if (database == "atltracking")
        //        {
        //            querytogetlivedata = $"select id,sys_proc_time as server_time,gps_time,gps_latitude,gps_longitude,gps_speed,latitude_direction,longitude_direction,battery_voltage from atltracking.dbo.tbl_latest_telemetry where sys_service_id = '{service_id}'";

        //            dt = _sql_qury_execution.DML_Select(querytogetlivedata);

        //            if (dt == null || dt.Rows.Count == 0)
        //            {
        //                querytogetlivedata = $"select id,sys_proc_time as server_time,gps_time,gps_latitude,gps_longitude,gps_speed,latitude_direction,longitude_direction,battery_voltage from newtrack.dbo.latest_telemetry where sys_service_id = '{service_id}'";

        //                dt = _sql_qury_execution.DML_Select(querytogetlivedata);
        //            }
        //        }

        //        if (dt!=null)
        //        {
        //            if (dt.Rows.Count>0)
        //            {

        //                    livetracking_var livetracking_Var = new livetracking_var
        //                    {
        //                        id = dt.Rows[0]["id"].ToString(),
        //                        server_time = dt.Rows[0]["server_time"].ToString(),
        //                        gps_time = dt.Rows[0]["gps_time"].ToString(),
        //                        gps_latitude = dt.Rows[0]["gps_latitude"].ToString(),
        //                        gps_longitude = dt.Rows[0]["gps_longitude"].ToString(),
        //                        gps_speed = dt.Rows[0]["gps_speed"].ToString(),
        //                        latitude_direction = dt.Rows[0]["latitude_direction"].ToString(),
        //                        longitude_direction = dt.Rows[0]["longitude_direction"].ToString(),
        //                        battery_voltage = dt.Rows[0]["battery_voltage"].ToString()
        //                    };

        //                livetracking trackingdata_newtrack = new livetracking
        //                {

        //                    msg = "success",
        //                    response = livetracking_Var,
        //                    status = "1"

        //                };
        //                return Content(JsonConvert.SerializeObject(trackingdata_newtrack), "application/json");
        //            }
        //            else
        //            {

        //                    livetracking trackingdata = new livetracking
        //                    {

        //                        msg = "fail",
        //                        //response = livetracking_Var1,
        //                        status = "-1"

        //                    };
        //                    return Content(JsonConvert.SerializeObject(trackingdata), "application/json");
        //                    //return "-1";

        //            }
        //        }
        //        livetracking trackingdata1 = new livetracking
        //        {

        //            msg = "fail",
        //            //response = livetracking_Var1,
        //            status = "0"

        //        };
        //        return Content(JsonConvert.SerializeObject(trackingdata1), "application/json");
        //        //return "0";
        //    }//try block ends.
        //    catch (Exception ex)
        //    {
        //        string err_msg = ex.Message;
        //        livetracking trackingdata2 = new livetracking
        //        {

        //            msg = "fail",
        //            //response = livetracking_Var1,
        //            status = "0"

        //        };
        //        return Content(JsonConvert.SerializeObject(trackingdata2), "application/json");
        //        //return Content("0");

        //    }//catch block ends.
        //}

        [HttpPost]

        ///<summary>
        /// editPassword actionmethod update current password with new one.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult editPassword([FromQuery] string parent_id, [FromQuery] string currentpassword, [FromQuery]string newpassword)
        {

            try
            {

                if (parent_id != "" && newpassword != "" && currentpassword !="")
                {
                    string checkuser = $"select * from bs_user_master where id = {parent_id}";
                    DataTable data = _sql_qury_execution.DML_Select(checkuser);
                    if (data!=null)
                    {
                        if (data.Rows.Count>0)
                        {
                            string query = $"update bs_user_master set bs_password = '{newpassword}' where id = {parent_id} and bs_password = '{currentpassword}'";
                            int rowaffected = _sql_qury_execution.DML_Insert_Update_Delete(query);
                            if (rowaffected > 0)
                            {
                                api_response_app_variables arav1 = new api_response_app_variables
                                {
                                    status = "1",
                                    msg = "success"

                                };
                                return Content(JsonConvert.SerializeObject(arav1), "application/json");

                            }
                            api_response_app_variables arav2 = new api_response_app_variables
                            {
                                status = "-1",
                                msg = "fail"

                            };
                            return Content(JsonConvert.SerializeObject(arav2), "application/json");
                        }
                        else
                        {
                            api_response_app_variables arav5 = new api_response_app_variables
                            {
                                status = "-1",
                                msg = "fail"

                            };
                            return Content(JsonConvert.SerializeObject(arav5), "application/json");
                        }
                    }
                    else
                    {
                        api_response_app_variables arav4 = new api_response_app_variables
                        {
                            status = "-1",
                            msg = "fail"

                        };
                        return Content(JsonConvert.SerializeObject(arav4), "application/json");
                    }

                    

                }
                api_response_app_variables arav3 = new api_response_app_variables
                {
                    status = "0",
                    msg = "fail"

                };
                return Content(JsonConvert.SerializeObject(arav3), "application/json");




            }//try block ends.
            catch (Exception ex)
            {
                api_response_app_variables arav4 = new api_response_app_variables
                {
                    status = "0",
                    msg = "fail"

                };
                return Content(JsonConvert.SerializeObject(arav4), "application/json");


            }//catch block ends.
        }

        [HttpPost]

        ///<summary>
        /// getAllStops actionmethod fetches all stops from databse.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult getAllStops([FromQuery] string route_id)
        {
            try
            {
                string query = $"select id,user_stop_name,stop_order,status from bs_stop_master where route_id = {route_id} order by stop_order";
                DataTable dt = _sql_qury_execution.DML_Select(query);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        List<stops_var> list = new List<stops_var>();
                        foreach (DataRow row in dt.Rows)
                        {
                            string stop_status = "none";
                            switch (Convert.ToInt32(row["status"].ToString()))
                            {
                                
                                case 1:
                                    stop_status = "upcoming";
                                    break;
                                case 2:
                                    stop_status = "reached";
                                    break;
                                case 3:
                                    stop_status = "passed";
                                    break;
                                case 4:
                                    stop_status = "completed";
                                    break;
                                default:
                                    stop_status = "none";
                                    break;

                            }

                            stops_var stop = new stops_var
                            {
                                id = row["id"].ToString(),
                                user_stop_name = row["user_stop_name"].ToString(),
                                stop_order = row["stop_order"].ToString(),
                                visited_upcoming = stop_status
                            };
                            
                            list.Add(stop);
                        }
                        stops stoplist = new stops
                        {

                            msg = "success",
                            response = list,
                            status = "1"

                        };
                        return Content(JsonConvert.SerializeObject(stoplist), "application/json");

                        //string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                        //return Content(json);
                    }
                   
                    stops stoplist1 = new stops
                    {

                        msg = "fail",
                        //response = list,
                        status = "-1"

                    };
                    return Content(JsonConvert.SerializeObject(stoplist1), "application/json");
                    //return Content("-1");

                }
                stops stoplist2 = new stops
                {

                    msg = "fail",
                    //response = list,
                    status = "0"

                };
                return Content(JsonConvert.SerializeObject(stoplist2), "application/json");
                //return Content("0");

            }//try block ends.
            catch (Exception ex)
            {
                stops stoplist3 = new stops
                {

                    msg = "fail",
                    //response = list,
                    status = "0"

                };
                return Content(JsonConvert.SerializeObject(stoplist3), "application/json");
                //string err_msg = ex.Message;
                //return Content("0");

            }//catch block ends.
        }

        [HttpPost]

        ///<summary>
        /// sendMailToSupport actionmethod sends queries to support team via APP.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult sendMailToSupport([FromQuery] string message, [FromQuery] string email)
        {
            try
            {
                general gen = new general();
                if (email.Contains("@"))
                {
                    string query_To_check_presence = $"select bsmb.email,bum.contact_no,bum.id  from bs_user_master  bum inner join bs_student_master_backup bsmb on bum.bs_user_name = bsmb.mobile_no1 where bsmb.email='{email}'";
                    DataTable dt = _sql_qury_execution.DML_Select(query_To_check_presence);
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            if(gen.generalsendemail("ticket@atlantasys.in", "tanyasuyalatl@gmail.com", message, dt.Rows[0]["contact_no"].ToString()))
                            {
                                api_response_app_variables arav_otp2 = new api_response_app_variables
                                {
                                    status = "1",
                                    msg = "success"

                                };
                                return Content(JsonConvert.SerializeObject(arav_otp2), "application/json");
                            }

                            
                        }
                        else
                        {
                            api_response_app_variables arav_otp2 = new api_response_app_variables
                            {
                                status = "-1",
                                msg = "fail"

                            };
                            return Content(JsonConvert.SerializeObject(arav_otp2), "application/json");
                        }
                    }
                    api_response_app_variables arav_otp_ = new api_response_app_variables
                    {
                        status = "0",
                        msg = "fail"

                    };
                    return Content(JsonConvert.SerializeObject(arav_otp_), "application/json");
                }
                api_response_app_variables arav_otp3 = new api_response_app_variables
                {
                    status = "0",
                    msg = "fail"

                };
                return Content(JsonConvert.SerializeObject(arav_otp3), "application/json");

            }
            catch
            {
                api_response_app_variables _arav_otp = new api_response_app_variables
                {
                    status = "0",
                    msg = "fail"

                };
                return Content(JsonConvert.SerializeObject(_arav_otp), "application/json");
            }
        }
        [HttpPost]

        ///<summary>
        /// getholidays actionmethod gets holidays.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult getholidays([FromQuery] string schoolid)
        {

            try
            {
                string query = $"select from_date,to_date,description from bs_holidays where sys_user_id = {schoolid}";
                DataTable dt = _sql_qury_execution.DML_Select(query);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        List<Holiday> list = new List<Holiday>();
                        foreach (DataRow row in dt.Rows)
                        {
                            Holiday holiday = new Holiday
                            {
                                from_date = row["from_date"].ToString(),
                                to_date = row["to_date"].ToString(),
                                description = row["description"].ToString()
                                
                            };
                            list.Add(holiday);
                        }
                        holiday_resonse resonse_ = new holiday_resonse
                        {

                            msg = "success",
                            holidays = list,
                            status = "1"

                        };
                        return Content(JsonConvert.SerializeObject(resonse_), "application/json");
                        //string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                        //return Content(json);
                    }
                    holiday_resonse resonse_1 = new holiday_resonse
                    {

                        msg = "fail",
                        //response = list,
                        status = "-1"

                    };
                    return Content(JsonConvert.SerializeObject(resonse_1), "application/json");
                    //return Content("-1");

                }
                holiday_resonse resonse_2 = new holiday_resonse
                {

                    msg = "fail",
                    //response = list,
                    status = "0"

                };
                return Content(JsonConvert.SerializeObject(resonse_2), "application/json");
                //return Content("0");

            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                holiday_resonse resonse_3 = new holiday_resonse
                {

                    msg = "fail",
                    //response = list,
                    status = "0"

                };
                return Content(JsonConvert.SerializeObject(resonse_3), "application/json");
                //return Content("0");

            }//catch block ends.
        }

        [HttpPost]
        public IActionResult Logout(
    [FromQuery] string parentid,
    [FromQuery] string FCM)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(parentid) ||
                    string.IsNullOrWhiteSpace(FCM))
                {
                    return Content(
                        JsonConvert.SerializeObject(
                            new api_response_app_variables
                            {
                                status = "0",
                                msg = "Parent ID or FCM token is missing"
                            }),
                        "application/json"
                    );
                }

                /*
                 * Step 1:
                 * Remove token from login log.
                 */
                string clearLoginLogQuery = $@"
            UPDATE bs_parent_login_log
            SET fcm = ''
            WHERE bs_user_id = {parentid}
              AND CAST(fcm AS VARCHAR(MAX)) = '{FCM.Replace("'", "''")}'";

                _sql_qury_execution
                    .DML_Insert_Update_Delete(clearLoginLogQuery);

                /*
                 * Step 2:
                 * Remove only the matching Android/iOS token
                 * from bs_user_master.
                 *
                 * This is the important missing step.
                 */
                string clearUserTokenQuery = $@"
            UPDATE bs_user_master
            SET
                auid = CASE
                           WHEN LTRIM(RTRIM(ISNULL(auid, ''))) =
                                '{FCM.Replace("'", "''")}'
                           THEN ''
                           ELSE auid
                       END,

                iuid = CASE
                           WHEN LTRIM(RTRIM(ISNULL(iuid, ''))) =
                                '{FCM.Replace("'", "''")}'
                           THEN ''
                           ELSE iuid
                       END
            WHERE id = {parentid}";

                int affectedRows = _sql_qury_execution
                    .DML_Insert_Update_Delete(clearUserTokenQuery);

                return Content(
                    JsonConvert.SerializeObject(
                        new api_response_app_variables
                        {
                            status = "1",
                            msg = "success"
                        }),
                    "application/json"
                );
            }
            catch (Exception ex)
            {
                return Content(
                    JsonConvert.SerializeObject(
                        new api_response_app_variables
                        {
                            status = "0",
                            msg = ex.Message
                        }),
                    "application/json"
                );
            }
        }

        ///<summary>
        /// Logout actionmethod remove FCM token from the table assigned to the user who login the APP.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        ///
        //        public IActionResult Logout([FromQuery] string parentid, [FromQuery] string FCM)
        //        {
        //            try
        //            {
        //                // Early validation
        //                if (string.IsNullOrWhiteSpace(parentid) || string.IsNullOrWhiteSpace(FCM))
        //                {
        //                    return Content(JsonConvert.SerializeObject(new api_response_app_variables
        //                    {
        //                        status = "0",
        //                        msg = "fail"
        //                    }), "application/json");
        //                }

        //                // Use faster EXISTS instead of full SELECT *
        //                //    string checkQuery = $@"
        //                //SELECT TOP 1 1 
        //                //FROM bs_parent_login_log 
        //                //WHERE bs_user_id = {parentid} AND fcm = '{FCM}'";

        //                string checkQuery = $@"
        //SELECT TOP 1 1
        //FROM bs_parent_login_log
        //WHERE bs_user_id = {parentid}
        //  AND CAST(fcm AS VARCHAR(MAX)) = '{FCM}'";

        //                DataTable dt = _sql_qury_execution.DML_Select(checkQuery);

        //                // If record exists → update FCM to empty
        //                if (dt != null && dt.Rows.Count > 0)
        //                {
        //                    //    string updateQuery = $@"
        //                    //UPDATE bs_parent_login_log 
        //                    //SET FCM = '' 
        //                    //WHERE bs_user_id = {parentid} AND fcm = '{FCM}'";

        //                    string updateQuery = $@"
        //UPDATE bs_parent_login_log
        //SET fcm = ''
        //WHERE bs_user_id = {parentid}
        //  AND CAST(fcm AS VARCHAR(MAX)) = '{FCM}'";

        //                    int row = _sql_qury_execution.DML_Insert_Update_Delete(updateQuery);

        //                    if (row > 0)
        //                    {
        //                        return Content(JsonConvert.SerializeObject(new api_response_app_variables
        //                        {
        //                            status = "1",
        //                            msg = "success"
        //                        }), "application/json");
        //                    }

        //                    return Content(JsonConvert.SerializeObject(new api_response_app_variables
        //                    {
        //                        status = "0",
        //                        msg = "fail"
        //                    }), "application/json");
        //                }

        //                // If entry not found but still want to return success
        //                return Content(JsonConvert.SerializeObject(new api_response_app_variables
        //                {
        //                    status = "1",
        //                    msg = "success"
        //                }), "application/json");
        //            }
        //            catch
        //            {
        //                return Content(JsonConvert.SerializeObject(new api_response_app_variables
        //                {
        //                    status = "0",
        //                    msg = "fail"
        //                }), "application/json");
        //            }
        //        }


        //public IActionResult Logout([FromQuery] string parentid, [FromQuery] string FCM)
        //{

        //    try
        //    {
        //        string query = $"select * from bs_parent_login_log where bs_user_id = {parentid} and cast(fcm as varchar(Max))='{FCM}'";
        //        DataTable dt = _sql_qury_execution.DML_Select(query);
        //        if (dt != null)
        //        {
        //            if (dt.Rows.Count > 0)
        //            {
        //                string query_update = $"Update bs_parent_login_log set FCM = \'\' where bs_user_id = {parentid} and cast(fcm as varchar(Max))='{FCM}'";
        //                int rowaffected = _sql_qury_execution.DML_Insert_Update_Delete(query_update);
        //                if (rowaffected > 0)
        //                {
        //                    api_response_app_variables arav_sucess = new api_response_app_variables
        //                    {
        //                        status = "1",
        //                        msg = "success"

        //                    };
        //                    return Content(JsonConvert.SerializeObject(arav_sucess), "application/json");
        //                }
        //                else
        //                {
        //                    api_response_app_variables arav_fail = new api_response_app_variables
        //                    {
        //                        status = "0",
        //                        msg = "fail"

        //                    };
        //                    return Content(JsonConvert.SerializeObject(arav_fail), "application/json");
        //                }


        //            }
        //            api_response_app_variables arav_sucess1 = new api_response_app_variables
        //            {
        //                status = "1",
        //                msg = "success"

        //            };
        //            return Content(JsonConvert.SerializeObject(arav_sucess1), "application/json");
        //        }
        //        api_response_app_variables arav_fail1 = new api_response_app_variables
        //        {
        //            status = "0",
        //            msg = "fail"

        //        };
        //        return Content(JsonConvert.SerializeObject(arav_fail1), "application/json");
        //        //return Content("0");

        //    }//try block ends.
        //    catch (Exception ex)
        //    {
        //        api_response_app_variables arav1 = new api_response_app_variables
        //        {
        //            status = "0",
        //            msg = "fail"

        //        };
        //        return Content(JsonConvert.SerializeObject(arav1), "application/json");
        //        //return Content("0");

        //    }//catch block ends.
        //}


        [HttpPost]
        public IActionResult Stop_ETA([FromQuery] string stopid)
        {

            try
            {
                string query = $"select eta from bs_stop_master where id = '{stopid}'";
                DataTable dt = _sql_qury_execution.DML_Select(query);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {

                        Stopeta stopeta = new Stopeta
                        {
                            status = "1",
                            ETA = dt.Rows[0][0].ToString(),
                            msg = "success"

                        };
                        return Content(JsonConvert.SerializeObject(stopeta), "application/json");


                    }
                    api_response_app_variables arav_f1 = new api_response_app_variables
                    {
                        status = "-1",
                        msg = "fail"

                    };
                    return Content(JsonConvert.SerializeObject(arav_f1), "application/json");
                }
                api_response_app_variables arav_fail1 = new api_response_app_variables
                {
                    status = "0",
                    msg = "fail"

                };
                return Content(JsonConvert.SerializeObject(arav_fail1), "application/json");
                //return Content("0");

            }//try block ends.
            catch (Exception ex)
            {
                api_response_app_variables arav1 = new api_response_app_variables
                {
                    status = "0",
                    msg = "fail"

                };
                return Content(JsonConvert.SerializeObject(arav1), "application/json");
                //return Content("0");

            }//catch block ends.
        }





        [HttpGet]
        public IActionResult GetHolidays([FromQuery] string schoolid)
        {
            try
            {
                if (!String.IsNullOrEmpty(schoolid))
                {
                    string getholidays = $@"SELECT * FROM bs_holidays WHERE sys_user_id = '{schoolid}'";
                    DataTable dataTable = _sql_qury_execution.DML_Select(getholidays);

                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        string json = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
                        return Content(json, "application/json");
                    }
                    return NoContent(); // Return HTTP 204 if no holidays found
                }
                return BadRequest(new { message = "Invalid School ID" });
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in GetHolidays: " + e.Message);
                return StatusCode(500, new { message = "Internal Server Error", error = e.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAttendanceDayWise([FromQuery] string schoolid, string parentid, string studentid, DateTime To, DateTime from)
        {
            try
            {
                if (!String.IsNullOrEmpty(schoolid) && !String.IsNullOrEmpty(parentid) && !String.IsNullOrEmpty(studentid))
                {
                    string FROM = from.ToString("yyyy-MM-dd");
                    string TO = To.ToString("yyyy-MM-dd");
                    string getattendance = $@"DECLARE @FromDate DATE = '{FROM}';
DECLARE @ToDate DATE = '{TO}';

WITH Dates AS
(
    SELECT @FromDate AS AttendanceDate

    UNION ALL

    SELECT DATEADD(DAY, 1, AttendanceDate)
    FROM Dates
    WHERE AttendanceDate < @ToDate
)

SELECT
    bs.student_name,
    d.AttendanceDate,
    CASE
        WHEN d.AttendanceDate > CAST(GETDATE() AS DATE)
            THEN 'Future'
        WHEN r.student_id IS NOT NULL
            THEN 'Punched'
        ELSE 'Not Punched'
    END AS AttendanceStatus

FROM bs_student_master_backup bs

CROSS JOIN Dates d

LEFT JOIN rf_punch_history r
    ON r.student_id = bs.id
    AND CAST(r.sys_proc_time AS DATE) = d.AttendanceDate

WHERE bs.sys_user_id = {schoolid}
  AND bs.bs_user_id = {parentid}
  AND bs.id = {studentid}

GROUP BY
    bs.student_name,
    d.AttendanceDate,
    r.student_id

ORDER BY
    d.AttendanceDate,
    bs.student_name

OPTION (MAXRECURSION 366);";
                    if (from > To)
                    {
                        return BadRequest(new { message = "Invalid date range" });
                    }
                    DataTable dataTable = _sql_qury_execution.DML_Select(getattendance);

                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        string json = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
                        return Content(json, "application/json");
                    }
                    return NoContent(); // Return HTTP 204 if no attendance found
                }
                return BadRequest(new { message = "Invalid School ID or parent_id or student_id" });
            }
            catch (Exception e)
            {
                Console.WriteLine(" Exception in Get Attendandance Day Wise: " + e.Message);
                return StatusCode(500, new { message = "Internal Server Error", error = e.Message });
            }
        }


        [HttpGet]
        public IActionResult GettodayAttendance([FromQuery] string schoolid, string parent_id)
        {
            try
            {
                if (!String.IsNullOrEmpty(schoolid) && !String.IsNullOrEmpty(parent_id))
                {
                    string query = $@"
DECLARE @SchoolId INT = {schoolid};
DECLARE @ParentId INT = {parent_id};

SELECT
    bs.student_name AS StudentName,
    CAST(GETDATE() AS DATE) AS AttendanceDate,

    CASE
        WHEN r.student_id IS NOT NULL
            THEN 'Punched'
        ELSE 'Not Punched'
    END AS AttendanceStatus

FROM bs_student_master_backup bs

LEFT JOIN
(
    SELECT DISTINCT
        student_id
    FROM rf_punch_history
    WHERE sys_proc_time >= CAST(GETDATE() AS DATE)
      AND sys_proc_time < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))
) r
    ON r.student_id = bs.id

WHERE bs.sys_user_id = @SchoolId
  AND bs.bs_user_id = @ParentId

ORDER BY bs.student_name;
";
                    DataTable dataTable = _sql_qury_execution.DML_Select(query);

                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        string json = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
                        return Content(json, "application/json");
                    }
                    return NoContent(); // Return HTTP 204 if no holidays found
                }
                return BadRequest(new { message = "Invalid School ID or Parent ID" });
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Get Today Attendance: " + e.Message);
                return StatusCode(500, new { message = "Internal Server Error", error = e.Message });
            }
        }





    }
}
