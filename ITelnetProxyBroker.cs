using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;

namespace telnet_proxy
{
    internal interface ITelnetProxyBroker
    {
        Task BeginAsync(Socket handler, CancellationToken cancellationToken = default);
    }
}
