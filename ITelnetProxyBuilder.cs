using Microsoft.Extensions.DependencyInjection;

namespace telnet_proxy
{
    internal interface ITelnetProxyBuilder
    {
        IServiceCollection Services { get; }
    }
}
