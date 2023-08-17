namespace DiscordBlueArchiveBot.DataBase.Table
{
    public class UserGacheCharacterRecord : DbEntity
    {
        public ulong UserId { get; set; }
        public int CharacterId { get; set; }
        public int Num { get; set; } = 0;
    }
}
