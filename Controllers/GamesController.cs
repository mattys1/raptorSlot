using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using raptorSlot.Models;
using raptorSlot.Services.Games;

namespace raptorSlot.Controllers
{
    [Authorize]
    public class GamesController : Controller
    {
        private readonly NumberDrawGameService _numberGame;
        private readonly UserManager<AppUser> _userManager;

        public GamesController(NumberDrawGameService numberGame, UserManager<AppUser> userManager)
        {
            _numberGame = numberGame;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> NumberDraw()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["UserId"] = userId;
            ViewData["Balance"] = user.Tokens;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlayNumberDraw([FromForm] int wager)
        {
            var userId = _userManager.GetUserId(User);
            var w = new Wager(wager);
            var res = await _numberGame.PlayNumberDrawAsync(w, userId);
            if (res.IsFailure) return Json(new { success = false, error = res.Error });

            return Json(new { success = true, draw = res.Value.Draw, delta = res.Value.TokensDelta, balance = res.Value.NewBalance });
        }
    }
}
