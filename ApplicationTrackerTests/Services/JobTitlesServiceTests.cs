﻿using ApplicationTracker.Data.Entities;
using ApplicationTracker.Services;
using ApplicationTrackerTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;

namespace ApplicationTrackerTests.Services
{
    [TestFixture]
    public class JobTitlesServiceTests
    {
        private Mock<ILogger<JobTitleService>> _mockLogger;
        private JobTitleService _service;

        [SetUp]
        public void Setup()
        {
            // create a in memery context with 4 rows.
            var context = ContextHelper.GetInMemoryContext<JobTitle>(4);
            
            _mockLogger = new Mock<ILogger<JobTitleService>>();
            _service = new JobTitleService(context, _mockLogger.Object);
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllJobTitles()
        {
            // Setup
            var testId = 1;
            var expected = $"Test {typeof(JobTitle).Name} {testId}";

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Count(), Is.EqualTo(4));
                Assert.That(result.First().Name, Is.EqualTo(expected));
            });
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenIdDoesNotExist()
        {
            // Setup 
            var testId = 99;

            // Act
            var result = await _service.GetByIdAsync(testId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task ExistsAsync_ReturnsTrue_WhenIdExists()
        {
            // Setup
            var testId = 1;

            // Act
            var result = await _service.ExistsAsync(testId);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ExistsAsync_ReturnsFalse_WhenIdDoesNotExist()
        {
            // Setup 
            var testId = 99;

            // Act
            var result = await _service.ExistsAsync(testId);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}