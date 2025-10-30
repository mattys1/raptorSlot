using System.Diagnostics;
using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raptorSlot.Services;
using raptorSlot.Util;
using raptorSlot.ViewModels.Account;

namespace raptorSlot.Controllers
{
	public class AccountController(AccountService accountService) : Controller
	{
		[HttpGet]
		[AllowAnonymous]
		public IActionResult Register() {
			return View();
		}

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model) {
			Console.WriteLine($"Registering user: {model.Username} with password: {model.Password} and confirmPassword: {model.RepeatPassword}");
			if(!ModelState.IsValid) {
				return View(model);
			}

			var registered = await accountService.RegisterUser(model);
			if(registered.IsFailure) {
				ModelState.AddModelError(string.Empty, registered.Error);
				return View(model);
			}

			return RedirectToAction(nameof(HomeController.Index), ControllerStripper.StripControllerSuffix(nameof(HomeController)));

		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login() {
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model) {
			if(!ModelState.IsValid) {
				return View(model);
			}

			var loginResult = await accountService.LoginUser(model);
			if(loginResult.IsFailure) {
				ModelState.AddModelError(string.Empty, loginResult.Error);
				Console.Error.WriteLine($"Login failed: {loginResult.Error}");
				return View(model);
			}

			return RedirectToAction(nameof(HomeController.Index), ControllerStripper.StripControllerSuffix(nameof(HomeController)));
		}

		[HttpGet]
		public async Task<IActionResult> Logout() {
			await accountService.LogoutUser();	

			return RedirectToAction(nameof(HomeController.Index), ControllerStripper.StripControllerSuffix(nameof(HomeController)));
		}
	}
}
