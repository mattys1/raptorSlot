using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using raptorSlot.Models;

namespace raptorSlot.Services.Games
{
    // Dziedziczymy po GameServiceBase<int[]>
    public class NumberDrawGameService : GameServiceBase<int[]>
    {
        private readonly Random _rnd = new Random();
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<NumberDrawGameService> _logger;

        // Konstruktor — przekazujemy userManager do bazowej klasy
        public NumberDrawGameService(UserManager<AppUser> userManager, ILogger<NumberDrawGameService> logger)
            : base(userManager)
        {
            _userManager = userManager;
            _logger = logger;
        }

        // Implementacja abstrakcyjnej metody Draw()
        // Zwraca 3 losowe liczby z przedzia³u 1..9
        protected override int[] Draw()
        {
            return new[] { NextSymbol(), NextSymbol(), NextSymbol() };

            int NextSymbol() => _rnd.Next(1, 10);
        }

        // Implementacja abstrakcyjnej metody GenerateMultiplierForDraw(...)
        // Zwraca 2 gdy wszystkie trzy równe (wygrana), -1 gdy przegrana (gracz traci obstawione)
        protected override Result<int> GenerateMultiplierForDraw(int[] drawResult)
        {
            if (drawResult == null || drawResult.Length != 3)
                return Result.Failure<int>("Invalid draw result");

            if (drawResult[0] == drawResult[1] && drawResult[1] == drawResult[2])
                return Result.Success(2);

            // Jeœli nie ma trafienia zwracamy -1, dziêki czemu base.Play obliczy tokenChange = wager * (-1) -> odejmie obstawione
            return Result.Success(-1);
        }

        // Wygodna metoda dla kontrolera: wywo³uje base.Play(...) — base.Play ju¿ PERSYSTUJE zmianê salda,
        // wiêc tutaj NIE wykonujemy ponownego update'u. Pobieramy zaktualizowane saldo z DB i zwracamy.
        public async Task<Result<(int[] Draw, int TokensDelta, int NewBalance)>> PlayNumberDrawAsync(Wager wager, string userId)
        {
            if (wager == null) return Result.Failure<(int[], int, int)>("Wager is null");

            // Wywo³anie logiki z klasy bazowej — ta metoda (GameServiceBase.Play) wykona ChangeTokensForUser(...) i zapisze do DB.
            var playResult = await Play(wager, userId); // metoda Play z GameServiceBase<TDrawResult>
            if (playResult.IsFailure) return Result.Failure<(int[], int, int)>(playResult.Error);

            var tuple = playResult.Value; // Tuple<TDrawResult, Wager>
            int[] draw = tuple.Item1;
            Wager returnedWager = tuple.Item2;

            // returnedWager.wagerAmount w bazowej implementacji to tokenChange (mo¿e byæ ujemne)
            int tokenChange = returnedWager.wagerAmount;

            // Pobierz zaktualizowane saldo z bazy — Play(...) ju¿ zapisa³ zmianê, wiêc to odzwierciedli aktualny stan.
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