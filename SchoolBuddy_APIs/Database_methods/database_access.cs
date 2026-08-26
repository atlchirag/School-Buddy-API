using System.Data;
using System.Data.SqlClient;

namespace SchoolBuddy_APIs.Database_methods
{
    public class database_access : Idatabase_access
    {
        private readonly string _connectionString;
        private readonly IConfiguration _config;

        public database_access(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("sqlconnectionstring");
        }

        // ================= INSERT / UPDATE / DELETE =================
        public int DML_Insert_Update_Delete(string query)
        {
            int row_affected = 0;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.CommandTimeout = 30;
                    sqlConnection.Open();
                    row_affected = command.ExecuteNonQuery();
                }

                return row_affected;
            }
            catch (Exception ex)
            {
                LogError("DML_Insert_Update_Delete", query, ex);
                return 0;
            }
        }

        // ================= SELECT =================
        public DataTable? DML_Select(string query, Dictionary<string, object>? parameters = null)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.CommandTimeout = 30;

                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        sqlConnection.Open();
                        dataAdapter.Fill(dataTable);
                    }
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                LogError("DML_Select", query, ex);
                return null;
            }
        }

        // ================= TRANSACTION =================
        public int DML_Insert_Update_Delete_with_Transaction(params string[] queries)
        {
            int row_affected = 0;

            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (SqlTransaction transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        foreach (var query in queries)
                        {
                            using (SqlCommand command = new SqlCommand(query, sqlConnection, transaction))
                            {
                                command.CommandTimeout = 30;
                                row_affected += command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return row_affected;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch { }

                        LogError("DML_Transaction", string.Join(" | ", queries), ex);
                        return 0;
                    }
                }
            }
        }

        // ================= COMMON LOGGER =================
        private void LogError(string method, string query, Exception ex)
        {
            try
            {
                string logFolder = @"D:\SB_LoginDebugLogs";

                if (!Directory.Exists(logFolder))
                {
                    Directory.CreateDirectory(logFolder);
                }

                string logFile = Path.Combine(logFolder, $"db_log_{DateTime.Now:yyyyMMdd}.txt");

                File.AppendAllText(
                    logFile,
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] METHOD: {method}{Environment.NewLine}" +
                    $"QUERY: {query}{Environment.NewLine}" +
                    $"ERROR: {ex}{Environment.NewLine}" +
                    $"--------------------------------------------------{Environment.NewLine}"
                );
            }
            catch
            {
                // avoid crash due to logging failure
            }
        }
    }
}











































//using System.Data;
//using System.Data.Common;
//using System.Data.SqlClient;

//namespace SchoolBuddy_APIs.Database_methods
//{
//    public class database_access :Idatabase_access
//    {
//        private static SqlConnection? _sqlConnection;
//        private IConfiguration _config;
//        public database_access(IConfiguration config)
//        {
//            _config = config; 
//            _sqlConnection = new SqlConnection(_config.GetConnectionString("sqlconnectionstring"));

//        }
//        public int DML_Insert_Update_Delete(string query)
//        {
//            int row_affected = 0;
//            try
//            {

//                SqlCommand command = new SqlCommand(query, _sqlConnection);
//                _sqlConnection.Open();
//                row_affected = command.ExecuteNonQuery();
//                if (row_affected>0)
//                {
//                    return row_affected;
//                }
//                return row_affected;
//            }
//            catch (Exception ex)
//            {
//                return row_affected;
//            }
//            finally
//            {
//                if (_sqlConnection.State == ConnectionState.Open)
//                {
//                    _sqlConnection.Close();
//                }

//            }

//        }

//        public DataTable DML_Select(string query)
//        {
//            DataTable? dataTable = new DataTable();
//            try
//            {
//                SqlCommand command = new SqlCommand(query, _sqlConnection);
//                _sqlConnection.Open();
//                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
//                dataAdapter.Fill(dataTable);
//                if (dataTable!=null)
//                {
//                    return dataTable;
//                }
//                return dataTable;
//            }
//            catch (Exception ex)
//            {
//                return dataTable;
//            }
//            finally
//            {
//                if (_sqlConnection.State==ConnectionState.Open)
//                {
//                    _sqlConnection.Close();
//                }

//            }

//        }

//        public int DML_Insert_Update_Delete_with_Transaction(params string[] queries)
//        {
//            SqlTransaction transaction = null;
//            int row_affected = 0;
//            try
//            {
//                _sqlConnection.Open();
//                transaction = _sqlConnection.BeginTransaction();
//                foreach (var query in queries)
//                {
//                    SqlCommand command = new SqlCommand(query, _sqlConnection,transaction);

//                    row_affected += command.ExecuteNonQuery();

//                }
//                transaction.Commit(); 
//                return row_affected;

//            }
//            catch (Exception ex)
//            {
//                transaction.Rollback();
//                return row_affected;
//            }
//            finally
//            {
//                if (_sqlConnection.State == ConnectionState.Open)
//                {
//                    _sqlConnection.Close();
//                }

//            }

//        }
//    }
//}
