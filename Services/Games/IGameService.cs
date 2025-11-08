using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using raptorSlot.Models;

namespace raptorSlot.Services.Games;

public record Wager(int wagerAmount, bool isPremiumToken = false);

public abstract class GameServiceBase<TDrawResult>(UserManager<AppUser> userManager)
{
    public async Task<Tuple<TDrawResult, Wager>> Play(Wager wager, string userId)
    {
        if(wager.isPremiumToken){
            throw new NotImplementedException("NIE MA PREMIUM TOKENOW JESZCZE");
        }
        var drawResult = Draw();
        var tokens = GenerateTokensForDraw(drawResult);
        await ChangeTokensForUser(tokens, userId);
        return Tuple.Create(drawResult, new Wager(tokens, wager.isPremiumToken));
    }

    protected abstract TDrawResult Draw();
    protected abstract int GenerateTokensForDraw(TDrawResult drawResult);

    protected async Task<Result> ChangeTokensForUser(int tokenChange, string userId) {
        var user = await userManager.FindByIdAsync(userId);
        if(user == null){
            return Result.Failure($"Cannot find user: {userId}");
        }
        
        user.Tokens += tokenChange;
        return Result.Success();
    }
}

public abstract class GameServiceBase<TPayload, TDrawResult>(UserManager<AppUser> userManager) : GameServiceBase<TDrawResult>(userManager)
{
    public async Task<Tuple<TDrawResult, Wager>> Play(Wager wager, TPayload payload, string userId)
    {
        if(wager.isPremiumToken){
            throw new NotImplementedException("NIE MA PREMIUM TOKENOW JESZCZE");
        }
        var drawResult = Draw();
        var tokens = GenerateTokensForDraw(payload, drawResult);
        await ChangeTokensForUser(tokens, userId);
        return Tuple.Create(drawResult, new Wager(tokens, wager.isPremiumToken));
    }

    protected override sealed int GenerateTokensForDraw(TDrawResult drawResult) {
        throw new NotImplementedException("TEGO NIE IMPLEMENTUJEMY TYLKO C# SSIE WIEC TRZEBA COS TAKIEGO ZROBIC");
    }
    
    protected abstract int GenerateTokensForDraw(TPayload payload, TDrawResult drawResult);
}