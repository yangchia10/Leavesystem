using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using leaveSystem_vue.Data;
using leaveSystem_vue.Models;
using Microsoft.Extensions.Logging;

namespace leaveSystem_vue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(ApplicationDbContext context , ILogger<EmployeeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeModel model)
        {
            _logger.LogInformation("Attempting to change password for user: {Username}", model.Username);
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.username == model.Username);

            if (employee == null)
            {
                _logger.LogWarning("Employee not found: {Username}", model.Username);
                return NotFound(new { message = "員工未找到。" });
            }

            if (!employee.ChangePassword(model.OldPassword, model.NewPassword))
            {
                return BadRequest(new { message = "原密碼錯誤。" });
            }

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "密碼已更改。" });
        }
    }
}
