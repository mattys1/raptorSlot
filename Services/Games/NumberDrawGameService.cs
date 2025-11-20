using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using raptorSlot.Models;

namespace raptorSlot.Services.Games
{
    public class NumberDrawGameService : GameServiceBase<int[]>
    {
        private readonly Random _rnd = new Random();
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<NumberDrawGameService> _logger;

        public NumberDrawGameService(UserManager<AppUser> userManager, ILogger<NumberDrawGameService> logger)
            : base(userManager)
        {
            _userManager = userManager;
            _logger = logger;
        }

        protected override int[] Draw()
        {
            return new[] { NextSymbol(), NextSymbol(), NextSymbol() };

            int NextSymbol() => _rnd.Next(1, 10);
        }

        protected override Result<int> GenerateMultiplierForDraw(int[] drawResult)
        {
            if (drawResult == null || drawResult.Length != 3)
                return Result.Failure<int>("Invalid draw result");

            if (drawResult[0] == drawResult[1] && drawResult[1] == drawResult[2])
                return Result.Success(2);

            return Result.Success(-1);
        }

        public async Task<Result<(int[] Draw, int TokensDelta, int NewBalance)>> PlayNumberDrawAsync(Wager wager, string userId)
        {
            if (wager == null) return Result.Failure<(int[], int, int)>("Wager is null");

            var playResult = await Play(wager, userId);
            if (playResult.IsFailure) return Result.Failure<(int[], int, int)>(playResult.Error);

            var tuple = playResult.Value;
            int[] draw = tuple.Item1;
            Wager returnedWager = tuple.Item2;

            int tokenChange = returnedWager.WagerAmount;

            var userAfter = await _userManager.FindByIdAsync(userId);
            if (userAfter == null)
            {
                _logger.LogError("User not found after Play for {UserId}.", userId);
                return Result.Failure<(int[], int, int)>("Cannot find user after play");
            }

            int newBalance = userAfter.Tokens;

            return Result.Success((draw, tokenChange, newBalance));
        }
    }
}