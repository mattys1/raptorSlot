using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using raptorSlot.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace raptorSlot.Services {
	public class AvatarService(UserManager<AppUser> userManager, IWebHostEnvironment webHostEnvironment, AvatarPathFactory avatarPathFactory) {
		private const int AvatarSize = 256;
		
		#pragma warning disable CS1998
		public async Task<Result<Maybe<Image>>> GetUserAvatar(AppUser user) {
			return user.AvatarPath.Match(
			path => {
				try {
					var image = Image.Load(path.Path);
					return Result.Success(Maybe.From(image));
				}
				catch (Exception ex){
					return Result.Failure<Maybe<Image>>($"Failed to load avatar image: {ex.Message}");
				}
			},
			() => Result.Success(Maybe<Image>.None)
			);
		}
		#pragma  warning restore CS1998

		public async Task<Result> SetUserAvatar(AppUser user, Image image) {
			var resizedImage = ResizeAvatarImage(image);

			var saveResult = await SaveAvatar(user, resizedImage);
			if (saveResult.IsFailure)
				return saveResult;

			var path = saveResult.Value;

			var updatedUser = await userManager.FindByIdAsync(user.Id);
			if (updatedUser == null) {
				return Result.Failure($"User {updatedUser} not found when updating avatar path");
			}

			updatedUser.AvatarPath = path;
			var updateResult = await userManager.UpdateAsync(updatedUser);

			return Result.SuccessIf(updateResult.Succeeded, $"Failed to update user: {updatedUser} avatar path");
		}
		
		public async Task<Result> RemoveUserAvatar(AppUser user) {
			var updatedUser = await userManager.FindByIdAsync(user.Id);
			if(updatedUser == null) {
				return Result.Failure($"User {updatedUser} not found when removing avatar path");
			}

			var deleted = user.AvatarPath.Match(
			path => {
				try{
					File.Delete(path.Path);
					return Result.Success();
				}
				catch (Exception e){
					return Result.Failure($"Failed to delete avatar image file: {e.Message}");
				}
			},
			() => Result.Failure($"User: {updatedUser} does not have an avatar to delete")
			);

			if(deleted.IsFailure) {
				return deleted;
			}

			updatedUser.AvatarPath = Maybe.None;
			var updateResult = await userManager.UpdateAsync(updatedUser);

			return Result.SuccessIf(updateResult.Succeeded, $"Failed to update user: {updatedUser} avatar path");
		}
		
		private Image ResizeAvatarImage(Image image) {
			image.Mutate(x => x.Resize(new ResizeOptions {
				Size = new Size(AvatarSize, AvatarSize),
				Mode = ResizeMode.Stretch
			}));
			return image;
		}

		private async Task<Result<AvatarPath>> SaveAvatar(AppUser user, Image image) {
			var path = avatarPathFactory.FromUserId(new Guid(user.Id));

			try {
				Directory.CreateDirectory($"{webHostEnvironment.WebRootPath}/avatars");
				await image.SaveAsync(path.Path, new PngEncoder(), CancellationToken.None);
				return path;
			}
			catch (Exception e) {
				return Result.Failure<AvatarPath>($"Failed to save avatar image: {e.Message}");
			}

		}
	}
}
