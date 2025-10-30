using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raptorSlot.Services;

namespace raptorSlot.Controllers
{
	[Authorize(Roles=Roles.ADMIN)]
    public class AdminController(AdminPanelService panelService) : Controller {
		[HttpGet]
		public IActionResult Panel() {
			return View(panelService.GetNonAdminUsers());
		}

		[HttpPost]
		public IActionResult DeleteUser(Guid id) {
			TempData["Error"] = "Failed to delete user!";
			panelService.DeleteUser(id);
			return RedirectToAction(nameof(Panel));	
		}
    }
}
