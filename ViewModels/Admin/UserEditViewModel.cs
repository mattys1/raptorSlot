using raptorSlot.Models;

namespace raptorSlot.ViewModels.Admin {
using System.ComponentModel.DataAnnotations;

namespace raptorSlot.ViewModels.Shared {
	public class UserEditViewModel
	{
		public string? Id { get; set; }
		public string? Username { get; set; }

		[EmailAddress]
		public string? Email { get; set; }

		[DataType(DataType.Password)]
		public string? Password { get; set; }

		[Compare(nameof(Password))]
		public string? RepeatPassword { get; set; }
	}
}
}
