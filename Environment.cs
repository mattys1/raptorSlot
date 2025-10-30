using System.Diagnostics;

namespace raptorSlot
{
    public static class EnvVars
    {
		public static readonly string ADMIN_EMAIL;
		public static readonly string ADMIN_PASSWORD;
		public static readonly string ADMIN_USERNAME;

        static EnvVars() {
            DotNetEnv.Env.Load();
            ADMIN_EMAIL = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "";
            ADMIN_USERNAME = Environment.GetEnvironmentVariable("ADMIN_USERNAME") ?? "";
            ADMIN_PASSWORD = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "";

			Debug.Assert(AreVarsSet(), "One or more required environment variables are not set.");
        }

		private static bool AreVarsSet() {
			var fields = typeof(EnvVars).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

			return fields
				.Select(f => f.GetValue(null) as string)
				.All(v => !string.IsNullOrEmpty(v));
		}
    }
}
