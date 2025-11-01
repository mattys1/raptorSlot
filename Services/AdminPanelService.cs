using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using raptorSlot.Models;
using raptorSlot.ViewModels.Account;
using raptorSlot.ViewModels.Admin.raptorSlot.ViewModels.Shared;
using raptorSlot.ViewModels.Shared;

namespace raptorSlot.Services
{
	public class AdminPanelService(UserManager<AppUser> userManager)
	{
		public async Task<Result> DeleteUser(Guid id) {
			var user = await userManager.FindByIdAsync(id.ToString());
			if(user == null) {
				return Result.Failure("User not found");
			}

			var result = await userManager.DeleteAsync(user);
			return !result.Succeeded
				       ? Result.Failure("Failed to delete user")
				       : Result.Success();
		}

		public List<AppUser> GetNonAdminUsers() {
			return [.. userManager.Users.Where(u => !u.Email!.Equals(EnvVars.ADMIN_EMAIL))];
		}

		public async Task<Result> CreateUser(UserCreateViewModel model) {
			var user = new AppUser {
				Email = model.Email,
				UserName = model.Username
			};
				
			var result = await userManager.CreateAsync(user, model.Password);
			if(result.Succeeded) {
				return Result.Success();
			} else {
				return Result.Failure<AppUser>(
						string.Join(", ", result.Errors.Select(e => e.Description))
				);
			}
		}

		public async Task<Result> EditUser(UserEditViewModel newData) {
			var user = await userManager.FindByIdAsync(newData.Id!);
			if(user == null) {
				return Result.Failure("User not found");
			}

			user.UserName = newData.Username;
			user.Email = newData.Email;

			var result = await userManager.UpdateAsync(user);
			if(!result.Succeeded) {
				var errors = string.Join(", ", result.Errors.Select(e => e.Description));
				return Result.Failure($"Failed to update user: {errors}");
			}

			if(!string.IsNullOrEmpty(newData.Password)) {
				var token = await userManager.GeneratePasswordResetTokenAsync(user);
				var passwordResult = await userManager.ResetPasswordAsync(user, token, newData.Password);
				if(!passwordResult.Succeeded) {
					var errors = string.Join(", ", passwordResult.Errors.Select(e => e.Description));
					return Result.Failure($"Failed to update password: {errors}");
				}
			}

			return Result.Success();
		}
	}
}
