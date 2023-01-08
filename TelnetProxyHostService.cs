using Microsoft.Extensions.Hosting;

namespace telnet_proxy
{
    internal class TelnetProxyHostService : IHostedService
    {
        private readonly ITelnetProxy telnetProxy;

        public TelnetProxyHostService(ITelnetProxy telnetProxy)
        {
            this.telnetProxy = telnetProxy;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await this.telnetProxy.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
