using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace telnet_proxy.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static ITelnetProxyBuilder AddTelnetProxy<TTelnetProxyInterceptor>(this IServiceCollection services)
            where TTelnetProxyInterceptor : class, ITelnetProxyInterceptor
        {
            services.TryAddSingleton<ITelnetProxy, TelnetProxy>();
            services.TryAddScoped<ITelnetProxyBroker, TelnetProxyBroker>();
            services.TryAddScoped<ITelnetProxyInterceptor, TTelnetProxyInterceptor>();
            return new TelnetProxyBuilder(services);
        }
    }
}
