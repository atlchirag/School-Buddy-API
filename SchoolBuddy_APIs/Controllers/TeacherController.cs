using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolBuddy_APIs.Database_methods;
using SchoolBuddy_APIs.Models.Master.Students;
using System.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace SchoolBuddy_APIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly Idatabase_access _sql_qury_execution;
        public TeacherController(Idatabase_access sql_qury_execution)
        {
            _sql_qury_execution = sql_qury_execution;
        }

        [HttpPost]
        public async Task<IActionResult> AddTeacher()
        {
            try
            {
                string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();

                TeacherModel model = JsonConvert.DeserializeObject<TeacherModel>(requestBody);


                if (model == null || model.Uid == 0)
                    return BadRequest("Invalid data: Uid is missing.");

                DataTable dt = _sql_qury_execution.DML_Select($"SELECT id FROM bs_teacher_master WHERE class={model.ClassId} AND section='{model.Section}' AND sys_user_id={model.Uid}");
                if (dt.Rows.Count > 0)
                    return Conflict();

                string query = $"INSERT INTO s VALUES ({model.Uid}, '{model.TeacherName}', '{model.TeacherLogin}', '{model.Password}', {model.ClassId}, '{model.Section}', '{model.StartTime}')";
                int result = _sql_qury_execution.DML_Insert_Update_Delete(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error in API: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTeacher()
        {
            string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();

            TeacherModel model = JsonConvert.DeserializeObject<TeacherModel>(requestBody);

            if (model == null || model.Id == 0)
                return BadRequest("Invalid teacher data");

            string query = $@"
        UPDATE bs_teacher_master
        SET teacher_name = '{model.TeacherName}', 
            teacher_login = '{model.TeacherLogin}', 
            teacher_pass = '{model.Password}', 
            class = {model.ClassId}, 
            section = '{model.Section}', 
            start_time = '{model.StartTime}'
        WHERE id = {model.Id}";

            int result = _sql_qury_execution.DML_Insert_Update_Delete(query);
            return result > 0 ? Ok("Teacher updated successfully") : BadRequest("Update failed");
        }

        [HttpDelete]
        public IActionResult DeleteTeacher(int id)
        {
            if (id == 0)
                return BadRequest("Invalid teacher ID");

            string query = $"DELETE FROM bs_teacher_master WHERE id = {id}";
            int result = _sql_qury_execution.DML_Insert_Update_Delete(query);

            return result > 0 ? Ok("Teacher deleted successfully") : BadRequest("Failed to delete teacher");
        }


        //[HttpDelete]
        //public IActionResult DeleteTeacher(int id)
        //{
        //    string query = $"DELETE FROM bs_teacher_master WHERE id={id}";
        //    int result = _sql_qury_execution.DML_Insert_Update_Delete(query);
        //    return Ok(result);
        //}

        [HttpGet]
        public IActionResult GetTeacherById(int id)
        {
            if (id == 0)
                return BadRequest("Invalid teacher ID");

          //  DataTable dt = _sql_qury_execution.DML_Select($"SELECT * FROM bs_teacher_master WHERE id={id}");
            DataTable dt = _sql_qury_execution.DML_Select($@"
        SELECT t.id, t.sys_user_id AS Uid, t.teacher_name, t.teacher_login, t.teacher_pass, 
               t.class AS ClassId, t.section, t.start_time, c.class_name 
        FROM bs_teacher_master t 
        INNER JOIN bs_class_master c ON c.id = t.class 
        WHERE t.id={id}");

            if (dt.Rows.Count == 0)
                return NotFound();

            DataRow row = dt.Rows[0];
            TeacherModel teacher = new TeacherModel
            {
                 Id = Convert.ToInt32(row["id"]),
                 Uid = Convert.ToInt32(row["Uid"]),
                TeacherName = row["teacher_name"].ToString(),
                TeacherLogin = row["teacher_login"].ToString(),
                Password = row["teacher_pass"].ToString(),
                ClassId = Convert.ToInt32(row["ClassId"]),
                Section = row["section"].ToString(),
                StartTime = row["start_time"].ToString(),
                ClassName = row["class_name"].ToString(),

                //  ConfirmPassword = row["teacher_pass"].ToString() // Match password
            };

            return Ok(teacher);
        }


        [HttpGet]
        public IActionResult GetAllTeachers(int uid)
        {
            if (uid == 0)
                return Unauthorized();

            DataTable dt = _sql_qury_execution.DML_Select($@"
        SELECT t.id, t.sys_user_id AS Uid, t.teacher_name, t.teacher_login, t.teacher_pass AS Password, 
               t.class AS ClassId, t.section, t.start_time, c.class_name 
        FROM bs_teacher_master t 
        INNER JOIN bs_class_master c ON c.id = t.class 
        WHERE sys_user_id={uid} 
        ORDER BY t.id");

            // Convert DataTable to List<TeacherModel>
            List<TeacherModel> teachers = new List<TeacherModel>();

           if(dt!=null && dt.Rows.Count>0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    teachers.Add(new TeacherModel
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Uid = Convert.ToInt32(row["Uid"]),
                        TeacherName = row["teacher_name"].ToString(),
                        TeacherLogin = row["teacher_login"].ToString(),
                        Password = row["Password"].ToString(),
                        ClassId = Convert.ToInt32(row["ClassId"]),
                        Section = row["section"].ToString(),
                        StartTime = row["start_time"].ToString(),
                        ClassName = row["class_name"].ToString(),
                        // ConfirmPassword = row["Password"].ToString() // Ensuring ConfirmPassword is same for compatibility
                    });
                }
            }
            else
            {
                return Ok(teachers);
            }

            return Ok(teachers);
        }


        [HttpGet]
        public IActionResult GetClasses()
        {
            DataTable dt = _sql_qury_execution.DML_Select("SELECT id, class_name FROM bs_class_master");
            return Ok(JsonConvert.SerializeObject(dt));
        }

        [HttpGet]
        public IActionResult GetSections(int uid)
        {
            if (uid == 0)
                return Unauthorized();

            DataTable dt = _sql_qury_execution.DML_Select($"SELECT DISTINCT LTRIM(RTRIM(UPPER(section))) AS section FROM bs_student_master_backup WHERE sys_user_id={uid} ORDER BY section");
            return Ok(JsonConvert.SerializeObject(dt));
        }
    }

    public class TeacherModel
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public string TeacherName { get; set; }
        public string TeacherLogin { get; set; }
        public string Password { get; set; }
        public int ClassId { get; set; }
        public string Section { get; set; }
        public string StartTime { get; set; }
        public string ClassName { get; set; }
    }
}
