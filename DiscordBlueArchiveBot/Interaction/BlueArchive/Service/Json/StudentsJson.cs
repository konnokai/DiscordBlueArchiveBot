#nullable disable

namespace DiscordBlueArchiveBot.Interaction.BlueArchive.Service.Json
{
    public class StudentsJson : IJson
    {
        public string Name { get; private set; }
        public bool Localization { get; private set; }
        public Action<string> DeserializeAction { get; private set; }

        public StudentsJson()
        {
            Name = GetType().Name.Replace("Json", "").ToLower();
            Localization = true;
            DeserializeAction = new Action<string>((str) => Data = JsonConvert.DeserializeObject<List<Student>>(str));
        }

        public List<Student> Data { get; private set; }
    }

    public class Student
    {

        [JsonProperty("Id")]
        public int? Id { get; set; }

        [JsonProperty("IsReleased")]
        public List<bool?> IsReleased { get; set; }

        [JsonProperty("DefaultOrder")]
        public int? DefaultOrder { get; set; }

        [JsonProperty("PathName")]
        public string PathName { get; set; }

        [JsonProperty("DevName")]
        public string DevName { get; set; }

        [JsonProperty("Name")]
        public string StudentName { get; set; }

        [JsonProperty("School")]
        public string School { get; set; }

        [JsonProperty("Club")]
        public string Club { get; set; }

        [JsonProperty("StarGrade")]
        public int? StarGrade { get; set; }

        [JsonProperty("SquadType")]
        public string SquadType { get; set; }

        [JsonProperty("TacticRole")]
        public string TacticRole { get; set; }

        [JsonProperty("Summons")]
        public List<Summon> Summons { get; set; }

        [JsonProperty("Position")]
        public string Position { get; set; }

        [JsonProperty("BulletType")]
        public string BulletType { get; set; }

        [JsonProperty("ArmorType")]
        public string ArmorType { get; set; }

        [JsonProperty("StreetBattleAdaptation")]
        public int? StreetBattleAdaptation { get; set; }

        [JsonProperty("OutdoorBattleAdaptation")]
        public int? OutdoorBattleAdaptation { get; set; }

        [JsonProperty("IndoorBattleAdaptation")]
        public int? IndoorBattleAdaptation { get; set; }

        [JsonProperty("WeaponType")]
        public string WeaponType { get; set; }

        [JsonProperty("WeaponImg")]
        public string WeaponImg { get; set; }

        [JsonProperty("Cover")]
        public bool? Cover { get; set; }

        [JsonProperty("Equipment")]
        public List<string> Equipment { get; set; }

        [JsonProperty("CollectionBG")]
        public string CollectionBG { get; set; }

        [JsonProperty("CollectionTexture")]
        public string CollectionTexture { get; set; }

        [JsonProperty("FamilyName")]
        public string FamilyName { get; set; }

        [JsonProperty("FamilyNameRuby")]
        public string FamilyNameRuby { get; set; }

        [JsonProperty("PersonalName")]
        public string PersonalName { get; set; }

        [JsonProperty("SchoolYear")]
        public string SchoolYear { get; set; }

        [JsonProperty("CharacterAge")]
        public string CharacterAge { get; set; }

        [JsonProperty("Birthday")]
        public string Birthday { get; set; }

        [JsonProperty("CharacterSSRNew")]
        public string CharacterSSRNew { get; set; }

        [JsonProperty("ProfileIntroduction")]
        public string ProfileIntroduction { get; set; }

        [JsonProperty("Hobby")]
        public string Hobby { get; set; }

        [JsonProperty("CharacterVoice")]
        public string CharacterVoice { get; set; }

        [JsonProperty("BirthDay")]
        public string BirthDay { get; set; }

        [JsonProperty("Illustrator")]
        public string Illustrator { get; set; }

        [JsonProperty("Designer")]
        public string Designer { get; set; }

        [JsonProperty("CharHeightMetric")]
        public string CharHeightMetric { get; set; }

        [JsonProperty("CharHeightImperial")]
        public string CharHeightImperial { get; set; }

        [JsonProperty("StabilityPoint")]
        public int? StabilityPoint { get; set; }

        [JsonProperty("AttackPower1")]
        public int? AttackPower1 { get; set; }

        [JsonProperty("AttackPower100")]
        public int? AttackPower100 { get; set; }

        [JsonProperty("MaxHP1")]
        public int? MaxHP1 { get; set; }

        [JsonProperty("MaxHP100")]
        public int? MaxHP100 { get; set; }

        [JsonProperty("DefensePower1")]
        public int? DefensePower1 { get; set; }

        [JsonProperty("DefensePower100")]
        public int? DefensePower100 { get; set; }

        [JsonProperty("HealPower1")]
        public int? HealPower1 { get; set; }

