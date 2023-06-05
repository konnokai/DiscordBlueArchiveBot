namespace DiscordBlueArchiveBot.DataBase.Table
{
    public class GuildConfig : DbEntity
    {
        public ulong GuildId { get; set; }
        public string OpenAIKey { get; set; } = "";
    }
}
