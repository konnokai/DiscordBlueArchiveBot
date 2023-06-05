public class BotConfig
{
    public string DiscordToken { get; set; } = "";
    public string RedisOption { get; set; } = "127.0.0.1,syncTimeout=3000";
    public ulong TestSlashCommandGuildId { get; set; } = 0;
    public string WebHookUrl { get; set; } = "";
    public string UptimeKumaPushUrl { get; set; } = "";

    public void InitBotConfig()
    {
        try { File.WriteAllText("bot_config_example.json", JsonConvert.SerializeObject(new BotConfig(), Formatting.Indented)); } catch { }
        if (!File.Exists("bot_config.json"))
        {
            Log.Error($"bot_config.json遺失，請依照 {Path.GetFullPath("bot_config_example.json")} 內的格式填入正確的數值");
            if (!Console.IsInputRedirected)
                Console.ReadKey();
            Environment.Exit(3);
        }

        try
        {
            var config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText("bot_config.json"))!;

            if (string.IsNullOrWhiteSpace(config.DiscordToken))
            {
                Log.Error($"{nameof(DiscordToken)}遺失，請輸入至bot_config.json後重開Bot");
                if (!Console.IsInputRedirected)
                    Console.ReadKey();
                Environment.Exit(3);
            }

            if (string.IsNullOrWhiteSpace(config.WebHookUrl))
            {
                Log.Error($"{nameof(WebHookUrl)}遺失，請輸入至bot_config.json後重開Bot");
                if (!Console.IsInputRedirected)
                    Console.ReadKey();
                Environment.Exit(3);
            }

            DiscordToken = config.DiscordToken;
            WebHookUrl = config.WebHookUrl;
            TestSlashCommandGuildId = config.TestSlashCommandGuildId;
            RedisOption = config.RedisOption;
            UptimeKumaPushUrl = config.UptimeKumaPushUrl;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "設定檔讀取失敗");
            throw;
        }
    }
}