        [JsonProperty("HealPower100")]
        public int? HealPower100 { get; set; }

        [JsonProperty("DodgePoint")]
        public int? DodgePoint { get; set; }

        [JsonProperty("AccuracyPoint")]
        public int? AccuracyPoint { get; set; }

        [JsonProperty("CriticalPoint")]
        public int? CriticalPoint { get; set; }

        [JsonProperty("CriticalDamageRate")]
        public int? CriticalDamageRate { get; set; }

        [JsonProperty("AmmoCount")]
        public int? AmmoCount { get; set; }

        [JsonProperty("AmmoCost")]
        public int? AmmoCost { get; set; }

        [JsonProperty("Range")]
        public int? Range { get; set; }

        [JsonProperty("RegenCost")]
        public int? RegenCost { get; set; }

        [JsonProperty("Skills")]
        public List<Skill> Skills { get; set; }

        [JsonProperty("FavorStatType")]
        public List<string> FavorStatType { get; set; }

        [JsonProperty("FavorStatValue")]
        public List<List<int?>> FavorStatValue { get; set; }

        [JsonProperty("FavorAlts")]
        public List<int?> FavorAlts { get; set; }

        [JsonProperty("MemoryLobby")]
        public List<int?> MemoryLobby { get; set; }

        [JsonProperty("MemoryLobbyBGM")]
        public string MemoryLobbyBGM { get; set; }

        [JsonProperty("FurnitureInteraction")]
        public List<List<int?>> FurnitureInteraction { get; set; }

        [JsonProperty("FavorItemTags")]
        public List<string> FavorItemTags { get; set; }

        [JsonProperty("FavorItemUniqueTags")]
        public List<string> FavorItemUniqueTags { get; set; }

        [JsonProperty("IsLimited")]
        public int? IsLimited { get; set; }

        [JsonProperty("Weapon")]
        public Weapon Weapon { get; set; }

        [JsonProperty("Gear")]
        public Gear Gear { get; set; }

        [JsonProperty("SkillExMaterial")]
        public List<List<int?>> SkillExMaterial { get; set; }

        [JsonProperty("SkillExMaterialAmount")]
        public List<List<int?>> SkillExMaterialAmount { get; set; }

        [JsonProperty("SkillMaterial")]
        public List<List<int?>> SkillMaterial { get; set; }

        [JsonProperty("SkillMaterialAmount")]
        public List<List<int?>> SkillMaterialAmount { get; set; }
    }

    public class Effect
    {
        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Hits")]
        public List<int?> Hits { get; set; }

        [JsonProperty("Scale")]
        public List<int?> Scale { get; set; }

        [JsonProperty("Frames")]
        public Frames Frames { get; set; }

        [JsonProperty("CriticalCheck")]
        public string CriticalCheck { get; set; }

        [JsonProperty("Stat")]
        public string Stat { get; set; }

        [JsonProperty("Value")]
        public List<List<int?>> Value { get; set; }

        [JsonProperty("Channel")]
        public int? Channel { get; set; }

        [JsonProperty("Duration")]
        public string Duration { get; set; }

        [JsonProperty("Period")]
        public string Period { get; set; }

        [JsonProperty("HitsParameter")]
        public int? HitsParameter { get; set; }

        [JsonProperty("Chance")]
        public string Chance { get; set; }

        [JsonProperty("Icon")]
        public string Icon { get; set; }

        [JsonProperty("SubstituteCondition")]
        public string SubstituteCondition { get; set; }

        [JsonProperty("SubstituteScale")]
        public List<int?> SubstituteScale { get; set; }

        [JsonProperty("HitFrames")]
        public List<int?> HitFrames { get; set; }

        [JsonProperty("StackSame")]
        public int? StackSame { get; set; }

        [JsonProperty("IgnoreDef")]
        public List<int?> IgnoreDef { get; set; }

        [JsonProperty("Restrictions")]
        public List<Restriction> Restrictions { get; set; }

        [JsonProperty("ZoneHitInterval")]
        public int? ZoneHitInterval { get; set; }

        [JsonProperty("ZoneDuration")]
        public int? ZoneDuration { get; set; }

        [JsonProperty("Critical")]
        public int? Critical { get; set; }

        [JsonProperty("HideFormChangeIcon")]
        public bool? HideFormChangeIcon { get; set; }

        [JsonProperty("SourceStat")]
        public string SourceStat { get; set; }

        [JsonProperty("ExtraDamageSource")]
        public ExtraDamageSource ExtraDamageSource { get; set; }
    }

    public class EffectCombineLabel
    {
        [JsonProperty("Icon")]
        public List<string> Icon { get; set; }

