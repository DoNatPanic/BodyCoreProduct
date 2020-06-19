using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BodyCore.Controllers
{
    public class ArticlesController : Controller
    {
		[HttpGet]
		public ActionResult Metabolism()
		{
			return View();
		}
	}
}