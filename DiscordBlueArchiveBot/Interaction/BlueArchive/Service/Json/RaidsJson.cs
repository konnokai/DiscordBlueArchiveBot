#nullable disable

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

        [JsonProperty("RaidSeasons")]
        public List<object> RaidSeasons { get; set; }

        [JsonProperty("TimeAttack")]
        public List<TimeAttack> TimeAttacks { get; set; }

        [JsonProperty("TimeAttackRules")]
        public List<TimeAttackRule> TimeAttackRules { get; set; }

        [JsonProperty("WorldRaid")]
        public List<WorldRaid> WorldRaids { get; set; }

        public class Effect
        {
            [JsonProperty("Type")]
            public string Type { get; set; }

            [JsonProperty("Scale")]
            public List<object> Scale { get; set; }

            [JsonProperty("RestrictTo")]
            public List<int?> RestrictTo { get; set; }

            [JsonProperty("CriticalCheck")]
            public string CriticalCheck { get; set; }

            [JsonProperty("CanEvade")]
            public bool? CanEvade { get; set; }

            [JsonProperty("HitFrames")]
            public List<int?> HitFrames { get; set; }

            [JsonProperty("Hits")]
            public List<int?> Hits { get; set; }

            [JsonProperty("Value")]
            public List<List<int?>> Value { get; set; }

            [JsonProperty("Stat")]
            public string Stat { get; set; }

            [JsonProperty("Channel")]
            public int? Channel { get; set; }

            [JsonProperty("CombineGroup")]
            public int? CombineGroup { get; set; }

            [JsonProperty("Chance")]
            public string Chance { get; set; }

            [JsonProperty("Icon")]
            public string Icon { get; set; }

            [JsonProperty("SubstituteCondition")]
            public string SubstituteCondition { get; set; }

            [JsonProperty("SubstituteScale")]
            public List<List<int?>> SubstituteScale { get; set; }

            [JsonProperty("Duration")]
            public string Duration { get; set; }

            [JsonProperty("Period")]
            public string Period { get; set; }

            [JsonProperty("StackSame")]
            public int? StackSame { get; set; }

            [JsonProperty("StackingIcon")]
            public List<string> StackingIcon { get; set; }
        }

        public class EffectCombineLabel
        {
            [JsonProperty("StackLabel")]
            public List<string> StackLabel { get; set; }

            [JsonProperty("StackLabelTranslated")]
            public List<string> StackLabelTranslated { get; set; }

            [JsonProperty("Icon")]
            public List<string> Icon { get; set; }

            [JsonProperty("DisableFirst")]
            public bool? DisableFirst { get; set; }
        }

        public class EffectStackLabel
        {
            [JsonProperty("Label")]
            public List<string> Label { get; set; }

            [JsonProperty("Icon")]
            public List<string> Icon { get; set; }
        }

        public class Formation
        {
            [JsonProperty("Id")]
            public int? Id { get; set; }

            [JsonProperty("Level")]
            public List<int?> Level { get; set; }

            [JsonProperty("Grade")]
            public List<int?> Grade { get; set; }

            [JsonProperty("EnemyList")]
            public List<int?> EnemyList { get; set; }
        }

        public class Raid
        {
            [JsonProperty("Id")]
            public int? Id { get; set; }

            [JsonProperty("IsReleased")]
            public List<bool?> IsReleased { get; set; }

            [JsonProperty("MaxDifficulty")]
            public List<int?> MaxDifficulty { get; set; }

            [JsonProperty("PathName")]
            public string PathName { get; set; }

            [JsonProperty("GroupName")]
            public string GroupName { get; set; }

            [JsonProperty("Faction")]
            public string Faction { get; set; }

            [JsonProperty("Terrain")]
            public List<string> Terrain { get; set; }

            [JsonProperty("BulletType")]
            public string BulletType { get; set; }

            [JsonProperty("BulletTypeInsane")]
            public string BulletTypeInsane { get; set; }

            [JsonProperty("ArmorType")]
            public string ArmorType { get; set; }

            [JsonProperty("EnemyList")]
            public List<List<int?>> EnemyList { get; set; }

            [JsonProperty("RaidSkill")]
            public List<RaidSkill> RaidSkill { get; set; }

            [JsonProperty("HasNormalAttack")]
            public List<int?> HasNormalAttack { get; set; }

            [JsonProperty("BattleDuration")]
            public List<int?> BattleDuration { get; set; }

            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Profile")]
            public string Profile { get; set; }

            [JsonProperty("Icon")]
            public string Icon { get; set; }

            [JsonProperty("IconBG")]
            public string IconBG { get; set; }
        }

        public class RaidSkill
        {
            [JsonProperty("Id")]
            public string Id { get; set; }

            [JsonProperty("SkillType")]
            public string SkillType { get; set; }

            [JsonProperty("MinDifficulty")]
            public int? MinDifficulty { get; set; }

            [JsonProperty("ATGCost")]
            public int? ATGCost { get; set; }

            [JsonProperty("Icon")]
            public string Icon { get; set; }

            [JsonProperty("ShowInfo")]
            public bool? ShowInfo { get; set; }

            [JsonProperty("Effects")]
            public List<Effect> Effects { get; set; }

            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Desc")]
            public string Desc { get; set; }

            [JsonProperty("Parameters")]
            public List<List<string>> Parameters { get; set; }

            [JsonProperty("MaxDifficulty")]
            public int? MaxDifficulty { get; set; }

            [JsonProperty("EffectCombine")]
            public List<string> EffectCombine { get; set; }

            [JsonProperty("EffectCombineLabel")]
            public EffectCombineLabel EffectCombineLabel { get; set; }

            [JsonProperty("EffectStackLabel")]
            public EffectStackLabel EffectStackLabel { get; set; }
        }

        public class Reward
        {
            [JsonProperty("Items")]
            public List<List<double?>> Items { get; set; }

            [JsonProperty("Groups")]
            public List<List<object>> Groups { get; set; }
        }

        public class RewardsGlobal
        {
            [JsonProperty("Items")]
            public List<List<double?>> Items { get; set; }

            [JsonProperty("Groups")]
            public List<List<object>> Groups { get; set; }
        }

        public class TimeAttack
        {
            [JsonProperty("Id")]
            public int? Id { get; set; }

            [JsonProperty("IsReleased")]
            public List<bool?> IsReleased { get; set; }

            [JsonProperty("DungeonType")]
            public string DungeonType { get; set; }

            [JsonProperty("Icon")]
            public string Icon { get; set; }

            [JsonProperty("MaxDifficulty")]
            public int? MaxDifficulty { get; set; }

            [JsonProperty("Terrain")]
            public string Terrain { get; set; }

            [JsonProperty("BulletType")]
            public string BulletType { get; set; }

            [JsonProperty("ArmorType")]
            public string ArmorType { get; set; }

            [JsonProperty("EnemyLevel")]
            public List<int?> EnemyLevel { get; set; }

            [JsonProperty("Formations")]
            public List<Formation> Formations { get; set; }

            [JsonProperty("Rules")]
            public List<List<object>> Rules { get; set; }

            [JsonProperty("BattleDuration")]
            public List<int?> BattleDuration { get; set; }
        }

        public class TimeAttackRule
        {
            [JsonProperty("Id")]
            public object Id { get; set; }

            [JsonProperty("Icon")]
            public string Icon { get; set; }

            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Desc")]
            public string Desc { get; set; }
        }

        public class WorldRaid
        {
            [JsonProperty("Id")]
            public int? Id { get; set; }

            [JsonProperty("IsReleased")]
            public List<bool?> IsReleased { get; set; }

            [JsonProperty("DifficultyMax")]
            public List<int?> DifficultyMax { get; set; }

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
            public List<int?> Level { get; set; }

            [JsonProperty("EnemyList")]
            public List<List<int?>> EnemyList { get; set; }

            [JsonProperty("RaidSkill")]
            public List<RaidSkill> RaidSkill { get; set; }

            [JsonProperty("HasNormalAttack")]
            public List<object> HasNormalAttack { get; set; }

            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Rewards")]
            public List<Reward> Rewards { get; set; }

            [JsonProperty("EntryCost")]
            public List<List<int?>> EntryCost { get; set; }

            [JsonProperty("BattleDuration")]
            public List<int?> BattleDuration { get; set; }

            [JsonProperty("RewardsGlobal")]
            public List<RewardsGlobal> RewardsGlobal { get; set; }

            [JsonProperty("BulletTypeInsane")]
            public string BulletTypeInsane { get; set; }

            [JsonProperty("UseRaidSkillList")]
            public int? UseRaidSkillList { get; set; }
        }
    }
}
