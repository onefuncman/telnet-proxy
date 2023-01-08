using System.Net.Sockets;
using System.Net;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using telnet_proxy.Logging.Extensions;

namespace telnet_proxy
{
    internal class TelnetProxyBroker : ITelnetProxyBroker
    {
        private readonly IOptions<TelnetProxyOptions> options;

        private readonly ITelnetProxyInterceptor? interceptor;

        private readonly ILogger logger;

        public TelnetProxyBroker(
            IOptions<TelnetProxyOptions> options,
            ILogger<TelnetProxy> logger)
            : this(options, logger, null)
        {
        }

        public TelnetProxyBroker(
            IOptions<TelnetProxyOptions> options,
            ILogger<TelnetProxy> logger,
            ITelnetProxyInterceptor? interceptor)
        {
            this.options = options;
            this.logger = logger;
            this.interceptor = interceptor;
        }

        private TelnetProxyOptions Options => this.options.Value;

        public async Task BeginAsync(
            Socket handler,
            CancellationToken cancellationToken = default)
        {
            var localEndPoint = handler.RemoteEndPoint;

            if (this.Options.ProxyAddress == null)
            {
                throw new Exception();
            }

            var proxyIP = Dns.GetHostEntry(this.Options.ProxyAddress).AddressList[0];
            var mudEndPoint = new IPEndPoint(proxyIP, this.Options.ProxyPort);

            var outbound = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            outbound.Connect(mudEndPoint);
            this.logger.StartLog()
                .WithMessage($"Proxy connection established for {handler.RemoteEndPoint} -> {outbound.RemoteEndPoint}")
                .WithLogLevel(LogLevel.Information)
                .Log();

            var waiter = new ManualResetEvent(false);
            BeginReceive(new byte[256], Direction.Outbound, handler, outbound, waiter);
            BeginReceive(new byte[2048], Direction.Inbound, outbound, handler, waiter);

            // Allow other tasks to continue while this connection is alive
            await Task.Run(() =>
            {
                while (!waiter.GetSafeWaitHandle().IsClosed)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    waiter.WaitOne();
                }
            });
            waiter.Dispose();

            this.logger.StartLog()
                .WithMessage($"Closed connection from {localEndPoint}")
                .WithLogLevel(LogLevel.Information)
                .Log();
        }

        private void BeginReceive(byte[] buffer, Direction direction, Socket origin, Socket destination, ManualResetEvent waiter) =>
            origin.BeginReceive(
                buffer,
                0,
                buffer.Length,
                SocketFlags.None,
                async result => await OnReceiveAsync(result, buffer, direction, origin, destination, waiter),
                null);

        private async Task OnReceiveAsync(
            IAsyncResult result,
            byte[] buffer,
            Direction direction,
            Socket origin,
            Socket destination,
            ManualResetEvent waiter)
        {
            var read = origin.EndReceive(result);
            if (read == 0 || !destination.Connected)
            {
                await origin.DisconnectAsync(false);
                origin.Dispose();

                if (!waiter.SafeWaitHandle.IsClosed)
                {
                    waiter.Set();
                    // The connection has closed so close the waiter to complete the loop
                    waiter.Close();
                }
                return;
            }

            this.logger.StartLog()
                .WithMessage($"{read} bytes received for {origin.RemoteEndPoint} -> {destination.RemoteEndPoint}")
                .WithLogLevel(LogLevel.Trace)
                .Log();

            if (this.interceptor != null)
            {
                read = this.interceptor.Intercept(ref buffer, direction, read);
            }

            await destination.SendAsync(buffer.Take(read).ToArray(), SocketFlags.None);

            this.logger.StartLog()
                .WithMessage($"{read} bytes sent for {origin.RemoteEndPoint} -> {destination.RemoteEndPoint}")
                .WithLogLevel(LogLevel.Trace)
                .Log();

            origin.BeginReceive(
                buffer,
                0,
                buffer.Length,
                SocketFlags.None,
                async result => await OnReceiveAsync(result, buffer, direction, origin, destination, waiter),
                null);
        }

        internal enum Direction
        {
            Outbound,
            Inbound
        }
    }
}
