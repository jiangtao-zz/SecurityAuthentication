using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SecurityAuthentication;
using System.Security;

namespace SampleWebSite
{
    public class SampleCustomPrincipal:CustomPrincipal
    {
        public SampleCustomPrincipal(SampleCustomIdentity identity)
            : base(identity)
        {
        }

        public SampleCustomPrincipal(CustomPrincipal principal)
            : base((new SampleCustomIdentity(principal.Identity as CustomIdentity)))
        {
            //Map external principal to local principal
            MapPrincipal(principal);
        }

        public static SampleCustomPrincipal Current
        {
            get
            {
                CustomPrincipal principal = System.Threading.Thread.CurrentPrincipal as CustomPrincipal;
                if (principal != null)
                {
                    SampleCustomPrincipal samplePrincipal = new SampleCustomPrincipal(principal);
                    return samplePrincipal;
                }
                throw new UnauthorizedAccessException("Couldn't find custom principal.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="privilege"></param>
        /// <param name="resourceUri"></param>
        /// <returns></returns>
        public override bool  hasPrivilege(string privilege, string resourceUri)
        {
            //TODO: Ask ACL engine to match the user permission
 	        return true;
        }
        
        /// <summary>
        /// Map external principal to local
        /// </summary>
        /// <param name="principal"></param>
        private void MapPrincipal(CustomPrincipal principal)
        {
            //TODO: Add your principal mapping logic here
            //The principal mapping solve the user permission data exchange problem between external and local system
        }
    }

    public class SampleCustomIdentity:CustomIdentity
    {
        public SampleCustomIdentity(string name)
            :base(name)
        {
        }

        public SampleCustomIdentity(CustomIdentity originalIdentity)
            : base(originalIdentity.Name)
        {
            MapIdentity(originalIdentity);
        }

        /// <summary>
        /// Map external identity attributes to local
        /// </summary>
        /// <param name="identity"></param>
        private void MapIdentity(CustomIdentity identity)
        {
            //TODO: Add your identity mapping logic here
            //The identity mapping solve the user attributes/profile data exchange problem between external and local system
        }
    }
}
