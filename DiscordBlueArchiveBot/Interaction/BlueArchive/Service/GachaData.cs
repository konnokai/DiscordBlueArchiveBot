using static DiscordBlueArchiveBot.DataBase.Table.NotifyConfig;

namespace DiscordBlueArchiveBot.Interaction.BlueArchive.Service
{
    public class GachaData
    {
        public RegionType RegionType { get; private set; }
        public List<int> Characters { get; private set; }
        public DateTime StartAt { get; private set; }
        public DateTime EndAt { get; private set; }

        public GachaData(RegionType regionType, List<int> characters, DateTime startAt, DateTime endAt)
        {
            RegionType = regionType;
            Characters = characters;
            StartAt = startAt;
            EndAt = endAt;
        }
    }
}