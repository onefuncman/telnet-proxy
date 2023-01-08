using Microsoft.Extensions.Logging;

namespace telnet_proxy.Logging.Extensions
{
    internal static class LoggerExtensions
    {
        internal static ILogMessageBuilder StartLog(this ILogger logger)
        {
            return new LogMessageBuilder(logger);
        }
    }
}
