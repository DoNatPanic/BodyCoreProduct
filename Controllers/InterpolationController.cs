using Microsoft.AspNetCore.Mvc;
using BodyCoreProduct.Models.Results;

namespace BodyCoreProduct.Controllers
{
	public class InterpolationController : Controller
	{
		[HttpGet]
		public IActionResult Interpolation()
		{
			InterpolationResults interpolationResults = new();
			return View(interpolationResults);
		}

		[HttpPost]
		public IActionResult Interpolation(float height, float weight, float dream_weight, float age, string gender,
			string activity, float waist, float hips, float neck, bool hardMode)
		{
			InterpolationResults interpolationResults = new(height, weight, dream_weight, age, gender,
				activity, waist, hips, neck, hardMode);
			return View(interpolationResults);
		}
	}
}