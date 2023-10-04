#nullable disable

namespace DiscordBlueArchiveBot.Interaction.BlueArchive.Service.Json
{
    public class ConfigJson : BaseJson
    {
        [JsonProperty("links")]
        public List<Link> Links { get; set; }

        [JsonProperty("build")]
        public int Build { get; set; }

        [JsonProperty("Regions")]
        public List<Region> Regions { get; set; }

        [JsonProperty("Changelog")]
        public List<Changelog> Changelogs { get; set; }

        [JsonProperty("GachaGroups")]
        public List<GachaGroup> GachaGroups { get; set; }

        public class Changelog
        {
            [JsonProperty("date")]
            public string Date { get; set; }

            [JsonProperty("contents")]
            public List<string> Contents { get; set; }
        }

        public class Content
        {
            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("author")]
            public string Author { get; set; }
        }

        public class CurrentEvent
        {
            [JsonProperty("event")]
            public int Event { get; set; }

            [JsonProperty("start")]
            public int Start { get; set; }

            [JsonProperty("end")]
            public int End { get; set; }
        }

        public class CurrentGacha
        {
            [JsonProperty("characters")]
            public List<int> Characters { get; set; }

            [JsonProperty("start")]
            public int Start { get; set; }

            [JsonProperty("end")]
            public int End { get; set; }
        }

        public class CurrentRaid
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("raid")]
            public int Raid { get; set; }

            [JsonProperty("terrain")]
            public string Terrain { get; set; }

            [JsonProperty("start")]
            public int Start { get; set; }

            [JsonProperty("end")]
            public int End { get; set; }
        }

        public class GachaGroup
        {
            [JsonProperty("Id")]
            public int Id { get; set; }

            [JsonProperty("ItemList")]
            public List<List<int>> ItemList { get; set; }
        }

        public class Link
        {
            [JsonProperty("section")]
            public string Section { get; set; }

            [JsonProperty("content")]
            public List<Content> Content { get; set; }
        }

        public class Region
        {
            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("StudentMaxLevel")]
            public int StudentMaxLevel { get; set; }

            [JsonProperty("WeaponMaxLevel")]
            public int WeaponMaxLevel { get; set; }

            [JsonProperty("BondMaxLevel")]
            public int BondMaxLevel { get; set; }

            [JsonProperty("EquipmentMaxLevel")]
            public List<int> EquipmentMaxLevel { get; set; }

            [JsonProperty("CampaignMax")]
            public int CampaignMax { get; set; }

            [JsonProperty("CampaignExtra")]
            public bool CampaignExtra { get; set; }

            [JsonProperty("Events")]
            public List<int> Events { get; set; }

            [JsonProperty("Event701Max")]
            public List<int> Event701Max { get; set; }

            [JsonProperty("ChaserMax")]
            public int ChaserMax { get; set; }

            [JsonProperty("BloodMax")]
            public int BloodMax { get; set; }

            [JsonProperty("FindGiftMax")]
            public int FindGiftMax { get; set; }

            [JsonProperty("SchoolDungeonMax")]
            public int SchoolDungeonMax { get; set; }

            [JsonProperty("FurnitureSetMax")]
            public int FurnitureSetMax { get; set; }

            [JsonProperty("FurnitureTemplateMax")]
            public int FurnitureTemplateMax { get; set; }

            [JsonProperty("CurrentGacha")]
            public List<CurrentGacha> CurrentGacha { get; set; }

            [JsonProperty("CurrentEvents")]
            public List<CurrentEvent> CurrentEvents { get; set; }

            [JsonProperty("CurrentRaid")]
            public List<CurrentRaid> CurrentRaid { get; set; }
        }
    }
}
