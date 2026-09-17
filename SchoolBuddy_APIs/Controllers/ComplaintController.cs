using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolBuddy_APIs.Database_methods;
using SchoolBuddy_APIs.Models.Parent_Complain_Record;
using System.Data;
using System.Reflection.Metadata.Ecma335;

namespace SchoolBuddy_APIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ComplaintController : ControllerBase
    {
        private readonly Idatabase_access _sql_qury_execution;
        public ComplaintController(Idatabase_access sql_qury_execution)
        {
            _sql_qury_execution = sql_qury_execution;
        }

        public class Complaint
        {
            public int Id { get; set; }
            public string ComplaintText { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Complaint>>> GetComplaints()
        {
            string query = "SELECT id, complaint FROM bs_complaint_master";

            // Execute the query and fetch data
            DataTable result = _sql_qury_execution.DML_Select(query);

            // Map the result to a list of Complaint objects
            var complaints = new List<Complaint>();
            foreach (DataRow row in result.Rows)
            {
                complaints.Add(new Complaint
                {
                    Id = row.Field<int>("id"),
                    ComplaintText = row.Field<string>("complaint")
                });
            }

            return Ok(complaints);
        }


        [HttpPost]
        public async Task<IActionResult> CreateParentComplaint([FromBody] ParentComplaintRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Description))
                {
                    return BadRequest(new { status = "0", msg = "Invalid input" });
                }

                string query = $"INSERT INTO bs_parent_complaint (complaint_id, bs_user_id, description, raised_on, is_active) " +
                               $"VALUES ('{request.ComplaintId}', '{request.BsUserId}', '{request.Description}', '{DateTime.Now:yyyy-MM-dd HH:mm:ss}', 1)";

                int rowsAffected = _sql_qury_execution.DML_Insert_Update_Delete(query);

                if (rowsAffected > 0)
                {
                    return Ok(new { status = "1", msg = "Complaint successfully created" });
                }
                else
                {
                    return StatusCode(500, new { status = "0", msg = "Failed to insert complaint" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "0", msg = "An error occurred", error = ex.Message });
            }
        }

        public class ParentComplaintRequest
        {
            public int ComplaintId { get; set; }
            public int BsUserId { get; set; }
            public string Description { get; set; }
        }




         [HttpPost]

        ///<summary>
        /// GetPendingComplaint actionmethod fetches pending request from the tables.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult GetPendingComplaint(string user_id)
        {
            try
            {
                //string user_id = getallstudents.user_id;
                string json = "";
                //if (user_id != null)
                {
                    //string query = $"select name from bs_all_students where user_id = '{user_id}'";
                    string query = @"select  p.id,p.description,p.raised_on,s.student_name,r.route_name,c.complaint,s.mobile_no1 
                                        from bs_parent_complaint p 
                                        inner join bs_student_master_backup s 
                                        on s.bs_user_id= p.bs_user_id 
                                        inner join bs_route_students rs on 
                                        rs.student_id = s.id inner join bs_route_master r on r.id = rs.route_id 
                                        inner join tbl_users u on u.id = r.sys_user_id
                                        inner join bs_complaint_master c on c.id=p.complaint_id  where p.is_active = 1 and u.id = " + user_id + " order by p.id";

                    DataTable datatable = _sql_qury_execution.DML_Select(query);
                    if (datatable != null)
                    {
                        if (datatable.Rows.Count > 0)
                        {
                            var settings = new JsonSerializerSettings
                            {
                                DateFormatString = "yyyy-MM-dd HH:mm:ss"
                            };
                            json = JsonConvert.SerializeObject(datatable, Formatting.Indented, settings);
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
                //{
                //    return Content("0");
                //}//user id null.
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return Content("0");

            }//catch block ends.

        }




        [HttpPost]

        public IActionResult ComplaintPage(string user_id)
        {
            try
            {
                if (string.IsNullOrEmpty(user_id))
                {
                    return Content("0");
                }


                string query = @"
            SELECT
                p.id AS complaint_id,

                -- Parent
                s.father_name AS parent_name,
                s.mobile_no1 AS mobile_no,

                -- Complaint
                c.complaint,
                p.description,
                p.raised_on,
                p.is_active,

                -- Student
                s.id AS student_id,
                s.student_name,

                -- Pick Route
                MAX(
                    CASE
                        WHEN UPPER(r.route_name) LIKE '%PICK%'
                        THEN r.route_name
                    END
                ) AS pick_route,

                -- Drop Route
                MAX(
                    CASE
                        WHEN UPPER(r.route_name) LIKE '%DROP%'
                        THEN r.route_name
                    END
                ) AS drop_route

            FROM bs_parent_complaint p

            INNER JOIN bs_student_master_backup s
                ON s.bs_user_id = p.bs_user_id

            INNER JOIN bs_route_students rs
                ON rs.student_id = s.id

            INNER JOIN bs_route_master r
                ON r.id = rs.route_id

            INNER JOIN tbl_users u
                ON u.id = r.sys_user_id

            INNER JOIN bs_complaint_master c
                ON c.id = p.complaint_id

            WHERE
                p.is_active = 1
                AND u.id = " + user_id + @"

            GROUP BY
                p.id,
                s.father_name,
                s.mobile_no1,
                c.complaint,
                p.description,
                p.raised_on,
                p.is_active,
                s.id,
                s.student_name

            ORDER BY
                p.id DESC,
                s.id;
        ";

                DataTable datatable =
                    _sql_qury_execution.DML_Select(query);


                // =========================================================
                // 2. GET SUMMARY COUNTS
                // =========================================================

                string summaryQuery = @"
            SELECT

                -- Total unique complaints
                COUNT(DISTINCT p.id) AS totalComplaints,

                -- Pending unique complaints
                COUNT(
                    DISTINCT CASE
                        WHEN p.is_active = 1
                        THEN p.id
                    END
                ) AS pendingComplaints,

                -- Resolved unique complaints
                COUNT(
                    DISTINCT CASE
                        WHEN p.is_active = 0
                        THEN p.id
                    END
                ) AS resolvedComplaints,

                -- Unique students having complaints
                COUNT(DISTINCT s.id) AS students

            FROM bs_parent_complaint p

            INNER JOIN bs_student_master_backup s
                ON s.bs_user_id = p.bs_user_id

            INNER JOIN bs_route_students rs
                ON rs.student_id = s.id

            INNER JOIN bs_route_master r
                ON r.id = rs.route_id

            INNER JOIN tbl_users u
                ON u.id = r.sys_user_id

            WHERE
                u.id = " + user_id + @";
        ";

                DataTable summaryTable =
                    _sql_qury_execution.DML_Select(summaryQuery);


                // =========================================================
                // 3. READ SUMMARY
                // =========================================================

                int totalComplaints = 0;
                int pendingComplaints = 0;
                int resolvedComplaints = 0;
                int students = 0;

                if (summaryTable != null && summaryTable.Rows.Count > 0)
                {
                    DataRow row = summaryTable.Rows[0];

                    totalComplaints =
                        Convert.ToInt32(row["totalComplaints"]);

                    pendingComplaints =
                        Convert.ToInt32(row["pendingComplaints"]);

                    resolvedComplaints =
                        Convert.ToInt32(row["resolvedComplaints"]);

                    students =
                        Convert.ToInt32(row["students"]);
                }


                // =========================================================
                // 4. PREPARE RESPONSE
                // =========================================================

                var result = new
                {
                    summary = new
                    {
                        totalComplaints = totalComplaints,
                        pendingComplaints = pendingComplaints,
                        resolvedComplaints = resolvedComplaints,
                        students = students
                    },

                    data = datatable
                };


                // =========================================================
                // 5. RETURN JSON
                // =========================================================

                return Content(
                    JsonConvert.SerializeObject(
                        result,
                        Formatting.Indented
                    ),
                    "application/json"
                );
            }
            catch (Exception ex)
            {
                string err_msg = ex.Message;

                return Content("0");
            }
        }


        [HttpPost]

        public IActionResult ResolvedComplaintPage(string user_id)
        {
            try
            {
                if (string.IsNullOrEmpty(user_id))
                {
                    return Content("0");
                }


                string query = @"
    SELECT
        p.id AS complaint_id,

        -- Parent
        s.father_name AS parent_name,

        -- Complaint
        c.complaint,
        p.description,
        p.raised_on,

        -- Resolution
        p.comments,
        p.resolved_on,

        p.is_active,

        -- Student
        s.id AS student_id,
        s.student_name,

        -- Pick Route
        MAX(
            CASE
                WHEN UPPER(r.route_name) LIKE '%PICK%'
                THEN r.route_name
            END
        ) AS pick_route,

        -- Drop Route
        MAX(
            CASE
                WHEN UPPER(r.route_name) LIKE '%DROP%'
                THEN r.route_name
            END
        ) AS drop_route

    FROM bs_parent_complaint p

    INNER JOIN bs_student_master_backup s
        ON s.bs_user_id = p.bs_user_id

    INNER JOIN bs_route_students rs
        ON rs.student_id = s.id

    INNER JOIN bs_route_master r
        ON r.id = rs.route_id

    INNER JOIN tbl_users u
        ON u.id = r.sys_user_id

    INNER JOIN bs_complaint_master c
        ON c.id = p.complaint_id

    WHERE
        p.is_active = 0
        AND u.id = " + user_id + @"

    GROUP BY
        p.id,
        s.father_name,
        c.complaint,
        p.description,
        p.raised_on,
        p.comments,
        p.resolved_on,
        p.is_active,
        s.id,
        s.student_name

    ORDER BY
        p.id DESC,
        s.id;
";

                DataTable datatable =
                    _sql_qury_execution.DML_Select(query);


                // =========================================================
                // 2. GET SUMMARY COUNTS
                // =========================================================

                string summaryQuery = @"
            SELECT

                -- Total unique complaints
                COUNT(DISTINCT p.id) AS totalComplaints,

                -- Pending unique complaints
                COUNT(
                    DISTINCT CASE
                        WHEN p.is_active = 1
                        THEN p.id
                    END
                ) AS pendingComplaints,

                -- Resolved unique complaints
                COUNT(
                    DISTINCT CASE
                        WHEN p.is_active = 0
                        THEN p.id
                    END
                ) AS resolvedComplaints,

                -- Unique students having complaints
                COUNT(DISTINCT s.id) AS students

            FROM bs_parent_complaint p

            INNER JOIN bs_student_master_backup s
                ON s.bs_user_id = p.bs_user_id

            INNER JOIN bs_route_students rs
                ON rs.student_id = s.id

            INNER JOIN bs_route_master r
                ON r.id = rs.route_id

            INNER JOIN tbl_users u
                ON u.id = r.sys_user_id

            WHERE
                u.id = " + user_id + @";
        ";

                DataTable summaryTable =
                    _sql_qury_execution.DML_Select(summaryQuery);


                // =========================================================
                // 3. READ SUMMARY
                // =========================================================

                int totalComplaints = 0;
                int pendingComplaints = 0;
                int resolvedComplaints = 0;
                int students = 0;

                if (summaryTable != null && summaryTable.Rows.Count > 0)
                {
                    DataRow row = summaryTable.Rows[0];

                    totalComplaints =
                        Convert.ToInt32(row["totalComplaints"]);

                    pendingComplaints =
                        Convert.ToInt32(row["pendingComplaints"]);

                    resolvedComplaints =
                        Convert.ToInt32(row["resolvedComplaints"]);

                    students =
                        Convert.ToInt32(row["students"]);
                }


                // =========================================================
                // 4. PREPARE RESPONSE
                // =========================================================

                var result = new
                {
                    summary = new
                    {
                        totalComplaints = totalComplaints,
                        pendingComplaints = pendingComplaints,
                        resolvedComplaints = resolvedComplaints,
                        students = students
                    },

                    data = datatable
                };


                // =========================================================
                // 5. RETURN JSON
                // =========================================================

                return Content(
                    JsonConvert.SerializeObject(
                        result,
                        Formatting.Indented
                    ),
                    "application/json"
                );
            }
            catch (Exception ex)
            {
                string err_msg = ex.Message;

                return Content("0");
            }
        }

        [HttpPost]

        ///<summary>
        /// GetResolvedComplaint actionmethod fetches all resolved requests from the tables.
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public IActionResult GetResolvedComplaint(string user_id)
        {
            try
            {
                //string user_id = getallstudents.user_id;
                string json = "";
                //if (user_id != null)
                {
                    //string query = $"select name from bs_all_students where user_id = '{user_id}'";
                    string query = @"select  p.*,s.student_name,r.route_name,u.sys_username,c.complaint,s.mobile_no1 from bs_parent_complaint p inner join bs_student_master_backup s on s.bs_user_id= p.bs_user_id inner join bs_route_students rs on 
                         rs.student_id = s.id inner join bs_route_master r on r.id = rs.route_id inner join tbl_users u on u.id = r.sys_user_id
                         inner join bs_complaint_master c on c.id=p.complaint_id  where p.is_active = 0 and u.id = " + user_id + "  order by p.id";

                    DataTable datatable = _sql_qury_execution.DML_Select(query);
                    if (datatable != null)
                    {
                        if (datatable.Rows.Count > 0)
                        {
                            var settings = new JsonSerializerSettings
                            {
                                DateFormatString = "yyyy-MM-dd HH:mm:ss"
                            };
                            json = JsonConvert.SerializeObject(datatable, Formatting.Indented, settings);
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
                //{
                //    return Content("0");
                //}//user id null.
            }//try block ends.
            catch (Exception ex)
            {
                string err_msg = ex.Message;
                return Content("0");

            }//catch block ends.

        }


        [HttpPost]


        ///<summary>
        /// UpdatedComplaint actionmethod update bs_parent_complaint tablewith is_active=0 when complaint resolved .
        ///if database is newtrack, table used : telemetry_month.
        ///if database is alttracking, table used : tbl_telemetry_month.
        ///</summary>
        public bool UpdatedComplaint(complain complain)
        {
            try
            {
                //string user_id = getallstudents.user_id;
                string json = "";
                if (complain.userid != 0)
                {
                    //string query = $"select name from bs_all_students where user_id = '{user_id}'";
                    string query = $@"update bs_parent_complaint set is_active=0 , resolved_on=getDate(),comments='{complain.comment}' where id= {complain.Id}";

                    int rowaffected = _sql_qury_execution.DML_Insert_Update_Delete(query);
                    if (rowaffected > 0)
                    {
                        return true;
                    }//datatable is not null

                    return false;

                }
                return false;
            }//try block ends.
            catch (Exception ex)
            {
                return false;

            }//catch block ends.

        }

    }
}
