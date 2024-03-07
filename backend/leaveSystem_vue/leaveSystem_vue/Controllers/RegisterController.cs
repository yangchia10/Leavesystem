using Microsoft.AspNetCore.Mvc;
using leaveSystem_vue.Models; // 如果您的 User 模型位於 Models 目錄，請確保使用正確的命名空間
using leaveSystem_vue.Data; // 如果您的 ApplicationDbContext 位於 Data 目錄，請確保使用正確的命名空間
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace leaveSystem_vue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RegisterController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> TestDatabaseConnection()
        {
            try
            {
                // 從資料庫提取數據
                var users = await _context.Employees.ToListAsync();
                //bool hasUsers = users.Any();
                //return Ok(new { hasUsers });
                return Ok(users); // 返回其他表示成功的信息
            }
            catch (Exception ex)
            {
                // 如果有異常，捕獲並返回錯誤訊息
                return StatusCode(500, "資料庫連接失敗: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] Employee employee)

        {
            try
            {
                // 檢查原編和信箱是否已存在
                var existingUserByUsername = await _context.Employees
                    .AnyAsync(e => e.username == employee.username);
                var existingUserByEmail = await _context.Employees
                    .AnyAsync(e => e.email == employee.email);

                if (existingUserByUsername)
                {
                    return BadRequest("員工編號已存在");
                }

                if (existingUserByEmail)
                {
                    return BadRequest("電子郵件已被使用過，請使用其他電子郵件。");
                }

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                // 返回註冊成功的消息和用戶資料
                return Ok(new
                {
                    message = "註冊成功",
                    employeeData = new
                    {
                        EmployeeName = employee.employeename,
                        Username = employee.username,
                        Email = employee.email,
                        Phone = employee.phone
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "註冊失敗: " + ex.Message);
            }
        }
    }
}
