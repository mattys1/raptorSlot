using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using raptorSlot.Models;

namespace raptorSlot.Services.Games;

public record Wager(int WagerAmount, bool IsPremiumToken = false);

public abstract class GameServiceBase<TDrawResult>(UserManager<AppUser> userManager)
{
    public async Task<Result<Tuple<TDrawResult, Wager>>> Play(Wager wager, string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        
        if(user == null) {
            return Result.Failure<Tuple<TDrawResult, Wager>>($"Cannot find user: {user}");
        }
        
        if(wager.IsPremiumToken) {
            if(wager.WagerAmount >= user.SuperTokens){
                return Result.Failure<Tuple<TDrawResult, Wager>>($"Not enough premium tokens! Have {user.SuperTokens} and need {wager.WagerAmount} for wager.");
            } 
        } else {
            if(wager.WagerAmount >= user.Tokens) {
                return Result.Failure<Tuple<TDrawResult, Wager>>($"Not enough tokens! Have {user.Tokens} and need {wager.WagerAmount} for wager.");
            } 
        }
        
        var drawResult = Draw(wager.IsPremiumToken);
        var multiplierResult = GenerateMultiplierForDraw(drawResult);
        if(multiplierResult.IsFailure) {
            return Result.Failure<Tuple<TDrawResult, Wager>>(multiplierResult.Error);
        }
        
        var tokens = wager.WagerAmount * multiplierResult.Value;
        var changed = await ChangeTokensForUser(tokens, wager.IsPremiumToken, user);
        if(changed.IsFailure) {
            return Result.Failure<Tuple<TDrawResult, Wager>>(changed.Error);
        }
        
        return Tuple.Create(drawResult, new Wager(tokens, wager.IsPremiumToken));
    }

    protected abstract TDrawResult Draw(bool isUsingSuperTokens = false);
    protected abstract Result<int> GenerateMultiplierForDraw(TDrawResult drawResult);

    protected async Task<Result> ChangeTokensForUser(int tokenChange, bool useSuperTokens, AppUser user) {
        if(useSuperTokens) {
            user.SuperTokens += tokenChange;
        } else{
            user.Tokens += tokenChange;
        }
        var updateResult = await userManager.UpdateAsync(user);
        if(!updateResult.Succeeded){
            return Result.Failure("Failed to update user tokens."); 
        } 
        
        return Result.Success();
    }
}

public abstract class GameServiceBase<TPayload, TDrawResult>(UserManager<AppUser> userManager) : GameServiceBase<TDrawResult>(userManager)
{
    public async Task<Result<Tuple<TDrawResult, Wager>>> Play(Wager wager, TPayload payload, string userId)
    {
        
        var user = await userManager.FindByIdAsync(userId);
        
        if(user == null){
            return Result.Failure<Tuple<TDrawResult, Wager>>($"Cannot find user: {user}");
        }
        
        if(wager.IsPremiumToken){
            if(wager.WagerAmount >= user.SuperTokens){
                return Result.Failure<Tuple<TDrawResult, Wager>>($"Not enough premium tokens! Have {user.SuperTokens} and need {wager.WagerAmount} for wager.");
            } 
        } else {
            if(wager.WagerAmount >= user.Tokens){
                return Result.Failure<Tuple<TDrawResult, Wager>>($"Not enough tokens! Have {user.Tokens} and need {wager.WagerAmount} for wager.");
            } 
        }

        var drawResult = Draw(payload, wager.IsPremiumToken);
        var multiplierResult = GenerateMultiplierForDraw(payload, drawResult);
        if(multiplierResult.IsFailure) {
            return Result.Failure<Tuple<TDrawResult, Wager>>(multiplierResult.Error);
        }
        
        var tokens = wager.WagerAmount * multiplierResult.Value;
        var changed = await ChangeTokensForUser(tokens, wager.IsPremiumToken, user);
        if(changed.IsFailure) {
            return Result.Failure<Tuple<TDrawResult, Wager>>(changed.Error);
        }
        
        return Tuple.Create(drawResult, new Wager(tokens, wager.IsPremiumToken));
    }
    
    protected abstract TDrawResult Draw(TPayload payload, bool isUsingSuperTokens);

    protected override sealed Result<int> GenerateMultiplierForDraw(TDrawResult drawResult) {
        throw new NotImplementedException("TEGO NIE IMPLEMENTUJEMY TYLKO C# SSIE WIEC TRZEBA COS TAKIEGO ZROBIC");
    }

    protected override sealed TDrawResult Draw(bool isUsingSuperTokens = false) {
        throw new NotImplementedException("TEGO NIE IMPLEMENTUJEMY TYLKO C# SSIE WIEC TRZEBA COS TAKIEGO ZROBIC");
    }

    protected abstract Result<int> GenerateMultiplierForDraw(TPayload payload, TDrawResult drawResult);
}