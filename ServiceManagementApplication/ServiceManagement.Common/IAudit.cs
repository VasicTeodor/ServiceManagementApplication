using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceManagement.Common
{
    [ServiceContract]
    public interface IAudit
    {
        [OperationContract]
        void DenialOfService();

        [OperationContract]
        void AuthorizationFailure(string port, string protocol, string machineName);

        [OperationContract]
        void ChecksumFailed();
    }
}
