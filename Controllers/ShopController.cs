using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using raptorSlot.Models;
using System.Linq;
using System.Threading.Tasks;

namespace raptorSlot.Controllers
{
    [Authorize]
    public class ShopController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public ShopController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.Tokens = user?.Tokens ?? 0;
            ViewBag.SuperTokens = user?.SuperTokens ?? 0;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(int amount)
        {
            if (amount <= 0)
            {
                TempData["ShopError"] = "Invalid token amount.";
                return RedirectToAction("Index");
            }

            var allowed = new[] { 10, 50, 100, 500 };
            if (!allowed.Contains(amount))
            {
                TempData["ShopError"] = "Invalid purchase option.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ShopError"] = "User not found.";
                return RedirectToAction("Index");
            }

            user.Tokens += amount;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["ShopSuccess"] = $"Successfully bought {amount} tokens!";
            }
            else
            {
                TempData["ShopError"] = "Error updating your tokens.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuySuper(int amount)
        {
            if (amount <= 0)
            {
                TempData["ShopError"] = "Invalid super-token amount.";
                return RedirectToAction("Index");
            }

            var allowedSuper = new[] { 1, 5, 10, 50 };
            if (!allowedSuper.Contains(amount))
            {
                TempData["ShopError"] = "Invalid super-token option.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ShopError"] = "User not found.";
                return RedirectToAction("Index");
            }

            user.SuperTokens += amount;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["ShopSuccess"] = $"Successfully bought {amount} super-tokens!";
            }
            else
            {
                TempData["ShopError"] = "Error updating your super-tokens.";
            }

            return RedirectToAction("Index");
        }
    }
}
