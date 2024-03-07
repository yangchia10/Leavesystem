using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using leaveSystem_vue.Controllers;
using leaveSystem_vue.Data;
using leaveSystem_vue.Models;

namespace leaveSystem_vue.UnitTests
{
    [TestFixture]
    public class LeaveControllerTests
    {
        private ApplicationDbContext _context;
        private LeaveController _controller;

        [SetUp]
        public async Task Setup()
        {
            // 设置使用内存数据库的选项
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // 确保每个测试方法都有一个干净的数据库
                .Options;
            
            _context = new ApplicationDbContext(options);

            // 预先填充测试数据
            var testEmployee = new Employee
            {
                employee_id = 1,
                username = "testuser",
                password = "password",
                employeename = "Test User",
                email = "test@example.com",
                phone = "1234567890"
            };
            _context.Employees.Add(testEmployee);
            await _context.SaveChangesAsync();

            _controller = new LeaveController(_context);
        }

        [Test]
        public async Task PostLeaveApplication_WithExistingEmployee_CreatesNewLeaveApplication()
        {
            // Arrange
            var newLeaveApplication = new LeaveApplication
            {
                employee_id = 1,
                type = "Annual Leave",
                reason = "Family event",
                start_date = DateTime.Today,
                end_date = DateTime.Today.AddDays(5),
                leavestart_time = "09:00",
                leaveend_time = "17:00",
                leave_time = 8,
                status = "Submitted"
            };

            // Act
            var result = await _controller.PostLeaveApplication(newLeaveApplication);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());

            var createdResult = result as CreatedAtActionResult;
            var createdLeaveApplication = createdResult.Value as LeaveApplication;
            Assert.That(createdLeaveApplication, Is.Not.Null);
            Assert.That(createdLeaveApplication.employee_id, Is.EqualTo(newLeaveApplication.employee_id));
            Assert.That(createdLeaveApplication.type, Is.EqualTo(newLeaveApplication.type));
            Assert.That(createdLeaveApplication.reason, Is.EqualTo(newLeaveApplication.reason));
            // 可以继续添加更多的断言来验证创建的请假申请的其他属性
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted(); // 测试完成后删除内存数据库
            _context.Dispose();
        }
    }
}
