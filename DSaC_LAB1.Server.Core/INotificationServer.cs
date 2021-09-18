using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DSaC_LAB1.Server.Core
{
    public interface INotificationServer : IDisposable
    {
        public void Start(IPAddress address, int port, int maxClients);
    }
}
