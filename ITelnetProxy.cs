namespace telnet_proxy
{
    internal interface ITelnetProxy
    {
        Task StartAsync(CancellationToken cancellationToken = default);
    }
}
