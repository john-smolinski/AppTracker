using ApplicationTracker.Common;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace ApplicationTracker.Tests.Common
{
    [TestFixture]
    public class ValidationHelperTests
    {
        [Test]
        public void IsValidId_ReturnsTrue_WhenIdIsGreaterThanZero()
        {
            // Setu
            int id = 5;

            // Act
            var result = ValidationHelper.IsValidId(id, out ActionResult badRequestResult);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(badRequestResult, Is.Null);
            });
        }

        [Test]
        public void IsValidId_ReturnsFalse_WhenIdIsZeroOrLess()
        {
            // Arrange
            int id = 0;

            // Act
            var result = ValidationHelper.IsValidId(id, out ActionResult badRequestResult);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.False);
                Assert.That(badRequestResult, Is.Not.Null);
                Assert.That(badRequestResult, Is.InstanceOf<BadRequestObjectResult>());

                var badRequestObjectResult = badRequestResult as BadRequestObjectResult;
                Assert.That(badRequestObjectResult, Is.Not.Null);
                Assert.That(badRequestObjectResult!.Value, Is.InstanceOf<ErrorResponse>());

                var errorResponse = badRequestObjectResult.Value as ErrorResponse;
                Assert.That(errorResponse, Is.Not.Null);
                Assert.That(errorResponse!.Message, Is.EqualTo("Invalid ID"));
                Assert.That(errorResponse.Detail, Is.EqualTo("The provided ID must be greater than zero."));
            });
        }
    }
}
