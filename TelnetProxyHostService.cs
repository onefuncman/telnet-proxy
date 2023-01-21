using Microsoft.Extensions.Hosting;

namespace telnet_proxy
{
    internal class TelnetProxyHostService : IHostedService
    {
        private readonly ITelnetProxy telnetProxy;

        private readonly IHostApplicationLifetime lifetime;

        public TelnetProxyHostService(ITelnetProxy telnetProxy, IHostApplicationLifetime lifetime)
        {
            this.telnetProxy = telnetProxy;
            this.lifetime = lifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            this.lifetime.ApplicationStarted.Register(async () =>
            {
                await this.telnetProxy.StartAsync(cancellationToken);
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
