namespace DiscordBlueArchiveBot.Interaction.BlueArchive.Service.Json
{
    public interface IJson
    {
        public string Name { get; }
        public bool Localization { get; }
        public abstract Action<string>? DeserializeAction { get; }
    }

    public class BaseJson : IJson
    {
        public virtual string Name
            => GetType().Name.Replace("Json", "").ToLower();

        public virtual bool Localization { get; set; } = false;

        public virtual Action<string>? DeserializeAction { get; set; } = null;
    }
}
