using System.Diagnostics;
using System.Security.Cryptography;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using OneOf;
using raptorSlot.Models;

namespace raptorSlot.Services.Games {
	public enum BetType {
		STRAIGHT_UP,
		SPLIT,
		STREET,
		RED_OR_BLACK,
		ODD_OR_EVEN
	}
	
	public record RouletteChoice(
		BetType BetType,
		int[] Selected
	);

	public record RouletteDraw(int Value);
	
	public class RouletteService(UserManager<AppUser> userManager) : GameServiceBase<RouletteChoice, RouletteDraw>(userManager) {

		protected override RouletteDraw Draw(RouletteChoice choice, bool isUsingSuperTokens) {
			var firstValue = RandomNumberGenerator.GetInt32(0, 37);
			var firstDraw = new RouletteDraw(firstValue);

			var firstWinResult = CheckIfHasWon(choice, firstDraw);
			if(firstWinResult.IsFailure || firstWinResult.Value)
				return firstDraw;

			const double secondChance = 0.2;

			var bytes = new byte[4];
			RandomNumberGenerator.Fill(bytes);
			var randUint = BitConverter.ToUInt32(bytes, 0);
			var randDouble = randUint / (double)uint.MaxValue;

			if(isUsingSuperTokens && randDouble <= secondChance && choice.Selected.Length > 0) {
				var forcedIndex = RandomNumberGenerator.GetInt32(0, choice.Selected.Length);
				return new RouletteDraw(choice.Selected[forcedIndex]);
			}

			return firstDraw;
		}
		protected override Result<int> GenerateMultiplierForDraw(RouletteChoice payload, RouletteDraw drawResult) {
			var typeToPayoutMultiplier = new Dictionary<BetType, int> {
				{ BetType.STRAIGHT_UP, 18 },
				{ BetType.SPLIT, 10 },
				{ BetType.STREET, 7 },
				{ BetType.RED_OR_BLACK, 1 },
				{ BetType.ODD_OR_EVEN, 1 },
			};

			return CheckIfHasWon(payload, drawResult).Map(won => won ? typeToPayoutMultiplier[payload.BetType] : -1);
		}
		
		private Result<OneOf<int, int[]>> GetSelectedValuesSequence(RouletteChoice choice, int length) {
			if(choice.Selected.Length != length){
				return Result.Failure<OneOf<int, int[]>>($"This bet type requires exactly {length} selected value{(length > 1 ? "s" : "")}, but got {choice.Selected.Length}");
			}
			return length == 1
				       ? Result.Success<OneOf<int, int[]>>(choice.Selected[0])
				       : Result.Success<OneOf<int, int[]>>(choice.Selected);
		}

		private Result<bool> CheckIfHasWon(RouletteChoice choice, RouletteDraw draw) {
			Result EnsureSelectedNotZeroSingle(int selected) {
				return Result.FailureIf(selected == 0, "0 is not a valid selection for this bet type");
			}
			
			Result EnsureSelectedNotZeroMulti(int[] selected) {
				return Result.FailureIf(selected.Contains(0), "0 found withing selection, not valid for this bet type");
			}

			switch(choice.BetType) {
				case BetType.RED_OR_BLACK: {
					HashSet<int> redNumbers = [1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36];
    
					return GetSelectedValuesSequence(choice, 1)
						.Ensure(val => EnsureSelectedNotZeroSingle(val.AsT0))
						.Map(val => redNumbers.Contains(draw.Value) == redNumbers.Contains(val.AsT0));
				}
				case BetType.ODD_OR_EVEN: {
					return GetSelectedValuesSequence(choice, 1)
						.Map(val => draw.Value % 2 == val.AsT0 % 2);
				}
				case BetType.STRAIGHT_UP: { 
					return GetSelectedValuesSequence(choice, 1).Map(
					val => draw.Value == val.AsT0  	
					);
				}
				case BetType.SPLIT: {
					return GetSelectedValuesSequence(choice, 2)
						.Ensure(val => EnsureSelectedNotZeroMulti(val.AsT1))
						.Ensure(val => Result.SuccessIf(val.AsT1[0] - val.AsT1[1] == 3, "Invalid split selection"))
						.Map(val => val.AsT1.Contains(draw.Value));
				}
				case BetType.STREET: { 
					return GetSelectedValuesSequence(choice, 3)
						.Ensure(val => EnsureSelectedNotZeroMulti(val.AsT1))
						.Ensure(val => 
							Result.SuccessIf(val.AsT1.Select(n => n - val.AsT1.Min()).SequenceEqual([0, 1, 2]), "Invalid street selection")
						).Map(val => val.AsT1.Contains(draw.Value));
				}
				default:
					throw new UnreachableException("Unknown bet type");
			}
		}
	}
}
