using System.Data.SqlClient;
using System.Data;

namespace SchoolBuddy_APIs.Database_methods
{
    public interface Idatabase_access
    {
        public int DML_Insert_Update_Delete(string query, Dictionary<string, object>? parameters = null);
        public DataTable? DML_Select(string query, Dictionary<string, object>? parameters = null);
        public int DML_Insert_Update_Delete_with_Transaction(params string[] queries);
        public int DML_Insert_Update_Delete_with_Transaction(params (string query, Dictionary<string, object>? parameters)[] commands);




    }
}
