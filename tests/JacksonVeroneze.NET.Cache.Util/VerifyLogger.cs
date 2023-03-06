using Microsoft.Extensions.Logging;

namespace JacksonVeroneze.NET.Cache.Util;

[ExcludeFromCodeCoverage]
public static class VerifyLogger
{
    public static Mock<ILogger<T>> Verify<T>(
        this Mock<ILogger<T>> logger,
        string expectedMessage,
        LogLevel expectedLogLevel = LogLevel.Information,
        Func<Times>? times = null)
    {
        times ??= Times.Once;

        Func<object, Type, bool> state = (x, __)
            => x.ToString()!.Contains(expectedMessage);

        logger.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == expectedLogLevel),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => state(v, t)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), times);

        return logger;
    }
}