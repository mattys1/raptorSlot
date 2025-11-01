using Microsoft.AspNetCore.Mvc;
using raptorSlot.Models;

namespace raptorSlot.Views.Shared.Components.UserCreateForm {
	public class UserCreateForm : ViewComponent {
		public IViewComponentResult Invoke(string controllerName, string methodName, string buttonText) {
			return View(new UserCreateFormInputViewModel {
				ActionName = methodName,
				ControllerName = controllerName,
				ButtonText = buttonText,
			});
		}		
	}

	public class UserCreateFormInputViewModel {
        required public string ActionName { get; set; }
        required public string ControllerName { get; set; }
        required public string ButtonText { get; set; }
	}
}
