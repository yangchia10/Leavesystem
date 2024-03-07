using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using leaveSystem_vue.Models;
using leaveSystem_vue.Data;
using System.Globalization;


// ... (using 语句和命名空间)

namespace leaveSystem_vue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeOvertimeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeeOvertimeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 獲取狀態為 "待呈核" 的加班記錄
        [HttpGet("pendingOvretime")]
        public async Task<ActionResult<IEnumerable<object>>> GetpendingOvretime()
        {
            var pendingOvretime = await _context.Overtimes
                .Where(l => l.Over_status == "待呈核")
                .Join(_context.Employees,
                    overtime => overtime.employee_id,
                    employee => employee.employee_id,
                    (overtime, employee) => new
                    {
                        overtimeId = overtime.overtime_id,
                        Employeename = employee.employeename, // 員工姓名
                        Username = employee.username,         // 員工編號
                        overDate = overtime.Overdate.ToString("yyyy-MM-dd"), // 格式化日期
                        overnight = overtime.Overnight,                    // 假别
                        overstart_time = overtime.Overstart_time,
                        overend_time = overtime.Overend_time,
                        overtimehours = $"{overtime.Overtime_hours / 60}小時{overtime.Overtime_hours % 60}分鐘", // 在內存中轉換為小時和分鐘
                        Description = overtime.description,
                        Status = overtime.Over_status                 // 狀態

                    })
                .ToListAsync();

            var result = pendingOvretime.Select(l => new
            {
                l.overtimeId,
                l.Employeename,
                l.Username,
                l.overDate,
                l.overnight,
                l.overstart_time,
                l.overend_time,
                l.overtimehours,
                l.Description,
                l.Status
            }).ToList();

            return Ok(pendingOvretime);
        }

        [HttpGet("pendingApprovals/{employeeId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetEmployeePendingApprovals(int employeeId)
        {
            var pendingOvretime = await _context.Overtimes
            .Where(l => l.Over_status == "待呈核")
            .Join(_context.Employees,
                overtime => overtime.employee_id,
                employee => employee.employee_id,
                (overtime, employee) => new
                {
                    overtimeId = overtime.overtime_id,
                    Employeename = employee.employeename, // 員工姓名
                    Username = employee.username,         // 員工編號
                    overDate = overtime.Overdate.ToString("yyyy-MM-dd"), // 格式化日期
                    overnight = overtime.Overnight,                    // 假别
                    overstart_time = overtime.Overstart_time,
                    overend_time = overtime.Overend_time,
                    overtimehours = $"{overtime.Overtime_hours / 60}小時{overtime.Overtime_hours % 60}分鐘", // 在內存中轉換為小時和分鐘
                    Description = overtime.description,
                    Status = overtime.Over_status                 // 狀態

                })
            .ToListAsync();

            return Ok(pendingOvretime);
        }

        // 更新請假狀態為 "批准"
        [HttpPost("approveOvertime/{overtimeId}")]
        public async Task<IActionResult> ApproveLeave(int overtimeId)
        {
            var ovettimeModel = await _context.Overtimes
                .FirstOrDefaultAsync(l => l.overtime_id == overtimeId);

            if (ovettimeModel == null)
            {
                return NotFound();
            }

            ovettimeModel.Over_status = "批准";
            await _context.SaveChangesAsync();

            return NoContent(); // 或者返回任何合適的響應
        }

        // 在 EmployeeLeaveController 中添加一個新的方法來處理駁回操作
        [HttpPost("rejectOvertime/{overtimeId}")]
        public async Task<IActionResult> RejectLeave(int overtimeId)
        {
            var ovettimeModel = await _context.Overtimes
                .FirstOrDefaultAsync(l => l.overtime_id == overtimeId);

            if (ovettimeModel == null)
            {
                return NotFound();
            }

            ovettimeModel.Over_status = "駁回"; // 更新狀態為“駁回”
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
