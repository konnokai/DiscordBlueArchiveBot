﻿using Discord.Interactions;
using DiscordBlueArchiveBot.DataBase.Table;
using DiscordBlueArchiveBot.Interaction.Attribute;
using DiscordBlueArchiveBot.Interaction.BlueArchive.Service;
using static DiscordBlueArchiveBot.DataBase.Table.NotifyConfig;

namespace DiscordBlueArchiveBot.Interaction.BlueArchive
{
    public class BlueArchive : TopLevelModule<BlueArchiveService>
    {
        private readonly DiscordSocketClient _client;

        public BlueArchive(DiscordSocketClient client)
        {
            _client = client;
        }

        [SlashCommand("set-notify", "設定要通知的類型")]
        [CommandSummary("若設定 `全部` 通知，則會將除了 `咖啡廳邀請券更新` 外的全部通知都設定\n" +
            "`咖啡廳邀請券更新` 採單獨通知設計，設定後 20 小時會通知一次並移除本通知，需重新設定")]
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
                        await channel.SendMessageAsync("這是測試用的訊息，僅會在第一次設定通知的時候出現，用來確定是否能發送通知\n" +
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
                    case NotifyType.Double:
                    case NotifyType.Raid:
                    case NotifyType.UnionOperation:
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
                    case NotifyType.CafeInviteTicketUpdate:
                        {
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
                            break;
                        }
                }
            }
        }
    }
}