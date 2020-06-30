using Microsoft.AspNetCore.Mvc;

namespace BodyCore.Controllers
{
	public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}