using Microsoft.Extensions.Logging;

namespace telnet_proxy.Logging
{
    internal interface ILogMessageBuilder
    {
        ILogger Logger { get; }

        string? Message { get; set; }

        LogLevel LogLevel { get; set; }
    }
}
