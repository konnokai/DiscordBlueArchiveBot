#nullable disable

namespace DiscordBlueArchiveBot.Interaction.BlueArchive.Service.Json
{
    public class StagesJson : IJson
    {
        public string Name { get; private set; }
        public bool Localization { get; private set; }
        public Action<string> DeserializeAction { get; private set; } = null;

        public StagesJson()
        {
            Name = GetType().Name.Replace("Json", "").ToLower();
            Localization = false;
        }

        [JsonProperty("Campaign")]
        public List<Campaign> Campaigns { get; set; }

        [JsonProperty("Event")]
        public List<Event> Events { get; set; }

        [JsonProperty("WeekDungeon")]
        public List<WeekDungeon> WeekDungeons { get; set; }

        [JsonProperty("SchoolDungeon")]
        public List<SchoolDungeon> SchoolDungeons { get; set; }

        [JsonProperty("Conquest")]
        public List<Conquest> Conquests { get; set; }

        [JsonProperty("ConquestMap")]
        public List<ConquestMap> ConquestMaps { get; set; }

        public class Campaign
        {
            [JsonProperty("Id")]
            public int Id { get; set; }

            [JsonProperty("Difficulty")]
            public int Difficulty { get; set; }

            [JsonProperty("Area")]
            public int Area { get; set; }

            [JsonProperty("Stage")]
            public object Stage { get; set; }

            [JsonProperty("NameEn")]
            public string NameEn { get; set; }

            [JsonProperty("NameJp")]
            public string NameJp { get; set; }

            [JsonProperty("NameKr")]
            public string NameKr { get; set; }

            [JsonProperty("NameTw")]
            public string NameTw { get; set; }

            [JsonProperty("NameCn")]
            public string NameCn { get; set; }

            [JsonProperty("NameTh")]
            public string NameTh { get; set; }

            [JsonProperty("NameVi")]
            public string NameVi { get; set; }

            [JsonProperty("EntryCost")]
            public List<List<int>> EntryCost { get; set; }

            [JsonProperty("StarCondition")]
            public List<int> StarCondition { get; set; }

            [JsonProperty("ChallengeCondition")]
            public List<List<object>> ChallengeCondition { get; set; }

            [JsonProperty("Terrain")]
            public string Terrain { get; set; }

            [JsonProperty("Level")]
            public int Level { get; set; }

            [JsonProperty("Rewards")]
            public Rewards Rewards { get; set; }

            [JsonProperty("Formations")]
            public List<Formation> Formations { get; set; }

            [JsonProperty("HexaMap")]
            public List<HexaMap> HexaMap { get; set; }

            [JsonProperty("RewardsGlobal")]
            public RewardsGlobal RewardsGlobal { get; set; }
        }

        public class Conquest
        {
            [JsonProperty("Id")]
            public int Id { get; set; }

            [JsonProperty("NameEn")]
            public string NameEn { get; set; }

            [JsonProperty("NameJp")]
            public string NameJp { get; set; }

            [JsonProperty("NameKr")]
            public string NameKr { get; set; }

            [JsonProperty("NameTw")]
            public string NameTw { get; set; }

            [JsonProperty("NameCn")]
            public string NameCn { get; set; }

            [JsonProperty("NameTh")]
            public string NameTh { get; set; }

            [JsonProperty("NameVi")]
            public string NameVi { get; set; }

            [JsonProperty("EventId")]
            public int EventId { get; set; }

            [JsonProperty("Formations")]
            public List<Formation> Formations { get; set; }

            [JsonProperty("Difficulty")]
            public string Difficulty { get; set; }

            [JsonProperty("Level")]
            public int Level { get; set; }

            [JsonProperty("EnemyType")]
            public string EnemyType { get; set; }

            [JsonProperty("Terrain")]
            public string Terrain { get; set; }

            [JsonProperty("Step")]
            public int Step { get; set; }

            [JsonProperty("Team")]
            public string Team { get; set; }

            [JsonProperty("SubStage")]
            public bool SubStage { get; set; }

            [JsonProperty("StarCondition")]
            public List<int> StarCondition { get; set; }

            [JsonProperty("EntryCost")]
            public List<List<int>> EntryCost { get; set; }

            [JsonProperty("SchoolBuff")]
            public List<List<string>> SchoolBuff { get; set; }

            [JsonProperty("Rewards")]
            public Rewards Rewards { get; set; }
        }

        public class ConquestMap
        {
            [JsonProperty("EventId")]
            public int EventId { get; set; }

            [JsonProperty("Maps")]
            public List<Map> Maps { get; set; }
        }

        public class Event
        {
            [JsonProperty("Id")]
            public int Id { get; set; }

            [JsonProperty("EventId")]
            public int EventId { get; set; }

            [JsonProperty("Difficulty")]
            public int Difficulty { get; set; }

            [JsonProperty("Stage")]
            public string Stage { get; set; }

            [JsonProperty("NameEn")]
            public string NameEn { get; set; }

            [JsonProperty("NameJp")]
            public string NameJp { get; set; }

