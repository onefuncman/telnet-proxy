using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using telnet_proxy;
using telnet_proxy.Extensions;

await CreateHostBuilder(args)
    .RunConsoleAsync();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) =>
        {
            services.AddHostedService<TelnetProxyHostService>();

            services.AddTelnetProxy<MudProxy>()
                .Configure(options =>
                {
                    options.ProxyAddress = "217.180.196.241";
                    options.ProxyPort = 2427;
                });
        });
