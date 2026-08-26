using Microsoft.AspNetCore.Mvc;
using SchoolBuddy_APIs.Database_methods;
using SchoolBuddy_APIs.Models.Indoor;
using System.Data;
using System.Data.SqlClient;

namespace SchoolBuddy_APIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class visitorController : Controller
    {
        private readonly Idatabase_access _sql_qury_execution;

        public visitorController(Idatabase_access sql_qury_execution)
        {
            _sql_qury_execution = sql_qury_execution;
        }
        [HttpPost]
        public IActionResult AddVisitor([FromBody] Visitor model)
        {
            try
            {
                //  SELECT (get last visitor no)
                string selectQuery = @"
                SELECT ISNULL(MAX(CAST(REPLACE(visitor_no, 'Visitor ', '') AS INT)), 0)
                FROM bs_visitor_master
                WHERE sys_user_id = " + model.SysUserId + @"
                AND in_time >= CAST(GETDATE() AS DATE)
                AND in_time < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))";

                DataTable result = _sql_qury_execution.DML_Select(selectQuery);
                int lastNo = 0;
                if (result.Rows.Count > 0)
                {
                    lastNo = Convert.ToInt32(result.Rows[0][0]);
                }


                int newNo = lastNo + 1;
                string visitorNo = "Visitor " + newNo;

                string insertQuery = @"
INSERT INTO bs_visitor_master
(
    name,
    mobile_no,
    card_id,
    card_no,
    visitor_no,
    in_time,
    out_time,
    sys_user_id
)
VALUES
(
    '" + model.Name + @"',
    '" + model.MobileNo + @"',
    " + model.CardId + @",
    '" + model.CardNo + @"',
    '" + visitorNo + @"',
    '" + Convert.ToDateTime(model.in_time).ToString("yyyy-MM-dd HH:mm:ss") + @"',
    '" + Convert.ToDateTime(model.out_time).ToString("yyyy-MM-dd HH:mm:ss") + @"',
    " + model.SysUserId + @"
)";


                int rowsAffected = _sql_qury_execution.DML_Insert_Update_Delete(insertQuery);

                if (rowsAffected > 0)
                {
                    return Ok(new
                    {
                        message = "Visitor added successfully",
                        visitor_no = visitorNo
                    });
                }
                return StatusCode(500, "Failed to add visitor");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }

        }
    }
}