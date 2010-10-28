using System;
using System.Security;
using System.Security.Principal;
using System.Threading;

namespace SecurityAuthentication
{
    public class CustomPrincipal :GenericPrincipal,ICustomPrincipal
    {
        /// <summary>
        /// Constractor of custom principal
        /// </summary>
        /// <param name="identity">user identity with custom attributes</param>
        public CustomPrincipal(ICustomIdentity identity) :
            base(identity,null)
        {
        }

        #region ICustomPrincipal Members
        /// <summary>
        /// Override Identity property
        /// </summary>
        public virtual new ICustomIdentity Identity
        {
            get
            {
                return base.Identity as ICustomIdentity;
            }
        }

        /// <summary>
        /// Determines current principal has privilege to access specific security objects
        /// </summary>
        /// <param name="privilege">specific privilege</param>
        /// <param name="resourceUri">Represents a single security object or a set of resource.
        /// The format of resourceUri looks like res://parent-Resource/child-resource/{*|single object}</param>
        /// <returns></returns>
        public virtual bool hasPrivilege(string privilege, string resourceUri)
        {
 	        throw new System.NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Retrieve current custom principal. The principal is null if the user is not authenticated by system
        /// </summary>
        public static CustomPrincipal Current
        {
            get
            {
                CustomPrincipal principal = System.Threading.Thread.CurrentPrincipal as CustomPrincipal;
                return principal;
            }
        }


    }
}
