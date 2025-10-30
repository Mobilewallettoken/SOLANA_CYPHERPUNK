using Microsoft.AspNetCore.SignalR.Client;

namespace MobileWallet.Desktop.Atm
{
    public class SignalRClient
    {
        private HubConnection _connection;
        private CancellationTokenSource cts = new CancellationTokenSource();

        public async Task InitializeConnectionAsync(string token)
        {
            // Create the connection
            _connection = new HubConnectionBuilder()
                .WithUrl(
                    Global.BaseUrl + "notification",
                    options =>
                    {
                        options.Headers.Add("Authorization", $"Bearer {token}");
                    }
                )
                .WithAutomaticReconnect()
                .Build();

            // Subscribe to reconnect events
            _connection.Reconnecting += error =>
            {
                Console.WriteLine("Reconnecting...");
                return Task.CompletedTask;
            };

            _connection.Reconnected += connectionId =>
            {
                Console.WriteLine($"Reconnected with connection ID: {connectionId}");
                return Task.CompletedTask;
            };

            _connection.Closed += async error =>
            {
                Console.WriteLine("Connection closed. Restarting...");
                await Task.Delay(5000);
                await StartConnectionAsync();
            };

            // Start the connection
            await StartConnectionAsync();
        }

        private async Task StartConnectionAsync()
        {
            try
            {
                await _connection.StartAsync(cts.Token);
                Console.WriteLine("Connection started.");
            }
            catch (Exception ex)
            {
                App.AppLogger.Error(ex,ex.Message);
                Console.WriteLine($"Failed to start connection: {ex.Message}");
            }
        }

        public async Task SendMessageAsync(string method, string sendTo, object message)
        {
            if (_connection?.State == HubConnectionState.Connected)
            {
                await _connection.InvokeAsync(method, sendTo, message);
            }
            else
            {
                Console.WriteLine("Connection is not active.");
            }
        }

        public async Task SendMessageAsync(string method, object message)
        {
            if (_connection?.State == HubConnectionState.Connected)
            {
                await _connection.InvokeAsync(method, message);
            }
            else
            {
                Console.WriteLine("Connection is not active.");
            }
        }

        public void OnMessageReceived(string methodName, Action<object> handler)
        {
            _connection.On(methodName, handler);
        }

        public async Task StopConnectionAsync()
        {
            if (_connection != null)
            {
                await _connection.StopAsync();
                Console.WriteLine("Connection stopped.");
            }
        }
    }
}
