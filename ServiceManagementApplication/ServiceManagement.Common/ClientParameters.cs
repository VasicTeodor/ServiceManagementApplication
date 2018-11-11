using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceManagement.Common
{
    public class ClientParameters
    {
        [Serializable]
        public struct ClientInfo
        {
            public string port;
            public string protocol;
            public string machineName;
        }
    }
}
