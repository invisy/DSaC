using System.Text;
using System.Net;
using System.Net.Sockets;

namespace DSaC_LAB1.Networking;

public class SocketClient : ISocketClient
{
    private Socket _client;

    public event ClientReceiveDelegate DataReceived;

    public Task Connect(IPAddress address, int port)
    {
        IPEndPoint endPoint = new IPEndPoint(address, port);
        _client = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        Task connectTask = Task.Factory.FromAsync(
                _client.BeginConnect(endPoint, null, null),
                _client.EndConnect
        );

        return connectTask.ContinueWith(_ =>
            {
                ClientStateObject state = new();
                _client.BeginReceive(state.buffer, 0, ClientStateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
        );
    }

    private void ReceiveCallback(IAsyncResult asyncResult)
    {
        ClientStateObject state = (ClientStateObject) asyncResult.AsyncState;
        int bytesRead = _client.EndReceive(asyncResult);

        if (bytesRead > 0)
        {
            state.sb.Append(Encoding.Unicode.GetString(state.buffer, 0, bytesRead));

            if (state.sb[state.sb.Length-1] == char.MinValue) // char.MinValue is \0 in Unicode
            {
                DataReceived?.Invoke(state.sb.ToString(0, state.sb.Length - 1));
                state.Clear();
            }

            _client.BeginReceive(state.buffer, 0, ClientStateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
        }
    }

    public Task<int> Send(string data)
    {
        byte[] byteData = Encoding.Unicode.GetBytes(data + char.MinValue); // char.MinValue is \0 in Unicode

        return Task.Factory.FromAsync<int>(
                _client.BeginSend(byteData, 0, byteData.Length, 0, null, null),
                _client.EndSend
        );
    }

    public void Close()
    {
        Dispose();
    }

    public void Dispose()
    {
        _client.Shutdown(SocketShutdown.Both);
        _client.Close();
    }
}
