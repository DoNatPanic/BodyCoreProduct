using Microsoft.AspNetCore.Mvc;

namespace BodyCore.Controllers
{
	public class HomeController : Controller
    {
		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public ActionResult Article()
		{
			return View();
		}

		[HttpGet]
		public IActionResult UserAgreement()
		{
			return View();
		}
	}
}