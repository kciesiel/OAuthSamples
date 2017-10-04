namespace OAuthNativeFlow
{
    public static class Constants
    {
        public static string AppName = "OAuthNativeFlow";

		// OAuth
		// For Google login, configure at https://console.developers.google.com/
		public static string iOSClientId = "";
		public static string AndroidClientId = "";

		// These values do not need changing
		public static string AuthorizeUrl = "https://m.facebook.com/dialog/oauth/";
		public static string UserInfoUrl = "https://graph.facebook.com/me?fields=id,email,name,first_name,last_name";

		// Set these to reversed iOS/Android client ids, with :/oauth2redirect appended
		public static string iOSRedirectUrl = "<insert IOS redirect URL here>:/oauth2redirect";
        public static string AndroidRedirectUrl = "http://www.facebook.com/connect/login_success.html";
    }
}
