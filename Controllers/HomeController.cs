using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BodyCore.Controllers
{
	public class HomeController : Controller
    {
		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Error()
		{
			var pathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
			Exception exception = pathFeature?.Error; // Here will be the exception details.
			return View();
		}
	}
}