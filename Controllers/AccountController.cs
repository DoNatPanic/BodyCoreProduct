using BodyCore.Models;
using BodyCore.ViewModels;
using BodyCore.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BodyCore.Controllers
{
	public class AccountController : Controller
    {
		private ApplicationContext _db;

		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		public AccountController( UserManager<ApplicationUser> userManager,
		   SignInManager<ApplicationUser> signInManager, ApplicationContext db)
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
				var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
				var result = await _userManager.CreateAsync(user, model.Password);
				if ( result.Succeeded )
				{
					//добавление пользователя в таблицу Users
					User ownTemplateUser = new User { Id = user.Id, Name = model.Username, EmailAddres = model.Email};
					await _db.Users.AddAsync(ownTemplateUser);
					await _db.SaveChangesAsync();

					var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

					/*var callbackUrl = Url.Action(
						"ConfirmEmail",
						"Account",
						new { userId = user.Id, code = code },
						protocol: HttpContext.Request.Scheme);*/

					var callbackUrl = Url.EmailConfirmationLink(user.Id.ToString(), code, Request.Scheme);
					EmailService emailService = new EmailService();
					await emailService.SendEmailAsync(model.Email, "Подтверждение аккаунта",
						$"Здравствуйте, {model.Username}. Спасибо, что зарегистрировались на нашем сайте! Для подтверждения Вашего Email, перейдите по <a href='{callbackUrl}'>ссылке</a>. Если вы не оставляли запрос, удалите это письмо. С уважением, служба поддержки сайта <a href='http://healthyweight.ru'>http://healthyweight.ru</a>.");
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
		public async Task<IActionResult> Login( string email, string password, bool rememberme)
		{
			LoginViewModel model = new LoginViewModel();
			model.Email = email;
			model.Password = password;
			model.RememberMe = rememberme;
			if ( ModelState.IsValid )
			{
				// This doesn't count login failures towards account lockout
				// To enable password failures to trigger account lockout, set lockoutOnFailure: true
				var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
				if ( result.Succeeded )
				{
					return RedirectToAction("Index", "Home");
				}
				//User account locked out.
				if ( result.IsLockedOut )
				{
					return RedirectToAction("Lockout");
				}
				else
				{
					//ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					return View(model);
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
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
					// Don't reveal that the user does not exist or is not confirmed
					return View();
				}

				// For more information on how to enable account confirmation and password reset please
				// visit https://go.microsoft.com/fwlink/?LinkID=532713
				var code = await _userManager.GeneratePasswordResetTokenAsync(user);
				var callbackUrl = Url.ResetPasswordCallbackLink(user.Id.ToString(), code, Request.Scheme); 
				EmailService emailService = new EmailService();
				await emailService.SendEmailAsync(model.Email, "Сброс пароля",
					$"Здравствуйте! Вы оставили запрос на сброс пароля. Для продолжения процедуры перейдите по <a href='{callbackUrl}'>ссылке</a>. Если вы не оставляли запрос, удалите это письмо. С уважением, служба поддержки сайта <a href='http://healthyweight.ru'>http://healthyweight.ru</a>.");

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

		//аккаунт заблокирован
		[HttpGet]
		[AllowAnonymous]
		public IActionResult Lockout()
		{
			return View();
		}

	}
}