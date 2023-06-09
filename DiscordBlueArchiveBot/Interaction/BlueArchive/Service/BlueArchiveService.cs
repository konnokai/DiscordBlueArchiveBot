﻿#nullable disable

using DiscordBlueArchiveBot.DataBase.Table;
using DiscordBlueArchiveBot.Interaction.BlueArchive.Service.Json;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using StackExchange.Redis;
using System.Collections.Concurrent;
using static DiscordBlueArchiveBot.DataBase.Table.NotifyConfig;
using Color = SixLabors.ImageSharp.Color;
using Image = SixLabors.ImageSharp.Image;
using Point = SixLabors.ImageSharp.Point;

namespace DiscordBlueArchiveBot.Interaction.BlueArchive.Service
{
    public class BlueArchiveService : IInteractionService
    {
        public bool IsRefreshingData { get; private set; } = true;

        // https://kamigame.jp/bluearchive/page/240666421468632572.html#%E9%85%8D%E5%B8%83%E3%82%AD%E3%83%A3%E3%83%A9%E4%B8%80%E8%A6%A7
        public ushort[] EventStudents { get; private set; } = new ushort[] { 13004, 26006, 16006, 26008, 16008, 16009, 26007, 20004, 16010, 16011, 16007, 16012, 26009, 26010 };
        public List<Student> Students => _students?.Data;
        public List<Student> JPPickUpDatas { get; private set; } = new();
        public List<Student> GlobalPickUpDatas { get; private set; } = new();

        private readonly DiscordSocketClient _client;
        private readonly HttpClient _httpClient;
        private readonly Timer _refreshTimer, _notify, _notifyCafeInviteTicketUpdateTimer;
        private CommonJson _common = null;
        private StagesJson _stages = null;
        private StudentsJson _students = null;
        private RaidsJson _jpRaids = null;
        private LocalizationJson _jpLocalizations = null;
        private RaidsJson _twRaids = null;
        private LocalizationJson _twLocalizations = null;
        private ConcurrentBag<EventData> _eventDatas = new();

        public BlueArchiveService(DiscordSocketClient client, IHttpClientFactory httpClientFactory)
        {
            _client = client;
            _httpClient = httpClientFactory.CreateClient();
            _refreshTimer = new Timer(new TimerCallback(async (obj) => await RefreshDataAsync()), null, TimeSpan.FromSeconds(1), TimeSpan.FromHours(1));
            _notify = new Timer(new TimerCallback(async (obj) => await NotifyAsync()), null, TimeSpan.FromSeconds((long)Math.Round(Convert.ToDateTime($"{DateTime.Now.AddHours(1):yyyy/MM/dd HH:00:00}").Subtract(DateTime.Now).TotalSeconds) + 1), TimeSpan.FromHours(1));
            _notifyCafeInviteTicketUpdateTimer = new Timer(new TimerCallback(async (obj) => await NotifyCafeInviteTicketUpdateAsync()), null, TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(1));

            _client.ButtonExecuted += _client_ButtonExecuted;
        }

        public string GetStudentAvatarPath(int id)
            => Program.GetDataFilePath($"Avatar{Program.GetPlatformSlash()}{id}.jpg");

