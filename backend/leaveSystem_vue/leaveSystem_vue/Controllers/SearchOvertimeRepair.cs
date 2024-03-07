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
    public class SearchOvertimeRepair : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public SearchOvertimeRepair(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("overtime")]
        public async Task<IActionResult> GetOvertimeRepair(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("搜索內容不能為空");
            }

            var overtimeRecords = await _context.Overtimes
                .Include(e => e.Employee)

                //.Where(e => (e.Employee.employeename.Contains(query)) || (e.Employee.username.Contains(query)))
                // 對於加班記錄的查詢
                .Where(e => e.Employee.employeename == query || e.Employee.username == query)

                .ToListAsync();



            if (!overtimeRecords.Any())
            {
                return NotFound("未找到相關數據");
            }

            var totalOvertimeMinutes = overtimeRecords.Sum(o => o.Overtime_hours);

            var overtimeRepair = overtimeRecords.Select(e => new
            {
                Employeename = e.Employee.employeename,
                Username = e.Employee.username,
                overDate = e.Overdate.ToString("yyyy-MM-dd"), // 格式化日期
                e.Overnight,
                overstart_time = e.Overstart_time,
                overend_time = e.Overend_time,
                overtimehours = $"{e.Overtime_hours / 60}小時{e.Overtime_hours % 60}分鐘", // 在內存中轉換為小時和分鐘
                e.description,
                e.Over_status,
            }).ToList();

            // 添加調試輸出
            Console.WriteLine($"Total Overtime Minutes: {totalOvertimeMinutes}");


            var response = new
            {
                OvertimeRepair = overtimeRepair,
                TotalOvertimeMinutes = totalOvertimeMinutes
            };


            if (overtimeRepair.Count == 0)
            {
                return NotFound("未找到相關數據");
            }

            return Ok(response);
        }

        [HttpGet("repair")]
        public async Task<IActionResult> GetRepair(string query)
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                {
                    return BadRequest("搜索內容不能為空");
                }

                var overRepairRaw = await _context.LeaveApplications
                    .Include(q => q.Employee) // 確保有Employee屬性
                                              //.Where(q => (q.Employee.employeename.Contains(query) || q.Employee.username.Contains(query))
                                              //&& q.type == "補休") // type為"補休"的記錄
                                              // 對於補休記錄的查詢
                    .Where(q => (q.Employee.employeename == query || q.Employee.username == query) && q.type == "補休")
                    .Select(q => new
                    {
                        Employeename = q.Employee.employeename,
                        Username = q.Employee.username,
                        q.type,
                        q.reason,
                        StartDate = q.start_date.ToString("yyyy-MM-dd"),
                        EndDate = q.end_date.ToString("yyyy-MM-dd"),
                        q.leavestart_time,
                        q.leaveend_time,
                        Leave_time = q.leave_time, // 直接獲取 leave_time

                        q.status
                    })
                    .ToListAsync();

                if (overRepairRaw.Count == 0)
                {
                    return NotFound(new
                    {
                        Message = "未找到相關數據",
                        Query = query,
                        TypeSearched = "補休"
                    });
                }

                // 計算已批准的補休總時長
                var approvedRepairTimeMinutes = overRepairRaw
                    .Where(q => q.status == "批准")
                    .Sum(q => q.Leave_time); // 假設 Leave_time 已經是分鐘數

                var response = new
                {
                    LeaveData = overRepairRaw,
                    TotalCompensationHours = approvedRepairTimeMinutes // 已批准的補休總時長
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服務器內部錯誤: {ex.Message}");
            }
        }


        [HttpGet("overtimerecords/{employeeId}")]
        public async Task<ActionResult<object>> GetEmployeeovertimerecords(int employeeId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {

            Console.WriteLine($"Received startDate: {startDate}, endDate: {endDate}");


            var overtimeQuery = _context.Overtimes
              .Where(a => a.employee_id == employeeId);

            // 根據提供的開始日期進行篩選
            if (startDate.HasValue)
            {
                overtimeQuery = overtimeQuery.Where(a => a.Overdate >= startDate.Value);
            }

            // 根據提供的結束日期進行篩選
            if (endDate.HasValue)
            {
                overtimeQuery = overtimeQuery.Where(a => a.Overdate <= endDate.Value);
            }

            var overtimeRecords = await overtimeQuery.ToListAsync();

            if (!overtimeRecords.Any())
            {
                return NotFound("未找到相關數據");
            }

            var totalOvertimeMinutes = overtimeRecords.Sum(o => o.Overtime_hours);

            var overtimeDetails = overtimeRecords
                .Join(_context.Employees,
                    OvertimeModel => OvertimeModel.employee_id,
                    employee => employee.employee_id,
                    (OvertimeModel, employee) => new
                    {
                        Overtime = OvertimeModel.overtime_id,
                        Employeename = employee.employeename,
                        Username = employee.username,
                        overDate = OvertimeModel.Overdate.ToString("yyyy-MM-dd"), // 格式化日期
                        OvertimeModel.Overnight,
                        overstart_time = OvertimeModel.Overstart_time,
                        overend_time = OvertimeModel.Overend_time,
                        overtimehours = $"{OvertimeModel.Overtime_hours / 60}小時{OvertimeModel.Overtime_hours % 60}分鐘", // 在內存中轉換為小時和分鐘
                        OvertimeModel.description,
                        OvertimeModel.Over_status,
                    })
                .ToList();

            var response = new
            {
                Details = overtimeDetails,
                TotalOvertimeMinutes = totalOvertimeMinutes
            };

            return Ok(response);
        }
        [HttpGet("approvedtime/{username}")]
        public async Task<IActionResult> GetApprovedTime(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("用戶名不能為空");
            }

            // 計算已批准的加班時間
            var overtimeMinutes = await _context.Overtimes
                .Where(o => o.Employee.username == username && o.Over_status == "批准")
                .SumAsync(o => (int?)o.Overtime_hours) ?? 0;

            // 計算所有提交的補休時間
            var repairTimeMinutes = await _context.LeaveApplications
                .Where(l => l.Employee.username == username && l.type == "補休")
                .SumAsync(l => (int?)l.leave_time) ?? 0;

            // 計算剩餘的時間
            var remainingMinutes = overtimeMinutes - repairTimeMinutes;

            return Ok(new { RemainingMinutes = remainingMinutes });
        }
    }
}