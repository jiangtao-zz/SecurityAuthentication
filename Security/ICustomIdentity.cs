using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace SecurityAuthentication
{
    /// <summary>
    /// a simple claims identity interface for custom user identity
    /// </summary>
    public interface ICustomIdentity :IIdentity
    {
        Dictionary<string, string> Claims { get; set; }
    }
}
