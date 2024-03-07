using leaveSystem_vue.Controllers;
using leaveSystem_vue.Data;
using leaveSystem_vue.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace leaveSystem_vue.UnitTests
{
    [TestFixture]
    public class EmployeeOvertimeControllerTests
    {
        private ApplicationDbContext _context;
        private EmployeeOvertimeController _controller;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            // Seed data with all required properties
            var employee = new Employee
            {
                employee_id = 1,
                username = "testUser",
                employeename = "Test Employee",
                email = "test@test.com", // 提供有效的 email
                password = "TestPassword123", // 提供有效的 password
                phone = "1234567890" // 提供有效的 phone
            };
            _context.Employees.Add(employee);

            var overtime = new OvertimeModel
            {
                overtime_id = 1,
                employee_id = 1,
                Overdate = DateTime.Today,
                Overstart_time = "18:00",
                Overend_time = "21:00",
                Overtime_hours = 180,
                description = "Project Deadline",
                Over_status = "待呈核",
                Overnight = "否" // 这里为 Overnight 提供了一个值
            };
            _context.Overtimes.Add(overtime);

            await _context.SaveChangesAsync();

            _controller = new EmployeeOvertimeController(_context);
        }



        [Test]
        public async Task GetpendingOvretime_ReturnsPendingOvertime()
        {
            var result = await _controller.GetpendingOvretime();

            // 首先，确认结果是 OkObjectResult 类型
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var okResult = result.Result as OkObjectResult;

            // 然后，确认 okResult.Value 是一个集合（IEnumerable），而不特定于 List<object>
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<dynamic>>()); // 使用 dynamic 来处理匿名类型的集合

            // 可以进一步检查集合是否不为空
            var collection = okResult.Value as IEnumerable<dynamic>;
            Assert.That(collection, Is.Not.Empty);

            // 如果需要，还可以对集合中的第一项或特定项进行更详细的检查
            // 例如，检查集合的第一项是否包含期望的属性值
        }


        [Test]
        public async Task ApproveOvertime_WithValidId_ChangesStatusToApproved()
        {
            int overtimeId = 1;

            var result = await _controller.ApproveLeave(overtimeId);

            Assert.That(result, Is.InstanceOf<NoContentResult>());

            var updatedOvertime = await _context.Overtimes.FindAsync(overtimeId);
            Assert.That(updatedOvertime.Over_status, Is.EqualTo("批准"));
        }

        [Test]
        public async Task RejectOvertime_WithValidId_ChangesStatusToRejected()
        {
            int overtimeId = 1;

            var result = await _controller.RejectLeave(overtimeId);

            Assert.That(result, Is.InstanceOf<NoContentResult>());

            var updatedOvertime = await _context.Overtimes.FindAsync(overtimeId);
            Assert.That(updatedOvertime.Over_status, Is.EqualTo("駁回"));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
