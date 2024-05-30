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
using LiveStreamBOT.Enums;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using BaseSouce.SharedSignalrData;
using BaseSouce.SharedSignalrData.Interfaces;
using BaseSouce.SharedSignalrData.Classes;
using BaseSouce.SharedSignalrData.Enums;

namespace LiveStreamBOT.Services
{
    internal class SignalrClient : IAsyncDisposable, IDisposable
    {
        class RetryPolicy : IRetryPolicy
        {
            public TimeSpan? NextRetryDelay(RetryContext retryContext)
            {
                return TimeSpan.FromSeconds(3);
            }
        }
        class BotClientHub : IBotClientHub
        {
            readonly SignalrClient _signalrClient;
            internal BotClientHub(SignalrClient signalrClient)
            {
                this._signalrClient = signalrClient ?? throw new ArgumentNullException(nameof(signalrClient));
            }

            public Task RenderAsync(RenderCommand renderCommand)
            {
                _signalrClient.OnRenderCommand?.Invoke(renderCommand);
                return Task.CompletedTask;
            }

            public Task SetSettingAsync(SettingInfo settingInfo)
            {
                _signalrClient.OnUpdateSettingCommand?.Invoke(settingInfo);
                return Task.CompletedTask;
            }
        }
        class SafeServerHub : IServerHub
        {
            readonly IServerHub _baseServerHub;
            internal SafeServerHub(IServerHub baseServerHub)
            {
                this._baseServerHub = baseServerHub ?? throw new ArgumentNullException(nameof(baseServerHub));
            }

            public async Task PingAsync(ClientInfo clientInfo)
            {
                try
                {
                    await _baseServerHub.PingAsync(clientInfo);
                }
                catch (Exception ex)
                {
                    if (ex is AggregateException ae) ex = ae.InnerException;

                    LogHelper.WriteLog($"{ex.GetType().FullName}: {ex.Message}, {ex.StackTrace}");
                }
            }

            public async Task RenderUpdateAsync(RenderMessage renderMessage)
            {
                try
                {
                    await _baseServerHub.RenderUpdateAsync(renderMessage);
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    if (ex is AggregateException ae) ex = ae.InnerException;

                    LogHelper.WriteLog($"{ex.GetType().FullName}: {ex.Message}, {ex.StackTrace}");
                    if (renderMessage.RenderStatus >= RenderStatus.Ended)
                    {
                        ThreadPool.QueueUserWorkItem((o) =>
                        {
                            Task.Delay(100).ContinueWith((t) =>
                            {
                                _ = this.RenderUpdateAsync(renderMessage);
                            });
                        });
                    }
                }
            }
        }


        internal event Action<RenderCommand> OnRenderCommand;
        internal event Action<SettingInfo> OnUpdateSettingCommand;
        internal event Action<SignalRConnectionState> OnConnectionStateChange;
        internal IServerHub ServerHub { get; }



        readonly HubConnection _connection;
        readonly IDisposable _clientSubscription;

        internal SignalrClient(Uri uri, string botId)
        {
            if (uri is null) throw new ArgumentNullException(nameof(uri));
            if (string.IsNullOrWhiteSpace(botId)) throw new ArgumentNullException(nameof(botId));

            Uri hubUri = new Uri($"{uri.Scheme}://{uri.Authority}/BotHub");
            HubConnectionBuilder hubConnectionBuilder = new();
            _connection = hubConnectionBuilder
                .WithAutomaticReconnect(new RetryPolicy())
                .WithUrl(hubUri
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
                    opts.Headers = new Dictionary<string, string>()
                    {
                        { "BotId", botId }
                    };
                }
                )
                .AddJsonProtocol(configure => configure.PayloadSerializerOptions.SignalR_JsonSerializerOptions())
            .Build();

            this.ServerHub = new SafeServerHub(_connection.CreateHubProxy<IServerHub>());
            _clientSubscription = _connection.Register<IBotClientHub>(new BotClientHub(this));

            _connection.KeepAliveInterval = TimeSpan.FromSeconds(5);
            _connection.Reconnecting += _connection_Reconnecting;
            _connection.Reconnected += _connection_Reconnected;
            _connection.Closed += _connection_Closed;
            //_timer.Elapsed += _timer_Elapsed;
            //_timer.AutoReset = false;
        }
        ~SignalrClient()
        {
            Dispose(false);
        }
        public async ValueTask DisposeAsync()
        {
            _clientSubscription.Dispose();
            await _connection.DisposeAsync();
            GC.SuppressFinalize(this);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        void Dispose(bool disposing)
        {
            _clientSubscription.Dispose();
            _connection.DisposeAsync().ConfigureAwait(false).GetAwaiter().GetResult();
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

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            //_timer.Start();
            return _connection.StartAsync(cancellationToken);
        }
        public Task StopAsync()
        {
            //_timer.Stop();
            return _connection.StopAsync();
        }
    }
}
