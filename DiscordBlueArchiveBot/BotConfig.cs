using DiscordBlueArchiveBot;

public class BotConfig
{
    public string? DiscordToken { get; set; } = default;
    public string RedisOption { get; set; } = "127.0.0.1,syncTimeout=3000";
    public ulong TestSlashCommandGuildId { get; set; } = default;
    public string? WebHookUrl { get; set; } = default;
    public string? UptimeKumaPushUrl { get; set; } = default;

    public void InitBotConfig()
    {
        if (Utility.InDocker)
        {
            Log.Info("從環境變數讀取設定");

            foreach (var item in GetType().GetProperties())
            {
                bool exitIfNoVar = false;
                object? origValue = item.GetValue(this);
                if (origValue == default) exitIfNoVar = true;

                object? setValue = Utility.GetEnvironmentVariable(item.Name, item.PropertyType, exitIfNoVar);
                setValue ??= origValue;

                item.SetValue(this, setValue);
            }
        }
        else
        {
            try { File.WriteAllText("bot_config_example.json", JsonConvert.SerializeObject(new BotConfig(), Formatting.Indented)); } catch { }
            if (!File.Exists("bot_config.json"))
            {
                Log.Error($"bot_config.json 遺失，請依照 {Path.GetFullPath("bot_config_example.json")} 內的格式填入正確的數值");
                if (!Console.IsInputRedirected)
                    Console.ReadKey();
                Environment.Exit(3);
            }

            try
            {
                var config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText("bot_config.json"))!;

                if (string.IsNullOrWhiteSpace(config.DiscordToken))
                {
                    Log.Error($"{nameof(DiscordToken)}遺失，請輸入至 bot_config.json 後重開 Bot");
                    if (!Console.IsInputRedirected)
                        Console.ReadKey();
                    Environment.Exit(3);
                }

                if (string.IsNullOrWhiteSpace(config.WebHookUrl))
                {
                    Log.Error($"{nameof(WebHookUrl)}遺失，請輸入至 bot_config.json 後重開 Bot");
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
}