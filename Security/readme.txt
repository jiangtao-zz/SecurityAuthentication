SecurityAuthentication provides a simple way to integrate user authentication with any web application
through an OpenID security token service (Janrain).

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
			<!-- Configuration section setting for janrain security service-->
		    <section name="janrain" type="System.Configuration.AppSettingsSection, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" />
		</configSections>
	<configuration>
	
	<!-- Configuration setting for janrain security service  -->
    <janrain>
		<!--replace it with your own home (default) page. This page should not be accessable by anonymous users.
		    When user failed or cancelled the login action, it will be redirect to this home page.-->
		<add key="homeUrl" value ="login.aspx"/>
		<!--replace it with your own applicationDomain from Janrain account -->
		<add key="applicationDomain" value ="golive.rpxnow.com"/>
		<!--replace it with your own appId from Janrain account -->
		<add key="appId" value ="cipolhlboagdhihckhgd"/>
		<!--replace it with your own apiKey from Janrain account -->
		<add key="apiKey" value ="cc3e8772a5a70ec1edcfb124ac396142e8e99b89"/>
		
		<!--
		Following options are only affect default popup login window. If you are using iframe to invoke a login widget, you need
		pass following properties through the querystring.
		-->
		
		<!--optional,default OpenId provider. See https://rpxnow.com/docs--.
		    You should not combine this option with flag property value "show_provider_list">
		<add key="default_provider" value="live_id"/>
		<!--optional,interface localization. See https://rpxnow.com/docs-->
		<add key="language_preference" value="en"/>
		<!--<add key="flags" value="show_provider_list" />-->
		<add key="flags" value="show_provider_list" />
		<!--optional,Janrain requires cross domain access file.-->
		<add key="xdReceiver" value="LoginAuthentication.axd?mode=xdReceiver" />
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

3. Access your page as normal, if the user is not authenticated, a login window popup.

4. If you want to add links for login and logout, simply add reference tag point to following url.

Login: ~/LoginAuthentication.axd?ReturnUrl=login.aspx
Logout: ~/LoginAuthentication.axd?mode=logout

5. If you want to embed a login widget in your page, just add following iframe. 
	
	<iframe src="loginAuthentication.axd?iframe=yes"  scrolling="no" 
        frameBorder="no" style="width:400px;height:240px;" 
        ></iframe>
	
	The janrain parameter values could pass through querystring. 
	(Ex: loginAuthentication.axd?iframe=yes&flags=show_provider_list)
		
		