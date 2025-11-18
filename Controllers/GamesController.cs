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
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Home");
            }

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
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, error = "Not authenticated" });
            }

            try
            {
                var w = new Wager(wager);
                var res = await _numberGame.PlayNumberDrawAsync(w, userId);

                if (res.IsFailure)
                {
                    var userOnFailure = await _userManager.FindByIdAsync(userId);
                    return Json(new
                    {
                        success = false,
                        error = res.Error,
                        tokens = userOnFailure?.Tokens ?? 0,
                        superTokens = userOnFailure?.SuperTokens ?? 0
                    });
                }

                var userAfter = await _userManager.FindByIdAsync(userId);
                if (userAfter == null)
                {
                    return Json(new { success = false, error = "User not found after play" });
                }

                return Json(new
                {
                    success = true,
                    draw = res.Value.Draw,
                    delta = res.Value.TokensDelta,
                    balance = res.Value.NewBalance,
                    tokens = userAfter.Tokens,
                    superTokens = userAfter.SuperTokens
                });
            }
            catch (Exception ex)
            {
                try
                {
                }
                catch {}

                var userAfterEx = await _userManager.FindByIdAsync(userId);
                return Json(new
                {
                    success = false,
                    error = "Internal server error",
                    details = ex.Message,
                    tokens = userAfterEx?.Tokens ?? 0,
                    superTokens = userAfterEx?.SuperTokens ?? 0
                });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetBalance()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, error = "User not found" });
            }

            return Json(new
            {
                success = true,
                tokens = user.Tokens,
                superTokens = user.SuperTokens
            });
        }
    }
}