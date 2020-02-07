using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HelloWorld.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Microsoft.Extensions.DependencyInjection;

namespace HelloWorld.Grains
{
    public class PoCGrain: Grain, IPoCGrain
    {
        private readonly ILogger<PoCGrain> _logger;
        private readonly IServiceProvider _services;

        private Manager _manager;
        private Gateway _gateway;
        private IDisposable _listener;

        public PoCGrain(IServiceProvider services, ILogger<PoCGrain> logger)
        {
            this._services = services;
            this._logger = logger;
        }

        public async Task<ITick> GetLastTickAsync()
        {
            if (this._manager is null)
            {
                return null;
            }

            return await this._manager.GetLastTickAsync();
        }

        public Task StopAsync()
        {
            this.DeactivateOnIdle();
            return Task.CompletedTask;
        }

        public override async Task OnActivateAsync()
        {
            var clientId = this.GetPrimaryKey();
            this._logger.LogInformation($"PoC Grain {clientId} activating.");

            this._manager = new Manager();
            this._gateway = new Gateway(clientId, this.GetStreamProvider("SMS"), this._services.GetService<ILogger<Gateway>>());

            this._listener = this._gateway.Ticks.Subscribe(this._manager.OnReceiveTick);

            await this._gateway.StartListeningAsync();

            this._logger.LogInformation($"PoC Grain {clientId} activated.");
        }

        public override async Task OnDeactivateAsync()
        {
            var clientId = this.GetPrimaryKey();
            this._logger.LogInformation($"PoC Grain {clientId} de-activating.");

            await this._gateway.StopListeningAsync();
            this._gateway.Dispose();
            this._gateway = null;
            this._listener.Dispose();
            this._listener = null;
            this._manager = null;
            this._logger.LogInformation($"PoC Grain {clientId} de-activated.");
        }
    }
}
