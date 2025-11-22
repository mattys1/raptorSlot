using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using raptorSlot.Models;
using raptorSlot.Services;
using raptorSlot.Util;

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
			if(result.IsFailure){
				ModelState.AddModelError(string.Empty, result.Error);
				
				return View(nameof(Index), user);
			}

			return RedirectToAction(nameof(HomeController.Index), ControllerStripper.StripControllerSuffix(nameof(HomeController)));
		}
	}
}
