using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DSaC_LAB1.Networking
{
    public delegate void ServerReceiveDelegate(string clientId, string data);
    public delegate void ServerClientConnectedDelegate(string clientId);

    public interface ISocketServer : IDisposable
    {
        public void StartListening(IPAddress address, int port, int maxClients);
        public Task<int> Send(string socketId, string data);
        public event ServerReceiveDelegate DataReceived;
        public event ServerClientConnectedDelegate ClientConnected;
        public void Close();
    }
}
