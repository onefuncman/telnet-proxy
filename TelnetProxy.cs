using System.Net.Sockets;
using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace telnet_proxy
{
    internal class TelnetProxy : ITelnetProxy
    {

        private readonly IServiceProvider serviceProvider;

        public TelnetProxy(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            using var inbound = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var localEndPoint = new IPEndPoint(IPAddress.Any, 1080);
            inbound.Bind(localEndPoint);
            inbound.Listen(10);

            var sessions = new List<Task>();
            while (true)
            {
                // Listen for a connection
                try
                {
                    var handler = await inbound.AcceptAsync(cancellationToken);
                    Console.WriteLine($"received connection from: {handler.RemoteEndPoint}");

                    using var scope = this.serviceProvider.CreateScope();
                    var broker = scope.ServiceProvider.GetService<ITelnetProxyBroker>();

                    if (broker == null)
                    {
                        Console.WriteLine("Proxy cannot be established: no broker service was found");
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
