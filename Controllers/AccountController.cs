using BodyCore.Models;
using BodyCore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BodyCore.Controllers
{
	public class AccountController : Controller
    {
		private ApplicationContext db;
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		public AccountController( ApplicationContext context, UserManager<User> userManager, SignInManager<User> signInManager )
		{
			db = context;
			_userManager = userManager;
			_signInManager = signInManager;
		}
		/*private void SaveToBD( string username, string email, string psw )
		{
			var id = db.Users.ToList().Last().Id;
			User p1 = new User { Id = id + 1, Name = username, Email = email, Password = psw };
			db.Users.Add(p1);
			db.SaveChanges();
		}*/

		[HttpGet]
		public IActionResult Register()
		{
			//return RedirectToAction("Index", "Home");
			return View();
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> ConfirmAsync( ConfirmAccountViewModel model )
		{
			//var id = db.Users.ToList().Last().Id;
			//User user = new User { Id = id + 1, Name = model.Username, Email = model.Email, Password = model.Password };
			//User user = new User { /*Id = (id + 1).ToString(),*/ UserName = model.Username, Email = model.Email, Password = model.Password, EmailConfirmed = false };
			User user = new User { /*Id = (id + 1).ToString(),*/ UserName = model.Username, Email = model.Email };
			/*await db.Users.AddAsync(user);
			await db.SaveChangesAsync();*/

			/*var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			var callbackUrl = Url.Action(
				"ConfirmEmail",
				"Account",
				new { userId = user.Id, code = code },
				protocol: HttpContext.Request.Scheme);
			EmailService emailService = new EmailService();
			await emailService.SendEmailAsync(model.Email, "Confirm your account",
				$"Подтвердите регистрацию, перейдя по: <a href='{callbackUrl}'>ссылке</a>");*/

			// добавляем пользователя
			var result = await _userManager.CreateAsync(user, model.Password);
			//if ( result.Succeeded )
			//{
				// генерация токена для пользователя
				var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
				var callbackUrl = Url.Action(
					"ConfirmEmail",
					"Account",
					new { userId = user.Id, code = code },
					protocol: HttpContext.Request.Scheme);
				EmailService emailService = new EmailService();
				await emailService.SendEmailAsync(model.Email, "Confirm your account",
					$"Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>link</a>");

				return Content("Для завершения регистрации проверьте электронную почту и перейдите по ссылке, указанной в письме");
			//}
			//else
			//{
				//return RedirectToAction("Index", "Home");
				foreach ( var error in result.Errors )
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			//}

			return View(model);
		}

		[HttpGet]
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
		}

		[HttpPost]
		public IActionResult Register(string username, string email, string password )
		{
			ConfirmAccountViewModel model = new ConfirmAccountViewModel();
			model.Username = username;
			model.Email = email;
			model.Password = password;
			//SaveToBD(username, email, psw);
			return RedirectToAction("Confirm", model);
		}

		/*[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login( LoginViewModel model )
		{
			if ( ModelState.IsValid )
			{
				var user = await _userManager.FindByNameAsync(model.Email);
				if ( user != null )
				{
					// проверяем, подтвержден ли email
					if ( !await _userManager.IsEmailConfirmedAsync(user) )
					{
						ModelState.AddModelError(string.Empty, "Вы не подтвердили свой email");
						return View(model);
					}
				}

				var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
				if ( result.Succeeded )
				{
					return RedirectToAction("Index", "Home");
				}
				else
				{
					ModelState.AddModelError("", "Неправильный логин и (или) пароль");
				}
			}
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LogOff()
		{
			// удаляем аутентификационные куки
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}*/
	}
}