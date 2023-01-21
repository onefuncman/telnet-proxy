namespace telnet_proxy
{
    internal class TelnetProxyOptions
    {
        private int proxyPort = 23;

        internal string? ProxyAddress { get; set; }

        internal int ProxyPort
        {
            get
            {
                return proxyPort;
            }
            set
            {
                this.proxyPort = value;
            }
        }
    }
}
