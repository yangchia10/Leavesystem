using System.ComponentModel.DataAnnotations;

namespace leaveSystem_vue.Models
{
    public class Employee
    {
        [Key]
        public int employee_id { get; set; }
        public string employeename { get; set; }
        public string username { get; set; }
        public string password { get; set; } // 存儲加密後的密碼
        public string email { get; set; }
        public string phone { get; set; }
        public int Role { get; set; } // Role属性，例如 "Admin" 或 "User"

        public bool ChangePassword(string oldPassword, string newPassword)
        {
            if (this.password == oldPassword)
            {
                this.password = newPassword;
                return true;
            }
            return false;
        }
    }
}
