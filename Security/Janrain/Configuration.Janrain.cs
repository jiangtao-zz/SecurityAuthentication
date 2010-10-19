using System;
using System.Collections.Specialized;
using System.Configuration;

namespace SecurityAuthentication.Configuration
{

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
            if (String.IsNullOrEmpty(value))
            {
                //set default language preference
                value = defaultValue;
            }
            return value;

        }

        /// <summary>
        /// Janrain appKey for each relay partner
        /// </summary>
        public static string AppKey
        {
            get
            {
                return NamedValue("appKey", "");
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
    }
}
