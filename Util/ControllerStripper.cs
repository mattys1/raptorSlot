namespace raptorSlot.Util
{
	public static class ControllerStripper
	{
		public static string StripControllerSuffix(string controllerName)
		{
			if (controllerName.EndsWith("Controller"))
			{
				return controllerName[..^"Controller".Length];
			}
			return controllerName;
		}
	}
}
