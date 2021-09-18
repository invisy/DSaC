using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;

namespace DSaC_LAB1.Networking;

public class SocketServer : ISocketServer
{
    private Socket _listener;
    private readonly ConcurrentDictionary<string, Socket> _clients = new();

    public event ServerReceiveDelegate DataReceived;
    public event ServerClientConnectedDelegate ClientConnected;

    public void StartListening(IPAddress address, int port, int maxClients)
    {
        IPEndPoint localEndPoint = new IPEndPoint(address, port);
        _listener = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _listener.Bind(localEndPoint);
        _listener.Listen(maxClients);

        Task listening = Task.Run(async () =>
        {
            while (true)
                await Accept();
        });
    }

    private Task Accept()
    {
        Task<Socket> acceptTask = Task.Factory.FromAsync<Socket>(
                _listener.BeginAccept(null, null),
                _listener.EndAccept
            );

        return acceptTask.ContinueWith(async (socket) =>
            {
                Socket client = socket.Result;

                IPEndPoint clientEndPoint = (IPEndPoint)client.RemoteEndPoint;
                string clientId = $"{clientEndPoint.Address.ToString()}:{clientEndPoint.Port.ToString()}";
                _clients.TryAdd(clientId, client);

                ServerStateObject state = new();
                state.clientId = clientId;
                state.clientSocket = client;
                client.BeginReceive(state.buffer, 0, ServerStateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                ClientConnected?.Invoke(clientId);
            }
        );
    }

    private void ReceiveCallback(IAsyncResult asyncResult)
    {
        ServerStateObject state = (ServerStateObject)asyncResult.AsyncState;
        Socket client = state.clientSocket;
        int bytesRead = client.EndReceive(asyncResult);

        if (bytesRead > 0)
        {
            state.sb.Append(Encoding.Unicode.GetString(state.buffer, 0, bytesRead));

            if (state.sb[state.sb.Length - 1] == char.MinValue) // char.MinValue is \0 in Unicode
            {
                DataReceived?.Invoke(state.clientId, state.sb.ToString(0, state.sb.Length - 1));
                state.Clear();
            }
            
            client.BeginReceive(state.buffer, 0, ServerStateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
        }
    }

    public Task<int> Send(string socketId, string data)
    {
        if (!_clients.ContainsKey(socketId))
            throw new Exception("Client does not exist!");

        Socket handler = _clients[socketId];
        byte[] byteData = Encoding.Unicode.GetBytes(data + char.MinValue); // char.MinValue is \0 in Unicode

        return Task.Factory.FromAsync<int>(
                handler.BeginSend(byteData, 0, byteData.Length, 0, null, null),
                handler.EndSend
        );
    }

    public void Close()
    {
        Dispose();
    }

    public void Dispose()
    {
        foreach(Socket client in _clients.Values)
            client.Shutdown(SocketShutdown.Both);
        _listener.Close();
    }
}