        private async Task _client_ButtonExecuted(SocketMessageComponent component)
        {
            if (component.HasResponded)
                return;

            try
            {
                string[] customId = component.Data.CustomId.Split(new char[] { ':' });

                if (!customId[0].StartsWith("roll") || customId.Length != 4)
                    return;

                if (customId[2] != component.User.Id.ToString() && component.User.Id != Program.ApplicatonOwner.Id)
                {
                    await component.SendErrorAsync("你不可使用本按鈕");
                    return;
                }

                try
                {
                    var pickUpStudentList = customId[1] == "0" ? JPPickUpDatas : GlobalPickUpDatas;
                    string[] studentsId = customId[3].Split('_', StringSplitOptions.RemoveEmptyEntries);

                    Color[] colors =
                    {
                        Color.Red, Color.Yellow, Color.Green, Color.Blue, Color.Purple,
                        Color.Red, Color.Yellow, Color.Green, Color.Blue, Color.Purple
                    };

                    using var memoryStream = new MemoryStream();
                    using Image image = Image.Load(Properties.Resources.Event_Main_Stage_Bg.AsSpan());

                    for (int i = 0; i < studentsId.Length; i++)
                    {
                        var item = Students.SingleOrDefault((x) => x.Id.ToString() == studentsId[i]);
                        if (item == null)
                        {
                            Log.Error($"Id: {studentsId[i]} 無學生資料!!");

                            await component.UpdateAsync((act) =>
                            {
                                act.Components = null;
                                act.Embed = new EmbedBuilder().WithErrorColor().WithDescription("缺少學生資料，無法繪製圖片，請向 Bot 擁有者確認").Build();
                            });

                            return;
                        }

                        using (var img = Image.Load(GetStudentAvatarPath(item.Id)))
                        {
                            try
                            {
                                int x = 100 + (img.Width + 50) * (i > 4 ? i - 5 : i);
                                int y = i > 4 ? 350 : 50;
                                var rect = new RectangularPolygon(x - 10, y - 10, img.Width + 20, img.Height + 20);

                                switch (item.StarGrade)
                                {
                                    case 1:
                                        image.Mutate((act) => act.Fill(Color.FromRgb(254, 254, 254), rect));
                                        break;
                                    case 2:
                                        image.Mutate((act) => act.Fill(Color.FromRgb(255, 247, 122), rect));
                                        break;
                                    case 3 when !pickUpStudentList.Any((x) => x.Id == item.Id):
                                        image.Mutate((act) => act.Fill(Color.FromRgb(239, 195, 220), rect));
                                        break;
                                    case 3 when pickUpStudentList.Any((x) => x.Id == item.Id):
                                        var brush = new PathGradientBrush(rect.Points.ToArray(), colors, Color.White);
                                        image.Mutate((act) => act.Fill(brush));
                                        break;
                                }

                                image.Mutate((act) => act.DrawImage(img, new Point(x, y), 1f));
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, $"Draw Student Error: {item.Id} ({i})");
                            }
                        }
                    }

                    string description = component.Message.Embeds.First().Description;
                    try
                    {
                        var valve = await Program.RedisDb.HashGetAsync(new RedisKey("bluearchive:gachaRecord"), new RedisValue(customId[2]));
                        if (valve.HasValue)
                        {
                            var userGachaRecord = JsonConvert.DeserializeObject<UserGachaRecord>(valve);
                            double threeStartPercentage = userGachaRecord.ThreeStarCount == 0 ? 0 : Math.Round((double)userGachaRecord.ThreeStarCount / userGachaRecord.TotalGachaCount * 100, 2);
                            double pickUpPercentage = userGachaRecord.PickUpCount == 0 ? 0 : Math.Round((double)userGachaRecord.PickUpCount / userGachaRecord.TotalGachaCount * 100, 2);

                            description += "\n" +
                                $"總抽數: {userGachaRecord.TotalGachaCount}\n" +
                                $"三星數: {userGachaRecord.ThreeStarCount} ({threeStartPercentage}%)\n" +
                                $"PickUp數: {userGachaRecord.PickUpCount} ({pickUpPercentage}%)";
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "DrawRoll - 資料庫存取失敗");
                    }

                    var eb = new EmbedBuilder().WithOkColor()
                        .WithDescription(description)
                        .WithFooter("僅供娛樂，模擬抽卡並不會跟遊戲結果一致，如有疑問建議換帳號重新開局")
                        .WithImageUrl("attachment://image.jpg");

                    image.Save(memoryStream, new JpegEncoder());

                    await component.UpdateAsync((act) =>
                    {
                        act.Attachments = new List<FileAttachment>() { new FileAttachment(memoryStream, "image.jpg") };
                        act.Embed = eb.Build();
                        act.Components = null;
                    });
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Draw Error");
                    await component.SendErrorAsync("繪圖錯誤，請向孤之界回報此問題", true);
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "RollButtonExecuted");
                await component.SendErrorAsync("錯誤，請向孤之界回報此問題", true);
                return;
            }
        }

