namespace leaveSystem_vue.Models
{
    public class PasswordChangeModel
    {
        public string Username { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

    }
    public class PasswordChangeModelTest : PasswordChangeModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
