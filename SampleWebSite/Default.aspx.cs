using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecurityAuthentication
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string UserData = "email'jhu@bljc.com'\rdisplayName'Jiangtao Hu'\ruserProvidersUniqueId'1234ABC'\r";

            CustomIdentity customIdentity = new CustomIdentity("ABC");

            const string replacementOfSeparator = "@'@";
            string[] lineSeparator = new string[] { "'\r" };
            string[] userClaims = UserData.Replace("''", replacementOfSeparator).Split(lineSeparator, StringSplitOptions.None);
            foreach (string x in userClaims)
            {
                string[] keyValuePairs = x.Split('\'');
                if (!String.IsNullOrEmpty(keyValuePairs[0]))
                {
                    customIdentity.Claims.Add(keyValuePairs[0], x.Substring(keyValuePairs[0].Length).Replace(replacementOfSeparator, "'"));
                }
            }

            Response.Write(customIdentity.UserProvidersUniqueId);
        
        }
    }
}
