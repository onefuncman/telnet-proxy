using System.Net.Sockets;
using System.Net;
using Microsoft.Extensions.Options;

namespace telnet_proxy
{
    internal class TelnetProxyBroker : ITelnetProxyBroker
    {
        private readonly IOptions<TelnetProxyOptions> options;

        private readonly ITelnetProxyInterceptor? interceptor;

        public TelnetProxyBroker(IOptions<TelnetProxyOptions> options)
            : this(options, null)
        {
        }

        public TelnetProxyBroker(
            IOptions<TelnetProxyOptions> options,
            ITelnetProxyInterceptor? interceptor)
        {
            this.options = options;
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

            var waiter = new ManualResetEvent(false);
            BeginReceive(new byte[256], Direction.Outbound, handler, outbound, waiter);
            BeginReceive(new byte[2048], Direction.Inbound, outbound, handler, waiter);

            // Allow other tasks to continue while this connection is alive
            await Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    waiter.WaitOne();
                }
            });
            waiter.Dispose();

            Console.WriteLine($"closed connection from: {localEndPoint}");
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
                    // The connection has closed so signal to complete the loop
                    waiter.Set();
                }
                return;
            }

            if (this.interceptor != null)
            {
                read = this.interceptor.Intercept(ref buffer, direction, read);
            }

            await destination.SendAsync(buffer.Take(read).ToArray(), SocketFlags.None);

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
