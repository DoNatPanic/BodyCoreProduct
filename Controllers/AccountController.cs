using BodyCore.Models;
using BodyCore.Models.Email;
using BodyCore.ViewModels;
using BodyCore.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BodyCore.Controllers
{
	public class AccountController : Controller
    {
		private ApplicationContext _db;

		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		public AccountController( UserManager<ApplicationUser> userManager,
		   SignInManager<ApplicationUser> signInManager, ApplicationContext db )
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_db = db;
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Register()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Confirm( RegisterViewModel model )
		{
			return View(model);
		}

		[HttpGet]
		public IActionResult UserNotFound()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register( string username, string email, string password )
		{
			RegisterViewModel model = new RegisterViewModel();
			model.Username = username.ToUpper();
			model.Email = email.ToUpper();
			model.Password = password;

			if ( ModelState.IsValid )
			{
				IdentityResult result;
				ApplicationUser user;
				string code = "";
				ApplicationUser find_user = await _userManager.FindByEmailAsync(model.Email);
				if ( find_user != null && find_user.EmailConfirmed == false )
				{
					user = find_user;
					result = await _userManager.UpdateAsync(find_user);

					if ( result.Succeeded ) { 
						_db.Users.Update(new User { Id = find_user.Id, Name = model.Username, EmailAddres = model.Email });
						await _db.SaveChangesAsync();
					}
				}
				else
				{
					user = new ApplicationUser { UserName = model.Email, Email = model.Email };
					result = await _userManager.CreateAsync(user, model.Password);
					var ownTemplateUser = new User { Id = user.Id, Name = model.Username, EmailAddres = model.Email };
					if ( result.Succeeded )
					{
						await _db.Users.AddAsync(ownTemplateUser);
						await _db.SaveChangesAsync();
					}
				}

				if ( result.Succeeded )
				{
					code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					var callbackUrl = Url.EmailConfirmationLink(user.Id.ToString(), code, Request.Scheme);
					EmailSender _emailSender = new EmailSender();
					Message message = new Message(new string[] { model.Email }, "Подтверждение аккаунта",
						$"<p>Здравствуйте, {model.Username}!</p><p>Спасибо, что зарегистрировались на нашем сайте! Для подтверждения Вашего Email, перейдите по <a href='{callbackUrl}'>ссылке</a>. Если вы не оставляли запрос, удалите это письмо.</p><p>С уважением, служба поддержки сайта <a href='http://healthyweight.ru'>http://healthyweight.ru</a></p>", null);

					await _emailSender.SendEmailAsync(message);
					
					await _signInManager.SignInAsync(user, isPersistent: false);
					return RedirectToAction("Confirm", model);
					
				}
			}
			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ConfirmEmail( string userId, string code )
		{
			if ( userId == null || code == null )
			{
				return Content("UserId or code is null");
			}
			var user = await _userManager.FindByIdAsync(userId);
			if ( user == null )
			{
				//throw new ApplicationException($"Unable to load user with ID '{userId}'.");
				return Content($"Unable to load user with ID '{userId}'.");
			}
			var result = await _userManager.ConfirmEmailAsync(user, code);
			if ( result.Succeeded )
				return RedirectToAction("Login");
			else return Content("Срок действия ссылки истек. Пожалуйста, пройдите заново процедуру регистрации.");
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login( string email, string password, bool rememberme )
		{
			LoginViewModel model = new LoginViewModel();
			model.Email = email;
			model.Password = password;
			model.RememberMe = rememberme;
			if ( ModelState.IsValid )
			{
				var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
				if ( result.Succeeded )
				{
					ApplicationUser find_user = await _userManager.FindByEmailAsync(model.Email);
					if ( find_user != null && find_user.EmailConfirmed == true )
					{
						await Authenticate(model.Email);
						return RedirectToAction("Index", "Home");
					}
				}
				if ( result.IsLockedOut )
				{
					return RedirectToAction("Lockout");
				}
				else
				{
					return View(model);
				}
			}
			return View(model);
		}

		private async Task Authenticate( string userName )
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
			};
			ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> ForgotPassword(string email)
		{
			ForgotPasswordViewModel model = new ForgotPasswordViewModel();
			model.Email = email;
			if ( ModelState.IsValid )
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if ( user == null || !( await _userManager.IsEmailConfirmedAsync(user) ) )
				{
					return RedirectToAction("UserNotFound");
				}

				var code = await _userManager.GeneratePasswordResetTokenAsync(user);
				var callbackUrl = Url.ResetPasswordCallbackLink(user.Id.ToString(), code, Request.Scheme);
				EmailSender _emailSender = new EmailSender();
				Message message = new Message(new string[] { model.Email }, "Сброс пароля",
					$"<p>Здравствуйте!</p><p>Вы оставили запрос на сброс пароля. Для продолжения процедуры перейдите по <a href='{callbackUrl}'>ссылке</a>. Если вы не оставляли запрос, удалите это письмо.</p><p>С уважением, служба поддержки сайта <a href='http://healthyweight.ru'>http://healthyweight.ru</a></p>", null);

				await _emailSender.SendEmailAsync(message);

				return RedirectToAction("ForgotPasswordConfirmation");
			}
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		//[ValidateAntiForgeryToken]
		public IActionResult ResetPassword( string code = null )
		{
			if ( code == null )
			{
				return Content("A code must be supplied for password reset.");
			}
			var model = new ResetPasswordViewModel { Code = code };
			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword( string email, string password, string confirmpassword, string code )
		{
			ResetPasswordViewModel model = new ResetPasswordViewModel();
			model.Email = email;
			model.Password = password;
			model.ConfirmPassword = confirmpassword;
			model.Code = code;

			if ( !ModelState.IsValid )
			{
				return View(model);
			}
			var user = await _userManager.FindByEmailAsync(model.Email);
			if ( user == null )
			{
				// Don't reveal that the user does not exist
				return RedirectToAction("ResetPassword");
			}
			var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
			if ( result.Succeeded )
			{
				return RedirectToAction("ResetPasswordConfirmation");
			}
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ResetPasswordConfirmation()
		{
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Lockout()
		{
			return View();
		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public ActionResult UserAccount()
		{
			return View();
		}
	}
}