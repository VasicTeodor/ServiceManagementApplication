using ServiceManagement.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace ServiceManagement.Client
{
    class ClientProxy : ChannelFactory<IServiceManagement>, IServiceManagement, IDisposable
    {
        IServiceManagement factory;

        public ClientProxy(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
        {
            factory = this.CreateChannel();
        }

        public void Dispose()
        {
            if (factory != null)
            {
                factory = null;
            }

            this.Close();
        }

        public void AddPort(string port, string group)
        {
            try
            {
                factory.AddPort(port, group);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR while trying to add new port to blacklist: {0}", ex.Message);
            }
        }

        public void AddPortAndProtocol(string port, string protocol, string group)
        {
            try
            {
                factory.AddPortAndProtocol(port, protocol, group);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR while trying to add new port and protocol to blacklist: {0}", ex.Message);
            }
        }

        public void AddProtocol(string protocol, string group)
        {
            try
            {
                factory.AddProtocol(protocol, group);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR while trying to add new protocol to blacklist: {0}", ex.Message);
            }
        }

        public bool Checksum()
        {
            try
            {
                return factory.Checksum();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR while trying to call Checksum(): {0}", ex.Message);
                Process.GetCurrentProcess().Kill();
            }
            return false;
        }

        public string Connect()
        {
            try
            {
                return factory.Connect();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR while trying to connect to server: {0}", ex.Message);
            }
            return "";
        }

        public void StartService(byte[] array, int size, byte[] iVector)
        {
            try
            {
                factory.StartService(array, size, iVector);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR while trying to start service: {0}", ex.Message);
            }
        }
    }
}
