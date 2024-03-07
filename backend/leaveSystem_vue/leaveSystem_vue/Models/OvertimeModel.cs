using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace leaveSystem_vue.Models
{
    public class OvertimeModel
    {
        [Key] 
        public int overtime_id { get; set; }

        public int employee_id { get; set; }

        public DateTime Overdate { get; set; }

        // 不映射到數據庫，僅用於返回日期部分
        [NotMapped]
        public string FormattedDate
        {
            get { return this.Overdate.ToString("yyyy-MM-dd"); }
        }

        public string Overnight { get; set; }

        public string Overstart_time { get; set; }
        public string Overend_time { get; set; }

        public int Overtime_hours { get; set; }

        public string description { get; set; }
        public string Over_status { get; set; }

        // 导航属性
        [ForeignKey("employee_id")]
        public Employee? Employee { get; set; }
        
    }
}
