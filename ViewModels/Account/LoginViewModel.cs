using System.ComponentModel.DataAnnotations;

namespace raptorSlot.ViewModels.Account
{
    public class LoginViewModel
    {
		[Required]
		[EmailAddress]
		required public string Email { get; set; }
		
		[Required]
		required public string Password { get; set; }
    }
}
