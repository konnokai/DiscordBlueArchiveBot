#nullable disable

namespace DiscordBlueArchiveBot.Interaction.BlueArchive.Service.Json
{
    public class StudentsJson : BaseJson
    {
        public StudentsJson()
        {
            Localization = true;
            DeserializeAction = new Action<string>((str) => Data = JsonConvert.DeserializeObject<Dictionary<int, Student>>(str));
        }

        public Dictionary<int, Student> Data { get; private set; }
    }

    public class Student
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("IsReleased")]
        public List<bool> IsReleased { get; set; }

        [JsonProperty("DefaultOrder")]
        public int DefaultOrder { get; set; }

        [JsonProperty("PathName")]
        public string PathName { get; set; }

        [JsonProperty("DevName")]
        public string DevName { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Icon")]
        public string Icon { get; set; }

        [JsonProperty("SearchTags")]
        public List<object> SearchTags { get; set; }

        [JsonProperty("School")]
        public string School { get; set; }

        [JsonProperty("Club")]
        public string Club { get; set; }

        [JsonProperty("StarGrade")]
        public int StarGrade { get; set; }

        [JsonProperty("SquadType")]
        public string SquadType { get; set; }

        [JsonProperty("TacticRole")]
        public string TacticRole { get; set; }

        [JsonProperty("Summons")]
        public List<object> Summons { get; set; }

        [JsonProperty("Position")]
        public string Position { get; set; }

        [JsonProperty("BulletType")]
        public string BulletType { get; set; }

        [JsonProperty("ArmorType")]
        public string ArmorType { get; set; }

        [JsonProperty("StreetBattleAdaptation")]
        public int StreetBattleAdaptation { get; set; }

        [JsonProperty("OutdoorBattleAdaptation")]
        public int OutdoorBattleAdaptation { get; set; }

        [JsonProperty("IndoorBattleAdaptation")]
        public int IndoorBattleAdaptation { get; set; }

        [JsonProperty("WeaponType")]
        public string WeaponType { get; set; }

        [JsonProperty("WeaponImg")]
        public string WeaponImg { get; set; }

        [JsonProperty("Cover")]
        public bool Cover { get; set; }

        [JsonProperty("Size")]
        public string Size { get; set; }

        [JsonProperty("Equipment")]
        public List<string> Equipment { get; set; }

        [JsonProperty("CollectionBG")]
        public string CollectionBG { get; set; }

        [JsonProperty("FamilyName")]
        public string FamilyName { get; set; }

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
        public int StabilityPoint { get; set; }

        [JsonProperty("AttackPower1")]
        public int AttackPower1 { get; set; }

        [JsonProperty("AttackPower100")]
        public int AttackPower100 { get; set; }

        [JsonProperty("MaxHP1")]
        public int MaxHP1 { get; set; }

        [JsonProperty("MaxHP100")]
        public int MaxHP100 { get; set; }

        [JsonProperty("DefensePower1")]
        public int DefensePower1 { get; set; }

        [JsonProperty("DefensePower100")]
        public int DefensePower100 { get; set; }

        [JsonProperty("HealPower1")]
        public int HealPower1 { get; set; }

        [JsonProperty("HealPower100")]
        public int HealPower100 { get; set; }

        [JsonProperty("DodgePoint")]
        public int DodgePoint { get; set; }

        [JsonProperty("AccuracyPoint")]
        public int AccuracyPoint { get; set; }

        [JsonProperty("CriticalPoint")]
        public int CriticalPoint { get; set; }

        [JsonProperty("CriticalDamageRate")]
        public int CriticalDamageRate { get; set; }

        [JsonProperty("AmmoCount")]
        public int AmmoCount { get; set; }

        [JsonProperty("AmmoCost")]
        public int AmmoCost { get; set; }

        [JsonProperty("Range")]
        public int Range { get; set; }

        [JsonProperty("RegenCost")]
        public int RegenCost { get; set; }

        [JsonProperty("FavorStatType")]
        public List<string> FavorStatType { get; set; }

        [JsonProperty("FavorStatValue")]
        public List<List<int>> FavorStatValue { get; set; }

        [JsonProperty("FavorAlts")]
        public List<int> FavorAlts { get; set; }

        [JsonProperty("MemoryLobby")]
        public List<int> MemoryLobby { get; set; }

        [JsonProperty("MemoryLobbyBGM")]
        public int MemoryLobbyBGM { get; set; }

        [JsonProperty("FurnitureInteraction")]
        public List<List<object>> FurnitureInteraction { get; set; }

        [JsonProperty("FavorItemTags")]
        public List<string> FavorItemTags { get; set; }

        [JsonProperty("FavorItemUniqueTags")]
        public List<string> FavorItemUniqueTags { get; set; }

        [JsonProperty("IsLimited")]
        public int IsLimited { get; set; }


        [JsonProperty("SkillExMaterial")]
        public List<List<int>> SkillExMaterial { get; set; }

        [JsonProperty("SkillExMaterialAmount")]
        public List<List<int>> SkillExMaterialAmount { get; set; }

        [JsonProperty("SkillMaterial")]
        public List<List<int>> SkillMaterial { get; set; }

        [JsonProperty("SkillMaterialAmount")]
        public List<List<int>> SkillMaterialAmount { get; set; }

        [JsonProperty("PotentialMaterial")]
        public int PotentialMaterial { get; set; }
    }
}