using System;
using System.Security;
using System.Security.Principal;
using System.Threading;

namespace SecurityAuthentication
{
    public class CustomPrincipal :GenericPrincipal,ICustomPrincipal
    {
        public CustomPrincipal(ICustomIdentity identity):
            base(identity,null)
        {
        }

        #region ICustomPrincipal Members

        public virtual bool hasPrivilege(string privilege, string resourceUri)
        {
 	        throw new System.NotImplementedException();
        }

        #endregion

    }
}
