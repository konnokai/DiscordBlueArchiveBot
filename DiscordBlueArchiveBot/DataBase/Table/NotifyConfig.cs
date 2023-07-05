using Discord.Interactions;

namespace DiscordBlueArchiveBot.DataBase.Table
{
    public class NotifyConfig : DbEntity
    {
        public enum NotifyType
        {
            [ChoiceDisplay("全部")]
            All = 0,
            [ChoiceDisplay("活動 (下午六點通知)")]
            Event = 1,
            [ChoiceDisplay("總力戰 (下午六點通知)")]
            Raid = 2,
            [ChoiceDisplay("綜合戰術考試 (下午六點通知)")]
            TimeAttack = 3,
            [ChoiceDisplay("晚上登入 (下午五點通知)")] // 固定 17. 更新
            NightLogin = 4,
            [ChoiceDisplay("PVP獎勵 (下午一點通知)")] // 固定 13. 更新
            PVPAward = 5,
            [ChoiceDisplay("咖啡廳換人 (早上九點及下午三點通知)")] // 固定 03. & 15. 更新
            CafeInterviewChange = 6,
            [ChoiceDisplay("今天誰生日 (早上九點通知，如果有的話)")]
            BirthdayStudent = 7,
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
