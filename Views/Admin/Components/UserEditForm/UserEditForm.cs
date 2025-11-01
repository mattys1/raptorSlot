using Microsoft.AspNetCore.Mvc;
using raptorSlot.Models;

namespace raptorSlot.Views.Admin.Components.UserEditForm {
	public class UserEditForm : ViewComponent {
		public IViewComponentResult Invoke(string controllerName, string methodName, string buttonText, string? guid, AppUser user) {
			return View(new UserEditFormInputViewModel {
				Id = guid,
				ActionName = methodName,
				ControllerName = controllerName,
				ButtonText = buttonText,
				ExistingUser = user
			});
		}		
	}

	public class UserEditFormInputViewModel {
		required public AppUser ExistingUser { get; set; }
        public string? Id { get; set; }
        required public string ActionName { get; set; }
        required public string ControllerName { get; set; }
        required public string ButtonText { get; set; }
	}
}