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
            : base(principal.Identity as ICustomIdentity)
        {
            //Map external principal to local principal
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
    }

    public class SampleCustomIdentity:CustomIdentity
    {
        public SampleCustomIdentity(string name)
            :base(name)
        {
        }
    }
}
