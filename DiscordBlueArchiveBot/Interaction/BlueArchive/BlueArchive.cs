using Discord.Interactions;
using DiscordBlueArchiveBot.DataBase.Table;
using DiscordBlueArchiveBot.Interaction.Attribute;
using DiscordBlueArchiveBot.Interaction.BlueArchive.Service;
using DiscordBlueArchiveBot.Interaction.BlueArchive.Service.Json;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Diagnostics;
using System.Security.Cryptography;
using static DiscordBlueArchiveBot.DataBase.Table.NotifyConfig;

namespace DiscordBlueArchiveBot.Interaction.BlueArchive
{
    public class BlueArchive : TopLevelModule<BlueArchiveService>
    {
        public class NotifyConfigAutocompleteHandler : AutocompleteHandler
        {
            public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
            {
                using var db = DataBase.MainDbContext.GetDbContext();
                IQueryable<NotifyConfig> notifyConfigs;

                if (!db.NotifyConfig.AsNoTracking().Any((x) => x.UserId == autocompleteInteraction.User.Id))
                    return AutocompletionResult.FromSuccess();

                notifyConfigs = db.NotifyConfig.AsNoTracking().Where((x) => x.UserId == autocompleteInteraction.User.Id);

                try
                {
                    List<AutocompleteResult> results = new();
                    foreach (var item in notifyConfigs)
                    {
                        results.Add(new AutocompleteResult($"({item.RegionTypeId}) {item.NotifyTypeId}", $"{(int)item.RegionTypeId}:{(int)item.NotifyTypeId}"));
                    }
                    return AutocompletionResult.FromSuccess(results.Take(25));
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                    return AutocompletionResult.FromSuccess();
                }
            }
        }

        public class CafeInviteTicketUpdateConfigAutocompleteHandler : AutocompleteHandler
        {
            public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
            {
                try
                {
                    using var db = DataBase.MainDbContext.GetDbContext();
                    IQueryable<CafeInviteTicketUpdateTime> cafeInviteTicketUpdateTimes;

                    if (!db.CafeInviteTicketUpdateTime.AsNoTracking().Any((x) => x.UserId == autocompleteInteraction.User.Id))
                        return AutocompletionResult.FromSuccess();

                    cafeInviteTicketUpdateTimes = db.CafeInviteTicketUpdateTime.AsNoTracking().Where((x) => x.UserId == autocompleteInteraction.User.Id);

                    List<AutocompleteResult> results = new();
                    foreach (var item in cafeInviteTicketUpdateTimes)
                    {
                        results.Add(new AutocompleteResult($"{item.RegionTypeId}", (int)item.RegionTypeId));
                    }
                    return AutocompletionResult.FromSuccess(results.Take(5));
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                    return AutocompletionResult.FromSuccess();
                }
            }
        }

