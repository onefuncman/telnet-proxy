using static telnet_proxy.TelnetProxyBroker;

namespace telnet_proxy
{
    internal interface ITelnetProxyInterceptor
    {
        int Intercept(ref byte[] buffer, Direction direction, int bytes);
    }
}