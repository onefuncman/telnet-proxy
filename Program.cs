using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

Socket inbound = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 1080);
inbound.Bind(localEndPoint);
inbound.Listen(10);

var handler = await inbound.AcceptAsync();
Console.WriteLine("received connection from: " + handler.RemoteEndPoint.ToString());

string proxyAddress = "217.180.196.241";//"greatermud.com";
var proxyIP = Dns.GetHostEntry(proxyAddress).AddressList[0];


IPEndPoint mudEndPoint = new IPEndPoint(proxyIP,23);

Socket outbound = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
outbound.Connect(mudEndPoint);

while (true) {
    var tasks = new List<Task>();
    if (handler.Available > 0) {
        //Console.WriteLine("inbound data available: " + handler.Available);
        var buffer = new byte[handler.Available];
        await handler.ReceiveAsync(buffer, SocketFlags.None);
        await outbound.SendAsync(buffer, SocketFlags.None);
    }
    if (outbound.Available > 0) {
        //Console.WriteLine("outbound data available: " + outbound.Available);
        var buffer = new byte[outbound.Available];
        await outbound.ReceiveAsync(buffer, SocketFlags.None);
        await handler.SendAsync(buffer, SocketFlags.None);
    }
}