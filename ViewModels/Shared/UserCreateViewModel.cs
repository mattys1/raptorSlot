using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace raptorSlot.ViewModels.Shared {
	public class UserCreateViewModel
	{
		public string? Id { get; set; }
		[Required]
		required public string Username { get; init; }

		[Required]
		[EmailAddress]
		required public string Email { get; init; }

		[Required]
		[DataType(DataType.Password)]
		required public string Password { get; init; }

		[Compare(nameof(Password))]
		required public string RepeatPassword { get; init; }
	}
}
