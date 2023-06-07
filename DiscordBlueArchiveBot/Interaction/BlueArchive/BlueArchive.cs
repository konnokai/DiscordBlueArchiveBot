using Discord.Interactions;
using DiscordBlueArchiveBot.Interaction.BlueArchive.Service;
using static DiscordBlueArchiveBot.DataBase.Table.ChannelConfig;

namespace DiscordBlueArchiveBot.Interaction.BlueArchive
{
    public class BlueArchive : TopLevelModule<BlueArchiveService>
    {
        private readonly DiscordSocketClient _client;
        public BlueArchive(DiscordSocketClient client)
        {
            _client = client;
        }

        [RequireBotPermission(ChannelPermission.SendMessages | ChannelPermission.EmbedLinks | ChannelPermission.MentionEveryone)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [SlashCommand("set-notify-message", "設定通知的訊息 (可以標記用戶組)")]
        public async Task SetNotifyRole(NotifyType notifyType, string message)
        {
            await Context.Interaction.SendConfirmAsync($"{notifyType}: {message}");
        }
    }
}
