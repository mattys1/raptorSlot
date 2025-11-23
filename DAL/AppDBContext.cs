using CSharpFunctionalExtensions;
using raptorSlot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace raptorSlot.DAL {
	public class AppDBContext(DbContextOptions<AppDBContext> options, AvatarPathFactory avatarPathFactory) : IdentityDbContext<AppUser>(options) {

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<AppUser>()
				.Property(e => e.AvatarPath)
				.HasConversion(
				m => m.HasValue
					     ? m.Value.Path
					     : "",
				s => !s.IsNullOrEmpty()
					     ? Maybe.From(avatarPathFactory.FromPath(s))
					     : Maybe.None
				);
		}
	}
}