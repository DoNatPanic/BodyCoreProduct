using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Xml;

namespace BodyCore.Controllers
{
	public class HomeController : Controller
    {
		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}
	}
}