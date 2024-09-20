#nullable disable

using static DiscordBlueArchiveBot.Interaction.BlueArchive.Service.Json.StagesJson;

namespace DiscordBlueArchiveBot.Interaction.BlueArchive.Service.Json
{
    public class RaidsJson : BaseJson
    {
        public RaidsJson()
        {
            Localization = true;
        }

        [JsonProperty("Raid")]
        public List<Raid> Raids { get; set; }

        [JsonProperty("TimeAttack")]
        public List<TimeAttack> TimeAttacks { get; set; }

        [JsonProperty("WorldRaid")]
        public List<WorldRaid> WorldRaids { get; set; }

        public class Raid
        {
            [JsonProperty("Id")]
            public int Id { get; set; }

            [JsonProperty("IsReleased")]
            public List<bool> IsReleased { get; set; }

            [JsonProperty("MaxDifficulty")]
            public List<int> MaxDifficulty { get; set; }

            [JsonProperty("PathName")]
            public string PathName { get; set; }

            [JsonProperty("Terrain")]
            public List<string> Terrain { get; set; }

            [JsonProperty("BulletType")]
            public string BulletType { get; set; }

            [JsonProperty("BulletTypeInsane")]
            public string BulletTypeInsane { get; set; }

            [JsonProperty("ArmorType")]
            public string ArmorType { get; set; }

            [JsonProperty("EnemyList")]
            public List<List<int>> EnemyList { get; set; }

            [JsonProperty("HasNormalAttack")]
            public List<int> HasNormalAttack { get; set; }

            [JsonProperty("BattleDuration")]
            public List<int> BattleDuration { get; set; }

            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("DevName")]
            public string DevName { get; set; }

            [JsonProperty("Icon")]
            public string Icon { get; set; }

            [JsonProperty("IconBG")]
            public string IconBG { get; set; }
        }

        public class TimeAttack
        {
            [JsonProperty("Id")]
            public int Id { get; set; }

            [JsonProperty("IsReleased")]
            public List<bool> IsReleased { get; set; }

            [JsonProperty("DungeonType")]
            public string DungeonType { get; set; }

            [JsonProperty("Icon")]
            public string Icon { get; set; }

            [JsonProperty("MaxDifficulty")]
            public List<int> MaxDifficulty { get; set; }

            [JsonProperty("Terrain")]
            public string Terrain { get; set; }

            [JsonProperty("BulletType")]
            public string BulletType { get; set; }

            [JsonProperty("ArmorType")]
            public string ArmorType { get; set; }

            [JsonProperty("Level")]
            public List<int> Level { get; set; }

            [JsonProperty("Formations")]
            public List<Formation> Formations { get; set; }

            [JsonProperty("BattleDuration")]
            public List<int> BattleDuration { get; set; }
        }

        public class WorldRaid
        {
            [JsonProperty("Id")]
            public int Id { get; set; }

            [JsonProperty("IsReleased")]
            public List<bool> IsReleased { get; set; }

            [JsonProperty("MaxDifficulty")]
            public List<int> MaxDifficulty { get; set; }

            [JsonProperty("DifficultyName")]
            public List<string> DifficultyName { get; set; }

            [JsonProperty("PathName")]
            public string PathName { get; set; }

            [JsonProperty("IconBG")]
            public string IconBG { get; set; }

            [JsonProperty("Terrain")]
            public List<string> Terrain { get; set; }

            [JsonProperty("BulletType")]
            public string BulletType { get; set; }

            [JsonProperty("ArmorType")]
            public string ArmorType { get; set; }

            [JsonProperty("WorldBossHP")]
            public object WorldBossHP { get; set; }

            [JsonProperty("Level")]
            public List<int> Level { get; set; }

            [JsonProperty("EnemyList")]
            public List<List<int>> EnemyList { get; set; }

            [JsonProperty("HasNormalAttack")]
            public List<object> HasNormalAttack { get; set; }

            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("EntryCost")]
            public List<List<int>> EntryCost { get; set; }

            [JsonProperty("BattleDuration")]
            public List<int> BattleDuration { get; set; }

            [JsonProperty("DevName")]
            public string DevName { get; set; }

            [JsonProperty("BulletTypeInsane")]
            public string BulletTypeInsane { get; set; }

            [JsonProperty("UseRaidSkillList")]
            public int? UseRaidSkillList { get; set; }
        }
    }
}
