using System;
using System.Configuration;
using System.Globalization;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Text;

namespace SecurityAuthentication
{
    public class AuthenticationModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PostAuthenticateRequest += PostAuthenticateRequest;
        }


        public void PostAuthenticateRequest(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            var context = application.Context;

            if (context.User.Identity.IsAuthenticated &&
                context.User.Identity.AuthenticationType == "Forms")
            {
                //Retrieve custom identity and sync it to both web context user and thread current principal
                FormsAuthenticationExt.AttachIdentityToRequest(application);
            }
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose() { }

    }
}
