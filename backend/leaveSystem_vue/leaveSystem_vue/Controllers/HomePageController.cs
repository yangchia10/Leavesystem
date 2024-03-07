using leaveSystem_vue.Data;
using leaveSystem_vue.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace leaveSystem_vue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomePageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomePageController> _logger;

        public HomePageController(ApplicationDbContext context , ILogger<HomePageController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        // 確保僅授權用戶可以訪問
        public async Task<IActionResult> GetUserInfo()
        {
            _logger.LogInformation("Attempting to fetch user info");

            var username = User.Identity.Name; // 從 JWT 令牌獲取用戶名
            var user = await _context.Employees.FirstOrDefaultAsync(u => u.username == username);

            if (user == null)
            {
                _logger.LogWarning("User not found: {Username}", username);
                return NotFound(new { message = "用户未找到" });
            }
            _logger.LogInformation("User found: {Username}, Role: {Role}, Password: {Password}", user.username, user.Role, user.password);

            // 返回用戶信息，您可以根據需要調整返回的信息
            return Ok(new
            {
                Employee_id = user.employee_id,
                Username = user.username,
                Password = user.password,
                Role = user.Role,
                // 其他需要的用户信息
            });
        }
    }
}