            [JsonProperty("NameKr")]
            public string NameKr { get; set; }

            [JsonProperty("NameTw")]
            public string NameTw { get; set; }

            [JsonProperty("NameCn")]
            public string NameCn { get; set; }

            [JsonProperty("NameTh")]
            public string NameTh { get; set; }

            [JsonProperty("NameVi")]
            public string NameVi { get; set; }

            [JsonProperty("EntryCost")]
            public List<List<int>> EntryCost { get; set; }

            [JsonProperty("StarCondition")]
            public List<int> StarCondition { get; set; }

            [JsonProperty("ChallengeCondition")]
            public List<List<object>> ChallengeCondition { get; set; }

            [JsonProperty("Terrain")]
            public string Terrain { get; set; }

            [JsonProperty("Level")]
            public int Level { get; set; }

            [JsonProperty("Rewards")]
            public Rewards Rewards { get; set; }

            [JsonProperty("RewardsGlobal")]
            public RewardsGlobal RewardsGlobal { get; set; }

            [JsonProperty("Formations")]
            public List<Formation> Formations { get; set; }

            [JsonProperty("HexaMap")]
            public List<HexaMap> HexaMap { get; set; }
        }

        public class Formation
        {
            [JsonProperty("Id")]
            public int Id { get; set; }

            [JsonProperty("MapIcon")]
            public string MapIcon { get; set; }

            [JsonProperty("MoveType")]
            public string MoveType { get; set; }

            [JsonProperty("UnitGrade")]
            public string UnitGrade { get; set; }

            [JsonProperty("Level")]
            public List<int> Level { get; set; }

            [JsonProperty("Grade")]
            public List<int> Grade { get; set; }

            [JsonProperty("EnemyList")]
            public List<int> EnemyList { get; set; }
        }

        public class HexaMap
        {
            [JsonProperty("Type")]
            public string Type { get; set; }

            [JsonProperty("Pos")]
            public List<int> Pos { get; set; }

            [JsonProperty("Entity")]
            public int Entity { get; set; }

            [JsonProperty("Target")]
            public int? Target { get; set; }

            [JsonProperty("Trigger")]
            public int? Trigger { get; set; }
        }

        public class Map
        {
            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Step")]
            public int Step { get; set; }

            [JsonProperty("Difficulty")]
            public string Difficulty { get; set; }

            [JsonProperty("Tiles")]
            public List<Tile> Tiles { get; set; }
        }

        public class Rewards
        {
            [JsonProperty("Default")]
            public List<List<double>> Default { get; set; }

            [JsonProperty("FirstClear")]
            public List<List<int>> FirstClear { get; set; }

            [JsonProperty("ThreeStar")]
            public List<List<int>> ThreeStar { get; set; }

            [JsonProperty("Calculate")]
            public List<List<List<int>>> Calculate { get; set; }
        }

        public class RewardsGlobal
        {
            [JsonProperty("Default")]
            public List<List<double>> Default { get; set; }

            [JsonProperty("FirstClear")]
            public List<List<int>> FirstClear { get; set; }

            [JsonProperty("ThreeStar")]
            public List<List<int>> ThreeStar { get; set; }
        }

        public class SchoolDungeon
        {
            [JsonProperty("Id")]
            public int Id { get; set; }

            [JsonProperty("Type")]
            public string Type { get; set; }

            [JsonProperty("Stage")]
            public int Stage { get; set; }

            [JsonProperty("EntryCost")]
            public List<List<int>> EntryCost { get; set; }

            [JsonProperty("StarCondition")]
            public List<int> StarCondition { get; set; }

            [JsonProperty("Terrain")]
            public string Terrain { get; set; }

            [JsonProperty("Level")]
            public int Level { get; set; }

            [JsonProperty("Rewards")]
            public Rewards Rewards { get; set; }

            [JsonProperty("Formations")]
            public List<Formation> Formations { get; set; }
        }

        public class Tile
        {
            [JsonProperty("Id")]
            public object Id { get; set; }

            [JsonProperty("Type")]
            public string Type { get; set; }

            [JsonProperty("Pos")]
            public List<int> Pos { get; set; }

            [JsonProperty("StageId")]
            public int StageId { get; set; }
        }

        public class WeekDungeon
        {
            [JsonProperty("Id")]
            public int Id { get; set; }

            [JsonProperty("Type")]
            public string Type { get; set; }

            [JsonProperty("Stage")]
            public int Stage { get; set; }

            [JsonProperty("EntryCost")]
            public List<List<int>> EntryCost { get; set; }

            [JsonProperty("StarCondition")]
            public List<int> StarCondition { get; set; }

            [JsonProperty("Terrain")]
            public string Terrain { get; set; }

            [JsonProperty("Level")]
            public int Level { get; set; }

            [JsonProperty("Rewards")]
            public Rewards Rewards { get; set; }

            [JsonProperty("RewardsGlobal")]
            public RewardsGlobal RewardsGlobal { get; set; }

            [JsonProperty("Formations")]
            public List<Formation> Formations { get; set; }
        }
    }
}
