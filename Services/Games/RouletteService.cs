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

		protected override RouletteDraw Draw() {
			return new RouletteDraw(RandomNumberGenerator.GetInt32(0, 37));
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

		private Result<bool> CheckIfHasWon(RouletteChoice choice, RouletteDraw draw) {
			Result<OneOf<int, int[]>> GetSelectedValuesSequence(int length) {
				if(choice.Selected.Length != length) {
					return Result.Failure<OneOf<int, int[]>>($"This bet type requires exactly {length} selected value{(length >1 ? "s" : "")}, but got {choice.Selected.Length}");
				}
				return length == 1
					       ? Result.Success<OneOf<int, int[]>>(choice.Selected[0]) 
					       : Result.Success<OneOf<int, int[]>>(choice.Selected);
			}
			
			switch(choice.BetType) {
				case BetType.RED_OR_BLACK: {
					if(choice.Selected.Length != 1) {
						return Result.Failure<bool>("This bet type requires exactly one selected value");
					}
					
					var choiceValue = choice.Selected[0];
					HashSet<int> redNumbers = [1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36];

					return redNumbers.Contains(draw.Value) == redNumbers.Contains(choiceValue);
				}
				case BetType.ODD_OR_EVEN: {
					return GetSelectedValuesSequence(1)
						.Ensure(val => val.AsT0 is 0 or 1 ? Result.Success() : Result.Failure<bool>("Invalid selection for odd/even bet type"))
						.Map(val => draw.Value % 2 == val.AsT0 % 2);
				}
				case BetType.STRAIGHT_UP: { 
					return GetSelectedValuesSequence(1).Map(
					val => draw.Value == val.AsT0  	
					);
				}
				case BetType.SPLIT: {
					return GetSelectedValuesSequence(2)
						.Ensure(val => Result.SuccessIf(val.AsT1[0] - val.AsT1[1] == 3, "Invalid split selection"))
						.Map(val => val.AsT1.Contains(draw.Value));
				}
				case BetType.STREET: { 
					return GetSelectedValuesSequence(3)
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
