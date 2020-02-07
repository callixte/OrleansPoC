using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HelloWorld.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;

namespace HelloWorld.Grains
{
    
    class TickProducerGrain: Grain, ITickProducerGrain
    {
        private readonly ILogger<TickProducerGrain> _logger;

        private IAsyncStream<Tick> _stream;

        public TickProducerGrain(ILogger<TickProducerGrain> logger)
        {
            this._logger = logger;
        }

        public override Task OnActivateAsync()
        {
            this._stream = this.GetStreamProvider("SMS").GetStream<Tick>(this.GetPrimaryKey(), "PoC.Ticks");
            this._logger.LogInformation($"Tick Grain {this.GetPrimaryKey()} activated.");
            return Task.CompletedTask;
        }

        public async Task PushTickAsync(string message)
        {
            if (this._stream is null)
            {
                throw new InvalidOperationException("Grain is not yet active");
            }

            await this._stream.OnNextAsync(new Tick(message));
        }

        public override async Task OnDeactivateAsync()
        {
            await this._stream.OnCompletedAsync();
            this._logger.LogInformation($"Tick Grain {this.GetPrimaryKey()} de-activated.");
        }
    }
}
