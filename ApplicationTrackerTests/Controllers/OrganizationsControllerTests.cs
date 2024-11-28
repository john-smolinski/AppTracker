using ApplicationTracker.Common;
using ApplicationTracker.Controllers;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Services;
using ApplicationTracker.Services.Factory;
using ApplicationTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;

namespace ApplicationTrackerTests.Controllers
{
    [TestFixture]
    public class OrganizationsControllerTests
    {
        private Mock<IService<OrganizationDto>> _mockService;
        private Mock<ILogger<OrganizationsController>> _mockLogger;
        private OrganizationsController _controller;

        private List<OrganizationDto> _organizations;
        private ServiceFactory _serviceFactory;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IService<OrganizationDto>>();
            _mockLogger = new Mock<ILogger<OrganizationsController>>();

            _organizations =
            [
                new() { Id = 1, Name = "Test 1" },
                new() { Id = 2, Name = "Test 2" },
                new() { Id = 3, Name = "Test 3" },
                new() { Id = 4, Name = "Test 4" }
            ];

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider
                .Setup(sp => sp.GetService(typeof(OrganizationService)))
                .Returns(_mockService.Object); 

            _serviceFactory = new ServiceFactory(mockServiceProvider.Object);
            _controller = new OrganizationsController(_serviceFactory, _mockLogger.Object);
        }

        [Test]
        public async Task GetOrganizations_ReturnsOk_WhenOrganizationsExists()
        {
            // Setup 
            _mockService
                .Setup(service => service.GetAllAsync())
                .ReturnsAsync(_organizations);

            // Act
            var result = await _controller.GetOrganizations();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);

            var returnedEnvironments = okResult.Value as IEnumerable<OrganizationDto>;
            Assert.That(returnedEnvironments, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(returnedEnvironments.Count(), Is.EqualTo(4));
                Assert.That(returnedEnvironments.First().Name, Does.StartWith($"Test"));
            });
        }

        [Test]
        public async Task GetOrganizations_ReturnsNotFound_WhenNoOrganizationsExists()
        {
            // Setup
            _mockService
                .Setup(service => service.GetAllAsync())
                .ReturnsAsync([]);

            // Act
            var result = await _controller.GetOrganizations();

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
            // Setup 
            var testId = 1;

            _mockService
                .Setup(service => service.ExistsAsync(testId))
                .ReturnsAsync(true);

            _mockService
                .Setup(service => service.GetByIdAsync(testId))
                .ReturnsAsync(_organizations.First(x => x.Id == testId));

            // Act 
            var result = await _controller.GetOrganization(testId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            var returnedOrganization = okResult.Value as OrganizationDto;

            Assert.That(returnedOrganization, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(returnedOrganization.Id, Is.EqualTo(testId));
                Assert.That(returnedOrganization.Name, Is.EqualTo($"Test {testId}"));
            });
        }

        [Test]
        public async Task GetOrganization_ReturnsNotFound_WhenNoOrganizationExist()
        {
            // Setup 
            var testId = 99;
            _mockService
                .Setup(service => service.ExistsAsync(testId))
                .ReturnsAsync(false);

            // Act 
            var result = await _controller.GetOrganization(testId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;

            Assert.That(notFoundResult, Is.Not.Null);
            var errorResponse = notFoundResult.Value as ErrorResponse;

            Assert.That(errorResponse, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(errorResponse.Message, Is.EqualTo("Organization not found"));
                Assert.That(errorResponse.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            });
        }
    }
}
