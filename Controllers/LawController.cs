using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BodyCore.Controllers
{
    public class LawController : Controller
    {
		[HttpGet]
		public IActionResult UserAgreement()
		{
			return View();
		}

		[HttpGet]
		public IActionResult PrivacyPolicy()
		{
			return View();
		}
	}
}