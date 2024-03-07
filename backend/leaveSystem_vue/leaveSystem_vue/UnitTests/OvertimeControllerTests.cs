using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using leaveSystem_vue.Controllers;
using leaveSystem_vue.Data;
using leaveSystem_vue.Models;
using System;
using System.Threading.Tasks;

namespace leaveSystem_vue.UnitTests
{
    public class OvertimeControllerTests
    {
        private OvertimeController _controller;
        private ApplicationDbContext _context;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _controller = new OvertimeController(_context);

            // Seed data with an Employee
            var employee = new Employee
            {
                employee_id = 1, // 使用这个ID作为后续加班记录的employee_id
                username = "testUser",
                employeename = "Test Employee",
                email = "test@example.com",
                password = "TestPassword",
                phone = "1234567890"
            };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        [Test]
        public async Task PostOvertime_WithValidModel_ReturnsCreatedAtActionResult()
        {
            // 保证请求的OvertimeModel使用了有效的employee_id
            var request = new OvertimeModel
            {
                employee_id = 1, // 确保这个ID匹配之前添加的员工
                Overdate = DateTime.Now,
                Overnight = "是",
                Overstart_time = "18:00",
                Overend_time = "21:00",
                Overtime_hours = 180,
                description = "Working late on project",
                Over_status = "Submitted",
            };

            var result = await _controller.PostOvertime(request);

            // 如果这个断言失败，意味着返回了一个BadRequest结果，可能是因为找不到员工
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>(), "Expected CreatedAtActionResult but received BadRequest due to missing employee or model validation failure.");
        }


        //[Test]
        //public async Task GetOvertime_WithExistingId_ReturnsOkObjectResult()
        //{
        //    // Arrange: 创建一个Overtime记录
        //    var overtimeModel = new OvertimeModel
        //    {
        //        employee_id = 1, // 使用已存在的员工ID
        //        Overdate = DateTime.Now,
        //        Overnight = "否",
        //        Overstart_time = "19:00",
        //        Overend_time = "22:00",
        //        Overtime_hours = 180,
        //        description = "加班完成项目",
        //        Over_status = "已提交",
        //    };
        //    _context.Overtimes.Add(overtimeModel);
        //    await _context.SaveChangesAsync();

        //    // Act: 获取这个刚创建的Overtime记录
        //    var getResult = await _controller.GetOvertime(overtimeModel.overtime_id);

        //    // Assert: 验证是否成功返回Overtime记录
        //    Assert.That(getResult.Result, Is.InstanceOf<OkObjectResult>(), "Expected result to be OkObjectResult");
        //    var okResult = getResult.Result as OkObjectResult;
        //    Assert.That(okResult.Value, Is.InstanceOf<OvertimeModel>(), "Expected OkObjectResult to contain an OvertimeModel");

        //    var returnedOvertime = okResult.Value as OvertimeModel;
        //    Assert.That(returnedOvertime.overtime_id, Is.EqualTo(overtimeModel.overtime_id), "Returned overtime ID should match the created one");
        //}



        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
