using Microsoft.Extensions.DependencyInjection;

namespace telnet_proxy.Extensions
{
    internal static class TelnetProxyBuilderExtensions
    {
        public static ITelnetProxyBuilder Configure(this ITelnetProxyBuilder builder, Action<TelnetProxyOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            return builder;
        }
    }
}
