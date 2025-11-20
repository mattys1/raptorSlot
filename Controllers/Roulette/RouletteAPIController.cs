using System.Security.Claims;
using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using raptorSlot.Services.Games;

namespace raptorSlot.Controllers.Roulette {
	public record RoulettePayload(Wager Wager, RouletteChoice RouletteChoice);

	[Authorize]
	[ApiController]
	[Route("api/games/roulette")]
	public class RouletteApiController(RouletteService rouletteService) : Controller {
		[HttpPost("play")]
		public async Task<IActionResult> Play([FromBody] RoulettePayload payload)
		{
			var userIdResult = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
				.ToResult("couldn't find user");

			if (userIdResult.IsFailure)
				return BadRequest(userIdResult.Error);

			var result = await rouletteService.Play(payload.Wager, payload.RouletteChoice, userIdResult.Value);

			if(result.IsSuccess) {
				if(result.Value.Item2.WagerAmount > 0) {
					TempData["GameResultMessage"] = $"You won! +{result.Value.Item2.WagerAmount} tokens!";
					TempData["GameResult"] = "w";
				} else {
					TempData["GameResultMessage"] = $"You lost! {result.Value.Item2.WagerAmount} tokens...";
					TempData["GameResult"] = "l";
				}
				return Ok(new { wager = result.Value.Item2, draw = result.Value.Item1 });
			}
    
			return BadRequest(result.Error);}
	}
}
