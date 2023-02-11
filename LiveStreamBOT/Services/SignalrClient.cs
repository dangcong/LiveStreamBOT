using BaseSouce.SharedSignalrData.BotClient;
using BaseSouce.SharedSignalrData.Server;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypedSignalR.Client;
using System.Timers;
using System.Net.Http;
using System.Threading;

namespace LiveStreamBOT.Services
{
    internal enum SignalRConnectionState
    {
        Connecting,
        DisConnected,
        Connected,
        Reconnecting
    }
    internal class SignalrClient : IBotClientHub
    {
        class RetryPolicy : IRetryPolicy
        {
            public TimeSpan? NextRetryDelay(RetryContext retryContext)
            {
                return TimeSpan.FromSeconds(3);
            }
        }



        internal event Action<SignalRConnectionState> OnConnectionStateChange;
        public IServerHub ServerHub { get; }

        readonly HubConnection _connection;
        readonly IDisposable _clientSubscription;
        //
        //readonly System.Timers.Timer _timer = new System.Timers.Timer(500);
        internal SignalrClient(Uri uri)
        {
            Uri hubUri = new Uri($"{uri.Scheme}://{uri.Authority}/BotHub");
            HubConnectionBuilder hubConnectionBuilder = new();
            _connection = hubConnectionBuilder
                .WithAutomaticReconnect(new RetryPolicy())
                .WithUrl(hubUri
#if DEBUG
                , (opts) =>
                {
                    opts.HttpMessageHandlerFactory = (message) =>
                    {
                        if (message is HttpClientHandler clientHandler)
                            // always verify the SSL certificate
                            clientHandler.ServerCertificateCustomValidationCallback +=
                                 (sender, certificate, chain, sslPolicyErrors) => { return true; };
                        return message;
                    };
                }
#endif
                )
            .Build();

            _connection.CreateHubProxy<IServerHub>();
            _clientSubscription = _connection.Register<IBotClientHub>(this);

            _connection.KeepAliveInterval = TimeSpan.FromSeconds(5);
            _connection.Reconnecting += _connection_Reconnecting;
            _connection.Reconnected += _connection_Reconnected;
            _connection.Closed += _connection_Closed;
            //_timer.Elapsed += _timer_Elapsed;
            //_timer.AutoReset = false;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {

        }

        private Task _connection_Closed(Exception arg)
        {
            OnConnectionStateChange?.Invoke(SignalRConnectionState.DisConnected);
            return Task.CompletedTask;
        }

        private Task _connection_Reconnected(string arg)
        {
            OnConnectionStateChange?.Invoke(SignalRConnectionState.Connected);
            return Task.CompletedTask;
        }

        private Task _connection_Reconnecting(Exception arg)
        {
            OnConnectionStateChange?.Invoke(SignalRConnectionState.Reconnecting);
            return Task.CompletedTask;
        }

        public Task Start(CancellationToken cancellationToken = default)
        {
            //_timer.Start();
            return _connection.StartAsync(cancellationToken);
        }
        public Task Stop(CancellationToken cancellationToken = default)
        {
            //_timer.Stop();
            return _connection.StopAsync(cancellationToken);
        }



        #region IBotClientHub
        public event Action<RenderCommand> OnRenderCommand;
        public Task RenderAsync(RenderCommand renderCommand)
        {
            OnRenderCommand?.Invoke(renderCommand);
            return Task.CompletedTask;
        }
        #endregion
    }
}
