using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSaC_LAB1.Server.Core
{
    public class Client
    {
        public string Id { get; private set; }
        public string Message { get; set; } = string.Empty;
        public ClientState State { get; set; }
        public TimeSpan NotificationTime { get; set; }

        public Client(string id, ClientState initialState)
        {
            Id = id;
            State = initialState;
        }
    }
}
