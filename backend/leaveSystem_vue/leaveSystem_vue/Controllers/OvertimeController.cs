using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using leaveSystem_vue.Models; // 引入您的模型命名空間
using leaveSystem_vue.Data; // 引入您的數據上下文命名空間


namespace leaveSystem_vue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OvertimeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OvertimeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]

        public async Task<IActionResult> PostOvertime(OvertimeModel request)
        {
            // 檢查 Employee 是否存在
            var employee = await _context.Employees.FindAsync(request.employee_id);
            if (employee == null)
            {
                return BadRequest("Employee not found");
            }


            // 從請求中創建 LeaveApplication 實體
            var overtime = new OvertimeModel
            {
                employee_id = request.employee_id,
                Overdate = request.Overdate,
                Overnight = request.Overnight,
                Overstart_time = request.Overstart_time,
                Overend_time = request.Overend_time,
                Overtime_hours = request.Overtime_hours,
                description = request.description,
                Over_status = request.Over_status,
            };
            // 添加 LeaveApplication
            _context.Overtimes.Add(overtime);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOvertime), new { id = overtime.overtime_id }, overtime);
        }

        // 實現 GetOvertime 方法以支持 CreatedAtAction
        [HttpGet("{id}")]
        public async Task<ActionResult<OvertimeModel>> GetOvertime(int id)
        {
            var overtime = await _context.Overtimes.FindAsync(id);

            if (overtime == null)
            {
                return NotFound();
            }

            return overtime;
        }

    }
}