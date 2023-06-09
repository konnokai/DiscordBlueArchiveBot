using static DiscordBlueArchiveBot.DataBase.Table.NotifyConfig;

namespace DiscordBlueArchiveBot.DataBase.Table
{
    public class CafeInviteTicketUpdateTime : DbEntity
    {
        public ulong UserId { get; set; }
        public RegionType RegionTypeId { get; set; }
        public DateTime NotifyDateTime { get; set; }
    }
}
