using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyCore.ViewModels.Account
{
	public class ResetPasswordViewModel
	{
		public string Email { get; set; }
		public string Password { get; set; }
		public string ConfirmPassword { get; set; }

		public string Code { get; set; }
	}
}