        [SlashCommand("set-notify", "設定通知")]
        [CommandSummary("若設定 `全部` 通知，則會將除了 `咖啡廳邀請券更新` 外的全部通知都設定")]
        public async Task SetNotify([Summary("遊戲版本")] RegionType regionType, [Summary("通知類型")] NotifyType notifyType)
        {
            await DeferAsync(true);

            using (var db = DataBase.MainDbContext.GetDbContext())
            {
                if (!db.NotifyConfig.Any((x) => x.UserId == Context.User.Id) && !db.CafeInviteTicketUpdateTime.Any((x) => x.UserId == Context.User.Id))
                {
                    try
                    {
                        var channel = await Context.User.CreateDMChannelAsync();
                        await channel.SendMessageAsync("這是測試用的訊息，用來確定是否能發送通知\n" +
                            "請勿關閉你與機器人任意共通伺服器的 `私人訊息` 設定，避免未來無法接收機器人的訊息");
                        await channel.CloseAsync();
                    }
                    catch (Discord.Net.HttpException discordEx) when (discordEx.DiscordCode == DiscordErrorCode.MissingPermissions)
                    {
                        await Context.Interaction.SendErrorAsync("無法發送私訊，請至本伺服器的 `隱私設定` 中開啟 `私人訊息`", true);
                        return;
                    }
                }

                switch (notifyType)
                {
                    case NotifyType.All:
                        {
                            if (db.NotifyConfig.Any((x) => x.UserId == Context.User.Id && x.RegionTypeId == regionType))
                            {
                                if (await PromptUserConfirmAsync("設定全部會將此遊戲版本設定過的其他定時通知給移除，是否繼續?"))
                                {
                                    db.NotifyConfig.RemoveRange(db.NotifyConfig.Where((x) => x.UserId == Context.User.Id && x.RegionTypeId == regionType));
                                }
                                else
                                {
                                    return;
                                }
                            }

                            db.NotifyConfig.Add(new NotifyConfig() { UserId = Context.User.Id, NotifyTypeId = notifyType, RegionTypeId = regionType });
                            await db.SaveChangesAsync();

                            await Context.Interaction.SendConfirmAsync($"已設定通知", true, true);
                            break;
                        }
                    case NotifyType.Event:
                    case NotifyType.Raid:
                    case NotifyType.TimeAttack:
                    case NotifyType.NightLogin:
                    case NotifyType.PVPAward:
                    case NotifyType.CafeInterviewChange:
                        {
                            NotifyConfig? notifyConfig = db.NotifyConfig.SingleOrDefault((x) => x.UserId == Context.User.Id && x.NotifyTypeId == NotifyType.All && x.RegionTypeId == regionType);
                            if (notifyConfig != null)
                            {
                                if (await PromptUserConfirmAsync("已設定全部通知，是否取消全部通知並設定此類型的通知?"))
                                {
                                    db.NotifyConfig.Remove(notifyConfig);
                                }
                                else
                                {
                                    return;
                                }
                            }
                            else if (db.NotifyConfig.Any((x) => x.UserId == Context.User.Id && x.NotifyTypeId == notifyType && x.RegionTypeId == regionType))
                            {
                                await Context.Interaction.SendErrorAsync("你已設定此通知", true, true);
                                return;
                            }

                            db.NotifyConfig.Add(new NotifyConfig() { UserId = Context.User.Id, NotifyTypeId = notifyType, RegionTypeId = regionType });
                            await db.SaveChangesAsync();

                            await Context.Interaction.SendConfirmAsync($"已設定通知", true, true);
                            break;
                        }
                }
            }
        }

        [SlashCommand("cancel-notify", "取消通知")]
        public async Task CancelNotify([Summary("通知設定"), Autocomplete(typeof(NotifyConfigAutocompleteHandler))] string config)
        {
            await DeferAsync(true);

            using (var db = DataBase.MainDbContext.GetDbContext())
            {
                string[] array = config.Split(':');
                RegionType regionType = (RegionType)int.Parse(array[0]);
                NotifyType notifyType = (NotifyType)int.Parse(array[1]);
                db.NotifyConfig.Remove(db.NotifyConfig.Single((x) => x.RegionTypeId == regionType && x.NotifyTypeId == notifyType));
                await db.SaveChangesAsync();

                await Context.Interaction.SendConfirmAsync("已移除", true);
            }
        }

