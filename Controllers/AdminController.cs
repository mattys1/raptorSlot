using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using raptorSlot.Models;
using raptorSlot.Services;
using raptorSlot.ViewModels.Account;
using raptorSlot.ViewModels.Admin.raptorSlot.ViewModels.Shared;
using raptorSlot.ViewModels.Shared;

namespace raptorSlot.Controllers
{
	[Authorize(Roles=Roles.ADMIN)]
	public class AdminController(AdminPanelService panelService, UserManager<AppUser> userManager) : Controller {
		[HttpGet]
		public IActionResult Panel() {
			return View(panelService.GetNonAdminUsers());
		}

		[HttpGet]
		public async Task<IActionResult> EditUser(string id) {
			var user = await userManager.FindByIdAsync(id);
			if(user == null){
				TempData["Error"] = "Failed to edit user! User not found.";
				return RedirectToAction(nameof(Panel));
			}	
			
			return View(new UserEditViewModel {
				OldData = user,
				Id = user.Id,
				Username = user.UserName,
				Email = user.Email
			});
		}

		[HttpPost]
		public async Task<IActionResult> DeleteUser(Guid id) {
			if((await panelService.DeleteUser(id)).IsFailure){
				TempData["Error"] = "Failed to delete user!";
			}
			return RedirectToAction(nameof(Panel));	
		}

		[HttpPost]
		public async Task<IActionResult> EditUser(UserEditViewModel newData) {
			Debug.Assert(newData.Id != null,  $"Id of {newData.Username} is null during admin edit");
			if(!ModelState.IsValid){
				newData.OldData = await userManager.FindByIdAsync(newData.Id!);
				return View(newData);
			}
			var result = await panelService.EditUser(newData);
			if(result.IsFailure) {
				newData.OldData = await userManager.FindByIdAsync(newData.Id!);
				ModelState.AddModelError(string.Empty, result.Error);
				return View(newData);
			}
			
			return RedirectToAction(nameof(Panel));
		}
	}
}
