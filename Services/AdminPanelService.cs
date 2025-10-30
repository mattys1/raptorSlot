using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using raptorSlot.Models;

namespace raptorSlot.Services
{
	public class AdminPanelService(UserManager<AppUser> userManager)
	{
		public Result DeleteUser(Guid id) {
			var user = userManager.FindByIdAsync(id.ToString()).Result;
			if(user == null) {
				return Result.Failure("User not found");
			}

			var result = userManager.DeleteAsync(user).Result;
			if(!result.Succeeded) {
				return Result.Failure("Failed to delete user");
			} else {
				return Result.Success();
			}
		}

		public List<AppUser> GetNonAdminUsers() {
			return [.. userManager.Users.Where(u => !u.Email!.Equals(EnvVars.ADMIN_EMAIL))];
		}
	}
}
