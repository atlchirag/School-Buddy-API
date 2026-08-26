namespace SchoolBuddy_APIs.Models.Master.Route
{
    public class Route
    {
        public string? id { get; set; }
        public string? route_name { get; set; }
        public string? start_time_up { get; set; }
        public string? end_time_up{ get; set; }

        public string? service_id { get; set; }
        public string? user_id { get; set;}
        public string? device_id { get; set; }

    }
    public class ERoute
    {
        public string? id { get; set; }
        public string? route_name { get; set; }
        public string? start_time_up { get; set; }
        public string? end_time_up { get; set; }
        public string? rowupdated { get; set; }
        public string? service_id { get; set; }

    }

    public class StudentDetails
    {
        public string? id { get; set; }
        public string? routeid { get; set; }
        public string? student_name { get; set; }
        public string? father_name { get; set; }
        public string? mobile_no { get; set; }
        public string? class_name { get; set; }
        public string? admission_no { get; set; }
        public string? section { get; set; }
        public string? rf_id { get; set; }
        public string? stop_name { get; set; }

    }
    public class StudentWiseModel
    {
        public string? stop_id { get; set; }
        public string? sys_user_id { get; set; }
        public string? student_id { get; set; }
        public string? student_name { get; set; }
        public string? father_name { get; set; }
        public string? mobile_no1 { get; set; }
        public string? admission_no { get; set; }
        public string? rf_id { get; set; }
        public string? section { get; set; }
        public string? class_name { get; set; }
        public string? pick_route_id { get; set; }
        public string? pick_route_name { get; set; }
        public string? pick_stop_id { get; set; }
        public string? pick_stop_name { get; set; }
        public string? drop_route_id { get; set; }
        public string? drop_route_name { get; set; }
        public string? drop_stop_id { get; set; }
        public string? drop_stop_name { get; set; }
        public string? stop_name { get; set; }
        public string? pick_latitude { get; set; }
        public string? pick_longitude { get; set; }
        public string? drop_latitude { get; set; }
        public string? drop_longitude { get; set; }
    }
    public class UpdateStudentModel
    {
        public string? student_id { get; set; }

        public string? change_type { get; set; }

        public string? pick_route_id { get; set; }

        public string? drop_route_id { get; set; }

        public string? pick_stop_id { get; set; }

        public string? drop_stop_id { get; set; }

        public string? prev_pick_route_id { get; set; }
                    
        public string? prev_drop_route_id { get; set; }

        public string? prev_pick_stop_id { get; set; }

        public string? prev_drop_stop_id { get; set; }
    }
}
