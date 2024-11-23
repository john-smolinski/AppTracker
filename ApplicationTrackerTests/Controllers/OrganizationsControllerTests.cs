using ApplicationTracker.Common;
using ApplicationTracker.Controllers;
using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;

namespace ApplicationTrackerTests.Controllers
{
    [TestFixture]
    public class OrganizationsControllerTests
    {
        private Mock<ILogger<OrganizationsController>> _mockLogger;
        private OrganizationsController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new TrackerDbContext(options);

            // seed data for tests
            context.Organizations.AddRange(new List<Organization>()
            {
                new() { Id = 1, Name = "Test1" },
                new() { Id = 2, Name = "Test2" },
                new() { Id = 3, Name = "Test3" },
                new() { Id = 4, Name = "Test4" }
             });
            context.SaveChanges();

            _mockLogger = new Mock<ILogger<OrganizationsController>>();
            _controller = new OrganizationsController(context, _mockLogger.Object);
        }

        [Test]
        public async Task GetOrganizations_ReturnsOk_WhenOrganizationsExists()
        {
            // Act
            var result = await _controller.GetOrganizations();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);

            var returnedEnvironments = okResult.Value as IEnumerable<OrganizationDto>;
            Assert.That(returnedEnvironments, Is.Not.Null);
            Assert.That(returnedEnvironments.Count(), Is.EqualTo(4));
            Assert.That(returnedEnvironments.First().Name, Is.EqualTo("Test1"));
        }

        [Test]
        public async Task GetOrganizations_ReturnsNotFound_WhenNoOrganizationsExists()
        {
            // Setup
            var options = new DbContextOptionsBuilder<TrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var emptyContext = new TrackerDbContext(options);
            var controller = new OrganizationsController(emptyContext, _mockLogger.Object);

            // Act
            var result = await controller.GetOrganizations();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;

            Assert.That(notFoundResult, Is.Not.Null);
            var errorResponse = notFoundResult.Value as ErrorResponse;

            Assert.That(errorResponse, Is.Not.Null);
            Assert.That(errorResponse.Message, Is.EqualTo("Organizations not found"));
        }

        [Test]
        public async Task GetOrganization_ReturnsOk_WhenOrganizationExists()
        {
            // Act 
            var result = await _controller.GetOrganization(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var returnedOrganization = okResult.Value as OrganizationDto;

            Assert.That(returnedOrganization, Is.Not.Null);
            Assert.That(returnedOrganization.Id, Is.EqualTo(1));
            Assert.That(returnedOrganization.Name, Is.EqualTo("Test1"));
        }

        [Test]
        public async Task GetOrganization_ReturnsNotFound_WhenOrganizationDoesNotExist()
        {
            // Act 
            var result = await _controller.GetOrganization(999); // non existant value

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;

            Assert.That(notFoundResult, Is.Not.Null);
            var errorResponse = notFoundResult.Value as ErrorResponse;

            Assert.That(errorResponse, Is.Not.Null);
            Assert.That(errorResponse.Message, Is.EqualTo("Organization not found"));
            Assert.That(errorResponse.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }
    }
}
