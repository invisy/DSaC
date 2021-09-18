using DSaC_LAB1.Networking;
using System.Collections.Concurrent;
using System.Net;


namespace DSaC_LAB1.Server.Core
{
    public class NotificationServer : INotificationServer
    {
        private readonly ISocketServer _server;
        private readonly ConcurrentDictionary<string, Client> _clients = new();

        public NotificationServer(ISocketServer server)
        {
            _server = server;
        }

        public void Start(IPAddress address, int port, int maxClients)
        {
            _server.StartListening(address, port, maxClients);
            _server.ClientConnected += OnClientConnected;
            _server.DataReceived += OnDataReceived;
        }

        private async void OnClientConnected(string clientId)
        {
            _clients.TryAdd(clientId, new Client(clientId, ClientState.Connected));
            await _server.Send(clientId, "Enter your message to be notified: ");
            _clients[clientId].State = ClientState.SettingMessage;
        }

        private async void OnDataReceived(string clientId, string data)
        {
            Client client = _clients[clientId];
            switch(client.State)
            {
                case ClientState.SettingMessage:
                    client.Message = data;
                    await _server.Send(clientId, "Enter the time after which you will be notified  (ex. '00:00:05'): ");
                    _clients[clientId].State = ClientState.SettingAge;
                    break;
                case ClientState.SettingAge:
                    client.NotificationTime = TimeSpan.Parse(data);
                    await _server.Send(clientId, "Ok! Wait for your notification!");
                    _clients[clientId].State = ClientState.Waiting;

                    Task t = Task.Run(async delegate
                        {
                            await Task.Delay(_clients[clientId].NotificationTime);
                            await _server.Send(clientId, _clients[clientId].Message);
                        }
                    );
                    break;
                default:
                    await _server.Send(clientId, "Wrong command!");
                    break;
            }
        }

        public void Dispose()
        {
            _server.Dispose();
        }
    }
}
