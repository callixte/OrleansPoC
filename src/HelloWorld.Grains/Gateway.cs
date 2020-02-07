using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using HelloWorld.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace HelloWorld.Grains
{
    public class Gateway: IDisposable
    {
        private readonly Subject<Tick> _ticks = new Subject<Tick>();

        private readonly Guid _clientId;
        private readonly IStreamProvider _streamProvider;
        private readonly ILogger<Gateway> _logger;
        private StreamSubscriptionHandle<Tick> _handle;

        public IObservable<Tick> Ticks => _ticks;

        public Gateway(Guid client, IStreamProvider streamProvider, ILogger<Gateway> logger)
        {
            _clientId = client;
            _logger = logger;
            _streamProvider = streamProvider;
        }

        public async Task StartListeningAsync()
        {
            var stream = this._streamProvider.GetStream<Tick>(this._clientId, "PoC.Ticks");
            _handle = await stream.SubscribeAsync(NextAsync, ErrorAsync, CompletedAsync);
        }

        public async Task StopListeningAsync()
        {
            if (_handle is null)
                return;
            try
            {
                await this._handle.UnsubscribeAsync();
            }
            catch (Exception e)
            {
                this._logger.LogError($"Error while unsubscribing Ticks: {e}");
            }
            finally
            {
                this._handle = null;
            }
        }

        private Task NextAsync(Tick item, StreamSequenceToken token = null)
        {
            this._ticks.OnNext(item);
            return Task.CompletedTask;
        }

        private Task ErrorAsync(Exception e)
        {
            _logger.LogError($"Error receiving ticks ({e})");
            return Task.CompletedTask;
        }

        private static Task CompletedAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this._ticks?.Dispose();
        }
    }
}
