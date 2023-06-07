namespace DiscordBlueArchiveBot.Interaction.BlueArchive.Service.Json
{
    public interface IJson
    {
        public string Name { get; }
        public bool Localization { get; }
        public Action<string> DeserializeAction { get; }
    }
}
