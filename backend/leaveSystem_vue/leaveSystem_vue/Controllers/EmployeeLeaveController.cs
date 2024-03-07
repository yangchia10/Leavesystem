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
    public class EmployeeLeaveController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeeLeaveController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 獲取狀態為 "待呈核" 的請假紀錄
        [HttpGet("pendingApprovals")]
        public async Task<ActionResult<IEnumerable<object>>> GetPendingApprovals()
        {
            var pendingLeaves = await _context.LeaveApplications
                .Where(l => l.status == "待呈核")
                .Join(_context.Employees,
                    leave => leave.employee_id,
                    employee => employee.employee_id,
                    (leave, employee) => new
                    {
                        LeaveId = leave.leave_id,
                        Employeename = employee.employeename, // 員工姓名
                        Username = employee.username,         // 員工編號
                        Type = leave.type,                    // 假别
                        Reason = leave.reason,                    // 請假原因
                        StartDate = leave.start_date.ToString("yyyy-MM-dd"), // 格式化起始時間
                        EndDate = leave.end_date.ToString("yyyy-MM-dd"),    // 格式化结束時間
                        Leavestart_time = leave.leavestart_time,
                        Leaveend_time = leave.leaveend_time,
                        Status = leave.status                 // 狀態

                    })
                .ToListAsync();

            var result = pendingLeaves.Select(l => new
            {
                l.LeaveId,
                l.Employeename,
                l.Username,
                l.Type,
                l.Reason,
                l.StartDate,
                l.EndDate,
                l.Leavestart_time,
                l.Leaveend_time,
                l.Status
            }).ToList();

            return Ok(pendingLeaves);
        }

        [HttpGet("pendingApprovals/{employeeId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetEmployeePendingApprovals(int employeeId)
        {
            var pendingLeaves = await _context.LeaveApplications
                .Where(l => l.employee_id == employeeId)
                .Join(_context.Employees,
                    leave => leave.employee_id,
                    employee => employee.employee_id,
                    (leave, employee) => new
                    {
                        LeaveId = leave.leave_id,
                        Employeename = employee.employeename,
                        Username = employee.username,
                        Type = leave.type,
                        Reason = leave.reason,
                        StartDate = leave.start_date.ToString("yyyy-MM-dd"),
                        EndDate = leave.end_date.ToString("yyyy-MM-dd"),
                        Leavestart_time = leave.leavestart_time,
                        Leaveend_time = leave.leaveend_time,
                        Status = leave.status
                    })
                .ToListAsync();

            return Ok(pendingLeaves);
        }

        // 更新請假狀態為 "批准"
        [HttpPost("approveLeave/{leaveId}")]
        public async Task<IActionResult> ApproveLeave(int leaveId)
        {
            var leaveApplication = await _context.LeaveApplications
                .FirstOrDefaultAsync(l => l.leave_id == leaveId);

            if (leaveApplication == null)
            {
                return NotFound();
            }

            leaveApplication.status = "批准";
            await _context.SaveChangesAsync();

            return NoContent(); // 或者返回任何合適的響應
        }

        // 在 EmployeeLeaveController 中添加一個新的方法来處理駁回操作
        [HttpPost("rejectLeave/{leaveId}")]
        public async Task<IActionResult> RejectLeave(int leaveId)
        {
            var leaveApplication = await _context.LeaveApplications
                .Include(l => l.Employee) // 確保加載 Employee 導航属性
                .FirstOrDefaultAsync(l => l.leave_id == leaveId);

            if (leaveApplication == null)
            {
                return NotFound();
            }

            // 只有當申請為 "補休" 類型且狀態更新為 “駁回” 時，才返還時數
            if (leaveApplication.type == "補休")
            {
                // 這里假設您有一個方法來找到對應的加班記錄並返還時數
                // 注意：這里的邏輯需要根據您的具體業務規則進行調整
                // 例如，您可能需要根據員工 ID 和某些條件來找到相應的加班記錄
                var overtimeRecord = _context.Overtimes.FirstOrDefault(o => o.employee_id == leaveApplication.employee_id);
                if (overtimeRecord != null)
                {
                    // 假設 leave_time 是以分鐘為單位，這里將其加回到加班記錄中
                    overtimeRecord.Overtime_hours += leaveApplication.leave_time;
                }
            }

            leaveApplication.status = "駁回"; // 更新状態為“駁回”
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
