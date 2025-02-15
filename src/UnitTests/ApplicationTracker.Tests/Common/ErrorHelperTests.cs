using ApplicationTracker.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.Tests.Common
{
    [TestFixture]
    public class ErrorHelperTests
    {
        [Test]
        public void InternalServerError_ReturnsExpectedObjectResult()
        {
            // Arrange
            var message = "Internal server error occurred";
            var detail = "An unexpected error happened.";

            // Act
            var result = ErrorHelper.InternalServerError(message, detail);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.InstanceOf<ObjectResult>());
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));

                var errorResponse = result.Value as ErrorResponse;
                Assert.That(errorResponse, Is.Not.Null);
                Assert.That(errorResponse!.Message, Is.EqualTo(message));
                Assert.That(errorResponse.Detail, Is.EqualTo(detail));
                Assert.That(errorResponse.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            });
        }

        [Test]
        public void NotFound_ReturnsExpectedNotFoundObjectResult()
        {
            // Arrange
            var message = "Resource not found";
            var detail = "The requested resource could not be found.";

            // Act
            var result = ErrorHelper.NotFound(message, detail);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());

                var errorResponse = result.Value as ErrorResponse;
                Assert.That(errorResponse, Is.Not.Null);
                Assert.That(errorResponse!.Message, Is.EqualTo(message));
                Assert.That(errorResponse.Detail, Is.EqualTo(detail));
                Assert.That(errorResponse.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            });
        }

        [Test]
        public void BadRequest_ReturnsExpectedBadRequestObjectResult()
        {
            // Arrange
            var message = "Invalid request";
            var detail = "The request parameters were invalid.";

            // Act
            var result = ErrorHelper.BadRequest(message, detail);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());

                var errorResponse = result.Value as ErrorResponse;
                Assert.That(errorResponse, Is.Not.Null);
                Assert.That(errorResponse!.Message, Is.EqualTo(message));
                Assert.That(errorResponse.Detail, Is.EqualTo(detail));
                Assert.That(errorResponse.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            });
        }

        [Test]
        public void ValidateParameters_ThrowsArgumentException_WhenMessageIsNullOrEmpty()
        {
            // Arrange
            var detail = "Detail message";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => ErrorHelper.InternalServerError(null!, detail));
            Assert.Throws<ArgumentException>(() => ErrorHelper.InternalServerError(string.Empty, detail));
        }

        [Test]
        public void ValidateParameters_ThrowsArgumentException_WhenDetailIsNullOrEmpty()
        {
            // Arrange
            var message = "Error message";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => ErrorHelper.InternalServerError(message, null!));
            Assert.Throws<ArgumentException>(() => ErrorHelper.InternalServerError(message, string.Empty));
        }
    }
}
