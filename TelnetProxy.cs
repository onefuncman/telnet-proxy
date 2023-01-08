using System.Net.Sockets;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using telnet_proxy.Logging.Extensions;

namespace telnet_proxy
{
    internal class TelnetProxy : ITelnetProxy
    {

        private readonly IServiceProvider serviceProvider;

        private readonly ILogger logger;

        public TelnetProxy(IServiceProvider serviceProvider, ILogger<TelnetProxy> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            using var inbound = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var localEndPoint = new IPEndPoint(IPAddress.Any, 1080);
            inbound.Bind(localEndPoint);
            inbound.Listen(10);

            var sessions = new List<Task>();
            while (!cancellationToken.IsCancellationRequested)
            {
                // Listen for a connection
                try
                {
                    var handler = await inbound.AcceptAsync(cancellationToken);
                    this.logger.StartLog()
                        .WithMessage($"Received connection from {handler.RemoteEndPoint}")
                        .WithLogLevel(LogLevel.Information)
                        .Log();

                    using var scope = this.serviceProvider.CreateScope();
                    var broker = scope.ServiceProvider.GetService<ITelnetProxyBroker>();

                    if (broker == null)
                    {
                        this.logger.StartLog()
                            .WithMessage("Proxy cannot be established. No broker service was found.")
                            .WithLogLevel(LogLevel.Error)
                            .Log();
                        handler.Dispose();
                        continue;
                    }

                    // Hand the connection off to a task
                    sessions.Add(broker.BeginAsync(handler));
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }
}
