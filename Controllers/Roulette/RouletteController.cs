using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace raptorSlot.Controllers.Roulette {
	[Authorize]
	public class RouletteController: Controller {
		[HttpGet]
		public IActionResult Index() {
			return View();
		}
	}
}
