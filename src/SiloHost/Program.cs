using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using HelloWorld.Grains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace OrleansSiloHost
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            var connectionString = System.IO.File.ReadAllText(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\db-connection-string.txt");
            return new HostBuilder()
                .UseOrleans(builder =>
                {
                    builder
                        .UseAdoNetClustering(options =>
                        {
                            options.Invariant = "MySql.Data.MySqlClient";
                            options.ConnectionString = connectionString;
                        })
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "PoC";
                            options.ServiceId = "HelloWorldApp";
                        })
                        .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                        .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(PoCGrain).Assembly).WithReferences())
                        .AddSimpleMessageStreamProvider("SMS", options => { options.FireAndForgetDelivery = true; })
                        .AddAdoNetGrainStorage("PubSubStore", optionsBuilder =>
                        {
                            optionsBuilder.Invariant = "MySql.Data.MySqlClient";
                            optionsBuilder.ConnectionString = connectionString;
                            optionsBuilder.UseJsonFormat = true;
                        })
                        .Configure<GrainCollectionOptions>(options => { options.CollectionAge = TimeSpan.FromMinutes(2); })
                        .Configure<ProcessExitHandlingOptions>(options => { options.FastKillOnProcessExit = false; });
                })
                .ConfigureServices(services =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });
                })
                .ConfigureLogging(builder =>
                {
                    builder.AddConsole();
                })
                .RunConsoleAsync();
        }
    }
}
