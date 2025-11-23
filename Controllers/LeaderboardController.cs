using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using raptorSlot.Models;

namespace raptorSlot.Controllers {
	[Authorize]
	public class LeaderboardController(UserManager<AppUser> userManager) : Controller {
		[HttpGet]
		public IActionResult Index() {
			var users = userManager.Users.OrderByDescending(u => u.Tokens + 2 * u.SuperTokens).ToList();
			return View(users);
		}	
	}
}
