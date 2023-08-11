#nullable disable

using Discord.Interactions;
using DiscordBlueArchiveBot.HttpClients;
using DiscordBlueArchiveBot.Interaction;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DiscordBlueArchiveBot
{
    internal class Program
    {
        public static string VERSION => GetLinkerTime(Assembly.GetEntryAssembly());
        public static ConnectionMultiplexer Redis { get; set; }
        public static ISubscriber RedisSub { get; set; }
        public static IDatabase RedisDb { get; set; }

        public static IUser ApplicatonOwner { get; private set; } = null;
        public static DiscordSocketClient _client;
        public static BotPlayingStatus Status = BotPlayingStatus.Guild;
        public static Stopwatch stopWatch = new Stopwatch();
        public static bool isConnect = false, isDisconnect = false, isNeedRegisterAppCommand = false;
        static Timer timerUpdateStatus;
        static readonly BotConfig botConfig = new();
        public enum BotPlayingStatus { Guild, Member, RollInfo, Info }

        static async Task Main(string[] args)
        {
            stopWatch.Start();

            Log.Info(VERSION + " 初始化中");
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CancelKeyPress += Console_CancelKeyPress;

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Exception e = (Exception)args.ExceptionObject;
                Log.Error(e, "UnhandledException!!!!!!!!!!!!!!!!!!!!!");
            };

            botConfig.InitBotConfig();
            timerUpdateStatus = new Timer(TimerHandler);

            if (!Directory.Exists(Path.GetDirectoryName(GetDataFilePath(""))))
                Directory.CreateDirectory(Path.GetDirectoryName(GetDataFilePath(""))!);

            using (var db = new DataBase.MainDbContext())
            {
                if (!File.Exists(GetDataFilePath("DataBase.db")))
                {
                    db.Database.EnsureCreated();
                }
            }

            try
            {
                RedisConnection.Init(botConfig.RedisOption);
                Redis = RedisConnection.Instance.ConnectionMultiplexer;
                RedisSub = Redis.GetSubscriber();
                RedisDb = Redis.GetDatabase();

                Log.Info("Redis已連線");
            }
            catch (Exception ex)
            {
                Log.Error("Redis連線錯誤，請確認伺服器是否已開啟");
                Log.Error(ex.Message);
                return;
            }

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                ConnectionTimeout = int.MaxValue,
                MessageCacheSize = 50,
                // 因為沒有註冊事件，Discord .NET建議可移除這兩個沒用到的特權
                // https://dotblogs.com.tw/yc421206/2015/10/20/c_scharp_enum_of_flags
                GatewayIntents = GatewayIntents.AllUnprivileged & ~GatewayIntents.GuildInvites & ~GatewayIntents.GuildScheduledEvents,
                AlwaysDownloadDefaultStickers = false,
                AlwaysResolveStickers = false,
                FormatUsersInBidirectionalUnicode = false,
                LogGatewayIntentWarnings = false,
            });

            #region 初始化Discord設定與事件
            _client.Log += Log.LogMsg;

            _client.Ready += async () =>
            {
                stopWatch.Start();
                timerUpdateStatus!.Change(0, 15 * 60 * 1000);

                ApplicatonOwner = (await _client.GetApplicationInfoAsync()).Owner;
                isConnect = true;
            };
            #endregion

#if DEBUG || RELEASE
            Log.Info("登入中...");
            await _client.LoginAsync(TokenType.Bot, botConfig.DiscordToken);
            await _client.StartAsync();

            do { await Task.Delay(200); }
            while (!isConnect);

            Log.Info("登入成功!");

            UptimeKumaClient.Init(botConfig.UptimeKumaPushUrl, _client);
