using System;
using System.ComponentModel.DataAnnotations;

namespace leaveSystem_vue.Models
{
    public abstract class BaseLeaveApplication
    {
        

        [Key]
        public int leave_id { get; set; } // 主键
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public int employee_id { get; set; }
        public string type { get; set; }
        public string reason { get; set; }
        public string status { get; set; }

    }
}
