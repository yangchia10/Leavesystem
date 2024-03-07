using System.ComponentModel.DataAnnotations;

namespace leaveSystem_vue.Models
{
    public class LoginModel
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }

        

    }
}
