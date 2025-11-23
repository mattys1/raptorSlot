using System.Net;
using System.Security.Claims;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using raptorSlot.Models;
using raptorSlot.Services;
using raptorSlot.Util;
using SixLabors.ImageSharp.Formats.Png;

namespace raptorSlot.Controllers {
	[Authorize]
	public class AvatarController(AvatarService avatarService, UserManager<AppUser> userManager) : Controller {
		[HttpGet]
		public async Task<IActionResult> Index() {
			var user = await userManager.GetUserAsync(User);
			if(user == null) {
				return BadRequest();
			}
			
			return View(user);
		}

		[HttpGet]
		public async Task<IActionResult> GetSelfAvatar() {
			var user = await userManager.GetUserAsync(User);
			if (user == null) {
				return BadRequest();
			}

			return await GetUserAvatar(user);
		}
		
		[HttpGet]
		public async Task<IActionResult> GetAvatarForUser(string userId) {
			var user = await userManager.FindByIdAsync(userId);
			if (user == null) {
				return BadRequest();
			}

			return await GetUserAvatar(user);
		}
		
		[HttpPost]
		public async Task<IActionResult> DeleteAvatar() {
			var user = await userManager.GetUserAsync(User);
			if(user == null) {
				ModelState.AddModelError(string.Empty, "Couldn't find user.");
				return View(nameof(Index), user);
			}
			
			var result = await avatarService.RemoveUserAvatar(user);
			if(result.IsFailure){
				ModelState.AddModelError(string.Empty, result.Error);
				
				return View(nameof(Index), user);
			}

			return RedirectToAction(nameof(HomeController.Index), ControllerStripper.StripControllerSuffix(nameof(HomeController)));
		}
		
		[HttpPost]
		public async Task<IActionResult> UploadAvatar(IFormFile? image) {
			var user = await userManager.GetUserAsync(User);
			
			if(user == null) {
				ModelState.AddModelError(string.Empty, "Couldn't find user.");
				return View(nameof(Index), user);
			}
			
			if(image == null || image.Length == 0) {
				ModelState.AddModelError(string.Empty, "No file selected.");
				// return RedirectToAction(nameof(HomeController.Index), ControllerStripper.StripControllerSuffix(nameof(HomeController)));
				return View(nameof(Index), user);
			}


			using var stream = image.OpenReadStream();
			var loaded = await SixLabors.ImageSharp.Image.LoadAsync(stream);

			var result = await avatarService.SetUserAvatar(user, loaded);
			if(result.IsFailure) {
				ModelState.AddModelError(string.Empty, result.Error);
				
				return View(nameof(Index), user);
			}

			return RedirectToAction(nameof(HomeController.Index), ControllerStripper.StripControllerSuffix(nameof(HomeController)));
		}

		private async Task<IActionResult> GetUserAvatar(AppUser user) {
			var avatarResult = await avatarService.GetUserAvatar(user);
			if (avatarResult.IsFailure) {
				ModelState.AddModelError(string.Empty, avatarResult.Error);
				return NotFound();
			}

			var maybeImage = avatarResult.Value;
			if(maybeImage.HasNoValue) {
				return NotFound();
			}

			var image = maybeImage.Value;

			using var ms = new MemoryStream();
			image.Save(ms, new PngEncoder());
			ms.Position = 0;

			return File(ms.ToArray(), "image/png");
		}
	}
}
