using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceManagement.Server
{
    class MyAuthorizationManager : ServiceAuthorizationManager
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            //WindowsIdentity principal = operationContext.ServiceSecurityContext.WindowsIdentity;
            //WindowsPrincipal windowsPrincipal = new WindowsPrincipal(principal);
            //WindowsPrincipal principal = operationContext.ServiceSecurityContext.AuthorizationContext.Properties["Principal"] as WindowsPrincipal;
            IPrincipal principal = operationContext.ServiceSecurityContext.AuthorizationContext.Properties["Principal"] as IPrincipal;
            MyPrincipal myPrincipal = principal as MyPrincipal;
            
            if (myPrincipal.IsInRole("ExchangeSessionKey"))
            {
                return true;
            }
            return false;
        }
    }
}
