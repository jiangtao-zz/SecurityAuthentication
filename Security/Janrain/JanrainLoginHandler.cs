using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Security;
using System.Threading;
using System.Xml;
using System.Text;
using System.Globalization;

using System.Web;
using System.Web.Security;

using SecurityAuthentication.Configuration;
using SecurityAuthentication;

namespace SecurityAuthentication
{
    /// <summary>
    /// It is a standard http handler which provides a user Sign In and Sign Out request.
    /// To use this handler you need modify web.config to enable Forms authentication and point login url to this Authentication handler,
    /// and also make the handler accessable by anonymous users:
    ///<code lang="xml" title="Web.config/App.config">
    ///<![CDATA[
    /// <system.web>
    ///   ...
    ///   <httpHandlers>
    ///   ...
    ///     <add verb="GET,POST" path="LoginAuthentication.axd" type="SecurityAuthentication.Authentication, SecurityAuthentication" validate="false"/>
    ///   </httpHandlers>
    ///  
    ///   <authentication mode="Forms">
    ///     <forms loginUrl="~/LoginAuthentication.axd" timeout="30" cookieless="AutoDetect"/>
    ///   </authentication>
    ///   
    ///   <authorization>
    ///    <deny users="?" />
    ///   </authorization>
    ///   
    /// </system.web>
    /// 
    ///   <location path="LoginAuthentication.axd">
    ///     <system.web>
    ///         <authorization>
    ///             <allow users="*" />
    ///         </authorization>
    ///     </system.web>
    ///   </location>
    ///]]>
    ///</code>
    ///   
    /// </summary>
    public class JanrainLoginHandler : System.Web.IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;

