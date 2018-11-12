using ServiceManagement.CertificatesManager;
using ServiceManagement.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceManagement.Server
{
    public class AuditProxy : ChannelFactory<IAudit>, IAudit, IDisposable
    {
        IAudit factory;

        public AuditProxy(NetTcpBinding binding, EndpointAddress address)
            : base(binding, address)
        {
            string cltCertCN = "smservice";

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

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

        public void DenialOfService()
        {
            try
            {
                factory.DenialOfService();
            }
            catch (Exception e)
            {
                Console.WriteLine("[DenialOfService] ERROR = {0}", e.Message);
            }
        }

        public void AuthorizationFailure(string port, string protocol, string machineName)
        {
            try
            {
                factory.AuthorizationFailure(port, protocol, machineName);
            }
            catch (Exception e)
            {
                Console.WriteLine("[AuthorizationFailure] ERROR = {0}", e.Message);
            }
        }

        public void ChecksumFailed()
        {
            try
            {
                factory.ChecksumFailed();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ChecksumFailed] ERROR = {0}", e.Message);
            }
        }
    }
}
