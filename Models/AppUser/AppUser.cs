using System.ComponentModel.DataAnnotations.Schema;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using SixLabors.ImageSharp;

namespace raptorSlot.Models {
	public class AppUser : IdentityUser {

        public int Tokens { get; set; } = 0;
        public int SuperTokens { get; set; } = 0;
        public Maybe<AvatarPath> AvatarUri { get; set; }= Maybe.None;
	}
}
