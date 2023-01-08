using Microsoft.Extensions.Logging;
using telnet_proxy.Logging.Extensions;

namespace telnet_proxy.Logging.Extensions
{
    internal static class LogMessageBuilderExtensions
    {
        internal static ILogMessageBuilder WithMessage(this ILogMessageBuilder builder, string message)
        {
            builder.Message = message;
            return builder;
        }

        internal static ILogMessageBuilder WithLogLevel(this ILogMessageBuilder builder, LogLevel logLevel)
        {
            builder.LogLevel = logLevel;
            return builder;
        }

        internal static void Log(this ILogMessageBuilder builder)
        {
            builder.Logger.Log(builder.LogLevel, $"{DateTime.Now} - {builder.Message}");
        }
    }
}
