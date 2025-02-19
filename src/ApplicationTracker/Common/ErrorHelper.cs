using Microsoft.AspNetCore.Mvc;

namespace ApplicationTracker.Common
{
    public static class ErrorHelper
    {
        /// <summary>
        /// Returns an InternalServerError response with the provided message and detail
        /// </summary>
        /// <param name="message"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns a NotFound response with the provided message and detail
        /// </summary>
        /// <param name="message"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns a BadRequest response with the provided message and detail
        /// </summary>
        /// <param name="message"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Validates that the provided message and detail are not null or empty
        /// </summary>
        /// <param name="message"></param>
        /// <param name="detail"></param>
        /// <exception cref="ArgumentException"></exception>
        private static void ValidateParameters(string message, string detail)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be null or empty.", nameof(message));

            if (string.IsNullOrWhiteSpace(detail))
                throw new ArgumentException("Detail cannot be null or empty.", nameof(detail));
        }
    }
}
