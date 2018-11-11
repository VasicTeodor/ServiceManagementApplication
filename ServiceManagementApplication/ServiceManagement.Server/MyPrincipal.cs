using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ServiceManagement.Server
{
    class MyPrincipal : IPrincipal
    {
        public List<String> Prms = new List<string>();
        public List<String> Groups = new List<string>();
        public IIdentity Identity
        {
            get;
            set;
        }

        public MyPrincipal(WindowsIdentity identity)
        {
            Identity = identity;

            foreach (IdentityReference group in identity.Groups)
            {
                SecurityIdentifier sid = (SecurityIdentifier)group.Translate(typeof(SecurityIdentifier));
                var name = sid.Translate(typeof(NTAccount));

                if (name.Value.Contains("Admin"))
                {
                    if (!Prms.Contains("ExchangeSessionKey"))
                    {
                        Prms.Add("ExchangeSessionKey");
                    }

                    if (!Prms.Contains("RunService"))
                    {
                        Prms.Add("RunService");
                    }
                    Groups.Add("Admin");
                    Prms.Add("ModifyBlackList");
                }
                else if (name.Value.Contains("Client"))
                {
                    if (!Prms.Contains("ExchangeSessionKey"))
                    {
                        Prms.Add("ExchangeSessionKey");
                    }
                    Groups.Add("Client");
                }
                else if (name.Value.Contains("Executor"))
                {
                    if (!Prms.Contains("ExchangeSessionKey"))
                    {
                        Prms.Add("ExchangeSessionKey");
                    }

                    if (!Prms.Contains("RunService"))
                    {
                        Prms.Add("RunService");
                    }
                    Groups.Add("Executor");
                }
            }
        }

        public bool IsInRole(string role)
        {
            return (Prms.Contains(role));
        }
    }
}
