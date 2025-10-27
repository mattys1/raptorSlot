using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using raptorSlot.Models;
using raptorSlot.ViewModels.Account;

namespace raptorSlot.Services {
	public class AccountService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) {
		public async Task<Result<AppUser>> RegisterUser(RegisterViewModel model) {
			var user = new AppUser {
				UserName = model.Username,
				Email = model.Email,
			};

			var result = await userManager.CreateAsync(user, model.Password);
			if(result.Succeeded) {
				await signInManager.SignInAsync(user, isPersistent: false);
				return Result.Success(user);
			} else {
				return Result.Failure<AppUser>(
						string.Join(", ", result.Errors.Select(e => e.Description))
				);
			}
		}

		public async Task<Result<AppUser>> LoginUser(LoginViewModel model) {
			var user = await userManager.FindByEmailAsync(model.Email);
			if(user == null) {
				return Result.Failure<AppUser>("User not found.");
			}
			// if(user == null) {
			// 	return Result.Failure<AppUser>("User not found.");
			// }
			//
			// var passwordValid = await userManager.CheckPasswordAsync(user, model.Password);
			// if(!passwordValid) {
			// 	return Result.Failure<AppUser>("Invalid password.");
			// }
			//
			// return Result.Success(user);


			var signedIn = signInManager.PasswordSignInAsync(
					user, 
					model.Password,
					isPersistent: false, lockoutOnFailure: false
			);

			return signedIn.Result.Succeeded
					? Result.Success(user)
					: Result.Failure<AppUser>(signedIn.Result.ToString());
		}

		public async Task LogoutUser() {
			await signInManager.SignOutAsync();
		}
	}
}
