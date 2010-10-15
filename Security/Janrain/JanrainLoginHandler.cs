using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Xml.Linq;
using System.Security;

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.XPath;

using SecurityAuthentication.Configuration;

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
            HttpResponse response = context.Response;

            if (request.QueryString["mode"] == "login")
            {
                string apiKey = JanrainSetting.AppKey;// "cc3e8772a5a70ec1edcfb124ac396142e8e99b89";
                const string paramToken = "token";

                //Check the token postback from Janrain Server
                String returnUrl = request.QueryString["originate"];

                // Get the login token passed back from the RPX authentication service
                string loginToken = request.Form[paramToken];

                // Create an RPX wrapper to get the user's data
                Rpx rpx = new Rpx(apiKey, "https://golive.rpxnow.com/");


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
                    errorHtml.AppendLine("<span style='display:none'>"+ex.Message+"</span>");
                    response.Write(errorHtml.ToString());
                }

            }
            else
            {
                //Add javascript into current login page
                StringBuilder janrainScript = new StringBuilder();
                String returnUrl = request.QueryString["ReturnUrl"];

                janrainScript.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
                janrainScript.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
                janrainScript.AppendLine("<head>");
                janrainScript.AppendLine("</head>");
                janrainScript.AppendLine("<body>");
                janrainScript.AppendLine("<a id=\"loginRef\" href=\"https://golive.rpxnow.com/openid/v2/signin?token_url=http%3A%2F%2Fgo.qshine.com%2FLoginAuthentication.axd%3Fmode%3Dlogin%26originate%3D" + returnUrl + "\">Redirect user to login page</a>");
                janrainScript.AppendLine("<script type=\"text/javascript\">");
                janrainScript.AppendLine("var rpxJsHost = ((\"https:\" == document.location.protocol) ? \"https://\" : \"http://static.\");");
                janrainScript.AppendLine("document.write(unescape(\"%3Cscript src='\" + rpxJsHost +");
                janrainScript.AppendLine("\"rpxnow.com/js/lib/rpx.js' type='text/javascript'%3E%3C/script%3E\"));");
                janrainScript.AppendLine("</script>");
                janrainScript.AppendLine("<script type=\"text/javascript\">");
                janrainScript.AppendLine("RPXNOW.overlay = "+JanrainSetting.Overlay+";");
                janrainScript.AppendLine("RPXNOW.language_preference = '" + JanrainSetting.LanguagePreference + "';");
                janrainScript.AppendLine("RPXNOW.default_provider = \"" + JanrainSetting.DefaultProvider + "\";");
                janrainScript.AppendLine("</script>");

                //Send request to Janrain for user login
                janrainScript.AppendLine("<script type=\"text/javascript\">");
                janrainScript.AppendLine("setTimeout('loginJanRain()',1);");
                janrainScript.AppendLine("function loginJanRain(){document.getElementById('loginRef').click();}");
                janrainScript.AppendLine("</script>");
                janrainScript.AppendLine("</body>");
                janrainScript.AppendLine("</html>");
                response.Write(janrainScript.ToString());
            }
        }

        private void loginUser(XmlElement authInfo, HttpResponse response,string returnUrl)
        {
            // Get the user's unique identifier (this will ALWAYS be returned regardless of the login provider
            string userProvidersUniqueID = authInfo.GetElementsByTagName("identifier")[0].InnerText;


            // See if the user's display name is provided (not supplied by some providers
            XmlNodeList displayNameNodeList = authInfo.GetElementsByTagName("displayName");
            string displayName = null;
            if (displayNameNodeList != null && displayNameNodeList.Count > 0)
            {
                // Got a display name
                displayName = displayNameNodeList[0].InnerText;
            }
            else
            {
                // No display name
            }
            // See if the user's email address is provided (not supplied by some providers)
            XmlNodeList emailAddressNodeList = authInfo.GetElementsByTagName("email");


            string emailAddress = null;


            if (emailAddressNodeList != null && emailAddressNodeList.Count > 0)
            {
                // Got an email address
                emailAddress = emailAddressNodeList[0].InnerText;
            }
            else
            {
                // No email address
            }
            // Set the authentication cookie and go back to the home page
            FormsAuthentication.SetAuthCookie(userProvidersUniqueID, false);
            response.Redirect(returnUrl);
        }

    }

    public class Rpx
    {
        private string apiKey;
        private string baseUrl;

        public Rpx(string apiKey, string baseUrl)
        {
            while (baseUrl.EndsWith("/"))
                baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);

            this.apiKey = apiKey;
            this.baseUrl = baseUrl;
        }

        public string getApiKey() { return apiKey; }
        public string getBaseUrl() { return baseUrl; }

        public XmlElement AuthInfo(string token)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();
            query.Add("token", token);
            return ApiCall("auth_info", query);
        }

        public ArrayList Mappings(string primaryKey)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();
            query.Add("primaryKey", primaryKey);
            XmlElement rsp = ApiCall("mappings", query);
            XmlElement oids = (XmlElement)rsp.FirstChild;

            ArrayList result = new ArrayList();

            for (int i = 0; i < oids.ChildNodes.Count; i++)
            {
                result.Add(oids.ChildNodes[i].InnerText);
            }

            return result;
        }

        public Dictionary<string, ArrayList> AllMappings()
        {
            Dictionary<string, string> query = new Dictionary<string, string>();
            XmlElement rsp = ApiCall("all_mappings", query);

            Dictionary<string, ArrayList> result = new Dictionary<string, ArrayList>();
            XPathNavigator nav = rsp.CreateNavigator();

            XPathNodeIterator mappings = (XPathNodeIterator)nav.Evaluate("/rsp/mappings/mapping");
            foreach (XPathNavigator m in mappings)
            {
                string remote_key = GetContents("./primaryKey/text()", m);
                XPathNodeIterator ident_nodes = (XPathNodeIterator)m.Evaluate("./identifiers/identifier");
                ArrayList identifiers = new ArrayList();
                foreach (XPathNavigator i in ident_nodes)
                {
                    identifiers.Add(i.ToString());
                }

                result.Add(remote_key, identifiers);
            }

            return result;
        }

        private string GetContents(string xpath_expr, XPathNavigator nav)
        {
            XPathNodeIterator rk_nodes = (XPathNodeIterator)nav.Evaluate(xpath_expr);
            while (rk_nodes.MoveNext())
            {
                return rk_nodes.Current.ToString();
            }
            return null;
        }

        public void Map(string identifier, string primaryKey)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();
            query.Add("identifier", identifier);
            query.Add("primaryKey", primaryKey);
            ApiCall("map", query);
        }

        public void Unmap(string identifier, string primaryKey)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();
            query.Add("identifier", identifier);
            query.Add("primaryKey", primaryKey);
            ApiCall("unmap", query);
        }

        private XmlElement ApiCall(string methodName, Dictionary<string, string> partialQuery)
        {
            Dictionary<string, string> query = new Dictionary<string, string>(partialQuery);
            query.Add("format", "xml");
            query.Add("apiKey", apiKey);

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> e in query)
            {
                if (sb.Length > 0)
                {
                    sb.Append('&');
                }

                sb.Append(System.Web.HttpUtility.UrlEncode(e.Key, Encoding.UTF8));
                sb.Append('=');
                sb.Append(HttpUtility.UrlEncode(e.Value, Encoding.UTF8));
            }
            string data = sb.ToString();

            Uri url = new Uri(baseUrl + "/api/v2/" + methodName);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            // Write the request
            StreamWriter stOut = new StreamWriter(request.GetRequestStream(),
                                                  Encoding.ASCII);
            stOut.Write(data);
            stOut.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();

            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.Load(dataStream);

            XmlElement resp = doc.DocumentElement;

            if (!resp.GetAttribute("stat").Equals("ok"))
            {
                throw new SystemException("Unexpected API error:"+resp.OuterXml);
            }

            return resp;
        }

        public static void Main(string[] args)
        {
            Rpx r = new Rpx(args[0], args[1]);

            if (args[2].Equals("mappings"))
            {
                Console.WriteLine("Mappings for " + args[3] + ":");
                foreach (string s in r.Mappings(args[3]))
                {
                    Console.WriteLine(s);
                }
            }

            if (args[2].Equals("all_mappings"))
            {
                Console.WriteLine("All mappings:");

                foreach (KeyValuePair<string, ArrayList> pair in r.AllMappings())
                {
                    Console.WriteLine(pair.Key + ":");
                    foreach (string identifier in pair.Value)
                    {
                        Console.WriteLine("  " + identifier);
                    }
                }
            }

            if (args[2].Equals("map"))
            {
                Console.WriteLine(args[3] + " mapped to " + args[4]);
                r.Map(args[3], args[4]);
            }

            if (args[2].Equals("unmap"))
            {
                Console.WriteLine(args[3] + " unmapped from " + args[4]);
                r.Unmap(args[3], args[4]);
            }
        }
    }
}
