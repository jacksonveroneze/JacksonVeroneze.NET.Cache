using Microsoft.Extensions.Logging;

namespace JacksonVeroneze.NET.Cache.Extensions;

public static partial class LogMessagesExtensions
{
    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Error,
        Message = "{className} - {methodName} - Key: '{key}'")]
    public static partial void LogGenericError(this ILogger logger,
        string className, string methodName,
        string key, Exception ex);

    [LoggerMessage(
        EventId = 2000,
        Level = LogLevel.Debug,
        Message = "{className} - {methodName} - Key: '{key}' - Exists: '{exists}'")]
    public static partial void LogGet(this ILogger logger,
        string className, string methodName,
        string key, bool exists);

    [LoggerMessage(
        EventId = 3000,
        Level = LogLevel.Debug,
        Message = "{className} - {methodName} - Key: '{key}' - Removed")]
    public static partial void LogRemove(this ILogger logger,
        string className, string methodName,
        string key);

    [LoggerMessage(
        EventId = 4000,
        Level = LogLevel.Debug,
        Message = "{className} - {methodName} - Key: '{key}' - Added")]
    public static partial void LogSet(this ILogger logger,
        string className, string methodName,
        string key);
}