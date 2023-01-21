using Microsoft.Extensions.Logging;

namespace telnet_proxy.Logging
{
    internal class LogMessageBuilder : ILogMessageBuilder
    {
        internal LogMessageBuilder(ILogger logger)
        {
            Logger = logger;
        }

        public ILogger Logger { get; }

        public string? Message { get; set; }

        public LogLevel LogLevel { get; set; }
    }
}
