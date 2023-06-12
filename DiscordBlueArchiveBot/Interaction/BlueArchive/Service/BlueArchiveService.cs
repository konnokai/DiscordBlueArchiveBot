#nullable disable

using DiscordBlueArchiveBot.Interaction.BlueArchive.Service.Json;

namespace DiscordBlueArchiveBot.Interaction.BlueArchive.Service
{
    public class BlueArchiveService : IInteractionService
    {
        private readonly DiscordSocketClient _client;
        private readonly HttpClient _httpClient;
        private readonly Timer _refreshTimer, _notifyCafeInviteTicketUpdateTimer;
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
            _refreshTimer = new Timer(new TimerCallback(async (obj) => await RefreshDataAsync()), null, TimeSpan.FromSeconds(10), TimeSpan.FromHours(1));
            _notifyCafeInviteTicketUpdateTimer = new Timer(new TimerCallback(async (obj) => await NotifyCafeInviteTicketUpdate()), null, TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(1));
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

                var jpCurrentRegionsData = common.Regions.FirstOrDefault((x) => x.Name == "jp");
                if (jpCurrentRegionsData != null)
                {
                    foreach (var item in jpCurrentRegionsData.CurrentRaid)
                    {
                        var jpNowRaidData = jpRaids.Raids.FirstOrDefault((x) => x.Id == item.Raid);
                        if (jpNowRaidData != null)
                        {
                            Log.Info($"JP Now Raid Name: {jpNowRaidData.Name} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                        }
                        else if (jpRaids.TimeAttacks.Any((x) => x.Id == item.Raid))
                        {
                            var timeAttack = jpRaids.TimeAttacks.FirstOrDefault((x) => x.Id == item.Raid);
                            if (jpLocalizations.Data["TimeAttackStage"].TryGetValue(timeAttack.DungeonType, out string timeAttackName))
                            {
                                Log.Info($"JP Now TimeAttacks Name: {timeAttackName} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                            }
                        }
                    }

                    foreach (var item in jpCurrentRegionsData.CurrentEvents)
                    {
                        if (jpLocalizations.Data["EventName"].TryGetValue(item.Event.ToString(), out string jpNowEventName))
                        {
                            Log.Info($"JP Now Event Name: {jpNowEventName} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                        }
                        else if (stages.Events.Any((x) => x.EventId == item.Event))
                        {
                            Log.Info($"JP Now Event Name: {stages.Events.FirstOrDefault((x) => x.EventId == item.Event).NameJp} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                        }
                        else
                        {
                            Log.Warn($"JP No Event Name: {item.Event} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                            continue;
                        }
                    }
                }

                var globalCurrentRegionsData = common.Regions.FirstOrDefault((x) => x.Name == "global");
                if (globalCurrentRegionsData != null)
                {
                    foreach (var item in globalCurrentRegionsData.CurrentRaid)
                    {
                        var globalNowRaidData = twRaids.Raids.FirstOrDefault((x) => x.Id == item.Raid);
                        if (globalNowRaidData != null)
                        {
                            Log.Info($"Global Now Raid Name: {globalNowRaidData.Name} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                        }
                        else if (twRaids.TimeAttacks.Any((x) => x.Id == item.Raid))
                        {
                            var timeAttack = twRaids.TimeAttacks.FirstOrDefault((x) => x.Id == item.Raid);
                            if (twLocalizations.Data["TimeAttackStage"].TryGetValue(timeAttack.DungeonType, out string timeAttackName))
                            {
                                Log.Info($"Global Now TimeAttacks Name: {timeAttackName} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                            }
                        }
                    }

                    foreach (var item in globalCurrentRegionsData.CurrentEvents)
                    {
                        if (twLocalizations.Data["EventName"].TryGetValue(item.Event.ToString(), out string globalNowEventName))
                        {
                            Log.Info($"Global Now Event Name: {globalNowEventName} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                        }
                        else if (stages.Events.Any((x) => x.EventId == item.Event))
                        {
                            Log.Info($"Global Now Event Name: {stages.Events.FirstOrDefault((x) => x.EventId == item.Event).NameTw} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                        }
                        else
                        {
                            Log.Warn($"Global No Event Name: {item.Event} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "RefreshDataAsync");
            }
        }

        private async Task NotifyCafeInviteTicketUpdate()
        {
            using (var db = DataBase.MainDbContext.GetDbContext())
            {
                foreach (var item in db.CafeInviteTicketUpdateTime.Where((x) => x.NotifyDateTime <= DateTime.Now))
                {
                    try
                    {
                        Log.Info($"向 {item.UserId} 發送 {item.RegionTypeId} 邀請券更新通知");

                        var user = await _client.Rest.GetUserAsync(item.UserId);
                        if (user != null)
                        {
                            var channel = await user.CreateDMChannelAsync();
                            await channel.SendMessageAsync(item.RegionTypeId == DataBase.Table.NotifyConfig.RegionType.Japan ? "日版" : "國際版" + $"的咖啡廳邀請券已更新!");
                        }
                    }
                    catch (Discord.Net.HttpException discordEx) when (discordEx.DiscordCode != null)
                    {
                        Log.Error(discordEx, $"發送邀請券更新通知失敗: {item.UserId}");
                    }
                    finally
                    {
                        db.CafeInviteTicketUpdateTime.Remove(item);
                        await db.SaveChangesAsync();
                    }
                }
            }
        }

        private DateTime ConvertTimestampToDatetime(int? timestamp)
        => new DateTime(1970, 1, 1, 0, 0, 0).AddHours(8).AddSeconds(timestamp.Value);

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