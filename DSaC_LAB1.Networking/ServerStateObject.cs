using System.Net.Sockets;
using System.Text;

namespace DSaC_LAB1.Networking
{
    public class ServerStateObject
    {
        public string clientId = string.Empty;
        public Socket clientSocket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new();

        public void Clear()
        {
            Array.Clear(buffer);
            sb.Clear();
        }
    }
}
