using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceManagement.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Proces for " + args[0] + " port and " + args[1] + " protocol";
            Console.WriteLine("Port: " + args[0]);
            Console.WriteLine("Protocol: " + args[1]);
           
            
            /*
            string port = args[0];
            string protocol = args[1];

            ServiceHost host = new ServiceHost(null);
            string address = "net.tcp://localhost:" + port + "/Something";

            switch (protocol)
            {
                case "tcp":
                    NetTcpBinding bindingTcp = new NetTcpBinding();
                    host.AddServiceEndpoint(null, bindingTcp, address);
                    break;
                case "http":
                    NetHttpBinding bindingHttp = new NetHttpBinding();
                    host.AddServiceEndpoint(null, bindingHttp, address);
                    break;
                case "namedpipe":
                    NetNamedPipeBinding bindingNamedPipe = new NetNamedPipeBinding();
                    host.AddServiceEndpoint(null, bindingNamedPipe, address);
                    break;
            }
            host.Open();

            host.Close();
            */
            Console.ReadLine();
        }
    }
}
