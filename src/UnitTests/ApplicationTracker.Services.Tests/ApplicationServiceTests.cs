using ApplicationTracker.Data;
using ApplicationTracker.Data.Dtos;
using ApplicationTracker.Data.Entities;
using ApplicationTracker.TestUtilities.Helpers;
using Microsoft.Extensions.Logging;

namespace ApplicationTracker.Services.Tests
{
    [TestFixture]
    public class ApplicationServiceTests
    {
        private TrackerDbContext _context;
        private ILogger<ApplicationService> _logger;
        private ApplicationService _service;

        [SetUp]
        public void SetUp()
        {
            _context = ContextHelper.GetInMemoryContext<Application>(rows: 5);
            _logger = new LoggerFactory().CreateLogger<ApplicationService>();
            _service = new ApplicationService(_context, _logger);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllApplications()
        {
            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count, Is.EqualTo(5));
            });
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnApplication_WhenIdExists()
        {
            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.Source.Id, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task PostAsync_ShouldAddNewApplication()
        {
            // Setup
            var applicationDto = new ApplicationDto
            {
                ApplicationDate = DateOnly.FromDateTime(DateTime.Now),
                Source = new SourceDto { Name = "Test Source 1" },
                Organization = new OrganizationDto { Name = "Test Organization 1" },
                JobTitle = new JobTitleDto {  Name = "Test JobTitle 1" },
                WorkEnvironment = new WorkEnvironmentDto {  Name = "Remote" },
            };

            // Act
            var result = await _service.PostAsync(applicationDto);

            // Assert 
            Assert.Multiple(() => 
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Source.Name, Is.EqualTo(applicationDto.Source.Name));
                Assert.That(result.Organization.Name, Is.EqualTo(applicationDto.Organization.Name));
                Assert.That(result.JobTitle.Name, Is.EqualTo(applicationDto.JobTitle.Name));
                Assert.That(result.WorkEnvironment.Name, Is.EqualTo(applicationDto.WorkEnvironment.Name));
            });
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnTrue_WhenApplicationExists()
        {
            // Act
            var result = await _service.ExistsAsync(1);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnFalse_WhenApplicationDoesNotExist()
        {
            // Act
            var result = await _service.ExistsAsync(999);

            // Assert
            Assert.That(result, Is.False);

        }

        [Test]
        public async Task ExistsAsync_ShouldReturnTrue_WhenApplicationWithDtoExists()
        {
            // Arrange
            var applicationDto = new ApplicationDto
            {
                ApplicationDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                Source = new SourceDto { Name = "Test Source 1" },
                Organization = new OrganizationDto { Name = "Test Organization 1" },
                JobTitle = new JobTitleDto { Name = "Test JobTitle 1" },
                WorkEnvironment = new WorkEnvironmentDto { Name = "Test WorkEnvironment 1" }
            };

            // Act
            var result = await _service.ExistsAsync(applicationDto);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnFalse_WhenApplicationWithDtoDoesNotExist()
        {
            // Setup
            var applicationDto = new ApplicationDto
            {
                ApplicationDate = DateOnly.FromDateTime(DateTime.Now),
                Source = new SourceDto { Name = "Nonexistent Source" },
                Organization = new OrganizationDto { Name = "Nonexistent Organization" },
                JobTitle = new JobTitleDto { Name = "Nonexistent JobTitle" },
                WorkEnvironment = new WorkEnvironmentDto { Name = "Nonexistent WorkEnvironment" }
            };

            // Act
            var result = await _service.ExistsAsync(applicationDto);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateAsync_ShouldThrowException_WhenApplicationIdIsNull()
        {
            // Setup
            var applicationDto = new ApplicationDto { Id = null };

            // Act 
            var exception = await Task.Run(async () => {
                try
                {
                    await _service.UpdateAsync(applicationDto);
                    return null;
                }
                catch (InvalidOperationException ex)
                {
                    return ex;
                }
            });

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(exception, Is.TypeOf<InvalidOperationException>());
                Assert.That(exception?.Message, Is.EqualTo("Application Id cannot be null."));
            });

        }

        [Test]
        public async Task UpdateAsync_ShouldThrowException_WhenSourceIdIsNull()
        {
            // Setup
            var applicationDto = new ApplicationDto
            {
                Id = 1,
                Source = new SourceDto { Id = null }
            };

            // Act 
            var exception = await Task.Run(async () => {
                try
                {
                    await _service.UpdateAsync(applicationDto);
                    return null;
                }
                catch (InvalidOperationException ex)
                {
                    return ex;
                }
            });

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(exception, Is.TypeOf<InvalidOperationException>());
                Assert.That(exception?.Message, Is.EqualTo("Source Id cannot be null."));
            });
        }

        [Test]
        public async Task UpdateAsync_ShouldThrowKeyNotFoundException_WhenApplicationNotFound()
        {
            // Setup
            var applicationDto = new ApplicationDto
            {
                Id = 999, // non-existent Id
                Source = new SourceDto { Id = 1 },
                Organization = new OrganizationDto { Id = 1 },
                JobTitle = new JobTitleDto { Id = 1 },
                WorkEnvironment = new WorkEnvironmentDto { Id = 1 }
            };

            // Act 
            var exception = await Task.Run(async () => {
                try
                {
                    await _service.UpdateAsync(applicationDto);
                    return null;
                }
                catch (KeyNotFoundException ex)
                {
                    return ex;
                }
            });

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(exception, Is.TypeOf<KeyNotFoundException>());
                Assert.That(exception?.Message, Is.EqualTo("Application with Id 999 not found."));
            });
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateApplication_WhenValidApplicationExists()
        {
            // Setup
            var existingApplication = _context.Applications.First(); // Get an existing application
            var applicationDto = new ApplicationDto
            {
                Id = existingApplication.Id,
                ApplicationDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Source = new SourceDto { Id = 1 },
                Organization = new OrganizationDto { Id = 1 },
                JobTitle = new JobTitleDto { Id = 1 },
                WorkEnvironment = new WorkEnvironmentDto { Id = 1 },
                City = "UpdatedCity",
                State = "UpdatedState"
            };

            // Act
            var result = await _service.UpdateAsync(applicationDto);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.Id, Is.EqualTo(applicationDto.Id));
                Assert.That(result?.Source?.Id, Is.EqualTo(applicationDto.Source.Id));
                Assert.That(result?.Organization?.Id, Is.EqualTo(applicationDto.Organization.Id));
                Assert.That(result?.JobTitle?.Id, Is.EqualTo(applicationDto.JobTitle.Id));
                Assert.That(result?.WorkEnvironment?.Id, Is.EqualTo(applicationDto.WorkEnvironment.Id));
                Assert.That(result?.City, Is.EqualTo("UpdatedCity"));
                Assert.That(result?.State, Is.EqualTo("UpdatedState"));
            });
        }




        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
