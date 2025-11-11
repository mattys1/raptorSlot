using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using raptorSlot.Models;
using System;
using System.Threading.Tasks;

namespace raptorSlot.Controllers
{
    [Authorize]
    public class GamesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly Random _rng = new Random();

        public GamesController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: /Games
        [AllowAnonymous]
        public IActionResult Index()
        {
            // Widok z list¹ gier (kafelki)
            return View();
        }

        // GET: /Games/Play/three
        public async Task<IActionResult> Play(string id)
        {
            // id = "three" (nasza gra trzy liczby) — dla teraz obs³ugujemy tylko tê
            // Przekazujemy saldo do widoku
            var user = await _userManager.GetUserAsync(User);
            ViewBag.Tokens = user?.Tokens ?? 0;
            return View(model: id ?? "three");
        }

        // POST: /Games/Spin  (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Spin(int wager)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "User not found." });

            if (wager <= 0)
                return Json(new { success = false, message = "Invalid wager." });

            if (user.Tokens < wager)
                return Json(new { success = false, message = "Not enough tokens." });

            // Odejmij stake natychmiast:
            user.Tokens -= wager;

            // Losowanie 3 liczb (1..6)
            int a = _rng.Next(1, 7);
            int b = _rng.Next(1, 7);
            int c = _rng.Next(1, 7);

            bool win = (a == b && b == c);

            int reward = 0;
            if (win)
            {
                // Nagroda: podwójna liczba postawionych ¿etonów (w skrócie: dopisujemy wager * 2)
                reward = wager * 2;
                user.Tokens += reward;
            }

            // Zapisz zmiany w DB
            var update = await _userManager.UpdateAsync(user);
            if (!update.Succeeded)
            {
                // w razie b³êdu zwróæ info (mo¿esz rozwin¹æ logowanie)
                return Json(new { success = false, message = "Could not update user tokens." });
            }

            // Zwroæ wynik (u¿ywany przez frontend do animacji / komunikatu)
            return Json(new
            {
                success = true,
                numbers = new[] { a, b, c },
                won = win,
                reward = reward,
                balance = user.Tokens
            });
        }
    }
}