            switch (request.QueryString["mode"])
            {
                case "login":
                    ProcessLoginRequest(context);
                    break;
                case "logout":
                    ProcessLogoutRequest(context);
                    break;
                case "xdReceiver":
                    ProcessXdReceiver(context);
                    break;
                default:
                    if (!String.IsNullOrEmpty(request.QueryString["iframe"]))
                    {
                        ProcessEmbedWidgetRequest(context);
                    }
                    else
                    {
                        ProcessDefaultWidgetRequest(context);
                    }
                    break;
            }
        }

        private void ProcessLogoutRequest(HttpContext context)
        {
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;

            if (context.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
            }
            response.Redirect(this.LogoutUrl);
        }

        private void ProcessLoginRequest(HttpContext context)
        {
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;

            string apiKey = JanrainSetting.ApiKey;
            const string paramToken = "token";

            //Check the token postback from Janrain Server
            String returnUrl = request.QueryString["originate"];

            // Get the login token passed back from the RPX authentication service
            string loginToken = request.Form[paramToken];

            if (String.IsNullOrEmpty(loginToken))
            {
                response.Redirect(JanrainSetting.HomeUrl);
                return;
            }

            // Create an RPX wrapper to get the user's data
            Rpx rpx = new Rpx(apiKey, this.RpxApplicationDomainUrl);


            try
            {
                // Get the user's details
                XmlElement authInfo = rpx.AuthInfo(loginToken);


                // Log the user in
                this.loginUser(authInfo, response, returnUrl);
            }
            catch (SystemException ex)
            {
                StringBuilder errorHtml = new StringBuilder();
                errorHtml.AppendLine("Failed to login through partner site.");
                errorHtml.AppendLine("<span style='display:none'>" + ex.Message + "</span>");
                response.Write(errorHtml.ToString());
            }

        }
        private void ProcessXdReceiver(HttpContext context)
        {
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;

            //Add javascript into current login page
            StringBuilder janrainScript = new StringBuilder();

            janrainScript.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            janrainScript.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            janrainScript.AppendLine("<head>");
            janrainScript.AppendLine("</head>");
            janrainScript.AppendLine("<body>");
            janrainScript.AppendLine("<script type=\"text/javascript\">");
            janrainScript.AppendLine("var rpxJsHost = ((\"https:\" == document.location.protocol) ? \"https://\" : \"http://static.\");");
            janrainScript.AppendLine("document.write(unescape(\"%3Cscript src='\" + rpxJsHost +");
            janrainScript.AppendLine("\"rpxnow.com/js/lib/rpx.js' type='text/javascript'%3E%3C/script%3E\"));");
            janrainScript.AppendLine("</script>");
            janrainScript.AppendLine("</body>");
            janrainScript.AppendLine("</html>");
            response.Write(janrainScript.ToString());

        }

        private void ProcessDefaultWidgetRequest(HttpContext context)
        {
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;

            //Add javascript into current login page
            StringBuilder janrainScript = new StringBuilder();

            janrainScript.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            janrainScript.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            janrainScript.AppendLine("<head>");
            janrainScript.AppendLine("</head>");
            janrainScript.AppendLine("<body>");
            janrainScript.AppendLine("<script type=\"text/javascript\">");
            janrainScript.AppendLine("var rpxJsHost = ((\"https:\" == document.location.protocol) ? \"https://\" : \"http://static.\");");
            janrainScript.AppendLine("document.write(unescape(\"%3Cscript src='\" + rpxJsHost +");
            janrainScript.AppendLine("\"rpxnow.com/js/lib/rpx.js' type='text/javascript'%3E%3C/script%3E\"));");
            janrainScript.AppendLine("</script>");
            janrainScript.AppendLine("<script type=\"text/javascript\">");

            string xdReceiver = JanrainSetting.XdReceiver;

            if (String.IsNullOrEmpty(xdReceiver))
            {
                xdReceiver = this.XdReceiverUrl;
            }
            if (!(xdReceiver.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) ||
                xdReceiver.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase)))
            {
                if (xdReceiver.StartsWith("/"))
                {
                    xdReceiver = xdReceiver.Substring(1);
                }
                xdReceiver = this.ApplicationRootUrl + xdReceiver;
            }
            janrainScript.AppendLine("RPXNOW.init ({ appId: \""+JanrainSetting.AppId+"\", xdReceiver: \""+xdReceiver+"\"});");
            if (!String.IsNullOrEmpty(JanrainSetting.Overlay))
            {
                janrainScript.AppendLine("RPXNOW.overlay = " + JanrainSetting.Overlay + ";");
            }
            if (!String.IsNullOrEmpty(JanrainSetting.LanguagePreference))
            {
                janrainScript.AppendLine("RPXNOW.language_preference = '" + JanrainSetting.LanguagePreference + "';");
            }
            if (!String.IsNullOrEmpty(JanrainSetting.DefaultProvider))
            {
                janrainScript.AppendLine("RPXNOW.default_provider = '" + JanrainSetting.DefaultProvider + "';");
            }
            if (!String.IsNullOrEmpty(JanrainSetting.Flags))
            {
                janrainScript.AppendLine("RPXNOW.flags = \"" + JanrainSetting.Flags + "\";");
            }
            janrainScript.AppendLine("RPXNOW.ssl = " + ((JanrainSetting.Ssl) ? "true" : "false") + ";");
            janrainScript.AppendLine("RPXNOW.realm = \"" + this.RpxApplicationDomain + "\";");
            janrainScript.AppendLine("RPXNOW.token_url = \"" + this.TokenUrl + "\";");
            janrainScript.AppendLine("RPXNOW.show();");
            janrainScript.AppendLine("</script>");

            //janrainScript.AppendLine("<div align=\"center\"><iframe src=\"" + GetLoginUrl(request) + "\"  scrolling=\"no\" frameBorder=\"no\" style=\"width:400px;height:240px; vertical-align: middle;\" align=\"center\"></iframe></center>");

            janrainScript.AppendLine("</body>");
            janrainScript.AppendLine("</html>");
            response.Write(janrainScript.ToString());
        }


        //private void ProcessDefaultWidgetRequest1(HttpContext context)
        //{
        //    HttpResponse response = context.Response;
        //    HttpRequest request = context.Request;

        //    Add javascript into current login page
        //    StringBuilder janrainScript = new StringBuilder();
        //    String returnUrl = request.QueryString["ReturnUrl"];

        //    janrainScript.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
        //    janrainScript.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
        //    janrainScript.AppendLine("<head>");
        //    janrainScript.AppendLine("</head>");
        //    janrainScript.AppendLine("<body>");
        //    janrainScript.AppendLine("<a id=\"loginRef\" href=\""+this.rpxApplicationDomain+"/openid/v2/signin?token_url=http%3A%2F%2Fgo.qshine.com%2FLoginAuthentication.axd%3Fmode%3Dlogin%26originate%3D" + returnUrl + "\">Redirect user to login page</a>");
        //    janrainScript.AppendLine("<script type=\"text/javascript\">");
        //    janrainScript.AppendLine("var rpxJsHost = ((\"https:\" == document.location.protocol) ? \"https://\" : \"http://static.\");");
        //    janrainScript.AppendLine("document.write(unescape(\"%3Cscript src='\" + rpxJsHost +");
        //    janrainScript.AppendLine("\"rpxnow.com/js/lib/rpx.js' type='text/javascript'%3E%3C/script%3E\"));");
        //    janrainScript.AppendLine("</script>");
        //    janrainScript.AppendLine("<script type=\"text/javascript\">");
        //    janrainScript.AppendLine("RPXNOW.overlay = "+JanrainSetting.Overlay+";");
        //    janrainScript.AppendLine("RPXNOW.language_preference = '" + JanrainSetting.LanguagePreference + "';");
        //    janrainScript.AppendLine("RPXNOW.default_provider = \"" + JanrainSetting.DefaultProvider + "\";");
        //    janrainScript.AppendLine("</script>");

        //    Send request to Janrain for user login
        //    janrainScript.AppendLine("<script type=\"text/javascript\">");
        //    janrainScript.AppendLine("setTimeout('loginJanRain()',1);");
        //    janrainScript.AppendLine("function loginJanRain(){document.getElementById('loginRef').click();}");
        //    janrainScript.AppendLine("</script>");
        //    janrainScript.AppendLine("</body>");
        //    janrainScript.AppendLine("</html>");
        //    response.Write(janrainScript.ToString());
        //}

        private void ProcessEmbedWidgetRequest(HttpContext context)
        {
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;
            string url = this.LoginUrl;
            foreach (string x in request.QueryString.AllKeys)
            {
                if (x != "iframe")
                {
                    url += String.Format("&{0}={1}", x, request.QueryString[x]);
                }
            }
            response.Redirect(url);
        }
        /// <summary>
        /// Get current web application root Url which end with "/"
        /// </summary>
        private string ApplicationRootUrl
        {
            get
            {
                HttpRequest request = HttpContext.Current.Request;
                string root = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath;
                if (!root.EndsWith("/"))
                {
                    root = root + "/";
                }
                return root;
            }
        }

        private string XdReceiverUrl
        {
            get
            {
                return ApplicationRootUrl + "LoginAuthentication.axd?mode=xdReceiver";
            }
        }

        private string TokenUrl
        {
            get
            {
                HttpRequest request = HttpContext.Current.Request;

                String returnUrl = request.QueryString["ReturnUrl"];

                if (String.IsNullOrEmpty(returnUrl))
                {
                    returnUrl = JanrainSetting.HomeUrl;
                }
                String tokenUrl = ApplicationRootUrl + "LoginAuthentication.axd?mode=login&originate=" + returnUrl;
                return tokenUrl;
            }
        }

        private string RpxApplicationDomainUrl
        {
            get
            {
                return "https://" + this.RpxApplicationDomain;
            }
        }

        private string RpxApplicationDomain
        {
            get
            {
                string appDomainUrl = JanrainSetting.ApplicationDomain;
                if (!appDomainUrl.StartsWith("https://"))
                {
                    appDomainUrl = "https://" + appDomainUrl;
                }
                Uri url = new Uri(appDomainUrl);

                return url.Authority;
            }
        }

        private string LoginUrl
        {
            get
            {
                return this.RpxApplicationDomainUrl + 
                    "/openid/embed?token_url=" + 
                    HttpContext.Current.Server.UrlEncode(this.TokenUrl);
            }
        }

        private string LogoutUrl
        {
            get
            {
                string url = JanrainSetting.HomeUrl;
                return url;
            }
        }


        private void loginUser(XmlElement authInfo, HttpResponse response,string returnUrl)
        {
            // Get the user's unique identifier (this will ALWAYS be returned regardless of the login provider
            string userProvidersUniqueID = authInfo.GetElementsByTagName("identifier")[0].InnerText;

            //Save janrain claims properties in cookie for future access.
            // get a unique identity name froma janrain
            CustomIdentity customIdentity = new CustomIdentity(userProvidersUniqueID);
            customIdentity.UserProvidersUniqueId = userProvidersUniqueID;

            string[] authFields = new string[] { "identifier", "displayName","providerName","primaryKey","preferredUsername",
            "gender","birthday","utcOffset","email","verifiedEmail","url"};

            foreach (var x in authFields)
            {
                // See if the user's display name is provided (not supplied by some providers
                XmlNodeList nodeList = authInfo.GetElementsByTagName(x);
                string value = null;
                if (nodeList != null && nodeList.Count > 0)
                {
                    // Got a display name
                    value = nodeList[0].InnerText;
                    customIdentity.Claims.Add(x, value);
                }
            }

            //Set the authentication cookie and go back to the home page
            FormsAuthenticationExt.SetAuthCookie(customIdentity);
            //FormsAuthentication.SetAuthCookie(userProvidersUniqueID, false);
            response.Redirect(returnUrl);
        }

    }
}
