using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceManagement.Server
{
    public class BlackClass
    {
        private string _port;
        private string _protocol;
        private string _group;

        public BlackClass() { }

        public BlackClass(string port, string protocol, string group)
        {
            Port = port;
            Protocol = protocol;
            Group = group;
        }

        public string Group
        {
            get { return _group; }
            set { _group = value; }
        }

        public string Protocol
        {
            get { return _protocol; }
            set { _protocol = value; }
        }

        public string Port
        {
            get { return _port; }
            set { _port = value; }
        }

    }
}
