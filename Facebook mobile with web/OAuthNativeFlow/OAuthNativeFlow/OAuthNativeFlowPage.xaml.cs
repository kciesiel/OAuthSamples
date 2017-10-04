using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Auth;

namespace OAuthNativeFlow
{
	public partial class OAuthNativeFlowPage : ContentPage
	{
		private Account _account;
		readonly AccountStore _store;

		public OAuthNativeFlowPage()
		{
			InitializeComponent();

			_store = AccountStore.Create();
			_account = _store.FindAccountsForService(Constants.AppName).FirstOrDefault();
		}

		void OnLoginClicked(object sender, EventArgs e)
		{
			string clientId = null;
			string redirectUri = null;

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
					clientId = Constants.iOSClientId;
					redirectUri = Constants.iOSRedirectUrl;
					break;

				case Device.Android:
					clientId = Constants.AndroidClientId;
					redirectUri = Constants.AndroidRedirectUrl;
					break;
			}

			var authenticator = new OAuth2Authenticator(
				clientId,
				"email",
				new Uri(Constants.AuthorizeUrl),
				new Uri(redirectUri));

			authenticator.Completed += OnAuthCompleted;
			authenticator.Error += OnAuthError;

			AuthenticationState.Authenticator = authenticator;

			var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
			presenter.Login(authenticator);
		}

		async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
		{
			var authenticator = sender as OAuth2Authenticator;
			if (authenticator != null)
			{
				authenticator.Completed -= OnAuthCompleted;
				authenticator.Error -= OnAuthError;
			}

			User user = null;
			if (e.IsAuthenticated)
			{
				// If the user is authenticated, request their basic user data from Google
				// UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
				IDictionary<string, string> param = new Dictionary<string, string>();
				param.Add("client_secret", "bee13e49483fbb62c957fbb23605c9dd");

				var request = new OAuth2Request("GET", new Uri(Constants.UserInfoUrl), param, e.Account);
				var response = await request.GetResponseAsync();
				if (response != null)
				{
					// Deserialize the data and store it in the account store
					// The users email address will be used to identify data in SimpleDB
					var userJson = await response.GetResponseTextAsync();
					user = JsonConvert.DeserializeObject<User>(userJson);
				}

				if (_account != null)
				{
					_store.Delete(_account, Constants.AppName);
				}

				var str = JsonConvert.SerializeObject(e.Account);
				await _store.SaveAsync(_account = e.Account, Constants.AppName);

				string accessToken = _account.Properties["access_token"];

				await SendToken(accessToken);
				await DisplayAlert("Email address", user.Email, "OK");
			}
		}

		private async Task SendToken(string accessToken)
		{
			using (var httpClient = new HttpClient())
			{
				var url = $"http://localhost:62814/api/verify/{accessToken}";
				var response = await httpClient.GetAsync(url);
				response.EnsureSuccessStatusCode();

				string content = await response.Content.ReadAsStringAsync();
			}

		}

		void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
		{
			var authenticator = sender as OAuth2Authenticator;
			if (authenticator != null)
			{
				authenticator.Completed -= OnAuthCompleted;
				authenticator.Error -= OnAuthError;
			}

			Debug.WriteLine("Authentication error: " + e.Message);
		}
	}
}
