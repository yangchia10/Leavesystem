using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using leaveSystem_vue.Controllers;
using leaveSystem_vue.Data;
using leaveSystem_vue.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace leaveSystem_vue.UnitTests
{
    [TestFixture]
    public class LoginControllerTests
    {
        private LoginController _controller;
        private ApplicationDbContext _context;
        private IConfiguration _configuration;
        private ILogger<LoginController> _logger;

        [SetUp]
        public void Setup()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options;
            _context = new ApplicationDbContext(options);

            // Mock IConfiguration
            var configurationMock = new Mock<IConfiguration>();
            // 使用足够长的密钥
            var longEnoughKey = new string('a', 32); // 生成一个32字符长度的字符串作为密钥
            configurationMock.SetupGet(m => m[It.Is<string>(s => s == "Jwt:SecretKey")]).Returns(longEnoughKey);
            _configuration = configurationMock.Object;

            // Mock ILogger
            var loggerMock = new Mock<ILogger<LoginController>>();
            _logger = loggerMock.Object;

            // Setup the controller
            _controller = new LoginController(_context, _configuration, _logger);

            // Seed data
            _context.Employees.Add(new Employee
            {
                username = "testUser",
                password = "testPassword",
                Role = 0,
                email = "test@example.com", // 添加了缺失的属性
                employeename = "Test User",
                phone = "1234567890"
            });
            _context.SaveChanges();

            var httpContext = new DefaultHttpContext(); // 创建一个新的 HttpContext
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
        }

        [Test]
        public async Task Login_WithCorrectCredentials_ReturnsOkResult()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Username = "testUser",
                Password = "testPassword"
            };

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            // 这里改用动态类型来访问Token属性
            dynamic data = okResult?.Value;
            Assert.That(data?.Token, Is.Not.Null);
        }

        [Test]
        public async Task Login_WithIncorrectCredentials_ReturnsUnauthorizedResult()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Username = "wrongUser",
                Password = "wrongPassword"
            };

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