        private async Task RefreshDataAsync()
        {
            try
            {
                IsRefreshingData = true;

                _eventDatas.Clear();

                _common = await GetDataFromServerAsync<CommonJson>();
                _stages = await GetDataFromServerAsync<StagesJson>();
                _students = await GetDataFromServerAsync<StudentsJson>("tw");
                _twRaids = await GetDataFromServerAsync<RaidsJson>("tw");
                _twLocalizations = await GetDataFromServerAsync<LocalizationJson>("tw");
                _jpRaids = await GetDataFromServerAsync<RaidsJson>("jp");
                _jpLocalizations = await GetDataFromServerAsync<LocalizationJson>("jp");

                Log.Info($"最新的學生名稱: \"{Students.Last().StudentName}\" | 總學生數量: {Students.Count}");
                if (!Directory.Exists(Program.GetDataFilePath($"Avatar")))
                    Directory.CreateDirectory(Program.GetDataFilePath($"Avatar"));

                foreach (var item in Students)
                {
                    if (!File.Exists(GetStudentAvatarPath(item.Id)))
                    {
                        try
                        {
                            Log.Info($"下載 {item.Id} 的頭像");
                            var stream = await _httpClient.GetStreamAsync($"https://schale.gg/images/student/collection/{item.CollectionTexture}.webp");
                            using (var img = await SixLabors.ImageSharp.Image.LoadAsync(stream))
                            {
                                await img.SaveAsJpegAsync(GetStudentAvatarPath(item.Id));
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, $"{item.Id} 頭像下載失敗");
                        }
                    }
                }

                var jpCurrentRegionsData = _common.Regions.FirstOrDefault((x) => x.Name == "jp");
                if (jpCurrentRegionsData != null)
                {
                    // 僅使用現在正開放池的當Pu資料
                    foreach (var item in jpCurrentRegionsData.CurrentGacha.Where((x) => ConvertTimestampToDatetime(x.Start) <= DateTime.Now && ConvertTimestampToDatetime(x.End) > DateTime.Now))
                    {
                        // 要限制只出三星的Pu不然機率會出問題
                        JPPickUpDatas.AddRange(Students.Where((x) => item.Characters.Any((x2) => x2.Value == x.Id && x.StarGrade == 3)));
                    }

                    foreach (var item in jpCurrentRegionsData.CurrentRaid)
                    {
                        var jpNowRaidData = _jpRaids.Raids.FirstOrDefault((x) => x.Id == item.Raid);
                        if (jpNowRaidData != null)
                        {
                            Log.Info($"JP Now Raid: {jpNowRaidData.Name} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                            _eventDatas.Add(new EventData(RegionType.Japan, NotifyType.Raid, jpNowRaidData.Name, ConvertTimestampToDatetime(item.Start), ConvertTimestampToDatetime(item.End)));
                        }
                        else if (_jpRaids.TimeAttacks.Any((x) => x.Id == item.Raid))
                        {
                            var timeAttack = _jpRaids.TimeAttacks.FirstOrDefault((x) => x.Id == item.Raid);
                            if (_jpLocalizations.Data["TimeAttackStage"].TryGetValue(timeAttack.DungeonType, out string timeAttackName))
                            {
                                Log.Info($"JP Now TimeAttacks: {timeAttackName} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                                _eventDatas.Add(new EventData(RegionType.Japan, NotifyType.TimeAttack, timeAttackName, ConvertTimestampToDatetime(item.Start), ConvertTimestampToDatetime(item.End)));
                            }
                        }
                    }

                    foreach (var item in jpCurrentRegionsData.CurrentEvents)
                    {
                        string jpNowEventName = "";
                        if (_jpLocalizations.Data["EventName"].TryGetValue(item.Event.ToString(), out jpNowEventName)) { }
                        else if (_stages.Events.Any((x) => x.EventId == item.Event))
                        {
                            jpNowEventName = _stages.Events.FirstOrDefault((x) => x.EventId == item.Event).NameJp;
                        }
                        else
                        {
                            Log.Warn($"JP No Event Data: {item.Event} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                            continue;
                        }

                        Log.Info($"JP Now Event: {jpNowEventName} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                        _eventDatas.Add(new EventData(RegionType.Japan, NotifyType.Event, jpNowEventName, ConvertTimestampToDatetime(item.Start), ConvertTimestampToDatetime(item.End)));
                    }
                }

                var globalCurrentRegionsData = _common.Regions.FirstOrDefault((x) => x.Name == "global");
                if (globalCurrentRegionsData != null)
                {
                    foreach (var item in globalCurrentRegionsData.CurrentGacha.Where((x) => ConvertTimestampToDatetime(x.Start) <= DateTime.Now && ConvertTimestampToDatetime(x.End) > DateTime.Now))
                    {
                        GlobalPickUpDatas.AddRange(Students.Where((x) => item.Characters.Any((x2) => x2.Value == x.Id && x.StarGrade == 3)));
                    }

                    foreach (var item in globalCurrentRegionsData.CurrentRaid)
                    {
                        var globalNowRaidData = _twRaids.Raids.FirstOrDefault((x) => x.Id == item.Raid);
                        if (globalNowRaidData != null)
                        {
                            Log.Info($"Global Now Raid: {globalNowRaidData.Name} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                            _eventDatas.Add(new EventData(RegionType.Global, NotifyType.Raid, globalNowRaidData.Name, ConvertTimestampToDatetime(item.Start), ConvertTimestampToDatetime(item.End)));
                        }
                        else if (_twRaids.TimeAttacks.Any((x) => x.Id == item.Raid))
                        {
                            var timeAttack = _twRaids.TimeAttacks.FirstOrDefault((x) => x.Id == item.Raid);
                            if (_twLocalizations.Data["TimeAttackStage"].TryGetValue(timeAttack.DungeonType, out string timeAttackName))
                            {
                                Log.Info($"Global Now TimeAttacks: {timeAttackName} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                                _eventDatas.Add(new EventData(RegionType.Global, NotifyType.TimeAttack, timeAttackName, ConvertTimestampToDatetime(item.Start), ConvertTimestampToDatetime(item.End)));
                            }
                        }
                    }

                    foreach (var item in globalCurrentRegionsData.CurrentEvents)
                    {
                        string globalNowEventName = "";
                        if (_twLocalizations.Data["EventName"].TryGetValue(item.Event.ToString(), out globalNowEventName)) { }
                        else if (_stages.Events.Any((x) => x.EventId == item.Event))
                        {
                            globalNowEventName = _stages.Events.FirstOrDefault((x) => x.EventId == item.Event).NameTw;
                        }
                        else
                        {
                            Log.Warn($"Global No Event Data: {item.Event} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                            continue;
                        }

                        Log.Info($"Global Now Event: {globalNowEventName} ({ConvertTimestampToDatetime(item.Start)} ~ {ConvertTimestampToDatetime(item.End)})");
                        _eventDatas.Add(new EventData(RegionType.Global, NotifyType.Event, globalNowEventName, ConvertTimestampToDatetime(item.Start), ConvertTimestampToDatetime(item.End)));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "RefreshDataAsync");
            }
            finally
            {
                IsRefreshingData = false;
            }
        }

        private async Task NotifyAsync()
        {
            using (var db = DataBase.MainDbContext.GetDbContext())
            {
                switch (DateTime.Now.Hour)
                {
                    case 9 or 15:
                        // 生日提醒
                        if (DateTime.Now.Hour == 9 && Students != null)
                        {
                            var birthdayStudent = Students.Where((x) => x.BirthDay == $"{DateTime.Now:M/d}").Distinct((x) => x.PersonalName);
                            if (birthdayStudent.Any())
                            {
                                Log.Info($"今天生日的學生: {string.Join(", ", birthdayStudent.Select((x) => x.PersonalName))}");

                                foreach (var item in db.NotifyConfig.AsNoTracking().Where((x) => x.NotifyTypeId == NotifyType.All || x.NotifyTypeId == NotifyType.BirthdayStudent).Distinct((x) => x.UserId))
                                {
                                    await _client.SendMessageToDMChannel(item.UserId, 
                                        $"今天是 `{string.Join(", ", birthdayStudent.Select((x) => x.PersonalName))}` 的生日!", 
                                        $"https://schale.gg/images/student/collection/{birthdayStudent.First().CollectionTexture}.webp");
                                }
                            }
                        }

                        // 咖啡廳換人
                        foreach (var item in db.NotifyConfig.AsNoTracking().Where((x) => x.NotifyTypeId == NotifyType.All || x.NotifyTypeId == NotifyType.CafeInterviewChange).Distinct((x) => x.UserId))
                        {
                            await _client.SendMessageToDMChannel(item.UserId, "咖啡廳已換人!");
                        }
                        break;
                    // PVP 獎勵
                    case 13:
                        foreach (var item in db.NotifyConfig.AsNoTracking().Where((x) => x.NotifyTypeId == NotifyType.All || x.NotifyTypeId == NotifyType.PVPAward).Distinct((x) => x.UserId))
                        {
                            await _client.SendMessageToDMChannel(item.UserId, "可以領PVP獎勵了\n" +
                                "記得先把排名打上去再領!!!");
                        }
                        break;
                    // 晚上登入
                    case 17:
                        foreach (var item in db.NotifyConfig.AsNoTracking().Where((x) => x.NotifyTypeId == NotifyType.All || x.NotifyTypeId == NotifyType.NightLogin).Distinct((x) => x.UserId))
                        {
                            await _client.SendMessageToDMChannel(item.UserId, "可以登入領晚上送的體力了\n" +
                                "記得順便領咖啡廳的體力!");
                        }
                        break;
                    // 活動 & 總力 & 協同
                    case 18:
                        {
                            // 日版
                            if (_eventDatas.Any((x) => x.RegionType == RegionType.Japan && x.EventType == NotifyType.Event && x.StartAt <= DateTime.Now && x.EndAt > DateTime.Now))
                            {
                                foreach (var item in db.NotifyConfig.AsNoTracking().Where((x) => x.RegionTypeId == RegionType.Japan && (x.NotifyTypeId == NotifyType.All || x.NotifyTypeId == NotifyType.Event)).Distinct((x) => x.UserId))
                                {
                                    await _client.SendMessageToDMChannel(item.UserId, "記得打日版活動!");
                                }
                            }

                            if (_eventDatas.Any((x) => x.RegionType == RegionType.Japan && x.EventType == NotifyType.Raid && x.StartAt <= DateTime.Now && x.EndAt > DateTime.Now))
                            {
                                foreach (var item in db.NotifyConfig.AsNoTracking().Where((x) => x.RegionTypeId == RegionType.Japan && (x.NotifyTypeId == NotifyType.All || x.NotifyTypeId == NotifyType.Raid)).Distinct((x) => x.UserId))
                                {
                                    await _client.SendMessageToDMChannel(item.UserId, "記得打日版總力戰!");
                                }
                            }

                            if (_eventDatas.Any((x) => x.RegionType == RegionType.Japan && x.EventType == NotifyType.TimeAttack && x.StartAt <= DateTime.Now && x.EndAt > DateTime.Now))
                            {
                                foreach (var item in db.NotifyConfig.AsNoTracking().Where((x) => x.RegionTypeId == RegionType.Japan && (x.NotifyTypeId == NotifyType.All || x.NotifyTypeId == NotifyType.TimeAttack)).Distinct((x) => x.UserId))
                                {
                                    await _client.SendMessageToDMChannel(item.UserId, "記得打日版協同!");
                                }
                            }

                            // 國際版
                            if (_eventDatas.Any((x) => x.RegionType == RegionType.Global && x.EventType == NotifyType.Event && x.StartAt <= DateTime.Now && x.EndAt > DateTime.Now))
                            {
                                foreach (var item in db.NotifyConfig.AsNoTracking().Where((x) => x.RegionTypeId == RegionType.Global && (x.NotifyTypeId == NotifyType.All || x.NotifyTypeId == NotifyType.Event)).Distinct((x) => x.UserId))
                                {
                                    await _client.SendMessageToDMChannel(item.UserId, "記得打國際版活動!");
                                }
                            }

                            if (_eventDatas.Any((x) => x.RegionType == RegionType.Global && x.EventType == NotifyType.Raid && x.StartAt <= DateTime.Now && x.EndAt > DateTime.Now))
                            {
                                foreach (var item in db.NotifyConfig.AsNoTracking().Where((x) => x.RegionTypeId == RegionType.Global && (x.NotifyTypeId == NotifyType.All || x.NotifyTypeId == NotifyType.Raid)).Distinct((x) => x.UserId))
                                {
                                    await _client.SendMessageToDMChannel(item.UserId, "記得打國際版總力戰!");
                                }
                            }

                            if (_eventDatas.Any((x) => x.RegionType == RegionType.Global && x.EventType == NotifyType.TimeAttack && x.StartAt <= DateTime.Now && x.EndAt > DateTime.Now))
                            {
                                foreach (var item in db.NotifyConfig.AsNoTracking().Where((x) => x.RegionTypeId == RegionType.Global && (x.NotifyTypeId == NotifyType.All || x.NotifyTypeId == NotifyType.TimeAttack)).Distinct((x) => x.UserId))
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

                    await _client.SendMessageToDMChannel(item.UserId, (item.RegionTypeId == RegionType.Japan ? "日版" : "國際版") + $"的咖啡廳邀請券已更新!");

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
                await Program.RedisDb.StringSetAsync(redisKey, json, TimeSpan.FromMinutes(59));
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


        public static async Task SendMessageToDMChannel(this DiscordSocketClient client, ulong userId, string message, string thumbnailUrl)
        {
            try
            {
                var user = await client.Rest.GetUserAsync(userId);
                if (user != null)
                {
                    var channel = await user.CreateDMChannelAsync();
                    await channel.SendMessageAsync(embed: new EmbedBuilder().WithOkColor().WithDescription(message).WithThumbnailUrl(thumbnailUrl).Build());
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