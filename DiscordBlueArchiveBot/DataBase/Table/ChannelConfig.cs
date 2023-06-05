namespace DiscordBlueArchiveBot.DataBase.Table
{
    public class ChannelConfig : DbEntity
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong RoleId { get; set; }
        public bool IsEnable { get; set; } = true;
    }
}
