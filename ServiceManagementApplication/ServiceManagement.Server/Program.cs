using ServiceManagement.CertificatesManager;
using ServiceManagement.Common;
using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace ServiceManagement.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.ReadLine();

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            string address = "net.tcp://localhost:9999/ServiceManagement";

            ServiceHost host = new ServiceHost(typeof(ServiceManagement));
            host.AddServiceEndpoint(typeof(IServiceManagement), binding, address);

            host.Authorization.ServiceAuthorizationManager = new MyAuthorizationManager();

            List<IAuthorizationPolicy> authorizationPolicies = new List<IAuthorizationPolicy>();
            authorizationPolicies.Add(new AuthorizationPolicy());

            host.Authorization.ExternalAuthorizationPolicies = authorizationPolicies.AsReadOnly();
            host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;

            host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });


            host.Open();
            Console.WriteLine("ServiceManagement is opened. Press <enter> to finish...");
            Console.ReadLine();

            host.Close();
        }
    }
}
