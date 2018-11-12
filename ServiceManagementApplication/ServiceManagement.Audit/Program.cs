using ServiceManagement.CertificatesManager;
using ServiceManagement.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;


namespace ServiceManagement.Audit
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadKey();
            
            string srvCertCN = "auditservice";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            string address = "net.tcp://localhost:9993/Audit";
            ServiceHost host = new ServiceHost(typeof(Audit));
            host.AddServiceEndpoint(typeof(IAudit), binding, address);
            
            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            
            host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            
            host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

            try
            {
                host.Open();
                Console.WriteLine("Audit service is started.\nPress <enter> to stop ...");
                Console.ReadLine();
                Audit.shutDown = false;
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }
            finally
            {
                host.Close();
            }
        }
    }
}
