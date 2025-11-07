using Microsoft.AspNetCore.Identity;

namespace raptorSlot.Models {
	public class AppUser : IdentityUser {

        public int Tokens { get; set; } = 0;
    }
}
