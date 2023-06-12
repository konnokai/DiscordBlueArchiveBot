using Discord.Interactions;

namespace DiscordBlueArchiveBot.DataBase.Table
{
    public class NotifyConfig : DbEntity
    {
        public enum NotifyType
        {
            [ChoiceDisplay("全部")]
            All = 0,
            [ChoiceDisplay("活動加倍")]
            Double = 1,
            [ChoiceDisplay("總力戰")]
            Raid = 2,
            [ChoiceDisplay("綜合戰術考試")]
            UnionOperation = 3,
            [ChoiceDisplay("晚上登入")] // 固定 17. 更新
            NightLogin = 4,
            [ChoiceDisplay("PVP獎勵")] // 固定 13. 更新
            PVPAward = 5,
            [ChoiceDisplay("咖啡廳換人")] // 固定 03. & 15. 更新
            CafeInterviewChange = 6,
            [ChoiceDisplay("咖啡廳邀請券更新")] // 設定後過 20 小時提醒
            CafeInviteTicketUpdate = 7,
        }

        public enum RegionType
        {
            [ChoiceDisplay("日版")]
            Japan = 0,
            [ChoiceDisplay("國際版")]
            Global = 1,
        }

        public ulong UserId { get; set; }
        public NotifyType NotifyTypeId { get; set; }
        public RegionType RegionTypeId { get; set; }
    }
}
