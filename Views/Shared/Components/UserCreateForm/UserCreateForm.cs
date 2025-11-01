using Microsoft.AspNetCore.Mvc;
using raptorSlot.Models;

namespace raptorSlot.Views.Shared.Components.UserCreateForm {
	public class UserCreateForm : ViewComponent {
		public IViewComponentResult Invoke(string controllerName, string methodName, string buttonText, string? guid, AppUser? user) {
			return View(new UserCreateFormInputViewModel {
				Id = guid,
				ActionName = methodName,
				ControllerName = controllerName,
				ButtonText = buttonText,
			});
		}		
	}

	public class UserCreateFormInputViewModel {
        public string? Id { get; set; }
        required public string ActionName { get; set; }
        required public string ControllerName { get; set; }
        required public string ButtonText { get; set; }
	}
}
