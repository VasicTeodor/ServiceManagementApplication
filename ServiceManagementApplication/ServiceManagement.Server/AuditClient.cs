using ServiceManagement.CertificatesManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceManagement.Server
{
    public class AuditClient
    {
        private static AuditClient _instance;
        private static AuditProxy _proxy;
        public AuditClient()
        {
            NetTcpBinding binding = new NetTcpBinding();
            
            string srvCertCN = "auditservice";
            
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
            EndpointAddress address2 = new EndpointAddress(new Uri("net.tcp://localhost:9993/Audit"),
                                      new X509CertificateEndpointIdentity(srvCert));

            Proxy = new AuditProxy(binding, address2);
        }

        public static AuditClient Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new AuditClient();
                }

                return _instance;
            }
        }

        public AuditProxy Proxy
        {
            get { return _proxy; }
            set
            {
                if (value != _proxy)
                {
                    _proxy = value;
                }
            }
        }
    }
}
