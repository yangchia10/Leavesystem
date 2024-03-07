using leaveSystem_vue.Controllers;
using leaveSystem_vue.Data;
using leaveSystem_vue.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace leaveSystem_vue.UnitTests
{
    [TestFixture]
    public class ChangePasswordTests
    {
        private EmployeeController _employeeController;
        private ApplicationDbContext _context;

        [SetUp]
        public void Setup()
        {
            // Set up your controller and database context
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationDbContext(options);

            _employeeController = new EmployeeController(_context, new FakeLogger<EmployeeController>());
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task ChangePassword_ValidModel_ReturnsOk()
        {
            // Arrange
            var employee = new Employee
            {
                username = "testUser",
                password = "oldPassword",
                email = "test@example.com", // 添加必需的属性
                employeename = "Test Employee", // 添加必需的属性
                phone = "1234567890" // 添加必需的属性
            };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var model = new PasswordChangeModel
            {
                Username = "testUser",
                OldPassword = "oldPassword",
                NewPassword = "newPassword"
            };

            // Act
            var result = await _employeeController.ChangePassword(model);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            dynamic data = okResult.Value;
            Assert.That(data.success, Is.True); // 这里确保返回值中有一个名为 success 的属性
            Assert.That(data.message, Is.EqualTo("密碼已更改。"));
        }

        //[Test]
        //public async Task ChangePassword_ValidModel_ReturnsOk()
        //{
        //    // Arrange
        //    var employee = new Employee
        //    {
        //        username = "testUser",
        //        password = "oldPassword" // Set the initial password
        //    };
        //    _context.Employees.Add(employee);
        //    await _context.SaveChangesAsync();

        //    var model = new PasswordChangeModel
        //    {
        //        Username = "testUser",
        //        OldPassword = "oldPassword",
        //        NewPassword = "newPassword"
        //    };

        //    // Act
        //    var result = await _employeeController.ChangePassword(model);

        //    // Assert
        //    Assert.That(result, Is.InstanceOf<OkObjectResult>());
        //    var okResult = (OkObjectResult)result;
        //    dynamic data = okResult.Value;
        //    Assert.That(data.success, Is.True);
        //    Assert.That(data.message, Is.EqualTo("密碼已更改。"));
        //}

        [Test]
        public async Task ChangePassword_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var model = new PasswordChangeModel
            {
                Username = "nonExistingUser",
                OldPassword = "oldPassword",
                NewPassword = "newPassword"
            };

            // Act
            var result = await _employeeController.ChangePassword(model);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = (NotFoundObjectResult)result;
            var message = notFoundResult.Value.GetType().GetProperty("message").GetValue(notFoundResult.Value, null);
            Assert.That(message, Is.EqualTo("員工未找到。")); // 修改为适当的消息

            //// Assert
            //Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            //var notFoundResult = (NotFoundObjectResult)result;
            //dynamic data = notFoundResult.Value;
            //Assert.That(data.success, Is.True);
            //Assert.That(data.message, Is.EqualTo("員工未找到。"));
        }

        // Add more test methods for edge cases, invalid input, etc.
    }

    // A fake logger implementation for testing purposes
    public class FakeLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState? state) where TState : notnull
        {
            // 为 BeginScope 方法提供空操作的实现，返回一个空的 IDisposable 实例
            return NullDisposable.Instance;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState? state, Exception? exception, Func<TState?, Exception?, string> formatter)
        {
            // Log 方法可以留空，因为在测试环境下通常不需要实际记录日志
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            // 可以根据需要返回 true 或 false
            // 在测试环境中，通常返回 false
            return false;
        }

        private class NullDisposable : IDisposable
        {
            public static NullDisposable Instance { get; } = new NullDisposable();

            public void Dispose()
            {
                // Dispose 方法留空
            }
        }
    }
}