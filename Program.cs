using System.Net;
using System.Net.Sockets;

// See https://aka.ms/new-console-template for more information

using var inbound = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
var localEndPoint = new IPEndPoint(IPAddress.Any, 1080);
inbound.Bind(localEndPoint);
inbound.Listen(10);

var sessions = new List<Task>();
while (true)
{
    // Listen for a connection
    var handler = await inbound.AcceptAsync();
    Console.WriteLine($"received connection from: {handler.RemoteEndPoint}");

    // Hand the connection off to a task
    sessions.Add(Begin(handler));
}

static async Task Begin(Socket handler)
{
    var localEndPoint = handler.RemoteEndPoint;
    string proxyAddress = "217.180.196.241";//"greatermud.com";
    var proxyIP = Dns.GetHostEntry(proxyAddress).AddressList[0];
    var mudEndPoint = new IPEndPoint(proxyIP, 2427);

    var outbound = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    outbound.Connect(mudEndPoint);

    var waiter = new ManualResetEvent(false);
    BeginReceive(new byte[1024], Direction.ToMud, handler, outbound, waiter);
    BeginReceive(new byte[1024], Direction.FromMud, outbound, handler, waiter);

    // Allow other tasks to continue while this connection is alive
    await Task.Run(() => waiter.WaitOne());
    waiter.Dispose();

    Console.WriteLine($"closed connection from: {localEndPoint}");
}

static void BeginReceive(byte[] buffer, Direction direction, Socket origin, Socket destination, ManualResetEvent waiter)
{
    origin.BeginReceive(
        buffer,
        0,
        buffer.Length,
        SocketFlags.None,
        async result => await OnReceiveAsync(result, buffer, direction, origin, destination, waiter),
        null);
}

static async Task OnReceiveAsync(IAsyncResult result, byte[] buffer, Direction direction, Socket origin, Socket destination, ManualResetEvent waiter)
{
    var read = origin.EndReceive(result);
    if (read == 0 || !destination.Connected)
    {
        await origin.DisconnectAsync(false);
        origin.Dispose();

        // Signals that the connection closed
        waiter.Set();
        return;
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
    ToMud,
    FromMud
}
