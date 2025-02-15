using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Services.Factory;
using ApplicationTracker.Services.Interfaces;
using Moq;

namespace ApplicationTracker.Services.Tests.Factory
{
    [TestFixture]
    public class ServiceFactoryTests
    {
        private Mock<IServiceProvider> _mockServiceProvider;
        private ServiceFactory _serviceFactory;

        private class UnmappedDto { }

        [SetUp]
        public void Setup()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
            _serviceFactory = new ServiceFactory(_mockServiceProvider.Object);
        }

        [Test]
        public void GetService_ReturnsService_WhenServiceExists()
        {
            // Arrange
            var mockJobTitleService = new Mock<IService<JobTitleDto>>();
            _mockServiceProvider
                .Setup(sp => sp.GetService(typeof(JobTitleService)))
                .Returns(mockJobTitleService.Object);

            // Act
            var result = _serviceFactory.GetService<JobTitleDto>();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.InstanceOf<IService<JobTitleDto>>());
                Assert.That(result, Is.EqualTo(mockJobTitleService.Object));
            });
        }

        [Test]
        public void GetService_ThrowsInvalidOperationException_WhenServiceExistsInMapButNotInProvider()
        {
            // Arrange
            _mockServiceProvider
                .Setup(sp => sp.GetService(typeof(JobTitleService)))
                .Returns(default(IService<JobTitleDto>));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _serviceFactory.GetService<JobTitleDto>());
            Assert.That(exception.Message, Does.Contain("No service found for type JobTitleService."));
        }

        [Test]
        public void GetService_ThrowsArgumentException_WhenServiceTypeNotInMap()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _serviceFactory.GetService<UnmappedDto>());
            Assert.That(exception.Message, Does.Contain("No service registered for type UnmappedDto"));
        }
    }
}