        [SlashCommand("set-ticket-update-notify", "設定咖啡廳邀請券更新通知")]
        [CommandSummary("`咖啡廳邀請券更新` 採單獨通知設計，設定後 20 小時會通知一次並移除本通知，需重新設定")]
        public async Task SetCafeInviteTicketUpdateNotify([Summary("遊戲版本")] RegionType regionType)
        {
            await DeferAsync(true);

            using (var db = DataBase.MainDbContext.GetDbContext())
            {
                if (!db.NotifyConfig.Any((x) => x.UserId == Context.User.Id) && !db.CafeInviteTicketUpdateTime.Any((x) => x.UserId == Context.User.Id))
                {
                    try
                    {
                        var channel = await Context.User.CreateDMChannelAsync();
                        await channel.SendMessageAsync("這是測試用的訊息，用來確定是否能發送通知\n" +
                            "請勿關閉你與機器人任意共通伺服器的 `私人訊息` 設定，避免未來無法接收機器人的訊息");
                        await channel.CloseAsync();
                    }
                    catch (Discord.Net.HttpException discordEx) when (discordEx.DiscordCode == DiscordErrorCode.MissingPermissions)
                    {
                        await Context.Interaction.SendErrorAsync("無法發送私訊，請至本伺服器的 `隱私設定` 中開啟 `私人訊息`", true);
                        return;
                    }
                }

                CafeInviteTicketUpdateTime? cafeInviteTicketUpdateTime;
                if ((cafeInviteTicketUpdateTime = db.CafeInviteTicketUpdateTime.SingleOrDefault((x) => x.UserId == Context.User.Id && x.RegionTypeId == regionType)) != null)
                {
                    if (await PromptUserConfirmAsync("你已設定此通知，要更新時間嗎?"))
                    {
                        cafeInviteTicketUpdateTime.NotifyDateTime = DateTime.Now.AddHours(20);
                        db.CafeInviteTicketUpdateTime.Update(cafeInviteTicketUpdateTime);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    db.CafeInviteTicketUpdateTime.Add(new CafeInviteTicketUpdateTime() { UserId = Context.User.Id, RegionTypeId = regionType, NotifyDateTime = DateTime.Now.AddHours(20) });
                }

                await db.SaveChangesAsync();

                await Context.Interaction.SendConfirmAsync($"已設定通知", true, true);
            }
        }

        [SlashCommand("cancel-ticket-update-notify", "取消咖啡廳邀請券更新通知")]
        public async Task CancelCafeInviteTicketUpdateNotify([Summary("通知設定"), Autocomplete(typeof(CafeInviteTicketUpdateConfigAutocompleteHandler))] int region)
        {
            await DeferAsync(true);

            using (var db = DataBase.MainDbContext.GetDbContext())
            {
                db.CafeInviteTicketUpdateTime.Remove(db.CafeInviteTicketUpdateTime.Single((x) => x.RegionTypeId == (RegionType)region));
                await db.SaveChangesAsync();

                await Context.Interaction.SendConfirmAsync("已移除", true);
            }
        }

        [SlashCommand("roll", "隨機十抽")]
        public async Task Roll([Summary("伺服器版本", "未輸入則使用日版資料")] RegionType regionType = RegionType.Japan)
        {
            if (_service.IsRefreshingData)
            {
                await Context.Interaction.SendErrorAsync("資料尚未初始化，請稍後再試");
                return;
            }

            if (await Program.RedisDb.KeyExistsAsync($"bluearchive:gachaExpire:{Context.User.Id}"))
            {
                var rollExpireTime = await Program.RedisDb.KeyExpireTimeAsync($"bluearchive:gachaExpire:{Context.User.Id}");
                await Context.Interaction.SendErrorAsync($"還在冷卻，剩餘時間: {rollExpireTime.Value.AddHours(8).Subtract(DateTime.Now):hh\\時mm\\分ss\\秒}");
                return;
            }

            List<Student> rollStudentList = new(), pickUpStudentList = regionType == RegionType.Japan ? _service.JPPickUpDatas : _service.GlobalPickUpDatas;
            bool isARONARoll = RandomNumberGenerator.GetInt32(0, 100) + 1 >= 25, isNeedEphemeral = true;

            for (int i = 0; i < 10; i++)
            {
                var rollChance = Math.Round(RandomNumberGenerator.GetInt32(0, 10001) / 100f, 1);
                if (isARONARoll)
                {
                    switch (rollChance)
                    {
                        case <= 78.5: // 1星
                            rollStudentList.Add(GetRandomStudentFromStarGrade(i == 9 ? 2 : 1, regionType));
                            break;
                        case > 78.5 and <= 94: // 二星
                            rollStudentList.Add(GetRandomStudentFromStarGrade(2, regionType));
                            break;
                        case > 94 and <= 98.6: // 三星
                            rollStudentList.Add(GetRandomStudentFromStarGrade(3, regionType));
                            break;
                        case > 98.6: // Pick Up
                            if (pickUpStudentList.Any())
                            {
                                rollStudentList.Add(pickUpStudentList[RandomNumberGenerator.GetInt32(0, pickUpStudentList.Count)]);
                            }
                            else
                            {
                                rollStudentList.Add(GetRandomStudentFromStarGrade(3, regionType));
                            }
                            break;
                    }
                }
                else
                {
                    switch (rollChance)
                    {
                        case <= 78.5: // 1星
                            rollStudentList.Add(GetRandomStudentFromStarGrade(i == 9 ? 2 : 1, regionType));
                            break;
                        case > 78.5 and <= 97: // 二星
                            rollStudentList.Add(GetRandomStudentFromStarGrade(2, regionType));
                            break;
                        case > 97 and <= 99.3: // 三星
                            rollStudentList.Add(GetRandomStudentFromStarGrade(3, regionType));
                            break;
                        case > 99.3: // Pick Up
                            if (pickUpStudentList.Any())
                            {
                                rollStudentList.Add(pickUpStudentList[RandomNumberGenerator.GetInt32(0, pickUpStudentList.Count)]);
                            }
                            else
                            {
                                rollStudentList.Add(GetRandomStudentFromStarGrade(3, regionType));
                            }
                            break;
                    }
                }
            }

            try
            {
                using (var db = DataBase.MainDbContext.GetDbContext())
                {
                    var userGachaRecord = db.UserGachaRecord.SingleOrDefault((x) => x.UserId == Context.User.Id);
                    if (userGachaRecord == null)
                        userGachaRecord = new UserGachaRecord() { UserId = Context.User.Id };

                    userGachaRecord.TotalGachaCount += 10;
                    userGachaRecord.ThreeStarCount += (uint)rollStudentList.Count((x) => x.StarGrade == 3);
                    userGachaRecord.PickUpCount += (uint)rollStudentList.Count((x) => pickUpStudentList.Any((x2) => x.Id == x2.Id));

                    db.UserGachaRecord.Update(userGachaRecord);
                    await db.SaveChangesAsync();

                    await Program.RedisDb.HashSetAsync(new RedisKey("bluearchive:gachaRecord"), new RedisValue(Context.User.Id.ToString()), new RedisValue(JsonConvert.SerializeObject(userGachaRecord)));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Roll - UserGachaRecord 資料庫保存失敗");
            }

            try
            {
                using (var db = DataBase.MainDbContext.GetDbContext())
                {
                    foreach (var item in rollStudentList)
                    {
                        var userGacheCharacterRecord = db.UserGacheCharacterRecord.SingleOrDefault((x) => x.UserId == Context.User.Id && x.CharacterId == item.Id);
                        if (userGacheCharacterRecord == null)
                            userGacheCharacterRecord = new UserGacheCharacterRecord() { UserId = Context.User.Id, CharacterId = item.Id };

                        userGacheCharacterRecord.Num += 1;

                        db.UserGacheCharacterRecord.Update(userGacheCharacterRecord);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Roll - UserGacheCharacterRecord 資料庫保存失敗");
            }

            if (!Debugger.IsAttached)
            {
                try
                {
                    await Program.RedisDb.StringSetAsync(new RedisKey($"bluearchive:gachaExpire:{Context.User.Id}"), new RedisValue("1"), TimeSpan.FromHours(1));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Roll - Redis 抽卡過期時間設定失敗");
                }
            }

            if (rollStudentList.Any((x) => x.StarGrade == 3))
                isNeedEphemeral = false;

            string backgroundUrl;
            if (!isNeedEphemeral && Math.Round(RandomNumberGenerator.GetInt32(0, 10001) / 100f, 1) >= 1) // 有 1% 的機率是藍背景
            {
                backgroundUrl = "https://static.wikia.nocookie.net/blue-archive/images/d/db/Gacha_-_Rainbow_2.png";
            }
            else
            {
                backgroundUrl = "https://static.wikia.nocookie.net/blue-archive/images/8/8a/Gacha_-_Blue_2.png";
            }

            var eb = new EmbedBuilder().WithOkColor()
                .WithFooter("僅供娛樂，模擬抽卡並不會跟遊戲結果一致，如有疑問建議換帳號重新開局")
                .WithDescription(isARONARoll ? "黑奈出現，機率加倍!" : "")
                .WithImageUrl(backgroundUrl);

            var cb = new ComponentBuilder()
                .WithButton("簽名開牌!", "roll:" + (regionType == RegionType.Japan ? "0" : "1") + $":{Context.User.Id}:{string.Join('_', rollStudentList.Select((x) => x.Id))}", ButtonStyle.Primary);

            await RespondAsync(embed: eb.Build(), components: cb.Build(), ephemeral: isNeedEphemeral);
        }

        private Student GetRandomStudentFromStarGrade(int starGrade, RegionType regionType)
        {
            List<Student> tempStudentList = _service.Students
                .Where((x) =>
                    x.StarGrade == starGrade &&
                    !_service.EventStudents.Any((x2) => x.Id == x2) &&
                    x.IsLimited == 0 &&
                    (regionType == RegionType.Japan || x.IsReleased.Last()))
                .ToList();

            return tempStudentList[RandomNumberGenerator.GetInt32(0, tempStudentList.Count)];
        }
    }
}