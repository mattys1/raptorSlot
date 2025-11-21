using System.ComponentModel.DataAnnotations.Schema;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;

namespace raptorSlot.Models {
	public readonly struct AvatarPath {
		public AvatarPath(Guid userId) {
			Path = $"/avatars/{userId}.png";
		}

		public AvatarPath(string avatarPath) {
			Path = avatarPath;
		}

		public readonly string Path;
	}	
	
	public class AppUser : IdentityUser {

        public int Tokens { get; set; } = 0;
        public int SuperTokens { get; set; } = 0;
        public Maybe<AvatarPath> AvatarUri { get; set; }= Maybe.None;
	}
}
