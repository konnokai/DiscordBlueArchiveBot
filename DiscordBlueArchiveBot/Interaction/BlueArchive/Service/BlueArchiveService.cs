#nullable disable

using DiscordBlueArchiveBot.Interaction.BlueArchive.Service.Json;

namespace DiscordBlueArchiveBot.Interaction.BlueArchive.Service
{
    public class BlueArchiveService : IInteractionService
    {
        private readonly DiscordSocketClient _client;
        private readonly HttpClient _httpClient;
        private readonly Timer _timer;
        private CommonJson common = null;
        private StagesJson stages = null;
        private RaidsJson jpRaids = null;
        private LocalizationJson jpLocalizations = null;
        private RaidsJson twRaids = null;
        private LocalizationJson twLocalizations = null;

        public BlueArchiveService(DiscordSocketClient client, IHttpClientFactory httpClientFactory)
        {
            _client = client;
            _httpClient = httpClientFactory.CreateClient();
            _timer = new Timer(new TimerCallback(async (obj) => await RefreshDataAsync()), null, TimeSpan.FromSeconds(1), TimeSpan.FromHours(1));
        }

        private async Task RefreshDataAsync()
        {
            try
            {
                common = await GetDataFromServerAsync<CommonJson>();
                stages = await GetDataFromServerAsync<StagesJson>();
                twRaids = await GetDataFromServerAsync<RaidsJson>("tw");
                twLocalizations = await GetDataFromServerAsync<LocalizationJson>("tw");
                jpRaids = await GetDataFromServerAsync<RaidsJson>("jp");
                jpLocalizations = await GetDataFromServerAsync<LocalizationJson>("jp");

                var jpNowRaidId = common.Regions.FirstOrDefault((x) => x.Name == "jp").CurrentRaid[0].Raid;
                var jpNowEventId = common.Regions.FirstOrDefault((x) => x.Name == "jp").CurrentEvents[0].Event;
                Log.Info($"JP Now Raid Name: {jpRaids.Raids.FirstOrDefault((x) => x.Id == jpNowRaidId).Name}");
                Log.Info($"JP Now Event Name: {jpLocalizations.Data["EventName"][jpNowEventId.ToString()]}");

                var globalNowRaidId = common.Regions.FirstOrDefault((x) => x.Name == "global").CurrentRaid[0].Raid;
                var globalNowEventId = common.Regions.FirstOrDefault((x) => x.Name == "global").CurrentEvents[0].Event;
                Log.Info($"Global Now Raid Name: {twRaids.Raids.FirstOrDefault((x) => x.Id == globalNowRaidId).Name}");
                Log.Info($"Global Now Event Name: {stages.Events.FirstOrDefault((x) => x.EventId == globalNowEventId).NameTw}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "RefreshDataAsync");
            }
        }

        private async Task<T> GetDataFromServerAsync<T>(string localization = "") where T : IJson, new()
        {
            T type = new(); string json;

            if (type.Localization && string.IsNullOrEmpty(localization))
                throw new NullReferenceException(nameof(localization));
            else if (!type.Localization && !string.IsNullOrEmpty(localization))
                throw new AggregateException(nameof(localization));

            string redisKey = $"bluearchive:{type.Name}:{localization}".TrimEnd(':');

            if (await Program.RedisDb.KeyExistsAsync(redisKey))
            {
                Log.Debug($"Load From Redis: {redisKey}");
                json = await Program.RedisDb.StringGetAsync(redisKey);
            }
            else
            {
                if (type.Localization) localization += "/";
                string url = $"https://schale.gg/data/{localization}{type.Name}.min.json?v={DateTime.Now.ToFileTimeUtc()}";
                Log.Debug($"Load From API: {url}");

                json = await _httpClient.GetStringAsync(url);
                await Program.RedisDb.StringSetAsync(redisKey, json, TimeSpan.FromHours(1));
            }

            if (type.DeserializeAction != null)
                type.DeserializeAction(json);
            else
                type = JsonConvert.DeserializeObject<T>(json);

            if (type == null)
                throw new NullReferenceException(nameof(type));

            return type;
        }
    }
}