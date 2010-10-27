using System;
using System.Security.Principal;

namespace SecurityAuthentication
{
    /// <summary>
    /// Defines the basic functionality for custom principal
    /// </summary>
    public interface ICustomPrincipal : IPrincipal
    {
        /// <summary>
        /// Determines whether the current principal has privilege to access the resource.
        /// </summary>
        /// <param name="privilege">A privilege is a particular right or permission that can be granted or denied to a principal.
        /// There are two type privileges: 
        ///     Atomic privilege - a privilege that cannot be subdivided: it does not include other privileges.
        ///     Aggregate privilege – a privilege that includes other privileges.
        /// A privilege has a pre-defined name (system level) or custom-defined name (aggregate privilege registered by user)
        /// </param>
        /// <param name="resourceUri">A resource uri represents a single or set of protected objects.
        /// </param>
        /// <returns>Whether the privilege granted or denied to access specified resource for current principal</returns>
        bool hasPrivilege(string privilege, string resourceUri);
    }
}