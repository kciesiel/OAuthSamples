using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Facebook;
using Newtonsoft.Json.Linq;

namespace OAuthNativeFlow.Web.Controllers
{

	public class ValuesController : ApiController
	{
		// GET api/values
		public IEnumerable<string> Get()
		{
			return new string[] { "value1", "value2" };
		}

		// GET api/values/5
		public string Get(int id)
		{
			return "value";
		}

		// POST api/values
		public void Post([FromBody]string value)
		{
		}

		// PUT api/values/5
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/values/5
		public void Delete(int id)
		{
		}

		[HttpGet]
		[Route("api/verify/{accessToken}")]
		public async Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string accessToken)
		{
			ParsedExternalAccessToken parsedToken = null;

			//You can get it from here: https://developers.facebook.com/tools/accesstoken/
			//More about debug_token here: http://stackoverflow.com/questions/16641083/how-does-one-get-the-app-access-token-for-debug-token-inspection-on-facebook

			var appToken = GetCurrectAccessToken();
			var verifyTokenEndPoint = string.Format("https://graph.facebook.com/debug_token?input_token={0}&access_token={1}", accessToken, appToken);

			var token1 = GetCurrectAccessToken();

			var client = new HttpClient();
			var uri = new Uri(verifyTokenEndPoint);
			var response = await client.GetAsync(uri);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				dynamic jObj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

				parsedToken = new ParsedExternalAccessToken
				{
					UserId = jObj["data"]["user_id"],
					AppId = jObj["data"]["app_id"]
				};

				if (!string.Equals(Startup.FacebookAuthOptions.AppId, parsedToken.AppId, StringComparison.OrdinalIgnoreCase))
				{
					return null;
				}
			}

			return parsedToken;
		}

		private string GetCurrectAccessToken()
		{
			var fb = new FacebookClient();
			dynamic result = fb.Get("oauth/access_token", new
			{
				client_id = Startup.FacebookAuthOptions.AppId,
				client_secret = Startup.FacebookAuthOptions.AppSecret,
				grant_type = "client_credentials"
			});
			return result.access_token;

		}
	}

	public class ParsedExternalAccessToken
	{
		public string UserId { get; set; }
		public string AppId { get; set; }
	}
}
