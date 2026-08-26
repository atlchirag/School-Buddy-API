using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using SchoolBuddy_APIs.Models.Halts;

namespace SchoolBuddy_APIs.Services
{
    public class HaltsTelemetryService
    {
        private readonly string _conn;

        public HaltsTelemetryService(IConfiguration cfg)
        {
            _conn = cfg.GetConnectionString("sqlconnectionstring")
                    ?? throw new InvalidOperationException("Missing DefaultConnection");
        }

        private static string Suffix(DateTime d) =>
            d.ToString("MMMyy", CultureInfo.InvariantCulture).ToLower();

        private async Task<string> ResolveUserDatabase(SqlConnection conn, int schoolId,string pass,string user)
        {
            using var cmd = new SqlCommand(@"
                IF EXISTS (SELECT 1 FROM atltracking.dbo.tbl_users WHERE id = @schoolId and sys_username=@user and sys_password=@pass)
                    SELECT 'atltracking'
                ELSE IF EXISTS (SELECT 1 FROM users WHERE id = @schoolId and sys_username=@user and sys_password=@pass)
                    SELECT 'newtrack'
                ELSE
                    SELECT '';", conn);

            cmd.Parameters.AddWithValue("@schoolId", schoolId);
            cmd.Parameters.AddWithValue("@user", user);
            cmd.Parameters.AddWithValue("@pass", pass);

            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString() ?? "";
        }

        private async Task<string?> ResolveTelemetryTable(SqlConnection conn, DateTime d, int schoolId,string pass,string user)
        {
            var suf = Suffix(d);
            var dbType = await ResolveUserDatabase(conn, schoolId,pass,user);

            List<string> candidates = new();

            if (dbType == "atltracking")
            {
                candidates.Add($"atltracking.dbo.tbl_telemetry_{suf}");
                candidates.Add($"atltracking.dbo.telemetry_{suf}");
            }
            else
            {
                candidates.Add($"dbo.telemetry_{suf}");
                candidates.Add($"dbo.tbl_telemetry_{suf}");
            }

            foreach (var t in candidates)
            {
                using var check = new SqlCommand($"SELECT OBJECT_ID('{t}')", conn);
                var obj = await check.ExecuteScalarAsync();
                if (obj != null && obj != DBNull.Value)
                    return t;
            }

            return null;
        }

        public async Task<List<RouteDto>> GetAllSchoolsAsync()
        {
            var list = new List<RouteDto>();
            using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(@"
                SELECT id, sys_username
                FROM users
                WHERE is_school_buddy = 1

                UNION

                SELECT id, sys_username
                FROM atltracking.dbo.tbl_users
                WHERE is_school_buddy = 1

                ORDER BY sys_username;", conn);

            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                list.Add(new RouteDto
                {
                    RouteId = Convert.ToInt32(r.GetInt64(0)),
                    RouteName = r.GetString(1)
                });
            }
            return list;
        }

        public async Task<List<RouteDto>> GetRoutesForSchoolAsync(int schoolId)
        {
            var list = new List<RouteDto>();
            using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(@"
                SELECT DISTINCT brm.Id AS RouteId, brm.route_name
                FROM bs_route_master brm
                WHERE brm.sys_user_id = @schoolId
                ORDER BY brm.route_name;", conn);

            cmd.Parameters.AddWithValue("@schoolId", schoolId);

            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                list.Add(new RouteDto
                {
                    RouteId = r.GetInt32(0),
                    RouteName = r.GetString(1)
                });
            }
            return list;
        }

        public async Task<RouteTelemetryResponse> GetRouteTelemetryAsync(int schoolId, int routeId, DateTime date,string pass, string user)
        {
            var resp = new RouteTelemetryResponse { RouteId = routeId };
            using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            var table = await ResolveTelemetryTable(conn, date, schoolId,pass,user)
                        ?? throw new Exception($"No telemetry table for {Suffix(date)}");

            var morningStart = date.Date.AddHours(5);
            var morningEnd = date.Date.AddHours(9).AddMinutes(30);
            var afternoonStart = date.Date.AddHours(12).AddMinutes(30);
            var afternoonEnd = date.Date.AddHours(17);

            var sql = $@"
SELECT brm.route_name, lt.gps_time, lt.gps_latitude, lt.gps_longitude, lt.gps_speed
FROM bs_route_master brm
JOIN {table} lt ON lt.sys_service_id = brm.sys_service_id
WHERE brm.Id = @routeId
  AND brm.sys_user_id = @schoolId
  AND (
        lt.sys_proc_time BETWEEN @mStart AND @mEnd
        OR
        lt.sys_proc_time BETWEEN @aStart AND @aEnd
      )
ORDER BY lt.sys_proc_time;";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@routeId", routeId);
            cmd.Parameters.AddWithValue("@schoolId", schoolId);
            cmd.Parameters.AddWithValue("@mStart", morningStart);
            cmd.Parameters.AddWithValue("@mEnd", morningEnd);
            cmd.Parameters.AddWithValue("@aStart", afternoonStart);
            cmd.Parameters.AddWithValue("@aEnd", afternoonEnd);

            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                if (string.IsNullOrWhiteSpace(resp.RouteName))
                    resp.RouteName = r.GetString(0);

                resp.Points.Add(new TelemetryPoint
                {
                    GpsTime = r.GetDateTime(1),
                    Latitude = r.GetDouble(2),
                    Longitude = r.GetDouble(3),
                    Speed = r.GetDouble(4)
                });
            }

