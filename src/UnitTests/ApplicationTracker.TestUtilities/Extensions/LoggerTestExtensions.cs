using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace ApplicationTracker.TestUtilities.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class LoggerTestExtensions
    {
        public static void VerifyLog(
            this Mock<ILogger> mockLogger,
            LogLevel logLevel,
            string expectedMessage,
            Times times)
        {
            mockLogger.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(expectedMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                times);
        }
    }
}
