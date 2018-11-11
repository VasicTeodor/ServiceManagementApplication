using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceManagement.Common
{
    [ServiceContract]
    public interface IServiceManagement
    {
        [OperationContract]
        string Connect();

        [OperationContract]
        void StartService(byte[] array, int size, byte[] iVector);

        [OperationContract]
        bool Checksum();

        [OperationContract]
        void AddPort(string port, string group);

        [OperationContract]
        void AddProtocol(string protocol, string group);

        [OperationContract]
        void AddPortAndProtocol(string port, string protocol, string group);
    }
}
