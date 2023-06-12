using static DiscordBlueArchiveBot.DataBase.Table.NotifyConfig;

namespace DiscordBlueArchiveBot.Interaction.BlueArchive.Service
{
    public class EventData
    {
        public RegionType RegionType { get; private set; }
        public NotifyType EventType { get; private set; }
        public string Name { get; private set; }
        public DateTime StartAt { get; private set; }
        public DateTime EndAt { get; private set; }

        public EventData(RegionType regionType, NotifyType eventType, string name, DateTime startAt, DateTime endAt)
        {
            RegionType = regionType;
            EventType = eventType;
            Name = name;
            StartAt = startAt;
            EndAt = endAt;
        }
    }
}