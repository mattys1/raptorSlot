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

        // Wygodna metoda dla kontrolera: wywo³uje base.Play(...) i PERSYSTUJE zmianê salda
        public async Task<Result<(int[] Draw, int TokensDelta, int NewBalance)>> PlayNumberDrawAsync(Wager wager, string userId)
        {
            if (wager == null) return Result.Failure<(int[], int, int)>("Wager is null");

            // Pobierz aktualne saldo przed wywo³aniem base.Play — u¿yjemy go do trwa³ego zapisu
            var userBefore = await _userManager.FindByIdAsync(userId);
            if (userBefore == null) return Result.Failure<(int[], int, int)>($"Cannot find user: {userId}");
            var startingBalance = userBefore.Tokens;

            // Wywo³anie logiki z klasy bazowej — ta metoda zmieni (w pamiêci) user.Tokens ale NIE zapisze do DB
            var playResult = await Play(wager, userId); // metoda Play z GameServiceBase<TDrawResult>
            if (playResult.IsFailure) return Result.Failure<(int[], int, int)>(playResult.Error);

            var tuple = playResult.Value; // Tuple<TDrawResult, Wager>
            int[] draw = tuple.Item1;
            Wager returnedWager = tuple.Item2;

            // returnedWager.wagerAmount w bazowej implementacji to tokenChange (mo¿e byæ ujemne)
            int tokenChange = returnedWager.wagerAmount;

            // Teraz ustawiamy trwa³e saldo: start + tokenChange
            int newBalance = startingBalance + tokenChange;
            if (newBalance < 0) newBalance = 0; // zabezpieczenie przed ujemnym stanem (opcjonalne)

            // Zapisujemy do bazy (UpdateAsync) — to powoduje, ¿e zmiana bêdzie trwa³a
            userBefore.Tokens = newBalance;
            var updateResult = await _userManager.UpdateAsync(userBefore);
            if (!updateResult.Succeeded)
            {
                _logger.LogError("Failed to persist user tokens for {UserId}. Errors: {Errors}", userId, updateResult.Errors);
                return Result.Failure<(int[], int, int)>("Failed to persist user tokens");
            }

            return Result.Success((draw, tokenChange, newBalance));
        }
    }
}