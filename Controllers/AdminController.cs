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
		public IActionResult CreateUser() {
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateUser(UserCreateViewModel newData) {
			if(!ModelState.IsValid){
				return View(newData);
			}	
			
			var result = await panelService.CreateUser(newData);

			if(result.IsFailure) {
				TempData["Error"] = "Failed to create user! " + result.Error;
			}

			return RedirectToAction(nameof(Panel));
		}

		[HttpPost]
		public async Task<IActionResult> DeleteUser(Guid id) {
			if((await panelService.DeleteUser(id)).IsFailure){
				TempData["Error"] = "Failed to delete user!";
			}
			return RedirectToAction(nameof(Panel));	
		}

		[HttpGet]
		public async Task<IActionResult> EditUser(string id) {
			var user = await userManager.FindByIdAsync(id);
			if(user == null){
				TempData["Error"] = "Failed to edit user! User not found.";
				return RedirectToAction(nameof(Panel));
			}	
			
			return View(user);
		}
		
		[HttpPost]
		public async Task<IActionResult> EditUser(UserEditViewModel newData) {
			Debug.Assert(newData.Id != null,  $"Id of {newData.Username} is null during admin edit");
			if(!ModelState.IsValid){
				var user = await userManager.FindByIdAsync(newData.Id!);
				return View(user);
			}
			var result = await panelService.EditUser(newData);
			if(result.IsFailure) {
				var user = await userManager.FindByIdAsync(newData.Id!);
				ModelState.AddModelError(string.Empty, result.Error);
				return View(user);
			}
			
			return RedirectToAction(nameof(Panel));
		}
	}
}
