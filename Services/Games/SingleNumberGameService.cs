using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using raptorSlot.Models;

namespace raptorSlot.Services.Games
{
    public record SingleNumberPlayResult(int Draw, int TokensDelta, int NewBalance);

    public class SingleNumberGameService : GameServiceBase<int, int>
    {
        private readonly UserManager<AppUser> _userManager;

        public SingleNumberGameService(UserManager<AppUser> userManager) : base(userManager)
        {
            _userManager = userManager;
        }

        protected override int Draw(int payload, bool isUsingSuperTokens)
        {
            return Random.Shared.Next(1, 10);
        }

        protected override Result<int> GenerateMultiplierForDraw(int payload, int drawResult)
        {
            if (payload < 1 || payload > 9)
                return Result.Failure<int>("Chosen number must be between 1 and 9.");

            if (payload == drawResult)
                return Result.Success(2);
            else
                return Result.Success(-1);
        }

        public async Task<Result<SingleNumberPlayResult>> PlaySingleNumberAsync(Wager wager, int chosenNumber, string userId)
        {
            var baseRes = await Play(wager, chosenNumber, userId);
            if (baseRes.IsFailure)
                return Result.Failure<SingleNumberPlayResult>(baseRes.Error);

            var draw = baseRes.Value.Item1;
            var returnedWager = baseRes.Value.Item2;
            var userAfter = await _userManager.FindByIdAsync(userId);
            if (userAfter == null)
                return Result.Failure<SingleNumberPlayResult>("User not found after play.");

            var newBalance = returnedWager.IsPremiumToken ? userAfter.SuperTokens : userAfter.Tokens;

            return Result.Success(new SingleNumberPlayResult(draw, returnedWager.WagerAmount, newBalance));
        }
    }
}
