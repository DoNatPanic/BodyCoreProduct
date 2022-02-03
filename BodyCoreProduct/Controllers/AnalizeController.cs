using BodyCoreProduct.Models;
using Microsoft.AspNetCore.Mvc;

namespace BodyCoreProduct.Controllers
{
	public class AnalizeController : Controller
	{
		[HttpGet]
		public IActionResult Analize()
		{
			var analizeResults = new AnalizeResults();
			return View(analizeResults);
		}

		[HttpPost]
		public IActionResult Analize(string gender, float age, float waist, float hips, float neck,
			float height, float weight, bool hardMode)
		{
			var analizeResults = new AnalizeResults(gender, age, waist, hips, neck, height, weight, hardMode);
			return View(analizeResults);
		}
	}
}