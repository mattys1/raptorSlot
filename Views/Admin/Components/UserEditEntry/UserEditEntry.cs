using Microsoft.AspNetCore.Mvc;
using raptorSlot.Models;

namespace raptorSlot.Views.Admin.Components.UserEditEntry
{
    public class UserEditEntry : ViewComponent
    {
		public IViewComponentResult Invoke(AppUser user)
		{
			return View(user);
		}
    }
}
