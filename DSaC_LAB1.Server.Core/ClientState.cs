using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSaC_LAB1.Server.Core
{
    public enum ClientState
    {
        Connected,
        SettingMessage,
        SettingAge,
        Waiting
    }
}
