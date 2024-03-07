using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using leaveSystem_vue.Data; // 引入數據模型命名空間
using System.Linq;
using System.Threading.Tasks;
using leaveSystem_vue.Models;

namespace leaveSystem_vue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchLeaveStatusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SearchLeaveStatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/SearchLeaveStatus
        [HttpGet]
        public async Task<IActionResult> GetLeaveStatus(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("搜索內容不能為空");
            }

            var leaveRecords = await _context.LeaveApplications
                .Include(l => l.Employee) // 確保有Employee屬性
                .Where(l => (l.Employee.employeename.Contains(query) || l.Employee.username.Contains(query))
                && l.type != "補休") // 排除type為"補修"的記錄
                .Select(l => new
                {
                    Employeename = l.Employee.employeename,
                    Username = l.Employee.username,
                    l.type,
                    l.reason,
                    startDate = l.start_date.ToString("yyyy-MM-dd"),
                    endDate = l.end_date.ToString("yyyy-MM-dd"),
                    l.leavestart_time,
                    l.leaveend_time,
                    l.status
                })
                .ToListAsync();

            if (leaveRecords.Count == 0)
            {
                return NotFound("未找到相關數據");
            }

            return Ok(leaveRecords);
        }
    }
}