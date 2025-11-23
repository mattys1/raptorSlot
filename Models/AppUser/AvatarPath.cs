namespace raptorSlot.Models {
    public readonly struct AvatarPath {
		internal AvatarPath(Guid userId, IWebHostEnvironment env) {
			Path = $"{env.WebRootPath}/avatars/{userId}.png";
		}

		internal AvatarPath(string avatarPath) {
			Path = avatarPath;
		}

		public readonly string Path;
	}	
    
    public class AvatarPathFactory(IWebHostEnvironment env) {
		public AvatarPath FromUserId(Guid userId) => new AvatarPath(userId, env);
		public AvatarPath FromPath(string path) => new AvatarPath(path);
	}
}	