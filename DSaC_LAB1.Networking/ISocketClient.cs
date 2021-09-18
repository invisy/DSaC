using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DSaC_LAB1.Networking
{
    public delegate void ClientReceiveDelegate(string data);

    public interface ISocketClient: IDisposable
    {
        public Task Connect(IPAddress address, int port);
        public Task<int> Send(string data);
        public event ClientReceiveDelegate DataReceived;
        public void Close();
    }
}
