using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;
using System.Security.Principal;



namespace SecurityAuthentication
{
    public static class FormsAuthenticationExt
    {
        public static void SetAuthCookie(ICustomIdentity customIdentity)
        {
            var cookie = FormsAuthentication.GetAuthCookie(customIdentity.Name, false);
            var ticket = FormsAuthentication.Decrypt(cookie.Value);

            StringBuilder userDataBuilder = new StringBuilder();
            foreach (var x in customIdentity.Claims)
            {
                userDataBuilder.AppendFormat("{0}'{1}'\r", x.Key, x.Value.Replace("'","''"));
            }
            ticket = new FormsAuthenticationTicket(ticket.Version,
                                                   ticket.Name,
                                                   ticket.IssueDate,
                                                   ticket.Expiration,
                                                   ticket.IsPersistent,
                                                   userDataBuilder.ToString(),
                                                   ticket.CookiePath);

            string encrypetedTicket = FormsAuthentication.Encrypt(ticket);
            //Set the authentication cookie and go back to the home page
            //HttpContext.Current.Response.Cookies.Set(cookie);

            if (!FormsAuthentication.CookiesSupported)
            {

                //This method works for both cookie and cookieless
                FormsAuthentication.SetAuthCookie(encrypetedTicket, false);

            }
            else
            {

                //This way only work for cookie
                cookie.Value = encrypetedTicket;
                HttpContext.Current.Response.Cookies.Set(cookie);
            }


        }

        public static void AttachIdentityToRequest(HttpApplication application)
        {
            var context = application.Context;
            var request = application.Request;

            var token = request.Cookies[FormsAuthentication.FormsCookieName];

            var ticket = FormsAuthentication.Decrypt(token.Value);

            CustomIdentity customIdentity = new CustomIdentity(context.User.Identity.Name);

            const string replacementOfSeparator = "@'@";
            string[] lineSeparator = new string[] { "'\r" };
            string[] userClaims = ticket.UserData.Replace("''", replacementOfSeparator).Split(lineSeparator, StringSplitOptions.None);
            foreach (string x in userClaims)
            {
                string[] keyValuePairs = x.Split('\'');
                if (!String.IsNullOrEmpty(keyValuePairs[0]))
                {
                    customIdentity.Claims.Add(keyValuePairs[0], x.Substring(keyValuePairs[0].Length+1).Replace(replacementOfSeparator, "'"));
                }
            }

            //Sync both web context user and current principal
            context.User = Thread.CurrentPrincipal = new CustomPrincipal(GetMappedCustomIdentity(customIdentity));

        }

        /// <summary>
        /// Mapping external system custom identity to local system identity.
        /// </summary>
        /// <param name="customIdentity">The custom Identity from login system</param>
        /// <returns>return a local custom identity</returns>
        private static ICustomIdentity GetMappedCustomIdentity(CustomIdentity customIdentity)
        {
            //get local identity from cache, if not get from plugin through configuarion setting
            //TODO--go through the autofac DI could easly solve the mapping issue.
            return customIdentity;
        }
    }
}
