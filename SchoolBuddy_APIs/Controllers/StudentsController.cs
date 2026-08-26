
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using SchoolBuddy_APIs.Database_methods;
using SchoolBuddy_APIs.Models.App;
using SchoolBuddy_APIs.Models.Master.Holidays_And_Events;
using SchoolBuddy_APIs.Models.Master.Route;
using SchoolBuddy_APIs.Models.Master.Students;
using System.Data;
using System.Text;

namespace SchoolBuddy_APIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        List<error_student_upload> error_Student_Uploads = new List<error_student_upload>();

        private readonly Idatabase_access _sql_qury_execution;
        public StudentsController(Idatabase_access sql_qury_execution)
        {
            _sql_qury_execution = sql_qury_execution;
        }

        [HttpPost]

        ///<summary>
        /// GetAllStudentDetails actionmethod gets all students details.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult GetAllStudentDetails(getallstudents getallstudents)
        {
            try
            {
                string user_id = getallstudents.user_id;
                string json = "";
                if (user_id != null)
                {
                    string query = "SELECT bsmb.id, bsmb.rf_id, bsmb.student_name, " +
       "bsmb.father_name AS parent_name, bsmb.mobile_no1, " +
       "bsmb.street, bsmb.dob AS birth, bsmb.section AS division, " +
       "bsmb.email, bsmb.admission_no, FORMAT(bsmb.added_on, 'yyyy-MM-dd HH:mm:ss') AS created_date, " +
       "bcm.class_name AS class_, bum.bs_password AS password " +
       "FROM bs_student_master_backup bsmb " +
       "LEFT JOIN bs_class_master bcm ON bcm.id = TRY_CONVERT(int, bsmb.class) " +  //  Safe conversion for class
       "LEFT JOIN bs_user_master bum ON bum.id = TRY_CONVERT(int, bsmb.parent_id) " + //  Safe conversion for parent_id
       $"WHERE bsmb.sys_user_id = '{user_id}' ORDER BY bsmb.student_name";

                    Console.WriteLine(query);   
                    //string query = $"select * from bs_student_master_backup where sys_user_id = '{user_id}'";
                    DataTable datatable = _sql_qury_execution.DML_Select(query);
                    if (datatable != null)
                    {
                        if (datatable.Rows.Count >= 0)
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

        [HttpGet]
        public async Task<IActionResult> DownloadStudentsExcel(string user_id)
        {
            try
            {
                if (string.IsNullOrEmpty(user_id))
                {
                    return BadRequest("User ID is required.");
                }

                string query = $@"
            SELECT 
                bsmb.admission_no,
                bsmb.student_name,
                bsmb.dob AS birth,
                bsmb.section AS division,
                bsmb.father_name AS parent_name,
                bsmb.mobile_no1,
                bsmb.street AS address,
                bsmb.email,
                bsmb.rf_id,
                bsmb.class AS class_id,
                bcm.class_name
            FROM bs_student_master_backup bsmb
            LEFT JOIN bs_class_master bcm 
                ON bcm.id = TRY_CONVERT(int, bsmb.class)
            WHERE bsmb.sys_user_id = '{user_id}'
            ORDER BY bsmb.student_name";

                //Console.WriteLine("DownloadStudentsExcel Query: " + query);

                DataTable dt = _sql_qury_execution.DML_Select(query);

                if (dt == null || dt.Rows.Count == 0)
                {
                    return NotFound("No student data found.");
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add("Students");

                    // Header
                    ws.Cells[1, 1].Value = "admission_no";
                    ws.Cells[1, 2].Value = "student_name";
                    ws.Cells[1, 3].Value = "birth";
                    ws.Cells[1, 4].Value = "division";
                    ws.Cells[1, 5].Value = "parent_name";
                    ws.Cells[1, 6].Value = "mobile_no1";
                    ws.Cells[1, 7].Value = "address";
                    ws.Cells[1, 8].Value = "email";
                    ws.Cells[1, 9].Value = "rf_id";
                    ws.Cells[1, 10].Value = "class_id";
                    ws.Cells[1, 11].Value = "class_name";

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ws.Cells[i + 2, 1].Value = dt.Rows[i]["admission_no"]?.ToString();
                        ws.Cells[i + 2, 2].Value = dt.Rows[i]["student_name"]?.ToString();
                        ws.Cells[i + 2, 3].Value = dt.Rows[i]["birth"]?.ToString();
                        ws.Cells[i + 2, 4].Value = dt.Rows[i]["division"]?.ToString();
                        ws.Cells[i + 2, 5].Value = dt.Rows[i]["parent_name"]?.ToString();
                        ws.Cells[i + 2, 6].Value = dt.Rows[i]["mobile_no1"]?.ToString();
                        ws.Cells[i + 2, 7].Value = dt.Rows[i]["address"]?.ToString();
                        ws.Cells[i + 2, 8].Value = dt.Rows[i]["email"]?.ToString();
                        ws.Cells[i + 2, 9].Value = dt.Rows[i]["rf_id"]?.ToString();
                        ws.Cells[i + 2, 10].Value = dt.Rows[i]["class_id"]?.ToString();
                        ws.Cells[i + 2, 11].Value = dt.Rows[i]["class_name"]?.ToString();
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
                        "Students.xlsx"
                    );
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error while generating excel: {ex.Message}");
            }
        }

        [HttpPost]
        ///<summary>
        /// EditStudent actionmethod edit students
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public bool EditStudent(getstudent students)
        {
            try
            {
                #region EDIT QUERY
                string query = $"UPDATE bs_student_master_backup SET " +
                               $"admission_no = '{students.admission_no}', " +
                               $"student_name = '{students.student_name}', " +
                               $"dob = '{students.birth}', " +
                               $"class = '{students.class_}', " +
                               $"section = '{students.division}', " +
                               $"father_name = '{students.parent_name}', " +
                               $"mobile_no1 = '{students.mobile_no1}', " +
                               $"street = '{students.street}', " +
                               $"rf_id = '{students.rf_id}', " +
                               $"email = '{students.email}' " +
                               $"WHERE id = {students.id}";
                #endregion

                string query_ = $@"SELECT parent_id FROM bs_student_master_backup WHERE id = '{students.id}'";
                DataTable dt = _sql_qury_execution.DML_Select(query_);

                if (dt != null && dt.Rows.Count > 0)
                {
                    string parentId = dt.Rows[0][0].ToString();

                    // Validate that parentId is an integer before using it in the query
                    if (!string.IsNullOrEmpty(parentId) && int.TryParse(parentId, out int parentIdInt))
                    {
                        string query1 = $@"UPDATE bs_user_master 
                                   SET name = '{students.parent_name}', 
                                       bs_password = '12345', 
                                       bs_user_name = '{students.mobile_no1}' 
                                   WHERE id = {parentIdInt}";

                        int res = _sql_qury_execution.DML_Insert_Update_Delete_with_Transaction(query, query1);
                        return res > 0;
                    }
                    else
                    {
                        // Only update the student record if parent_id is NULL or invalid
                        int res = _sql_qury_execution.DML_Insert_Update_Delete(query);
                        return res > 0;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while updating student: " + ex.Message);
                return false;
            }
        }

        [HttpPost]
        ///<summary>
        /// GetStudentById actionmethod gets student by id.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult GetStudentById(getstudentbyid std)
        {
            try
            {
                string query = $@"SELECT bsmb.id, bsmb.rf_id, bsmb.student_name,
                                    bsmb.father_name AS parent_name, bsmb.mobile_no1,
                                    bsmb.street, bsmb.dob AS birth, bsmb.section AS division,
                                    bsmb.email, bsmb.admission_no, FORMAT(bsmb.added_on, 'yyyy-MM-dd HH:mm:ss') AS created_date,
                                    bcm.class_name AS class_, bum.bs_password AS password
                                    FROM bs_student_master_backup bsmb
                                    LEFT JOIN bs_class_master bcm ON bcm.id = bsmb.class
                                    LEFT JOIN bs_user_master bum ON bum.id = bsmb.parent_id
                                    WHERE bsmb.id = '{std.student_id}'";
                //Console.WriteLine(query);

                DataTable dataTable = _sql_qury_execution.DML_Select(query);

                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    var student = new getstudent
                    {
                        id = Convert.ToInt32(dataTable.Rows[0]["id"]),
                        student_name = dataTable.Rows[0]["student_name"].ToString(),
                        class_ = dataTable.Rows[0]["class_"].ToString(),
                        admission_no = dataTable.Rows[0]["admission_no"].ToString(),
                        birth = dataTable.Rows[0]["birth"].ToString(),
                        division = dataTable.Rows[0]["division"].ToString(),
                        parent_name = dataTable.Rows[0]["parent_name"].ToString(),
                        mobile_no1 = dataTable.Rows[0]["mobile_no1"].ToString(),
                        password = dataTable.Rows[0]["password"].ToString(),
                        street = dataTable.Rows[0]["street"].ToString(),
                        rf_id = dataTable.Rows[0]["rf_id"].ToString(),
                        email = dataTable.Rows[0]["email"].ToString(),
                    };

                    return Ok(student);
                }
                else
                {
                    return NotFound(new { status = "error", message = "Student not found!" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = ex.Message });
            }
        }

        [HttpPost]
        ///<summary>
        /// DeleteStudentById actionmethod  delete student by id.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public bool DeleteStudentById(getstudentbyid std)
        {
            try
            {
                string query = $"DELETE FROM bs_student_master_backup WHERE id = '{std.student_id}'";

                string query_ = $@"SELECT parent_id FROM bs_student_master_backup WHERE id = '{std.student_id}'";
                DataTable dt = _sql_qury_execution.DML_Select(query_);

                if (dt != null && dt.Rows.Count > 0)
                {
                    string parentId = dt.Rows[0][0].ToString();

                    if (!string.IsNullOrEmpty(parentId) && int.TryParse(parentId, out int parentIdInt))
                    {
                        string query1 = $@"DELETE FROM bs_user_master WHERE id = {parentIdInt}";
                        int result = _sql_qury_execution.DML_Insert_Update_Delete_with_Transaction(query, query1);

                        return result > 0;
                    }
                    else
                    {
                        // Only delete the student if parent_id is NULL or invalid
                        int result = _sql_qury_execution.DML_Insert_Update_Delete(query);
                        return result > 0;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while deleting student: " + ex.Message);
                return false;
            }
        }

        [HttpPost]
        ///<summary>
        /// DeleteStudentById actionmethod add students .
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult AddStudent([FromBody] getstudent students)
        {
            try
            {
                string parent_id = "";
                string queryCheckUser = $"SELECT id FROM bs_user_master WHERE bs_user_name = '{students.mobile_no1}';";
                DataTable userTable = _sql_qury_execution.DML_Select(queryCheckUser);

                if (userTable != null && userTable.Rows.Count > 0)
                {
                    // User already exists, retrieve parent_id
                    parent_id = userTable.Rows[0]["id"].ToString();
                }
                else
                {
                    // Insert new user into bs_user_master
                    string insertUserQuery = $"INSERT INTO bs_user_master (sys_user_id, role_id, bs_user_name, bs_password, name) " +
                                             $"VALUES ('{students.user_id}', 1, '{students.mobile_no1}', '12345', '{students.parent_name}');";
                    int userInserted = _sql_qury_execution.DML_Insert_Update_Delete(insertUserQuery);

                    if (userInserted > 0)
                    {
                        // Retrieve new parent_id
                        DataTable newUserTable = _sql_qury_execution.DML_Select(queryCheckUser);
                        if (newUserTable != null && newUserTable.Rows.Count > 0)
                        {
                            parent_id = newUserTable.Rows[0]["id"].ToString();
                        }
                    }
                    else
                    {
                        return BadRequest(new { status = "error", message = "Failed to create user record!" });
                    }
                }

                // Check if student record already exists
                string queryCheckStudent = $"SELECT mobile_no1, admission_no FROM bs_student_master_backup " +
                                            $"WHERE admission_no = '{students.admission_no}' AND mobile_no1 = '{students.mobile_no1}';";
                DataTable studentTable = _sql_qury_execution.DML_Select(queryCheckStudent);

                if (studentTable != null && studentTable.Rows.Count > 0)
                {
                    return BadRequest(new { status = "error", message = "Duplicate student record!" });
                }

                // Insert student data into bs_student_master_backup
                string insertStudentQuery = $"INSERT INTO bs_student_master_backup (bs_user_id, parent_id, admission_no, student_name, dob, class, section, " +
                                            $"father_name, mobile_no1, street, rf_id, email, added_on, sys_user_id) " +
                                            $"VALUES ('{parent_id}', '{parent_id}', '{students.admission_no}', '{students.student_name}', " +
                                            $"'{students.birth}', '{students.class_}', '{students.division}', '{students.parent_name}', " +
                                            $"'{students.mobile_no1}', '{students.street}', '{students.rf_id}', '{students.email}', GETDATE(), '{students.user_id}');";

                int studentInserted = _sql_qury_execution.DML_Insert_Update_Delete(insertStudentQuery);
                if (studentInserted > 0)
                {
                    return Ok(new { status = "success", message = "Student added successfully!" });
                }
                else
                {
                    return BadRequest(new { status = "error", message = "Failed to add student!" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = ex.Message });
            }
        }


        [HttpPost]
        public IActionResult GetAllStudents(getallstudents getallstudents)
        {
            try
            {
                string user_id = getallstudents.user_id;
                string json = "";
                if (user_id != null)
                {
                    //string query = $"select name from bs_all_students where user_id = '{user_id}'";
                    string query = $"select id,student_name from bs_student_master_backup where sys_user_id = '{user_id}'";

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

        public bool deleteAllStudents(getallstudents std)
        {
            string json = "";
            try
            {
                #region DELETE ALL STUDENTS QUERY
                string query = $"delete  from bs_student_master_backup where id = '{std.user_id}'";
                #endregion

                int rowaffected = _sql_qury_execution.DML_Insert_Update_Delete(query);
                if (rowaffected > 0)
                {
                    return true;
                }

                return false;
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return false;

            }//catch block ends.

        }



        [HttpPost]
        ///<summary>
        /// 
        /// sInBulk actionmethod assign students to stops in bulk.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public async Task<IActionResult> assignStudentsInBulk([FromForm] IFormFile file, [FromForm] string r_id, [FromForm] string schoolid)
        {
            List<error> errors = new List<error>();
            general gen = new general();
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No File Uploaded");
                }
                if (!file.FileName.EndsWith(".xlsx"))
                {
                    return BadRequest("Only .xlsx files are accepted.");
                }

                string get_all_students_from_school = $"SELECT * FROM bs_student_master_backup WHERE sys_user_id = {schoolid}";
                DataTable allstudents = _sql_qury_execution.DML_Select(get_all_students_from_school);

                if (allstudents != null && allstudents.Rows.Count > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        stream.Position = 0;
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                        using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                        {
                            DataSet dataset = excelReader.AsDataSet(new ExcelDataSetConfiguration
                            {
                                ConfigureDataTable = data => new ExcelDataTableConfiguration
                                {
                                    UseHeaderRow = true
                                }
                            });

                            DataTable dataTable = dataset.Tables[0];
                            int result = 0;
                            int stopid;

                            foreach (DataRow dataRow in dataTable.Rows)
                            {
                                string student_name = dataRow["Student Name"].ToString();
                                string admission_no = dataRow["Admission No"].ToString();
                                string Stopid = dataRow["Stop Id"].ToString();
                                string RF_ID = dataRow["RF_ID"].ToString();

                                if (string.IsNullOrEmpty(student_name) || string.IsNullOrEmpty(admission_no))
                                {
                                    errors.Add(new error
                                    {
                                        admission_no = admission_no,
                                        student_name = student_name,
                                        Stopid = Stopid,
                                        rfid = RF_ID,
                                        reason_for_not_linked = "Student name or Admission No is missing"
                                    });
                                    continue;
                                }

                                if (!int.TryParse(Stopid, out stopid))
                                {
                                    errors.Add(new error
                                    {
                                        admission_no = admission_no,
                                        student_name = student_name,
                                        Stopid = Stopid,
                                        rfid = RF_ID,
                                        reason_for_not_linked = "Invalid Stop ID"
                                    });
                                    continue;
                                }

                                var res = from allstudent in allstudents.AsEnumerable()
                                          where allstudent.Field<string>("student_name").Trim().ToLower() == student_name.Trim().ToLower()
                                          && allstudent.Field<string>("admission_no").Trim() == admission_no.Trim()
                                          select allstudent;


                                if (res.Any())
                                {
                                    foreach (DataRow row in res)
                                    {
                                        string getrecord_from_bs_user_master = $"SELECT id FROM bs_user_master WHERE bs_user_name='{row["mobile_no1"]}'";
                                        //Console.WriteLine(getrecord_from_bs_user_master);
                                        DataTable dt = _sql_qury_execution.DML_Select(getrecord_from_bs_user_master);

                                        //  RFID update only if provided
                                        string query_for_update_RFID = !string.IsNullOrWhiteSpace(RF_ID)
                                            ? $@"UPDATE bs_student_master_backup SET rf_id = '{RF_ID}' WHERE id = {row["Id"]}"
                                            : null;

                                        string query_for_insert_in_bs_route_students = $@"INSERT INTO bs_route_students
                                                                    (route_id, stop_id, student_id)
                                                                    VALUES ({r_id}, {Stopid}, {row["Id"]})";

                                        string query_to_check_stop = $"SELECT id FROM bs_stop_master WHERE id = {Stopid}";
                                        DataTable stops = _sql_qury_execution.DML_Select(query_to_check_stop);

                                        if (stops != null && stops.Rows.Count > 0)
                                        {
                                            if (stops.Rows[0]["id"].ToString() == Stopid)
                                            {
                                                if (dt != null && dt.Rows.Count > 0)
                                                {
                                                    int rowaffected = query_for_update_RFID != null
                                                        ? _sql_qury_execution.DML_Insert_Update_Delete_with_Transaction(query_for_update_RFID, query_for_insert_in_bs_route_students)
                                                        : _sql_qury_execution.DML_Insert_Update_Delete_with_Transaction(query_for_insert_in_bs_route_students);

                                                    if (rowaffected >= 1)
                                                        result += 1;
                                                }
                                                else if (dt.Rows.Count == 0)
                                                {
                                                    string query_for_insert_in_bs_user_master = $@"INSERT INTO bs_user_master
                                                                               (sys_user_id, bs_user_name, bs_password)
                                                                                VALUES ({schoolid}, '{row["mobile_1"]}', 12345)";

                                                    int rowaffected = query_for_update_RFID != null
                                                        ? _sql_qury_execution.DML_Insert_Update_Delete_with_Transaction(query_for_update_RFID, query_for_insert_in_bs_route_students, query_for_insert_in_bs_user_master)
                                                        : _sql_qury_execution.DML_Insert_Update_Delete_with_Transaction(query_for_insert_in_bs_route_students, query_for_insert_in_bs_user_master);

                                                    if (rowaffected >= 2)
                                                        result += 1;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            errors.Add(new error
                                            {
                                                admission_no = admission_no,
                                                student_name = student_name,
                                                Stopid = Stopid,
                                                rfid = RF_ID,
                                                reason_for_not_linked = "Stop is not added"
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    errors.Add(new error
                                    {
                                        admission_no = admission_no,
                                        student_name = student_name,
                                        Stopid = Stopid,
                                        rfid = RF_ID,
                                        reason_for_not_linked = "Student not found in school records"
                                    });
                                }
                            }

                            if (errors.Count() > 0)
                            {
                                gen.saveListToExcel(errors, r_id);
                            }
                            return Content($"{result} students are assigned successfully");
                        }
                    }
                }
                else
                {
                    return Content("No students found in the school records.");
                }
            }
            catch (Exception ex)
            {
                return Content("An error occurred while processing the bulk student assignment.");
            }
        }





        // [HttpPost]
        ///<summary>
        /// assignStudentsInBulk actionmethod assign students to stops in bulk.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        //public async Task<IActionResult> assignStudentsInBulk([FromForm] IFormFile file, [FromForm] string r_id, [FromForm] string schoolid)
        //{

        //    List<error> errors = new List<error>();
        //    general gen = new general();
        //    try
        //    {
        //        if (file == null || file.Length == 0)
        //        {
        //            return BadRequest("No File Uploaded");
        //        }
        //        if (!file.FileName.EndsWith(".xlsx"))
        //        {
        //            return BadRequest("Only .xlsx files are accepted.");

        //        }
        //        string get_all_students_from_school = $"select * from bs_student_master_backup where sys_user_id = {schoolid}";
        //        DataTable allstudents = _sql_qury_execution.DML_Select(get_all_students_from_school);
        //        if (allstudents != null)
        //        {
        //            if (allstudents.Rows.Count > 0)
        //            {

        //                //process the file
        //                using (var stream = new MemoryStream())
        //                {

        //                    await file.CopyToAsync(stream);
        //                    stream.Position = 0;
        //                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        //                    using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
        //                    {
        //                        DataSet dataset = excelReader.AsDataSet(new ExcelDataSetConfiguration
        //                        {
        //                            ConfigureDataTable = data => new ExcelDataTableConfiguration
        //                            {
        //                                UseHeaderRow = true // This treats the first row as headers
        //                            }
        //                        });
        //                        DataTable dataTable = dataset.Tables[0];
        //                        int result = 0;
        //                        int stopid;
        //                        foreach (DataRow dataRow in dataTable.Rows)
        //                        {

        //                            string student_name = dataRow["Student Name"].ToString();
        //                            if (string.IsNullOrEmpty(student_name))
        //                            {
        //                                errors.Add(new error
        //                                {
        //                                    admission_no = $"{dataRow["Admission No"].ToString()}",
        //                                    student_name
        //                                = $"{dataRow["Student Name"].ToString()}",
        //                                    Stopid = $"{dataRow["Stop Id"].ToString()}",
        //                                    rfid
        //                                = $"{dataRow["RF_ID"].ToString()}",
        //                                    reason_for_not_linked = "Student name is null or empty"
        //                                });
        //                                continue;
        //                            }
        //                            string admission_no = dataRow["Admission No"].ToString();
        //                            if (string.IsNullOrEmpty(student_name))
        //                            {
        //                                errors.Add(new error
        //                                {
        //                                    admission_no = $"{dataRow["Admission No"].ToString()}",
        //                                    student_name
        //                                = $"{dataRow["Student Name"].ToString()}",
        //                                    Stopid = $"{dataRow["Stop Id"].ToString()}",
        //                                    rfid
        //                                = $"{dataRow["RF_ID"].ToString()}",
        //                                    reason_for_not_linked = "Admission number is null or empty"
        //                                });
        //                                continue;
        //                            }

        //                            string Stopid = dataRow["Stop Id"].ToString();
        //                            if (!(int.TryParse(Stopid, out stopid)))
        //                            {
        //                                errors.Add(new error
        //                                {
        //                                    admission_no = $"{dataRow["Admission No"].ToString()}",
        //                                    student_name
        //                                = $"{dataRow["Student Name"].ToString()}",
        //                                    Stopid = $"{dataRow["Stop Id"].ToString()}",
        //                                    rfid
        //                                = $"{dataRow["RF_ID"].ToString()}",
        //                                    reason_for_not_linked = "Stop id is wrong"
        //                                });
        //                                continue;
        //                            }
        //                            string RF_ID = dataRow["RF_ID"].ToString();
        //                            if (string.IsNullOrEmpty(RF_ID))
        //                            {
        //                                errors.Add(new error
        //                                {
        //                                    admission_no = $"{dataRow["Admission No"].ToString()}",
        //                                    student_name
        //                                = $"{dataRow["Student Name"].ToString()}",
        //                                    Stopid = $"{dataRow["Stop Id"].ToString()}",
        //                                    rfid
        //                                = $"{dataRow["RF_ID"].ToString()}",
        //                                    reason_for_not_linked = "RFID is null or empty"
        //                                });
        //                                continue;
        //                            }

        //                            var res = from allstudent in allstudents.AsEnumerable()
        //                                      where allstudent.Field<string>("student_name") == student_name
        //                                      && allstudent.Field<string>("admission_no") == admission_no
        //                                      select allstudent;
        //                            if (res.Count() == 1)
        //                            {
        //                                foreach (DataRow row in res)
        //                                {
        //                                    string getrecord_from_bs_user_master = $"select id from bs_user_master where bs_user_name='{row["mobile_no1"]}'";
        //                                    DataTable dt = _sql_qury_execution.DML_Select(getrecord_from_bs_user_master);
        //                                    string query_for_update_RFID = $@"update bs_student_master_backup
        //                                                                  set rf_id = '{RF_ID}'
        //                                                                  where id = {row["Id"]}";
        //                                    string query_for_insert_in_bs_route_students = $@"insert into bs_route_students
        //                                                                                  (route_id,stop_id,student_id)
        //                                                                                  values({r_id},{Stopid},{row["Id"]})";

        //                                    string query_to_check_stop = $"select id,user_stop_name from bs_stop_master where id = {Stopid}";
        //                                    DataTable stops = _sql_qury_execution.DML_Select(query_to_check_stop);
        //                                    if (stops != null && stops.Rows.Count > 0)
        //                                    {
        //                                        if (stops.Rows[0]["id"].ToString() == Stopid)
        //                                        {
        //                                            if (dt != null && dt.Rows.Count > 0)
        //                                            {
        //                                                int rowaffected = _sql_qury_execution.DML_Insert_Update_Delete_with_Transaction(query_for_update_RFID, query_for_insert_in_bs_route_students);
        //                                                if (rowaffected == 2)
        //                                                {
        //                                                    result += 1;
        //                                                }
        //                                            }
        //                                            else if (dt.Rows.Count == 0)
        //                                            {
        //                                                string query_for_insert_in_bs_user_master = $@"insert into bs_user_master
        //                                                                                   (sys_user_id,bs_user_name,bs_password)
        //                                                                                    values ({schoolid},'{row["mobile_1"]}',12345)";
        //                                                int rowaffected = _sql_qury_execution.DML_Insert_Update_Delete_with_Transaction(query_for_update_RFID, query_for_insert_in_bs_route_students, query_for_insert_in_bs_user_master);
        //                                                if (rowaffected == 3)
        //                                                {
        //                                                    result += 1;
        //                                                }
        //                                            }
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        errors.Add(new error
        //                                        {
        //                                            admission_no = $"{dataRow["Admission No"].ToString()}",
        //                                            student_name
        //                                = $"{dataRow["Student Name"].ToString()}",
        //                                            Stopid = $"{dataRow["Stop Id"].ToString()}",
        //                                            rfid
        //                                = $"{dataRow["RF_ID"].ToString()}",
        //                                            reason_for_not_linked = $"Stop is not added"
        //                                        });
        //                                    }

        //                                }
        //                            }

        //                            else if (res.Count() > 1)
        //                            {
        //                                errors.Add(new error
        //                                {
        //                                    admission_no = $"{dataRow["Admission No"].ToString()}",
        //                                    student_name
        //                                = $"{dataRow["Student Name"].ToString()}",
        //                                    Stopid = $"{dataRow["Stop Id"].ToString()}",
        //                                    rfid
        //                                = $"{dataRow["RF_ID"].ToString()}",
        //                                    reason_for_not_linked = "Duplicated Entries"
        //                                });
        //                            }
        //                            else if (res.Count() == 0)
        //                            {
        //                                errors.Add(new error
        //                                {
        //                                    admission_no = $"{dataRow["Admission No"].ToString()}",
        //                                    student_name
        //                                = $"{dataRow["Student Name"].ToString()}",
        //                                    Stopid = $"{dataRow["Stop Id"].ToString()}",
        //                                    rfid
        //                                = $"{dataRow["RF_ID"].ToString()}",
        //                                    reason_for_not_linked = "Student are not in the school"
        //                                });
        //                            }


        //                        }

        //                        if (errors.Count() > 0)
        //                        {
        //                            gen.saveListToExcel(errors, r_id);
        //                        }
        //                        return Content($"{result} students are assigned successfully");
        //                    }




        //                }
        //            }
        //            else
        //            {
        //                return Content("Students are not in the school");
        //            }
        //        }

        //    }

        //    catch (Exception ex)
        //    {
        //        return Content("Invalid Excel Sheet.");
        //    }



        //    return BadRequest("Invalid Excel Sheet.");


        //}





        //[HttpPost]
        /////<summary>
        ///// assignStudentsInBulk actionmethod assign students to stops in bulk.
        /////if database is newtrack, table used : telemetry_month.
        /////if database is alttracking, table used : tbl_telemetry_month.
        /////</summary>
        //public async Task<IActionResult> assignStudent_(assign_student AS)
        //{


        //    general gen = new general();
        //    try
        //    {

        //        string get_all_students_from_school = $"select * from bs_student_master_backup where sys_user_id = {AS.schoolid}";
        //        DataTable allstudents = _sql_qury_execution.DML_Select(get_all_students_from_school);
        //        if (allstudents != null)
        //        {
        //            if (allstudents.Rows.Count > 0)
        //            {

        //                //process the file
        //                using (var stream = new MemoryStream())
        //                {

        //                    await file.CopyToAsync(stream);
        //                    stream.Position = 0;
        //                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        //                    using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
        //                    {
        //                        DataSet dataset = excelReader.AsDataSet(new ExcelDataSetConfiguration
        //                        {
        //                            ConfigureDataTable = data => new ExcelDataTableConfiguration
        //                            {
        //                                UseHeaderRow = true // This treats the first row as headers
        //                            }
        //                        });
        //                        DataTable dataTable = dataset.Tables[0];
        //                        int result = 0;
        //                        int stopid;
        //                        foreach (DataRow dataRow in dataTable.Rows)
        //                        {

        //                            string student_name = dataRow["Student Name"].ToString();
        //                            if (string.IsNullOrEmpty(student_name))
        //                            {
        //                                errors.Add(new error
        //                                {
        //                                    admission_no = $"{dataRow["Admission No"].ToString()}",
        //                                    student_name
        //                                = $"{dataRow["Student Name"].ToString()}",
        //                                    Stopid = $"{dataRow["Stop Id"].ToString()}",
        //                                    rfid
        //                                = $"{dataRow["RF_ID"].ToString()}",
        //                                    reason_for_not_linked = "Student name is null or empty"
        //                                });
        //                                continue;
        //                            }
        //                            string admission_no = dataRow["Admission No"].ToString();
        //                            if (string.IsNullOrEmpty(student_name))
        //                            {
        //                                errors.Add(new error
        //                                {
        //                                    admission_no = $"{dataRow["Admission No"].ToString()}",
        //                                    student_name
        //                                = $"{dataRow["Student Name"].ToString()}",
        //                                    Stopid = $"{dataRow["Stop Id"].ToString()}",
        //                                    rfid
        //                                = $"{dataRow["RF_ID"].ToString()}",
        //                                    reason_for_not_linked = "Admission number is null or empty"
        //                                });
        //                                continue;
        //                            }

        //                            string Stopid = dataRow["Stop Id"].ToString();
        //                            if (!(int.TryParse(Stopid, out stopid)))
        //                            {
        //                                errors.Add(new error
        //                                {
        //                                    admission_no = $"{dataRow["Admission No"].ToString()}",
        //                                    student_name
        //                                = $"{dataRow["Student Name"].ToString()}",
        //                                    Stopid = $"{dataRow["Stop Id"].ToString()}",
        //                                    rfid
        //                                = $"{dataRow["RF_ID"].ToString()}",
        //                                    reason_for_not_linked = "Stop id is wrong"
        //                                });
        //                                continue;
        //                            }
        //                            string RF_ID = dataRow["RF_ID"].ToString();
        //                            if (string.IsNullOrEmpty(RF_ID))
        //                            {
        //                                errors.Add(new error
        //                                {
        //                                    admission_no = $"{dataRow["Admission No"].ToString()}",
        //                                    student_name
        //                                = $"{dataRow["Student Name"].ToString()}",
        //                                    Stopid = $"{dataRow["Stop Id"].ToString()}",
        //                                    rfid
        //                                = $"{dataRow["RF_ID"].ToString()}",
        //                                    reason_for_not_linked = "RFID is null or empty"
        //                                });
        //                                continue;
        //                            }

        //                            var res = from allstudent in allstudents.AsEnumerable()
        //                                      where allstudent.Field<string>("student_name") == student_name
        //                                      && allstudent.Field<string>("admission_no") == admission_no
        //                                      select allstudent;
        //                            if (res.Count() == 1)
        //                            {
        //                                foreach (DataRow row in res)
        //                                {
        //                                    string getrecord_from_bs_user_master = $"select id from bs_user_master where bs_user_name='{row["mobile_no1"]}'";
        //                                    DataTable dt = _sql_qury_execution.DML_Select(getrecord_from_bs_user_master);
        //                                    string query_for_update_RFID = $@"update bs_student_master_backup
        //                                                                  set rf_id = '{RF_ID}'
        //                                                                  where id = {row["Id"]}";
        //                                    string query_for_insert_in_bs_route_students = $@"insert into bs_route_students
        //                                                                                  (route_id,stop_id,student_id)
        //                                                                                  values({r_id},{Stopid},{row["Id"]})";

        //                                    string query_to_check_stop = $"select id,user_stop_name from bs_stop_master where id = {Stopid}";
        //                                    DataTable stops = _sql_qury_execution.DML_Select(query_to_check_stop);
        //                                    if (stops != null && stops.Rows.Count > 0)
        //                                    {
        //                                        if (stops.Rows[0]["id"].ToString() == Stopid)
        //                                        {
        //                                            if (dt != null && dt.Rows.Count > 0)
        //                                            {
        //                                                int rowaffected = _sql_qury_execution.DML_Insert_Update_Delete_with_Transaction(query_for_update_RFID, query_for_insert_in_bs_route_students);
        //                                                if (rowaffected == 2)
        //                                                {
        //                                                    result += 1;
        //                                                }
        //                                            }
        //                                            else if (dt.Rows.Count == 0)
        //                                            {
        //                                                string query_for_insert_in_bs_user_master = $@"insert into bs_user_master
        //                                                                                   (sys_user_id,bs_user_name,bs_password)
        //                                                                                    values ({schoolid},'{row["mobile_1"]}',12345)";
        //                                                int rowaffected = _sql_qury_execution.DML_Insert_Update_Delete_with_Transaction(query_for_update_RFID, query_for_insert_in_bs_route_students, query_for_insert_in_bs_user_master);
        //                                                if (rowaffected == 3)
        //                                                {
        //                                                    result += 1;
        //                                                }
        //                                            }
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        errors.Add(new error
        //                                        {
        //                                            admission_no = $"{dataRow["Admission No"].ToString()}",
        //                                            student_name
        //                                = $"{dataRow["Student Name"].ToString()}",
        //                                            Stopid = $"{dataRow["Stop Id"].ToString()}",
        //                                            rfid
        //                                = $"{dataRow["RF_ID"].ToString()}",
        //                                            reason_for_not_linked = $"Stop is not added"
        //                                        });
        //                                    }

        //                                }
        //                            }

        //                            else if (res.Count() > 1)
        //                            {
        //                                errors.Add(new error
        //                                {
        //                                    admission_no = $"{dataRow["Admission No"].ToString()}",
        //                                    student_name
        //                                = $"{dataRow["Student Name"].ToString()}",
        //                                    Stopid = $"{dataRow["Stop Id"].ToString()}",
        //                                    rfid
        //                                = $"{dataRow["RF_ID"].ToString()}",
        //                                    reason_for_not_linked = "Duplicated Entries"
        //                                });
        //                            }
        //                            else if (res.Count() == 0)
        //                            {
        //                                errors.Add(new error
        //                                {
        //                                    admission_no = $"{dataRow["Admission No"].ToString()}",
        //                                    student_name
        //                                = $"{dataRow["Student Name"].ToString()}",
        //                                    Stopid = $"{dataRow["Stop Id"].ToString()}",
        //                                    rfid
        //                                = $"{dataRow["RF_ID"].ToString()}",
        //                                    reason_for_not_linked = "Student are not in the school"
        //                                });
        //                            }


        //                        }

        //                        if (errors.Count() > 0)
        //                        {
        //                            gen.saveListToExcel(errors, r_id);
        //                        }
        //                        return Content($"{result} students are assigned successfully");
        //                    }




        //                }
        //            }
        //            else
        //            {
        //                return Content("Students are not in the school");
        //            }
        //        }

        //    }

        //    catch (Exception ex)
        //    {
        //        return Content("Invalid Excel Sheet.");
        //    }



        //    return BadRequest("Invalid Excel Sheet.");


        //}




        [HttpPost]
        ///<summary>
        /// assignStudent actionmethod assign students to stops.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        //public async Task<IActionResult> assignStudent(assign_singal_student assign_Student)
        //{
        //    List<error> errors = new List<error>();
        //    general gen = new general();
        //    try
        //    {
        //        int result = 0;
        //        string get_all_students_from_school = $"select * from bs_student_master_backup where sys_user_id = {assign_Student.schoolid}";
        //        DataTable allstudents = _sql_qury_execution.DML_Select(get_all_students_from_school);
        //        if (allstudents != null)
        //        {
        //            if (allstudents.Rows.Count > 0)
        //            {
        //                var res = from allstudent in allstudents.AsEnumerable()
        //                          where allstudent.Field<string>("student_name") == assign_Student.student_name
        //                          && allstudent.Field<string>("admission_no") == assign_Student.Admission_no
        //                          && (allstudent.Field<string>("rf_id") == assign_Student.rfid
        //                          || allstudent.Field<string>("rf_id") == "NULL"
        //                          || allstudent.Field<string>("rf_id") == " ")
        //                          select allstudent;
        //                if (res.Count() == 1)
        //                {
        //                    foreach (DataRow row in res)
        //                    {
        //                        string getrecord_from_bs_user_master = $"select id from bs_user_master where bs_user_name='{row["mobile_no1"]}'";
        //                        DataTable dt = _sql_qury_execution.DML_Select(getrecord_from_bs_user_master);
        //                        string query_for_update_RFID = $@"update bs_student_master_backup
        //                                                            set rf_id = '{assign_Student.rfid}'
        //                                                            where id = {row["Id"]}";
        //                        string query_for_insert_in_bs_route_students = $@"insert into bs_route_students
        //                                                                            (route_id,stop_id,student_id)
        //                                                                            values({assign_Student.route_id},{assign_Student.stopid},{row["Id"]})";

        //                        string query_to_check_stop = $"select id,user_stop_name from bs_stop_master where id = {assign_Student.stopid}";
        //                        DataTable stops = _sql_qury_execution.DML_Select(query_to_check_stop);
        //                        if (stops != null && stops.Rows.Count > 0)
        //                        {
        //                            if (stops.Rows[0]["id"].ToString() == assign_Student.stopid)
        //                            {
        //                                if (dt != null && dt.Rows.Count > 0)
        //                                {
        //                                    int rowaffected = _sql_qury_execution.DML_Insert_Update_Delete_with_Transaction(query_for_update_RFID, query_for_insert_in_bs_route_students);
        //                                    if (rowaffected == 2)
        //                                    {
        //                                        result += 1;
        //                                    }
        //                                }
        //                                else if (dt.Rows.Count == 0)
        //                                {
        //                                    string query_for_insert_in_bs_user_master = $@"insert into bs_user_master
        //                                                                            (sys_user_id,bs_user_name,bs_password)
        //                                                                            values ({assign_Student.schoolid},'{row["mobile_1"]}',12345)";
        //                                    int rowaffected = _sql_qury_execution.DML_Insert_Update_Delete_with_Transaction(query_for_update_RFID, query_for_insert_in_bs_route_students, query_for_insert_in_bs_user_master);
        //                                    if (rowaffected == 3)
        //                                    {
        //                                        result += 1;
        //                                    }
        //                                }
        //                            }
        //                        }


        //                    }
        //                }

        //                return Content($"{result} students are assigned successfully");



        //            }
        //            else
        //            {
        //                return Content("Students are not in the school");
        //            }
        //        }

        //    }

        //    catch (Exception ex)
        //    {
        //        return Content("Invalid Excel Sheet.");
        //    }



        //    return BadRequest("Invalid Excel Sheet.");



        //}


        public async Task<IActionResult> assignStudent(assign_singal_student assign_Student)
        {
            List<error> errors = new List<error>();
            general gen = new general();
            try
            {
                int result = 0;
                string get_all_students_from_school = $"SELECT * FROM bs_student_master_backup WHERE sys_user_id = {assign_Student.schoolid}";
                DataTable allstudents = _sql_qury_execution.DML_Select(get_all_students_from_school);

                if (allstudents != null && allstudents.Rows.Count > 0)
                {
                    var res = from allstudent in allstudents.AsEnumerable()
                              where allstudent.Field<string>("student_name") == assign_Student.student_name
                              && allstudent.Field<string>("admission_no") == assign_Student.Admission_no
                              select allstudent;

                    if (res.Any()) // If student exists
                    {
                        foreach (DataRow row in res)
                        {
                            string getrecord_from_bs_user_master = $"SELECT id FROM bs_user_master WHERE bs_user_name='{row["mobile_no1"]}'";
                            DataTable dt = _sql_qury_execution.DML_Select(getrecord_from_bs_user_master);

                            // If RFID is provided, update it; otherwise, don't update the RFID field.
                            string query_for_update_RFID = !string.IsNullOrWhiteSpace(assign_Student.rfid)
                                ? $@"UPDATE bs_student_master_backup SET rf_id = '{assign_Student.rfid}' WHERE id = {row["Id"]}"
                                : null;

                            string query_for_insert_in_bs_route_students = $@"INSERT INTO bs_route_students
                                                                       (route_id, stop_id, student_id)
                                                                       VALUES ({assign_Student.route_id}, {assign_Student.stopid}, {row["Id"]})";

                            string query_to_check_stop = $"SELECT id FROM bs_stop_master WHERE id = {assign_Student.stopid}";
                            DataTable stops = _sql_qury_execution.DML_Select(query_to_check_stop);

                            if (stops != null && stops.Rows.Count > 0)
                            {
                                if (stops.Rows[0]["id"].ToString() == assign_Student.stopid)
                                {
                                    if (dt != null && dt.Rows.Count > 0)
                                    {
                                        int rowaffected = query_for_update_RFID != null
                                            ? _sql_qury_execution.DML_Insert_Update_Delete_with_Transaction(query_for_update_RFID, query_for_insert_in_bs_route_students)
                                            : _sql_qury_execution.DML_Insert_Update_Delete_with_Transaction(query_for_insert_in_bs_route_students);

                                        if (rowaffected >= 1)
                                            result += 1;
                                    }
                                    else if (dt.Rows.Count == 0)
                                    {
                                        string query_for_insert_in_bs_user_master = $@"INSERT INTO bs_user_master
                                                                               (sys_user_id, bs_user_name, bs_password)
                                                                               VALUES ({assign_Student.schoolid}, '{row["mobile_1"]}', 12345)";

                                        int rowaffected = query_for_update_RFID != null
                                            ? _sql_qury_execution.DML_Insert_Update_Delete_with_Transaction(query_for_update_RFID, query_for_insert_in_bs_route_students, query_for_insert_in_bs_user_master)
                                            : _sql_qury_execution.DML_Insert_Update_Delete_with_Transaction(query_for_insert_in_bs_route_students, query_for_insert_in_bs_user_master);

                                        if (rowaffected >= 2)
                                            result += 1;
                                    }
                                }
                            }
                        }
                        return Content($"{result} students are assigned successfully");
                    }
                    else
                    {
                        return Content("Student not found in the school records.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("An error occurred while assigning the student.");
            }

            return BadRequest("Invalid request.");
        }




        [HttpPost]
        ///<summary>
        /// addStudentsInBulk actionmethod add students in bulk.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public async Task<IActionResult> addStudentsInBulk([FromForm] IFormFile file, [FromForm] string user_id)
        {
            List<string> errors = new List<string>();
            int insertedCount = 0;
            int duplicateCount = 0;

            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { status = "error", message = "No File Uploaded" });
                }
                if (!file.FileName.EndsWith(".xlsx"))
                {
                    return BadRequest(new { status = "error", message = "Only .xlsx files are accepted." });
                }

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    using (IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(stream))
                    {
                        DataSet dataset = excelReader.AsDataSet(new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = data => new ExcelDataTableConfiguration { UseHeaderRow = true }
                        });

                        DataTable dataTable = dataset.Tables[0];

                        foreach (DataRow row in dataTable.Rows)
                        {
                            try
                            {
                                //  Extract and sanitize fields
                                string student_name = row["student_name"]?.ToString()?.Replace("'", "''").Trim() ?? "";
                                string admission_no = row["admission_no"]?.ToString()?.Replace("'", "''").Trim() ?? "";
                                string mobile_no1 = row["mobile_no1"]?.ToString()?.Replace("'", "''").Trim() ?? "";
                                string birth_date = row["birth"]?.ToString()?.Trim() ?? "";
                                string division = row["division"]?.ToString()?.Replace("'", "''").Trim() ?? "";
                                string parent_name = row["parent_name"]?.ToString()?.Replace("'", "''").Trim() ?? "";
                                string address = row["address"]?.ToString()?.Replace("'", "''").Trim() ?? "";
                                string email = row["email"]?.ToString()?.Replace("'", "''").Trim() ?? "";
                                string class_name = row["class"]?.ToString()?.Replace("'", "''").Trim() ?? "";

                                if (!int.TryParse(class_name, out int classId) || classId < 1 || classId > 18)
                                {
                                    errors.Add($"Invalid class value for Admission No: {admission_no}. Class should be must in 1 to 18 digits only.");
                                    continue;
                                }

                                if (string.IsNullOrEmpty(student_name) || string.IsNullOrEmpty(admission_no) || string.IsNullOrEmpty(mobile_no1))
                                {
                                    errors.Add($"Skipping row: Missing required fields - Admission No: {admission_no}, Name: {student_name}");
                                    continue;
                                }

                                //  Convert date format
                                string formatted_birth_date = "NULL";
                                if (DateTime.TryParse(birth_date, out DateTime parsedDate))
                                {
                                    formatted_birth_date = $"'{parsedDate:yyyy-MM-dd}'";
                                }

                                // ✅ Check if parent exists in bs_user_master
                                string queryCheckUser = $"SELECT id FROM bs_user_master WHERE bs_user_name = '{mobile_no1}';";
                                DataTable userTable = _sql_qury_execution.DML_Select(queryCheckUser);
                                string parent_id = "";

                                if (userTable != null && userTable.Rows.Count > 0)
                                {
                                    parent_id = userTable.Rows[0]["id"].ToString();
                                }
                                else
                                {
                                    //  Insert new user into bs_user_master
                                    string insertUserQuery = $@"
                            INSERT INTO bs_user_master (sys_user_id, role_id, bs_user_name, bs_password, name) 
                            VALUES ('{user_id}', 1, '{mobile_no1}', '12345', '{parent_name}');";

                                    int userInserted = _sql_qury_execution.DML_Insert_Update_Delete(insertUserQuery);

                                    if (userInserted > 0)
                                    {
                                        //  Retrieve the newly inserted parent_id
                                        DataTable newUserTable = _sql_qury_execution.DML_Select(queryCheckUser);
                                        if (newUserTable != null && newUserTable.Rows.Count > 0)
                                        {
                                            parent_id = newUserTable.Rows[0]["id"].ToString();
                                        }
                                    }
                                    else
                                    {
                                        errors.Add($"Failed to create user record for: {parent_name} ({mobile_no1})");
                                        continue;
                                    }
                                }

                                //  Check if student already exists in the same school
                                string queryCheckStudent = $@"
                                SELECT id, admission_no 
                                FROM bs_student_master_backup 
                                WHERE sys_user_id = '{user_id}' 
                                  AND admission_no = '{admission_no}'";

                                DataTable studentTable = _sql_qury_execution.DML_Select(queryCheckStudent);
                                if (studentTable != null && studentTable.Rows.Count > 0)
                                {
                                    duplicateCount++;
                                    errors.Add($"Admission No {admission_no} already exists in this school.");
                                    continue;
                                }

                                //  Insert student record
                                string insertStudentQuery = $@"
                        INSERT INTO bs_student_master_backup 
                        (bs_user_id, parent_id, admission_no, student_name, dob, class, section, father_name, mobile_no1, street, email, added_on, sys_user_id) 
                        VALUES 
                        ('{parent_id}', '{parent_id}', '{admission_no}', '{student_name}', {formatted_birth_date}, 
                        '{class_name}', '{division}', '{parent_name}', '{mobile_no1}', '{address}', '{email}', GETDATE(), '{user_id}');";

                                int rowsAffected = _sql_qury_execution.DML_Insert_Update_Delete(insertStudentQuery);
                                if (rowsAffected > 0)
                                {
                                    insertedCount++;
                                }
                                else
                                {
                                    errors.Add($"Database insert failed for: {student_name}");
                                }
                            }
                            catch (Exception rowEx)
                            {
                                errors.Add($"Error inserting student: {rowEx.Message}");
                            }
                        }
                    }
                }

                if (insertedCount > 0)
                {
                    string message = $"{insertedCount} students added successfully!";
                    if (duplicateCount > 0)
                    {
                        message += $" {duplicateCount} admission numbers already exist in this school, so duplicate records were not inserted.";
                    }

                    return Ok(new
                    {
                        status = "success",
                        message = message,
                        errors
                    });
                }
                else
                {
                    string message = "No students were added.";
                    if (duplicateCount > 0)
                    {
                        message = $"{duplicateCount} admission numbers already exist in this school, so duplicate records were not inserted.";
                    }

                    return BadRequest(new
                    {
                        status = "error",
                        message = message,
                        errors
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = $"An error occurred: {ex.Message}", errors });
            }
        }


        [HttpGet]
        ///<summary>
        /// getclasses actionmethod gets all classes.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult getclasses()
        {
            try
            {
                //string user_id = getallstudents.user_id;
                string json = "";
                //if (user_id != null)
                {
                    //string query = $"select name from bs_all_students where user_id = '{user_id}'";
                    //string query = $"select * from bs_class_master";
                    string query = @"SELECT
    id,
    CASE id
        WHEN 11 THEN 'PRE PRIMARY'
        WHEN 12 THEN 'PRE SCHOOL'
        WHEN 17 THEN 'NURSERY'
        WHEN 18 THEN 'PRE NURSERY'
        WHEN 1  THEN 'K.G./BAL VATIKA 1'
        WHEN 16 THEN 'UKG/BAL VATIKA 3'
        WHEN 2  THEN 'I'
        WHEN 3  THEN 'II'
        WHEN 4  THEN 'III'
        WHEN 5  THEN 'IV'
        WHEN 6  THEN 'V'
        WHEN 7  THEN 'VI'
        WHEN 8  THEN 'VII'
        WHEN 9  THEN 'VIII'
        WHEN 10 THEN 'IX'
        WHEN 15 THEN 'X'
        WHEN 13 THEN 'XI'
        WHEN 14 THEN 'XII'
        ELSE class_name
    END AS class_name
FROM bs_class_master
ORDER BY
    CASE id
        WHEN 11 THEN 1
        WHEN 12 THEN 2
        WHEN 17 THEN 3
        WHEN 18 THEN 4
        WHEN 1  THEN 5
        WHEN 16 THEN 6
        WHEN 2  THEN 7
        WHEN 3  THEN 8
        WHEN 4  THEN 9
        WHEN 5  THEN 10
        WHEN 6  THEN 11
        WHEN 7  THEN 12
        WHEN 8  THEN 13
        WHEN 9  THEN 14
        WHEN 10 THEN 15
        WHEN 15 THEN 16
        WHEN 13 THEN 17
        WHEN 14 THEN 18
        ELSE 999
    END;";

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
                 //else

            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return Content("0");

            }//catch block ends.

        }

        [HttpPost]
        public async Task<IActionResult> UpdateRFIDInBulk([FromForm] IFormFile file, [FromForm] string user_id)
        {
            List<string> errors = new List<string>();
            int updatedCount = 0;

            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { status = "error", message = "No file uploaded" });

                if (!file.FileName.EndsWith(".xlsx"))
                    return BadRequest(new { status = "error", message = "Only .xlsx files are accepted." });

                Dictionary<string, string> admissionRfidMap = new Dictionary<string, string>();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true
                            }
                        });

                        DataTable table = result.Tables[0];

                        foreach (DataRow row in table.Rows)
                        {
                            string admissionNo = row["Admission No"]?.ToString()?.Trim();
                            string rfid = row["RFID"]?.ToString()?.Trim();

                            if (string.IsNullOrEmpty(admissionNo) || string.IsNullOrEmpty(rfid))
                            {
                                errors.Add($"Missing Admission No or RFID in row: {JsonConvert.SerializeObject(row.ItemArray)}");
                                continue;
                            }

                            if (!admissionRfidMap.ContainsKey(admissionNo))
                            {
                                admissionRfidMap.Add(admissionNo, rfid);
                            }
                        }
                    }
                }

                if (admissionRfidMap.Count == 0)
                {
                    return BadRequest(new { status = "error", message = "No valid records found.", errors });
                }

                // Step 1: Validate all admission numbers in one query
                string inClause = string.Join(",", admissionRfidMap.Keys.Select(x => $"'{x}'"));
                string validateQuery = $"SELECT admission_no FROM bs_student_master_backup WHERE admission_no IN ({inClause}) AND sys_user_id = '{user_id}'";

                DataTable existing = _sql_qury_execution.DML_Select(validateQuery);
                HashSet<string> existingAdmissionNos = new HashSet<string>(
                    existing.AsEnumerable().Select(row => row["admission_no"].ToString())
                );

                // Step 2: Prepare CASE WHEN update block
                var updateCases = new StringBuilder();
                var validAdmissionNos = new List<string>();

                foreach (var kvp in admissionRfidMap)
                {
                    if (existingAdmissionNos.Contains(kvp.Key))
                    {
                        updateCases.AppendLine($"WHEN '{kvp.Key}' THEN '{kvp.Value}'");
                        validAdmissionNos.Add($"'{kvp.Key}'");
                    }
                    else
                    {
                        errors.Add($"Student with Admission No '{kvp.Key}' not found.");
                    }
                }

                if (validAdmissionNos.Count > 0)
                {
                    string updateQuery = $@"
                UPDATE bs_student_master_backup
                SET rf_id = CASE admission_no
                    {updateCases.ToString()}
                END
                WHERE admission_no IN ({string.Join(",", validAdmissionNos)})
                  AND sys_user_id = '{user_id}'
            ";

                    updatedCount = _sql_qury_execution.DML_Insert_Update_Delete(updateQuery);
                }

                if (updatedCount > 0)
                {
                    return Ok(new
                    {
                        status = "success",
                        message = $"{updatedCount} RFID(s) updated successfully.",
                        errors
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        status = "error",
                        message = "No records updated.",
                        errors
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Exception occurred.",
                    error = ex.Message
                });
            }
        }

    }
}
