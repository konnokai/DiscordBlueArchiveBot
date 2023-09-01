#nullable disable

namespace DiscordBlueArchiveBot.Interaction.BlueArchive.Service.Json
{
    public class LocalizationJson : BaseJson
    {
        public LocalizationJson()
        {
            Localization = true;
            DeserializeAction = new Action<string>((str) => Data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(str));
        }

        public Dictionary<string, Dictionary<string, string>> Data { get; set; }
    }
}
