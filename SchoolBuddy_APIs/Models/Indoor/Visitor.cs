using SchoolBuddy_APIs.Models.Master.Route;
using System.Xml.Linq;

namespace SchoolBuddy_APIs.Models.Indoor
{
    public class Visitor
    {
        public string Name { get; set; }
        public string MobileNo { get; set; }
        public int CardId { get; set; }
        public string CardNo { get; set; }
        public int SysUserId { get; set; }
        public DateTime in_time { get; set; }
        public DateTime out_time { get; set; }

    }
}