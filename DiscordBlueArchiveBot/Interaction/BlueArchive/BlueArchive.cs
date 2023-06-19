using Discord.Interactions;
using DiscordBlueArchiveBot.DataBase.Table;
using DiscordBlueArchiveBot.Interaction.Attribute;
using DiscordBlueArchiveBot.Interaction.BlueArchive.Service;
using DiscordBlueArchiveBot.Interaction.BlueArchive.Service.Json;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using static DiscordBlueArchiveBot.DataBase.Table.NotifyConfig;
using Color = SixLabors.ImageSharp.Color;
using Image = SixLabors.ImageSharp.Image;
using Point = SixLabors.ImageSharp.Point;

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

            List<Student> rollStudentList = new(), tempStudentList, pickUpStudentList = regionType == RegionType.Japan ? _service.JPPickUpDatas : _service.GlobalPickUpDatas;

            var random = new Random();
            bool isARONARoll = random.Next(0, 100) >= 25;

            for (int i = 0; i < 10; i++)
            {
                var rollChance = Math.Round(random.NextDouble() * 100, 1);
                if (isARONARoll)
                {
                    switch (rollChance)
                    {
                        case <= 78.5: // 1星
                            int guarantStarGrade = i == 9 ? 2 : 1;
                            tempStudentList = _service.Students.Where((x) => x.StarGrade == guarantStarGrade && !_service.EventStudents.Any((x2) => x.Id!.Value == x2) && x.IsLimited == 0).ToList();
                            rollStudentList.Add(tempStudentList[random.Next(0, tempStudentList.Count - 1)]);
                            break;
                        case > 78.5 and <= 94: // 二星
                            tempStudentList = _service.Students.Where((x) => x.StarGrade == 2 && !_service.EventStudents.Any((x2) => x.Id!.Value == x2) && x.IsLimited == 0).ToList();
                            rollStudentList.Add(tempStudentList[random.Next(0, tempStudentList.Count - 1)]);
                            break;
                        case > 94 and <= 98.6: // 三星
                            tempStudentList = _service.Students.Where((x) => x.StarGrade == 3 && !_service.EventStudents.Any((x2) => x.Id!.Value == x2) && x.IsLimited == 0).ToList();
                            rollStudentList.Add(tempStudentList[random.Next(0, tempStudentList.Count - 1)]);
                            break;
                        case > 98.6: // Pick Up
                            rollStudentList.Add(pickUpStudentList[random.Next(0, pickUpStudentList.Count - 1)]);
                            break;
                    }
                }
                else
                {
                    switch (rollChance)
                    {
                        case <= 78.5: // 1星
                            int guarantStarGrade = i == 9 ? 2 : 1;
                            tempStudentList = _service.Students.Where((x) => x.StarGrade == guarantStarGrade && !_service.EventStudents.Any((x2) => x.Id!.Value == x2) && x.IsLimited == 0).ToList();
                            rollStudentList.Add(tempStudentList[random.Next(0, tempStudentList.Count - 1)]);
                            break;
                        case > 78.5 and <= 97: // 二星
                            tempStudentList = _service.Students.Where((x) => x.StarGrade == 2 && !_service.EventStudents.Any((x2) => x.Id!.Value == x2) && x.IsLimited == 0).ToList();
                            rollStudentList.Add(tempStudentList[random.Next(0, tempStudentList.Count - 1)]);
                            break;
                        case > 97 and <= 99.3: // 三星
                            tempStudentList = _service.Students.Where((x) => x.StarGrade == 3 && !_service.EventStudents.Any((x2) => x.Id!.Value == x2) && x.IsLimited == 0).ToList();
                            rollStudentList.Add(tempStudentList[random.Next(0, tempStudentList.Count - 1)]);
                            break;
                        case > 99.3: // Pick Up
                            rollStudentList.Add(pickUpStudentList[random.Next(0, pickUpStudentList.Count - 1)]);
                            break;
                    }
                }
            }

            string des = isARONARoll ? "黑奈出現，機率加倍!\n" : "";
            if (rollStudentList.Any((x) => x.StarGrade == 3))
                //Todo: 這個改成針對單一使用者紀錄
                des += $"十抽出三星機率: {Math.Round((rollStudentList.Count((x) => x.StarGrade == 3) / (double)10) * 100, 1)}%";
            else
                des += $"本次十抽沒有出彩...";

            var eb = new EmbedBuilder().WithOkColor()
                .WithFooter("僅供娛樂，模擬抽卡並不會跟遊戲結果一致，如有疑問建議換帳號重新開局")
                .WithDescription(des)
                .WithImageUrl($"attachment://image.jpg");

            try
            {
                Color[] colors =
                {
                    Color.Red, Color.Yellow, Color.Green, Color.Blue, Color.Purple,
                    Color.Red, Color.Yellow, Color.Green, Color.Blue, Color.Purple
                };

                using var memoryStream = new MemoryStream();
                using Image image = Image.Load(Properties.Resources.Event_Main_Stage_Bg.AsSpan());

                for (int i = 0; i < rollStudentList.Count; i++)
                {
                    var item = rollStudentList[i];
                    using (var img = Image.Load(_service.GetStudentAvatarPath(item.Id!.Value)))
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

                            //.AddField("星級", string.Join('\\', Enumerable.Range(1, item.StarGrade!.Value).Select((x) => "★")), i % 5 != 0);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, i.ToString());
                        }
                    }
                }

                image.Save(memoryStream, new JpegEncoder());
                await RespondWithFileAsync(memoryStream, "image.jpg", embed: eb.Build());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Draw Error");
            }
        }
    }
}