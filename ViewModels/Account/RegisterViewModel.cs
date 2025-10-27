using System.ComponentModel.DataAnnotations;

namespace raptorSlot.ViewModels.Account {
	public class RegisterViewModel
	{
		[Required]
		required public string Username { get; set; }

		[Required]
		[EmailAddress]
		required public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		required public string Password { get; set; }

		[Compare(nameof(Password))]
		required public string RepeatPassword { get; set; }
	}
}
