# telnet proxy

The intent is to use this Socks5Proxy class from https://www.codeproject.com/Articles/5954/C-class-for-connecting-via-a-SOCKS5-Proxy-Server to proxy telnet connections to MUD servers to enable additional features.

# sample client usage
```csharp
Socket client;
client = LMKR.SocksProxy.ConnectToSocks5Proxy(
  "172.16.64.145",1080,"www.microsoft.com",
  80,"U$er","Pa$$word!");
string strGet = "GET //\r\n";
s.Send(System.Text.Encoding.ASCII.GetBytes(strGet));
```