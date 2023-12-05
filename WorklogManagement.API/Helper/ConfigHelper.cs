namespace WorklogManagement.API.Helper
{
    internal static class ConfigHelper
    {
        internal static IConfiguration Config;
        internal static void Initialize(IConfiguration config)
        {
            Config = config;
        }
    }
}
