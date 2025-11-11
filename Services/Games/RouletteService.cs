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
		OneOf<int, int[]> Selected
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
			switch(choice.BetType) {
				case BetType.RED_OR_BLACK: {
					var choiceValue = choice.Selected.AsT0;
					HashSet<int> redNumbers = [1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36];

					return redNumbers.Contains(draw.Value) == redNumbers.Contains(choiceValue);
				}
				case BetType.ODD_OR_EVEN: {
					var choiceValue = choice.Selected.AsT0;
					return draw.Value % 2 == choiceValue % 2;
				}
				case BetType.STRAIGHT_UP: { 
					var choiceValue = choice.Selected.AsT0;
					Debug.Assert(choiceValue is >= 1 and <= 37, "Value chosen by user is out of range");	
					return draw.Value == choiceValue;
				}
				case BetType.SPLIT: {
					var choiceValue = choice.Selected.AsT1;
					if(choiceValue.Length != 2){
						return Result.Failure<bool>("Split bet must have exactly two numbers selected");
					}
					
					return choiceValue.Contains(draw.Value);
				}
				case BetType.STREET: { 
					var choiceValue = choice.Selected.AsT1;
					if(choiceValue.Length != 3){
						return Result.Failure<bool>("Street bet must have exactly three numbers selected");
					}
					
					return choiceValue.Contains(draw.Value);
				}
				default:
					throw new UnreachableException("Unknown bet type");
			}
		}
	}
}
