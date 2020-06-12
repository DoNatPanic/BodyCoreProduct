using System;
using System.Threading.Tasks;
using BodyCore.Models;
using BodyCore.SiteMap;
using Microsoft.AspNetCore.Mvc;

namespace BodyCore.Controllers
{
	public class SitemapController : Controller
	{

		[Route("sitemap")]
		public async Task<ActionResult> SitemapAsync()
		{
			string baseUrl = "http://healthyweight.ru/";

			// get last modified date of the home page
			var siteMapBuilder = new SitemapBuilder();

			// add the home page to the sitemap
			siteMapBuilder.AddUrl(baseUrl, modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Weekly, priority: 1.0);

			// add the blog posts to the sitemap
			siteMapBuilder.AddUrl(baseUrl + "Home/Index/", modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Weekly, priority: 0.9);
			siteMapBuilder.AddUrl(baseUrl + "Analize/GetAnalize/", modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Weekly, priority: 0.9);
			siteMapBuilder.AddUrl(baseUrl + "Interpolation/Input/", modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Weekly, priority: 0.9);
			siteMapBuilder.AddUrl(baseUrl + "Home/Article/", modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Weekly, priority: 0.9);
			// generate the sitemap xml
			string xml = siteMapBuilder.ToString();
			return Content(xml, "text/xml");
		}
	}
}