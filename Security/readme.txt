SecurityAuthentication provides a simple way to integrate user authentication with any web application 
through an OpenID security token service.

Usage:

1. Add a reference of SecurityAuthentication.dll in your web project.
2. Modify web.config, make sure following section and elements set correctly:

	<?xml version="1.0"?>
	<!--Set login process related parameters configuration setting.
	In this case, we need Janrain service related parameters-->
	-->
	<configuration>
		<configSections>
			...
		    <section name="janrain" type="System.Configuration.AppSettingsSection, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" />
		</configSections>
	<configuration>
	
    <janrain>
      <add key="default_provider" value="live_id"/>
      <add key="language_preference" value="en"/>
      <!--replace it with your own applicationDomain from Janrain account -->
      <add key="applicationDomain" value ="https://golive.rpxnow.com/"/>
      <!--replace it with your own apiKey from Janrain account -->
      <add key="apiKey" value ="ac3e8772a5a70ec1edcfb124ac396142e8e99b89"/>
      <!--replace it with your own home page when login failed--> 
      <add key="homeUrl" value ="http://www.yahoo.com"/>
    </janrain>

	<!--make sure everyone can access the login page-->
    <location path="LoginAuthentication.axd">
      <system.web>
        <authorization>
          <allow users="*" />
        </authorization>
      </system.web>
    </location>
  
    <system.web>
	  ...
	  <!--set Forms authentication mode and login Url. The login Url is a security authentication handler-->
	  <authentication mode="Forms">
		<forms loginUrl="~/LoginAuthentication.axd" timeout="30" cookieless="AutoDetect"/>
	  </authentication>
	  <!--deny anonymous users access->
      <authorization>
        <deny users="?" />
      </authorization>

	  <!--specifies a user login service, it could be any built-in http handler or a custom http handler which implemented 
	  ICustomIdentity in user principal.
	  In this sample, we are using Janrain service to process the user login -->
	  <httpHandlers>
		<add verb="GET,POST" path="LoginAuthentication.axd" type="SecurityAuthentication.JanrainLoginHandler, SecurityAuthentication" validate="false"/>
	  </httpHandlers>
	
	  <!--Add http module to validate user authentication and set user principal ICustomIdentity in web context and thread-->
	  <httpModules>
	    <add name="AuthenticationModule" type="SecurityAuthentication.AuthenticationModule, SecurityAuthentication"/>
      </httpModules>
	</system.web>
</configuration>

	