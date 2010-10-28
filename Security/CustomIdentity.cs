using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace SecurityAuthentication
{
    /// <summary>
    /// A generic custom identity which contains three claims:
    ///     displayName: an Identity name to be display
    ///     userProvidersUniqueId: a unique Id within a user identity provider
    ///     email: email address associate to this identity
    ///     
    /// Any other claims could be found from Claims property collection.
    /// 
    /// </summary>
    /// <remarks>
    /// The CustomIdentity implemented a generic custom identity interface represent a user.
    /// 
    /// </remarks>
    public class CustomIdentity : GenericIdentity,ICustomIdentity
    {
        private const string userProvidersUniqueId = "userProvidersUniqueId";
        private const string displayName = "displayName";
        private const string email = "email";
        private const string providerName = "providerName";

        /// <summary>
        /// Construct a CustomIdentity object with a name property
        /// </summary>
        /// <param name="Name">Identity name</param>
        public CustomIdentity(string Name)
            : base(Name)
        {
            Claims = new Dictionary<string, string>();
        }

        /// <summary>
        /// Any claims provided by Identity provider
        /// </summary>
        public Dictionary<string, string> Claims {get;set;}

        /// <summary>
        /// user identity provider unique ID
        /// </summary>
        public string UserProvidersUniqueId
        {
            get
            {
                string value = null;
                Claims.TryGetValue(userProvidersUniqueId, out value);
                return value;
            }
            set
            {
                if (Claims.ContainsKey(userProvidersUniqueId))
                {
                    Claims[userProvidersUniqueId] = value;
                }
                else
                {
                    Claims.Add(userProvidersUniqueId, value);
                }
            }
        }

        /// <summary>
        /// User display name
        /// </summary>
        public string DisplayName
        {
            get
            {
                string value = null;
                Claims.TryGetValue(displayName, out value);
                return value;
            }
            set
            {
                if (Claims.ContainsKey(displayName))
                {
                    Claims[displayName] = value;
                }
                else
                {
                    Claims.Add(displayName, value);
                }
            }
        }

        /// <summary>
        /// basic email claim for user
        /// </summary>
        public string Email
        {
            get
            {
                string value = null;
                Claims.TryGetValue(email, out value);
                return value;
            }
            set
            {
                if (Claims.ContainsKey(email))
                {
                    Claims[email] = value;
                }
                else
                {
                    Claims.Add(email, value);
                }
            }

        }

        /// <summary>
        /// Claim provider name
        /// </summary>
        /// <remarks>
        /// The custom identity provided by external Identity System. Following are some provider name:
        /// "AOL"
        /// "Facebook"
        /// "Google"
        /// "Windows Lives"
        /// "MySpace"
        /// "OpenID"
        /// "Yahoo!"
        /// "Flickr"
        /// "MyOpenID"
        /// "Twitter "
        /// "LinkedIn"
        /// "Paypal"
        /// "Livejournal"
        /// "Verisign"
        /// "Wordpress"
        /// "Blogger"
        /// "Hyves"
        /// "Netlog"
        /// </remarks>
        public string ProviderName
        {
            get
            {
                string value = null;
                Claims.TryGetValue(providerName, out value);
                return value;
            }
            set
            {
                if (Claims.ContainsKey(providerName))
                {
                    Claims[providerName] = value;
                }
                else
                {
                    Claims.Add(providerName, value);
                }
            }
        }

    }
}
