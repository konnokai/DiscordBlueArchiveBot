﻿using Discord.Interactions;
using DiscordBlueArchiveBot.DataBase.Table;
using DiscordBlueArchiveBot.Interaction.Attribute;
using DiscordBlueArchiveBot.Interaction.BlueArchive.Service;
using Microsoft.EntityFrameworkCore;
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
    }
}