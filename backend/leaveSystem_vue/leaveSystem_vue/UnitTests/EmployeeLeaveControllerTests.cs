using leaveSystem_vue.Controllers;
using leaveSystem_vue.Data;
using leaveSystem_vue.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace leaveSystem_vue.UnitTests
{
    [TestFixture]
    public class EmployeeLeaveControllerTests
    {
        private ApplicationDbContext _context;
        private EmployeeLeaveController _controller;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            // Seed data
            var employee = new Employee { employee_id = 1, username = "testUser", employeename = "Test User", email = "test@example.com", password = "testPass", phone = "1234567890" };
            _context.Employees.Add(employee);

            var leaveApplication = new LeaveApplication { leave_id = 1, employee_id = 1, status = "待呈核", type = "Annual", reason = "Vacation", start_date = DateTime.Today, end_date = DateTime.Today.AddDays(5), leavestart_time = "09:00", leaveend_time = "18:00" };
            _context.LeaveApplications.Add(leaveApplication);

            await _context.SaveChangesAsync();

            _controller = new EmployeeLeaveController(_context);

            // Mock User.Identity.Name
            var user = new ClaimsPrincipal(new GenericIdentity("testUser"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task GetPendingApprovals_ReturnsPendingApprovals()
        {
            var result = await _controller.GetPendingApprovals();

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.Value, Is.Not.Null);

            var leaves = okResult.Value as IEnumerable<object>;
            Assert.That(leaves, Is.Not.Empty);
        }

        [Test]
        public async Task ApproveLeave_WithValidId_ChangesStatusToApproved()
        {
            int leaveId = 1;

            var result = await _controller.ApproveLeave(leaveId);

            Assert.That(result, Is.InstanceOf<NoContentResult>());

            var leaveApplication = await _context.LeaveApplications.AsNoTracking().FirstOrDefaultAsync(l => l.leave_id == leaveId);
            Assert.That(leaveApplication.status, Is.EqualTo("批准"));
        }

        [Test]
        public async Task RejectLeave_WithValidId_ChangesStatusToRejected()
        {
            int leaveId = 1;

            var result = await _controller.RejectLeave(leaveId);

            Assert.That(result, Is.InstanceOf<NoContentResult>());

            var leaveApplication = await _context.LeaveApplications.AsNoTracking().FirstOrDefaultAsync(l => l.leave_id == leaveId);
            Assert.That(leaveApplication.status, Is.EqualTo("駁回"));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
