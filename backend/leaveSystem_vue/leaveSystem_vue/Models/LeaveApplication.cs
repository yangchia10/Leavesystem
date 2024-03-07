using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace leaveSystem_vue.Models
{
    public class LeaveApplication
    {
        [Key]
        public int leave_id { get; set; } // 主键
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }

        public int employee_id { get; set; }
        public string leavestart_time { get; set; }
        public string leaveend_time { get; set; }
        public int leave_time { get; set; }
        public string type { get; set; }
        public string reason { get; set; }

        public string status { get; set; }
        // 導航屬性
        [ForeignKey("employee_id")]
        public Employee? Employee { get; set; }

    }
}