        [JsonProperty("StackLabelTranslated")]
        public List<string> StackLabelTranslated { get; set; }

        [JsonProperty("DisableFirst")]
        public bool? DisableFirst { get; set; }

        [JsonProperty("StackLabel")]
        public List<string> StackLabel { get; set; }
    }

    public class ExtraDamageSource
    {
        [JsonProperty("Side")]
        public string Side { get; set; }

        [JsonProperty("Stat")]
        public string Stat { get; set; }

        [JsonProperty("Multiplier")]
        public List<int?> Multiplier { get; set; }

        [JsonProperty("SliderTranslation")]
        public string SliderTranslation { get; set; }

        [JsonProperty("SliderStep")]
        public List<double?> SliderStep { get; set; }

        [JsonProperty("SliderLabel")]
        public List<int?> SliderLabel { get; set; }

        [JsonProperty("SliderLabelSuffix")]
        public string SliderLabelSuffix { get; set; }

        [JsonProperty("SimulatePerHit")]
        public bool? SimulatePerHit { get; set; }
    }

    public class Frames
    {
        [JsonProperty("AttackEnterDuration")]
        public int? AttackEnterDuration { get; set; }

        [JsonProperty("AttackStartDuration")]
        public int? AttackStartDuration { get; set; }

        [JsonProperty("AttackEndDuration")]
        public int? AttackEndDuration { get; set; }

        [JsonProperty("AttackBurstRoundOverDelay")]
        public int? AttackBurstRoundOverDelay { get; set; }

        [JsonProperty("AttackIngDuration")]
        public int? AttackIngDuration { get; set; }

        [JsonProperty("AttackReloadDuration")]
        public int? AttackReloadDuration { get; set; }

        [JsonProperty("AttackReadyStartDuration")]
        public int? AttackReadyStartDuration { get; set; }

        [JsonProperty("AttackReadyEndDuration")]
        public int? AttackReadyEndDuration { get; set; }
    }

    public class Gear
    {
        [JsonProperty("Released")]
        public List<bool?> Released { get; set; }

        [JsonProperty("StatType")]
        public List<string> StatType { get; set; }

        [JsonProperty("StatValue")]
        public List<List<int?>> StatValue { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Desc")]
        public string Desc { get; set; }

        [JsonProperty("Icon")]
        public string Icon { get; set; }

        [JsonProperty("TierUpMaterial")]
        public List<List<int?>> TierUpMaterial { get; set; }

        [JsonProperty("TierUpMaterialAmount")]
        public List<List<int?>> TierUpMaterialAmount { get; set; }
    }

    public class Restriction
    {
        [JsonProperty("Property")]
        public string Property { get; set; }

        [JsonProperty("Operand")]
        public string Operand { get; set; }

        [JsonProperty("Value")]
        public object Value { get; set; }
    }


    public class Skill
    {
        [JsonProperty("SkillType")]
        public string SkillType { get; set; }

        [JsonProperty("Effects")]
        public List<Effect> Effects { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Desc")]
        public string Desc { get; set; }

        [JsonProperty("Parameters")]
        public List<List<string>> Parameters { get; set; }

        [JsonProperty("Cost")]
        public List<int?> Cost { get; set; }

        [JsonProperty("Icon")]
        public string Icon { get; set; }

        [JsonProperty("EffectCombine")]
        public List<string> EffectCombine { get; set; }

        [JsonProperty("EffectCombineLabel")]
        public EffectCombineLabel EffectCombineLabel { get; set; }
    }

    public class Summon
    {
        [JsonProperty("Id")]
        public int? Id { get; set; }

        [JsonProperty("SourceSkill")]
        public string SourceSkill { get; set; }

        [JsonProperty("InheritCasterStat")]
        public List<string> InheritCasterStat { get; set; }

        [JsonProperty("InheritCasterAmount")]
        public List<List<int?>> InheritCasterAmount { get; set; }
    }

    public class Weapon
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Desc")]
        public string Desc { get; set; }

        [JsonProperty("AdaptationType")]
        public string AdaptationType { get; set; }

        [JsonProperty("AdaptationValue")]
        public int? AdaptationValue { get; set; }

        [JsonProperty("AttackPower1")]
        public int? AttackPower1 { get; set; }

        [JsonProperty("AttackPower100")]
        public int? AttackPower100 { get; set; }

        [JsonProperty("MaxHP1")]
        public int? MaxHP1 { get; set; }

        [JsonProperty("MaxHP100")]
        public int? MaxHP100 { get; set; }

        [JsonProperty("HealPower1")]
        public int? HealPower1 { get; set; }

        [JsonProperty("HealPower100")]
        public int? HealPower100 { get; set; }

        [JsonProperty("StatLevelUpType")]
        public string StatLevelUpType { get; set; }
    }
}