#endif

            #region 初始化互動指令系統
            var interactionServices = new ServiceCollection()
                .AddHttpClient()
                .AddSingleton(_client)
                .AddSingleton(botConfig)
                .AddSingleton(new InteractionService(_client, new InteractionServiceConfig()
                {
                    AutoServiceScopes = true,
                    UseCompiledLambda = true,
                    EnableAutocompleteHandlers = true,
                    DefaultRunMode = RunMode.Async,
                    ExitOnMissingModalField = true,
                }));

            interactionServices.AddHttpClient<DiscordWebhookClient>();

            interactionServices.LoadInteractionFrom(Assembly.GetAssembly(typeof(InteractionHandler)));
            IServiceProvider iService = interactionServices.BuildServiceProvider();
            await iService.GetService<InteractionHandler>().InitializeAsync();
            #endregion

#if DEBUG_API
            do { await Task.Delay(200); }
            while (!isDisconnect);

            return;
#endif
            #region 註冊互動指令
            try
            {
                int commandCount = 0;

                if (File.Exists(GetDataFilePath("CommandCount.bin")))
                    commandCount = BitConverter.ToInt32(File.ReadAllBytes(GetDataFilePath("CommandCount.bin")));

                if (commandCount != iService.GetService<InteractionHandler>().CommandCount)
                {
                    try
                    {
                        InteractionService interactionService = iService.GetService<InteractionService>();
#if DEBUG
                        if (botConfig.TestSlashCommandGuildId == 0 || _client.GetGuild(botConfig.TestSlashCommandGuildId) == null)
                            Log.Warn("未設定測試Slash指令的伺服器或伺服器不存在，略過");
                        else
                        {
                            try
                            {
                                var result = await interactionService.RegisterCommandsToGuildAsync(botConfig.TestSlashCommandGuildId);
                                Log.Info($"已註冊指令 ({botConfig.TestSlashCommandGuildId}): {string.Join(", ", result.Select((x) => x.Name))}");

                                result = await interactionService.AddModulesToGuildAsync(botConfig.TestSlashCommandGuildId, false, interactionService.Modules.Where((x) => x.DontAutoRegister).ToArray());
                                Log.Info($"已註冊指令 ({botConfig.TestSlashCommandGuildId}): {string.Join(", ", result.Select((x) => x.Name))}");
                            }
                            catch (Exception ex)
                            {
                                Log.Error("註冊伺服器專用Slash指令失敗");
                                Log.Error(ex.ToString());
                            }
                        }
#else
                        try
                        {
                            if (botConfig.TestSlashCommandGuildId != 0 && _client.GetGuild(botConfig.TestSlashCommandGuildId) != null)
                            {
                                var result = await interactionService.RemoveModulesFromGuildAsync(botConfig.TestSlashCommandGuildId, interactionService.Modules.Where((x) => !x.DontAutoRegister).ToArray());
                                Log.Info($"({botConfig.TestSlashCommandGuildId}) 已移除測試指令，剩餘指令: {string.Join(", ", result.Select((x) => x.Name))}");
                            }

                            try
                            {
                                foreach (var item in interactionService.Modules.Where((x) => x.Preconditions.Any((x) => x is Interaction.Attribute.RequireGuildAttribute)))
                                {
                                    var guildId = ((Interaction.Attribute.RequireGuildAttribute)item.Preconditions.FirstOrDefault((x) => x is Interaction.Attribute.RequireGuildAttribute)).GuildId;
                                    var guild = _client.GetGuild(guildId.Value);

                                    if (guild == null)
                                    {
                                        Log.Warn($"{item.Name} 註冊失敗，伺服器 {guildId} 不存在");
                                        continue;
                                    }

                                    var result = await interactionService.AddModulesToGuildAsync(guild, false, item);
                                    Log.Info($"已在 {guild.Name}({guild.Id}) 註冊指令: {string.Join(", ", item.SlashCommands.Select((x) => x.Name))}");
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, "註冊伺服器專用Slash指令失敗");
                            }

                            await iService.GetService<InteractionService>().RegisterCommandsGloballyAsync();
                            Log.Info("已註冊全球指令");
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "設定指令數量失敗，請確認檔案是否正常");
                            if (File.Exists(GetDataFilePath("CommandCount.bin")))
                                File.Delete(GetDataFilePath("CommandCount.bin"));

                            isDisconnect = true;
                            return;
                        }
#endif
                        File.WriteAllBytes(GetDataFilePath("CommandCount.bin"), BitConverter.GetBytes(iService.GetService<InteractionHandler>().CommandCount));
                    }
                    catch (Exception ex)
                    {
                        Log.Error("註冊Slash指令失敗，關閉中...");
                        Log.Error(ex.ToString());
                        isDisconnect = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "取得指令數量失敗");
                isDisconnect = true;
            }
            #endregion

            _client.JoinedGuild += (guild) =>
            {
                iService.GetService<DiscordWebhookClient>().SendMessageToDiscord($"加入 {guild.Name}({guild.Id})\n擁有者: {guild.OwnerId}");
                return Task.CompletedTask;
            };

            Log.Info("已初始化完成!");

            do { await Task.Delay(1000); }
            while (!isDisconnect);

            await _client.StopAsync();
        }

        private static void TimerHandler(object state)
        {
            if (isDisconnect) return;

            ChangeStatus();
        }

        public static void ChangeStatus()
        {
            Task.Run(async () =>
            {
                switch (Status)
                {
                    case BotPlayingStatus.Guild:
                        await _client.SetCustomStatusAsync($"在 {_client.Guilds.Count} 個伺服器");
                        Status = BotPlayingStatus.Member;
                        break;
                    case BotPlayingStatus.Member:
                        try
                        {
                            await _client.SetCustomStatusAsync($"服務 {_client.Guilds.Sum((x) => x.MemberCount)} 個成員");
                            Status = BotPlayingStatus.RollInfo;
                        }
                        catch (Exception) { Status = BotPlayingStatus.RollInfo; ChangeStatus(); }
                        break;
                    case BotPlayingStatus.RollInfo:
                        using (var db = DataBase.MainDbContext.GetDbContext())
                        {
                            var totalGachaCount = db.UserGachaRecord.Sum(x => x.TotalGachaCount);
                            var totalThreeStarCount = db.UserGachaRecord.Sum(x => x.ThreeStarCount);
                            var totalPickUpCount = db.UserGachaRecord.Sum(x => x.PickUpCount);

                            double threeStartPercentage = totalThreeStarCount == 0 ? 0 : Math.Round((double)totalThreeStarCount / totalGachaCount * 100, 2);
                            double pickUpPercentage = totalPickUpCount == 0 ? 0 : Math.Round((double)totalPickUpCount / totalGachaCount * 100, 2);

                            await _client.SetCustomStatusAsync($"總抽卡人數: {db.UserGachaRecord.Count()}, " +
                                $"總抽卡次數: {totalGachaCount}, " +
                                $"三星率: {threeStartPercentage}%, " +
                                $"PickUp率: {pickUpPercentage}%");
                        }
                        Status = BotPlayingStatus.Info;
                        break;
                    case BotPlayingStatus.Info:
                        await _client.SetCustomStatusAsync("去玩檔案啦");
                        Status = BotPlayingStatus.Guild;
                        break;
                }
            });
        }

        public static string GetDataFilePath(string fileName)
            => $"{AppDomain.CurrentDomain.BaseDirectory}Data{GetPlatformSlash()}{fileName}";

        public static string GetPlatformSlash()
            => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "\\" : "/";

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            isDisconnect = true;
            e.Cancel = true;
        }

        public static string GetLinkerTime(Assembly assembly)
        {
            const string BuildVersionMetadataPrefix = "+build";

            var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attribute?.InformationalVersion != null)
            {
                var value = attribute.InformationalVersion;
                var index = value.IndexOf(BuildVersionMetadataPrefix);
                if (index > 0)
                {
                    value = value[(index + BuildVersionMetadataPrefix.Length)..];
                    return value;
                }
            }
            return "";
        }
    }
}