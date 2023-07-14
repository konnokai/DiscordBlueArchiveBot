namespace DiscordBlueArchiveBot.DataBase.Table
{
    public class UserGachaRecord : DbEntity
    {
        public ulong UserId { get; set; }
        public uint TotalGachaCount { get; set; } = 0;
        public uint ThreeStarCount { get; set; } = 0;
        public uint PickUpCount { get; set; } = 0;
    }
}
