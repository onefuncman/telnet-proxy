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

string proxyAddress = "217.180.196.241";//"greatermud.com";
var proxyIP = Dns.GetHostEntry(proxyAddress).AddressList[0];


IPEndPoint proxyEndPoint = new IPEndPoint(proxyIP,23);

Socket outbound = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
outbound.Connect(proxyEndPoint);

while (true) {
    // Receive message.
    var buffer = new byte[1_024];
    var received = await handler.ReceiveAsync(buffer, SocketFlags.None);

    var outboundBuffer = new byte[1_024];
    var outboundData = await outbound.ReceiveAsync(outboundBuffer, SocketFlags.None);

    await handler.SendAsync(outboundBuffer, SocketFlags.None);
    await outbound.SendAsync(buffer, SocketFlags.None);
}