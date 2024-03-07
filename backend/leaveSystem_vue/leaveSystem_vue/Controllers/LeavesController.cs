using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using leaveSystem_vue.Models; // 引入您的模型命名空間
using leaveSystem_vue.Data; // 引入您的數據上下文命名空間

namespace leaveSystem_vue.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LeaveController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostLeaveApplication(LeaveApplication request)
        {
            // 檢查 Employee 是否存在
            var employee = await _context.Employees.FindAsync(request.employee_id);
            if (employee == null)
            {
                return BadRequest("Employee not found");
            }

            // 從請求中創建 LeaveApplication 實體
            var leaveApplication = new LeaveApplication
            {
                employee_id = request.employee_id,
                type = request.type,
                reason = request.reason,
                start_date = request.start_date,
                end_date = request.end_date,
                leavestart_time = request.leavestart_time,
                leaveend_time = request.leaveend_time,
                leave_time = request.leave_time,
                status = request.status,

            };

            // 添加 LeaveApplication
            _context.LeaveApplications.Add(leaveApplication);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLeaveApplication), new { id = leaveApplication.leave_id }, leaveApplication);
        }

        // 獲取單個請假申請的示例方法
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveApplication>> GetLeaveApplication(int id)
        {
            var leaveApplication = await _context.LeaveApplications.FindAsync(id);

            if (leaveApplication == null)
            {
                return NotFound();
            }

            return leaveApplication;
        }
    }
}