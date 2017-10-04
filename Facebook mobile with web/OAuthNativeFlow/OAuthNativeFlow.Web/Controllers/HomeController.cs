using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Facebook;
using Newtonsoft.Json.Linq;

namespace OAuthNativeFlow.Web.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.Title = "Home Page";
			
			return View();
		}

		
	}
}
