using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using leaveSystem_vue.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using leaveSystem_vue.Data;

namespace leaveSystem_vue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePermissionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolePermissionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/RolePermissions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        // GET: api/RolePermissions/Roles
        [HttpGet("Roles")]
        public ActionResult<IEnumerable<object>> GetRoles()
        {
            var roles = new List<object>
            {
                new { Id = 1, Name = "Admin" },
                new { Id = 2, Name = "User" }
                // 根據實際情況添加更多角色
            };

            return roles;
        }

        // GET: api/RolePermissions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }



        // PUT: api/RolePermissions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
        {
            if (id != employee.employee_id)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RolePermissions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.employee_id }, employee);
        }

        // DELETE: api/RolePermissions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.employee_id == id);
        }
    }
}