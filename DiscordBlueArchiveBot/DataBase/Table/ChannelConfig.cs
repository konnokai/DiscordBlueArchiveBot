using Discord.Interactions;

namespace DiscordBlueArchiveBot.DataBase.Table
{
    public class ChannelConfig : DbEntity
    {
        public enum NotifyType
        {
            [ChoiceDisplay("活動加倍")]
            Double,
            [ChoiceDisplay("總力戰")]
            Raid,
            [ChoiceDisplay("合同火力演習")]
            UnionOperation,
        }

        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public string? NotifyMessage { get; set; }
        public NotifyType NotifyTypeId { get; set; }
    }
}
