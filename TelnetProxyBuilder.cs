using Microsoft.Extensions.DependencyInjection;

namespace telnet_proxy
{
    internal class TelnetProxyBuilder : ITelnetProxyBuilder
    {
        private readonly IServiceCollection services;

        internal TelnetProxyBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        public IServiceCollection Services => services;
    }
}