            return resp;
        }

        public async Task<List<MultiDayTelemetryResponse>> GetMultiDayTelemetryAsync(
            int schoolId, int routeId, DateTime startDate, DateTime endDate,string pass,string user)
        {
            var outList = new List<MultiDayTelemetryResponse>();
            using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            int idx = 0;
            for (var d = startDate.Date; d <= endDate.Date; d = d.AddDays(1), idx++)
            {
                var table = await ResolveTelemetryTable(conn, d, schoolId,pass,user);
                if (table is null)
                {
                    outList.Add(new MultiDayTelemetryResponse
                    {
                        DayIndex = idx,
                        Date = d,
                        Points = new List<TelemetryPoint>()
                    });
                    continue;
                }

                var mStart = d.Date.AddHours(5);
                var mEnd = d.Date.AddHours(9).AddMinutes(30);
                var aStart = d.Date.AddHours(12).AddMinutes(30);
                var aEnd = d.Date.AddHours(17);

                var sql = $@"
SELECT lt.gps_time, lt.gps_latitude, lt.gps_longitude, lt.gps_speed
FROM bs_route_master brm
JOIN {table} lt ON lt.sys_service_id = brm.sys_service_id
WHERE brm.Id = @routeId
  AND brm.sys_user_id = @schoolId
  AND (
        lt.sys_proc_time BETWEEN @mStart AND @mEnd
        OR
        lt.sys_proc_time BETWEEN @aStart AND @aEnd
      )
ORDER BY lt.sys_proc_time;";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@routeId", routeId);
                cmd.Parameters.AddWithValue("@schoolId", schoolId);
                cmd.Parameters.AddWithValue("@mStart", mStart);
                cmd.Parameters.AddWithValue("@mEnd", mEnd);
                cmd.Parameters.AddWithValue("@aStart", aStart);
                cmd.Parameters.AddWithValue("@aEnd", aEnd);

                var res = new MultiDayTelemetryResponse
                {
                    DayIndex = idx,
                    Date = d,
                    Points = new List<TelemetryPoint>()
                };

                using var r = await cmd.ExecuteReaderAsync();
                while (await r.ReadAsync())
                {
                    res.Points.Add(new TelemetryPoint
                    {
                        GpsTime = r.GetDateTime(0),
                        Latitude = r.GetDouble(1),
                        Longitude = r.GetDouble(2),
                        Speed = r.GetDouble(3)
                    });
                }

                outList.Add(res);
            }

            return outList;
        }
    }
}