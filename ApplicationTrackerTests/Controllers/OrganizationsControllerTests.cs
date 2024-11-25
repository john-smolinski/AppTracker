using ApplicationTracker.Common;
using ApplicationTracker.Controllers;
using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Entities;
using ApplicationTrackerTests.Helpers;
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
            // create an in memory context with 4 rows of test data 
            var context = ContextHelper.GetInMemoryContext<Organization>(4);

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
            Assert.That(returnedEnvironments.First().Name, Does.StartWith($"Test {typeof(Organization).Name}"));
        }

        [Test]
        public async Task GetOrganizations_ReturnsNotFound_WhenNoOrganizationsExists()
        {
            // Setup
            var emptyContext = ContextHelper.GetInMemoryContext<Organization>();
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
            var id = 1;
            var result = await _controller.GetOrganization(id);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var returnedOrganization = okResult.Value as OrganizationDto;

            Assert.That(returnedOrganization, Is.Not.Null);
            Assert.That(returnedOrganization.Id, Is.EqualTo(id));
            Assert.That(returnedOrganization.Name, Is.EqualTo($"Test {typeof(Organization).Name} {id}"));
        }

        [Test]
        public async Task GetOrganization_ReturnsNotFound_WhenOrganizationDoesNotExist()
        {
            // Act 
            var result = await _controller.GetOrganization(999); 

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
