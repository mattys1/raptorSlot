namespace raptorSlot
{
    public class EnvVars
    {
		public static readonly EnvVars Get = new();

		public readonly string ADMIN_EMAIL;
		public readonly string ADMIN_PASSWORD;
		public readonly string ADMIN_USERNAME;

        private EnvVars()
        {
            DotNetEnv.Env.Load();
            ADMIN_EMAIL = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "";
            ADMIN_USERNAME = Environment.GetEnvironmentVariable("ADMIN_USERNAME") ?? "";
            ADMIN_PASSWORD = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "";
        }
    }
}
