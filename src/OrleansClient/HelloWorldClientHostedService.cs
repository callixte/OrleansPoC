using System;
using System.Threading;
using System.Threading.Tasks;
using HelloWorld.Interfaces;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Runtime;

namespace OrleansClient
{
    public class HelloWorldClientHostedService : IHostedService
    {
        private readonly IClusterClient _client;

        public HelloWorldClientHostedService(IClusterClient client)
        {
            this._client = client;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var clientId = Guid.NewGuid();
                Console.WriteLine($"Client is {clientId}");
                var producer = this._client.GetGrain<ITickProducerGrain>(clientId);
                var grain = this._client.GetGrain<IPoCGrain>(clientId);
                var command = "";
                var arg = "";
                while (command != "quit")
                {
                    switch (command)
                    {
                        case "tick":
                            await producer.PushTickAsync(arg);
                            Console.WriteLine($"Pushed tick - {arg}");
                            break;
                        case "read":
                            var tick = await grain.GetLastTickAsync();
                            if (tick is null)
                            {
                                Console.WriteLine("no tick received");
                            }
                            else
                            {
                                Console.WriteLine($"Grain says: {tick.GetMessage()}");
                            }

                            break;
                        case "stop":
                            await grain.StopAsync();
                            break;
                        default:
                            Console.WriteLine("Command can be: 'tick <message>', 'read' or 'stop'");
                            break;
                    }

                    Console.WriteLine("Command?");
                    var input = Console.ReadLine();
                    var parts = input.Split(' ');
                    command = parts[0];
                    if (parts.Length > 1)
                    {
                        arg = input.Substring(command.Length + 1);
                    }
                    else
                    {
                        arg = "";
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
