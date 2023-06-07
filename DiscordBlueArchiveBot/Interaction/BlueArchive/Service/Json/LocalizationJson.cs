#nullable disable

namespace DiscordBlueArchiveBot.Interaction.BlueArchive.Service.Json
{
    public class LocalizationJson : IJson
    {
        public string Name { get; private set; }
        public bool Localization { get; private set; }
        public Action<string> DeserializeAction { get; private set; }

        public LocalizationJson()
        {
            Name = GetType().Name.Replace("Json", "").ToLower();
            Localization = true;
            DeserializeAction = new Action<string>((str) => Data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(str));
        }

        public Dictionary<string, Dictionary<string, string>> Data { get; set; }
    }
}
