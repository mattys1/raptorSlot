using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Mvc;
using raptorSlot.ViewModels.Shared;

namespace raptorSlot.Views.Shared.Components.UserPostForm {
	public class UserPostForm : ViewComponent {
		public IViewComponentResult Invoke(string controllerName, string methodName, string buttonText, Guid? guid) {
			return View(new UserPostFormInputViewModel {
				Id = guid,
				ActionName = methodName,
				ControllerName = controllerName,
				ButtonText = buttonText
			});
		}		
	}

	public class UserPostFormInputViewModel {
        public Guid? Id { get; set; }
        required public string ActionName { get; set; }
        required public string ControllerName { get; set; }
        required public string ButtonText { get; set; }
	}
}
