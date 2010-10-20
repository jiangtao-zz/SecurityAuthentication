using System;
using System.Collections.Specialized;
using System.Configuration;

namespace SecurityAuthentication.Configuration
{
    /// <summary>
    /// Configuration setting for Janrain login service.
    /// Janrain provides a gateway service to access most social website OpenId security token service.
    /// We need create a janrain account for all your domain applications to consume Janrain login service.
    /// </summary>
    public class JanrainSetting
    {
        private static NameValueCollection section;
        private const string sectionName = "janrain";

        /// <summary>
        /// Get Janrain configuration section from web.config/app.config
        /// </summary>
        /// <returns>Return a repository configuration section instance</returns>
        internal static NameValueCollection Section
        {
            get
            {
                if (section == null)
                {
                    section = (NameValueCollection)ConfigurationManager.GetSection(sectionName);
                }
                if (section == null)
                {
                    throw (new SystemException("Couldn't find janrain configuration setting."));
                }
                return section;
            }
        }
        /// <summary>
        /// Retrieve named value pair
        /// </summary>
        /// <param name="name">name property</param>
        /// <param name="defaultValue">default value when the configuration setting is missing</param>
        /// <returns></returns>
        internal static string NamedValue(string name, string defaultValue)
        {
            string value = "";

            value = Section[name];
            if (value==null)
            {
                if (defaultValue == null)
                {
                    throw new SystemException(String.Format("The key element \"{0}\" is missing in janrain section", name));
                }
                //set default language preference
                value = defaultValue;
            }
            return value;

        }

        internal static string NamedValue(string name)
        {
            return NamedValue(name, null);
        }

        /// <summary>
        /// apiKey for each Janrain account.
        /// </summary>
        /// <remarks>
        /// Janrain will assign an API key for each Janrain account.
        /// The designated domains could use this apiKey to query security token from its STS service.
        /// </remarks>
        public static string ApiKey
        {
            get
            {
                return NamedValue("apiKey");
            }
        }

        /// <summary>
        /// login application domain for each Janrain account.
        /// </summary>
        /// <remarks>
        /// Janrain will assign a login application domain for each Janrain account.
        /// This application domain is a gateway to query STS service by Janrain system.
        /// </remarks>
        public static string ApplicationDomain
        {
            get
            {
                string value =NamedValue("applicationDomain");
                if (!value.EndsWith("/"))
                {
                    value += "/";
                }
                return value;
            }
        }
        /// <summary>
        /// property to get the Language preference 
        /// </summary>
        public static string LanguagePreference { 
            get {
                return NamedValue("language_preference","en");
            } 
        }

        /// <summary>
        /// property to get the default preference
        /// </summary>
        public static string DefaultProvider
        {
            get
            {
                return NamedValue("default_provider", "live_id");
            }
        }

        /// <summary>
        /// property to get the default preference
        /// </summary>
        public static string Overlay
        {
            get
            {
                return NamedValue("overlay", "true");
            }
        }
        /// <summary>
        /// property to get a home Url
        /// </summary>
        /// <remarks>
        /// The user will be re-direct to this home Url when the authentication process cancelled, 
        /// </remarks>
        public static string HomeUrl
        {
            get
            {
                return NamedValue("homeUrl", "");
            }
        }
    }
}
