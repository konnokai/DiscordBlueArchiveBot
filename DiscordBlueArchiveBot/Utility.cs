namespace DiscordBlueArchiveBot
{
    public static class Utility
    {
        public static bool InDocker { get { return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"; } }

        public const string PatreonUrl = "https://patreon.com/konnokai";
        public const string PaypalUrl = "https://paypal.me/jun112561";
    }
}
