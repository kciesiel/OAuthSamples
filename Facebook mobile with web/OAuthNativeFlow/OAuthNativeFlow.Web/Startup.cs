using System;
using Microsoft.Owin;
using Microsoft.Owin.Security.Facebook;
using Owin;

[assembly: OwinStartup(typeof(OAuthNativeFlow.Web.Startup))]

namespace OAuthNativeFlow.Web
{
	public class Startup
	{
		public static FacebookAuthenticationOptions FacebookAuthOptions { get; private set; }

		public void Configuration(IAppBuilder app)
		{
			// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
			//Configure Facebook External Login
			FacebookAuthOptions = new FacebookAuthenticationOptions
			{
				AppId = "",
				AppSecret = "",
				Provider = new FacebookAuthProvider()
			};
		}
	}
}
