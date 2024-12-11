using Microsoft.AspNetCore.Mvc;

namespace ApplicationTracker.Common
{
    public static class ErrorHelper
    {
        public static ObjectResult InternalServerError(string message, string detail)
        {
            ValidateParameters(message, detail);

            return new ObjectResult(new ErrorResponse
            {
                Message = message,
                StatusCode = StatusCodes.Status500InternalServerError,
                Detail = detail
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        public static NotFoundObjectResult NotFound(string message, string detail)
        {
            ValidateParameters(message, detail);

            return new NotFoundObjectResult(new ErrorResponse
            {
                Message = message,
                StatusCode = StatusCodes.Status404NotFound,
                Detail = detail
            });
        }

        public static BadRequestObjectResult BadRequest(string message, string detail)
        {
            ValidateParameters(message, detail);

            return new BadRequestObjectResult(new ErrorResponse
            {
                Message = message,
                StatusCode = StatusCodes.Status400BadRequest,
                Detail = detail
            });
        }

        private static void ValidateParameters(string message, string detail)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be null or empty.", nameof(message));

            if (string.IsNullOrWhiteSpace(detail))
                throw new ArgumentException("Detail cannot be null or empty.", nameof(detail));
        }
    }
}
