using BodyCore.Models;
using BodyCore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BodyCore.Controllers
{
	public class AccountController : Controller
    {
		//private ApplicationContext db;

		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		public AccountController( UserManager<ApplicationUser> userManager,
		   SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Register()
		{
			return View();
		}

		public IActionResult Confirm(ConfirmAccountViewModel model)
		{
			return View(model);
		}


		[HttpPost]
		public async Task<IActionResult> Register( string username, string email, string password )
		{
			ConfirmAccountViewModel model = new ConfirmAccountViewModel();
			model.Username = username;
			model.Email = email;
			model.Password = password;

			if ( ModelState.IsValid )
			{
				var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
				var result = await _userManager.CreateAsync(user, model.Password);
				//if ( result.Succeeded )
				//{
					var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					var callbackUrl = Url.Action(
						"ConfirmEmail",
						"Account",
						new { userId = user.Id, code = code },
						protocol: HttpContext.Request.Scheme);
					EmailService emailService = new EmailService();
					await emailService.SendEmailAsync(model.Email, "Подтверждение аккаунта",
						$"Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>link</a>");
					await _signInManager.SignInAsync(user, isPersistent: false);
					return RedirectToAction("Confirm");
				//}
			}
			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ConfirmEmail( string userId, string code )
		{
			if ( userId == null || code == null )
			{
				return Content("userId or code is null");
			}
			var user = await _userManager.FindByIdAsync(userId);
			if ( user == null )
			{
				throw new ApplicationException($"Unable to load user with ID '{userId}'.");
			}
			var result = await _userManager.ConfirmEmailAsync(user, code);
			return result.Succeeded ? RedirectToAction("Index", "Home") : RedirectToAction("Index", "Home");
		}


		/*private void SaveToBD( string username, string email, string psw )
		{
			var id = db.Users.ToList().Last().Id;
			User p1 = new User { Id = id + 1, Name = username, Email = email, Password = psw };
			db.Users.Add(p1);
			db.SaveChanges();
		}*/


		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		

		/*[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ConfirmEmail( string userId, string code )
		{
			if ( userId == null || code == null )
			{
				//return RedirectToAction("Index", "Home");
				return View("Error");
			}
			var user = await _userManager.FindByIdAsync(userId);
			if ( user == null )
			{
				//return RedirectToAction("Index", "Home");
				return View("Error");
			}
			var result = await _userManager.ConfirmEmailAsync(user, code);
			if ( result.Succeeded )
				return RedirectToAction("Index", "Home");
			else
				return View("Error");
		}*/


		
	}
}