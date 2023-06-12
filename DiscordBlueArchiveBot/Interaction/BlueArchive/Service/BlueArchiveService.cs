#nullable disable

using DiscordBlueArchiveBot.DataBase.Table;
using DiscordBlueArchiveBot.Interaction.BlueArchive.Service.Json;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace DiscordBlueArchiveBot.Interaction.BlueArchive.Service
{
    public class BlueArchiveService : IInteractionService
    {
        private readonly DiscordSocketClient _client;
        private readonly HttpClient _httpClient;
        private readonly Timer _refreshTimer, _notify, _notifyCafeInviteTicketUpdateTimer;
        private CommonJson common = null;
        private StagesJson stages = null;
        private RaidsJson jpRaids = null;
        private LocalizationJson jpLocalizations = null;
        private RaidsJson twRaids = null;
        private LocalizationJson twLocalizations = null;
        private ConcurrentBag<EventData> eventDatas = new ConcurrentBag<EventData>();

        public BlueArchiveService(DiscordSocketClient client, IHttpClientFactory httpClientFactory)
        {
            _client = client;
            _httpClient = httpClientFactory.CreateClient();
            _refreshTimer = new Timer(new TimerCallback(async (obj) => await RefreshDataAsync()), null, TimeSpan.FromSeconds(1), TimeSpan.FromHours(1));
            _notify = new Timer(new TimerCallback(async (obj) => await NotifyAsync()), null, TimeSpan.FromSeconds((long)Math.Round(Convert.ToDateTime($"{DateTime.Now.AddHours(1):yyyy/MM/dd HH:00:00}").Subtract(DateTime.Now).TotalSeconds + 3)), TimeSpan.FromHours(1));
            _notifyCafeInviteTicketUpdateTimer = new Timer(new TimerCallback(async (obj) => await NotifyCafeInviteTicketUpdateAsync()), null, TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(1));
        }

        private async Task RefreshDataAsync()
        {
            try
            {
                eventDatas.Clear();

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
                            Log.Info($"JP Now Raid: {jpNowRaidData.Name} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                            eventDatas.Add(new EventData(NotifyConfig.RegionType.Japan, NotifyConfig.NotifyType.Raid, jpNowRaidData.Name, ConvertTimestampToDatetime(item.Start), ConvertTimestampToDatetime(item.End)));
                        }
                        else if (jpRaids.TimeAttacks.Any((x) => x.Id == item.Raid))
                        {
                            var timeAttack = jpRaids.TimeAttacks.FirstOrDefault((x) => x.Id == item.Raid);
                            if (jpLocalizations.Data["TimeAttackStage"].TryGetValue(timeAttack.DungeonType, out string timeAttackName))
                            {
                                Log.Info($"JP Now TimeAttacks: {timeAttackName} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                                eventDatas.Add(new EventData(NotifyConfig.RegionType.Japan, NotifyConfig.NotifyType.TimeAttack, timeAttackName, ConvertTimestampToDatetime(item.Start), ConvertTimestampToDatetime(item.End)));
                            }
                        }
                    }

                    foreach (var item in jpCurrentRegionsData.CurrentEvents)
                    {
                        string jpNowEventName = "";
                        if (jpLocalizations.Data["EventName"].TryGetValue(item.Event.ToString(), out jpNowEventName)) { }
                        else if (stages.Events.Any((x) => x.EventId == item.Event))
                        {
                            jpNowEventName = stages.Events.FirstOrDefault((x) => x.EventId == item.Event).NameJp;
                        }
                        else
                        {
                            Log.Warn($"JP No Event Data: {item.Event} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                            continue;
                        }

                        Log.Info($"JP Now Event: {jpNowEventName} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                        eventDatas.Add(new EventData(NotifyConfig.RegionType.Japan, NotifyConfig.NotifyType.Event, jpNowEventName, ConvertTimestampToDatetime(item.Start), ConvertTimestampToDatetime(item.End)));
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
                            Log.Info($"Global Now Raid: {globalNowRaidData.Name} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                            eventDatas.Add(new EventData(NotifyConfig.RegionType.Global, NotifyConfig.NotifyType.Raid, globalNowRaidData.Name, ConvertTimestampToDatetime(item.Start), ConvertTimestampToDatetime(item.End)));
                        }
                        else if (twRaids.TimeAttacks.Any((x) => x.Id == item.Raid))
                        {
                            var timeAttack = twRaids.TimeAttacks.FirstOrDefault((x) => x.Id == item.Raid);
                            if (twLocalizations.Data["TimeAttackStage"].TryGetValue(timeAttack.DungeonType, out string timeAttackName))
                            {
                                Log.Info($"Global Now TimeAttacks: {timeAttackName} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                                eventDatas.Add(new EventData(NotifyConfig.RegionType.Global, NotifyConfig.NotifyType.TimeAttack, timeAttackName, ConvertTimestampToDatetime(item.Start), ConvertTimestampToDatetime(item.End)));
                            }
                        }
                    }

                    foreach (var item in globalCurrentRegionsData.CurrentEvents)
                    {
                        string globalNowEventName = "";
                        if (twLocalizations.Data["EventName"].TryGetValue(item.Event.ToString(), out globalNowEventName)) { }
                        else if (stages.Events.Any((x) => x.EventId == item.Event))
                        {
                            globalNowEventName = stages.Events.FirstOrDefault((x) => x.EventId == item.Event).NameTw;
                        }
                        else
                        {
                            Log.Warn($"Global No Event Data: {item.Event} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                            continue;
                        }

                        Log.Info($"Global Now Event: {globalNowEventName} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                        eventDatas.Add(new EventData(NotifyConfig.RegionType.Japan, NotifyConfig.NotifyType.Event, globalNowEventName, ConvertTimestampToDatetime(item.Start), ConvertTimestampToDatetime(item.End)));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "RefreshDataAsync");
            }
        }

        private async Task NotifyAsync()
        {
            using (var db = DataBase.MainDbContext.GetDbContext())
            {
                switch (DateTime.Now.Hour)
                {
                    // 咖啡廳換人
                    case 9:
                    case 15:
                        foreach (var item in db.NotifyConfig.AsNoTracking().Where((x) => x.NotifyTypeId == NotifyConfig.NotifyType.All || x.NotifyTypeId == NotifyConfig.NotifyType.CafeInterviewChange).Distinct((x) => x.UserId))
                        {
                            await _client.SendMessageToDMChannel(item.UserId, "咖啡廳已換人!");
                        }
                        break;
                    // PVP 獎勵
                    case 13:
                        foreach (var item in db.NotifyConfig.AsNoTracking().Where((x) => x.NotifyTypeId == NotifyConfig.NotifyType.All || x.NotifyTypeId == NotifyConfig.NotifyType.PVPAward).Distinct((x) => x.UserId))
                        {
                            await _client.SendMessageToDMChannel(item.UserId, "可以領PVP獎勵了!");
                        }
                        break;
                    // 晚上登入
                    case 17:
                        foreach (var item in db.NotifyConfig.AsNoTracking().Where((x) => x.NotifyTypeId == NotifyConfig.NotifyType.All || x.NotifyTypeId == NotifyConfig.NotifyType.NightLogin).Distinct((x) => x.UserId))
                        {
                            await _client.SendMessageToDMChannel(item.UserId, "記得登入領晚上送的體力!");
                        }
                        break;
                    // 活動 & 總力 & 協同
                    case 18:
                        {
                            // 日版
                            if (eventDatas.Any((x) => x.RegionType == NotifyConfig.RegionType.Japan && x.EventType == NotifyConfig.NotifyType.Event && x.StartAt <= DateTime.Now && x.EndAt > DateTime.Now))
                            {
                                foreach (var item in db.NotifyConfig.AsNoTracking().Where((x) => x.RegionTypeId == NotifyConfig.RegionType.Japan && (x.NotifyTypeId == NotifyConfig.NotifyType.All || x.NotifyTypeId == NotifyConfig.NotifyType.Event)).Distinct((x) => x.UserId))
                                {
                                    await _client.SendMessageToDMChannel(item.UserId, "記得打日版活動!");
                                }
                            }

                            if (eventDatas.Any((x) => x.RegionType == NotifyConfig.RegionType.Japan && x.EventType == NotifyConfig.NotifyType.Raid && x.StartAt <= DateTime.Now && x.EndAt > DateTime.Now))
                            {
                                foreach (var item in db.NotifyConfig.AsNoTracking().Where((x) => x.RegionTypeId == NotifyConfig.RegionType.Japan && (x.NotifyTypeId == NotifyConfig.NotifyType.All || x.NotifyTypeId == NotifyConfig.NotifyType.Raid)).Distinct((x) => x.UserId))
                                {
                                    await _client.SendMessageToDMChannel(item.UserId, "記得打日版總力戰!");
                                }
                            }

                            if (eventDatas.Any((x) => x.RegionType == NotifyConfig.RegionType.Japan && x.EventType == NotifyConfig.NotifyType.TimeAttack && x.StartAt <= DateTime.Now && x.EndAt > DateTime.Now))
                            {
                                foreach (var item in db.NotifyConfig.AsNoTracking().Where((x) => x.RegionTypeId == NotifyConfig.RegionType.Japan && (x.NotifyTypeId == NotifyConfig.NotifyType.All || x.NotifyTypeId == NotifyConfig.NotifyType.TimeAttack)).Distinct((x) => x.UserId))
                                {
                                    await _client.SendMessageToDMChannel(item.UserId, "記得打日版協同!");
                                }
                            }

                            // 國際版
                            if (eventDatas.Any((x) => x.RegionType == NotifyConfig.RegionType.Global && x.EventType == NotifyConfig.NotifyType.Event && x.StartAt <= DateTime.Now && x.EndAt > DateTime.Now))
                            {
                                foreach (var item in db.NotifyConfig.AsNoTracking().Where((x) => x.RegionTypeId == NotifyConfig.RegionType.Global && (x.NotifyTypeId == NotifyConfig.NotifyType.All || x.NotifyTypeId == NotifyConfig.NotifyType.Event)).Distinct((x) => x.UserId))
                                {
                                    await _client.SendMessageToDMChannel(item.UserId, "記得打國際版活動!");
                                }
                            }

                            if (eventDatas.Any((x) => x.RegionType == NotifyConfig.RegionType.Global && x.EventType == NotifyConfig.NotifyType.Raid && x.StartAt <= DateTime.Now && x.EndAt > DateTime.Now))
                            {
                                foreach (var item in db.NotifyConfig.AsNoTracking().Where((x) => x.RegionTypeId == NotifyConfig.RegionType.Global && (x.NotifyTypeId == NotifyConfig.NotifyType.All || x.NotifyTypeId == NotifyConfig.NotifyType.Raid)).Distinct((x) => x.UserId))
                                {
                                    await _client.SendMessageToDMChannel(item.UserId, "記得打國際版總力戰!");
                                }
                            }

                            if (eventDatas.Any((x) => x.RegionType == NotifyConfig.RegionType.Global && x.EventType == NotifyConfig.NotifyType.TimeAttack && x.StartAt <= DateTime.Now && x.EndAt > DateTime.Now))
                            {
                                foreach (var item in db.NotifyConfig.AsNoTracking().Where((x) => x.RegionTypeId == NotifyConfig.RegionType.Global && (x.NotifyTypeId == NotifyConfig.NotifyType.All || x.NotifyTypeId == NotifyConfig.NotifyType.TimeAttack)).Distinct((x) => x.UserId))
                                {
                                    await _client.SendMessageToDMChannel(item.UserId, "記得打國際版協同!");
                                }
                            }

                            break;
                        }
                    default:
                        break;
                }
            }
        }

        private async Task NotifyCafeInviteTicketUpdateAsync()
        {
            using (var db = DataBase.MainDbContext.GetDbContext())
            {
                foreach (var item in db.CafeInviteTicketUpdateTime.Where((x) => x.NotifyDateTime <= DateTime.Now))
                {
                    Log.Info($"向 {item.UserId} 發送 {item.RegionTypeId} 邀請券更新通知");

                    await _client.SendMessageToDMChannel(item.UserId, (item.RegionTypeId == NotifyConfig.RegionType.Japan ? "日版" : "國際版") + $"的咖啡廳邀請券已更新!");

                    db.CafeInviteTicketUpdateTime.Remove(item);
                    await db.SaveChangesAsync();
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

    public static class Ext
    {
        public static async Task SendMessageToDMChannel(this DiscordSocketClient client, ulong userId, string message)
        {
            try
            {
                var user = await client.Rest.GetUserAsync(userId);
                if (user != null)
                {
                    var channel = await user.CreateDMChannelAsync();
                    await channel.SendMessageAsync(embed: new EmbedBuilder().WithOkColor().WithDescription(message).Build());
                    await channel.CloseAsync();
                }
            }
            catch (Discord.Net.HttpException discordEx) when (discordEx.DiscordCode != null)
            {
                Log.Error(discordEx, $"向 {userId} 發送訊息 `{message}` 失敗");
            }
        }
    }
}