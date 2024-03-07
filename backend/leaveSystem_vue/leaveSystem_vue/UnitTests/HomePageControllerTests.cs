using leaveSystem_vue.Controllers;
using leaveSystem_vue.Data;
using leaveSystem_vue.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace leaveSystem_vue.UnitTests
{
    [TestFixture]
    public class HomePageControllerTests
    {
        private ApplicationDbContext _context;
        private HomePageController _controller;
        private Mock<ILogger<HomePageController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _context = new ApplicationDbContext(options);

            // Seed the database with a test user
            _context.Employees.Add(new Employee
            {
                employee_id = 1,
                username = "testUser",
                password = "testPass",
                Role = 0, // 根据您的实际模型调整
                email = "testuser@example.com", // 提供 email 值
                employeename = "Test User", // 提供 employeename 值
                phone = "1234567890" // 提供 phone 值
            });
            _context.SaveChanges();

            // Mock ILogger
            _loggerMock = new Mock<ILogger<HomePageController>>();

            // Setup controller
            _controller = new HomePageController(_context, _loggerMock.Object);

            // Mock User.Identity.Name
            var user = new ClaimsPrincipal(new GenericIdentity("testUser"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task GetUserInfo_UserExists_ReturnsUserInfo()
        {
            // Act
            var result = await _controller.GetUserInfo();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var userInfo = okResult.Value as dynamic;
            Assert.That(userInfo.Username, Is.EqualTo("testUser"));
            // You can add more assertions here to validate the returned user info
        }

        [Test]
        public async Task GetUserInfo_UserDoesNotExist_ReturnsNotFound()
        {
            // Change User.Identity.Name to a non-existing user
            var user = new ClaimsPrincipal(new GenericIdentity("nonExistingUser"));
            _controller.ControllerContext.HttpContext.User = user;

            // Act
            var result = await _controller.GetUserInfo();

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
