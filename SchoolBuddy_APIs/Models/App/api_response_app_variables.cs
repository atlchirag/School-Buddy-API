using System.Data;

namespace SchoolBuddy_APIs.Models.App
{

    
    public class api_response_app_variables
    {
        public string msg { get; set; }
        public login response { get; set; }
        public string status { get; set; }

    }

    public class api_response_app_leave_request
    {
        public string msg { get; set; }

        public List<LeaveRequestResponse> response { get; set; }
        public string status { get; set; }

    }
    public class api_app_leave_request
    {
        public string msg { get; set; }
        public string status { get; set; }

    }


    public class LeaveRequestResponse
    {
        public int Id { get; set; }
        public string student_name { get; set; }
        public string student_id { get; set; }
        public string Reason { get; set; }
        public DateTime LeaveDate { get; set; }
        public string IsApproved { get; set; }
        public DateTime AppliedOn { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>bs_leave_master.status values.</summary>
    public static class LeaveStatus
    {
        public const int Applied = 0;
        public const int Approved = 1;
        public const int Rejected = 2;
        public const int Cancelled = 3;

        public static bool IsActionable(int status) =>
            status == Approved || status == Rejected || status == Cancelled;

        public static string Text(int status) => status switch
        {
            Approved => "Approved",
            Rejected => "Rejected",
            Cancelled => "Cancelled",
            _ => "Applied"
        };
    }

    public class login
    {
        public string? id { get; set; }
        public string ?schoolId { get; set; }
        public string? contact { get; set; }
        public string? fatherName { get; set; }
        public string? motherName { get; set; }
        public string? address { get; set; }
        public string? email { get; set; }
        public string database { get; set; }
    }

    public class student
    {
        public string? id { get; set; }
        public string? routeid { get; set; }
        public string? student_name { get; set; }
        public string? class_name { get; set; }
        public string ?admission_no { get; set; }
        public string? rf_id { get; set; }
        public string? route_name { get; set; }
        public string? sys_service_id { get; set; }
        public string? start_time_up { get; set; }
        public string? end_time_up { get; set;}
        public string? veh_reg { get; set; }
        public string? driver_name { get; set; }
        public string? driver_mobileno { get; set; }
    }

    public class studentlist
    {
        public string msg { get; set; }
        public List<student>? response { get; set; }
        public string status { get; set; }

    }

    public class notificationlist
    {
        public string msg { get; set; }
        public List<message_details>? response { get; set; }
        public string status { get; set; }
    }

    public class message_details
    {
        public string? message { get; set; }
        public string? date_time { get; set; }
    }

    public class livetracking_var
    {
        public string? id { get; set; }
        public string? server_time { get; set; }
        public string? gps_time { get; set; }
        public string? gps_latitude { get; set; }
        public string? gps_longitude { get; set; }
        public string? latitude_direction { get; set; }
        public string? longitude_direction { get; set; }
        public string? battery_voltage { get; set; }
        public string? gps_speed { get; set; }
    }


    public class histrytracking_var
    {
        public string? server_time { get; set; }
        public string? gps_latitude { get; set; }
        public string? gps_longitude { get; set; }
        public string? latitude_direction { get; set; }
        public string? longitude_direction { get; set; }
    }



    public class stops_var
    { 
        public string id { get; set; }
        public string user_stop_name { get; set; }
        public string stop_order { get; set; }

        public string visited_upcoming { get; set; }

    }

    public class stops
    {
        public string msg { get; set; }
        public List<stops_var>? response { get; set; }
        public string status { get; set; }
    }



    public class livetracking
    {
        public string msg { get; set; }
        public livetracking_var? response { get; set; }
        public string status { get; set; }
    }

    public class histrytracking
    {
        public string msg { get; set; }
        public List<histrytracking_var> trackingList { get; set; }
        public string status { get; set; }
    }


    public class Holiday
    {
        public string from_date { get; set; }
        public string to_date { get; set; }
        public string description { get; set; }
    }

    public class holiday_resonse
    {
        public string msg { get; set; }
        public List<Holiday> holidays { get; set; }
        public string status { get; set; }

    }

    public class Stopeta
    {
        public string msg { get; set; }
        public string ETA { get; set; }
        public string status { get; set; }

    }




}
