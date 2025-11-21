using CSharpFunctionalExtensions;
using raptorSlot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace raptorSlot.DAL {
	public class AppDBContext(DbContextOptions<AppDBContext> options) : IdentityDbContext<AppUser>(options) {

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<AppUser>()
				.Property(e => e.AvatarUri)
				.HasConversion(
				m => m.HasValue
					     ? m.Value.Path
					     : null,
				s => s != null
					     ? Maybe.From(new AvatarPath(s))
					     : Maybe.None
				);
		}
	}
}