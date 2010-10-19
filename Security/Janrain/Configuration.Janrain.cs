using System;
using System.Configuration;
using System.Web.Configuration;
using QShine.Framework.Configuration;

namespace SecurityAuthentication.Configuration
{
    internal class JanrainSection : ConfigurationSection
    {
        private const string signInElement = "signIn";

        #region properties

        //defines section element collection
        [ConfigurationProperty(signInElement)]
        public NamedElementCollection<NamedValueElement> SignIn
        {
            get { return (NamedElementCollection<NamedValueElement>)this[signInElement]; }
            set { this[signInElement] = value;}
        }

        #endregion

    }

    public class JanrainSetting
    {
        private static JanrainSection section;
        private const string sectionName = "janrain";

        /// <summary>
        /// Get Janrain configuration section from web.config/app.config
        /// </summary>
        /// <returns>Return a repository configuration section instance</returns>
        internal static JanrainSection Section
        {
            get
            {
                if (section == null)
                {
                    section = (JanrainSection)ConfigurationManager.GetSection(sectionName);
                    if (section == null)
                    {
                        //Get web.config object
                        System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                        //Get apps.config for EXE only
                        //System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration( ConfigurationUserLevel.None) ;
                        foreach (var janrainSection in config.Sections)
                        {
                            if (janrainSection is JanrainSection)
                            {
                                section = (JanrainSection)janrainSection;
                                break;
                            }
                        }
                    }

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
            var signCollection = Section.SignIn;

            if (signCollection!=null && signCollection.Contains(name))
            {
                value = signCollection[name].Value;
            }
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
