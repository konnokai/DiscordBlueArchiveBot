#nullable disable

namespace DiscordBlueArchiveBot.Interaction.BlueArchive.Service.Json
{
    public class CommonJson : IJson
    {
        public string Name { get; private set; }
        public bool Localization { get; private set; }
        public Action<string> DeserializeAction { get; private set; } = null;

        public CommonJson()
        {
            Name = GetType().Name.Replace("Json", "").ToLower();
            Localization = false;
        }

        [JsonProperty("GachaGroup")]
        public List<GachaGroup> GachaGroups { get; set; }

        [JsonProperty("regions")]
        public List<Region> Regions { get; set; }

        public class CurrentEvent
        {
            [JsonProperty("event")]
            public int? Event { get; set; }

            [JsonProperty("start")]
            public int? Start { get; set; }

            [JsonProperty("end")]
            public int? End { get; set; }
        }

        public class CurrentGacha
        {
            [JsonProperty("characters")]
            public List<int?> Characters { get; set; }

            [JsonProperty("start")]
            public int? Start { get; set; }

            [JsonProperty("end")]
            public int? End { get; set; }
        }

        public class CurrentRaid
        {
            [JsonProperty("raid")]
            public int? Raid { get; set; }

            [JsonProperty("terrain")]
            public string Terrain { get; set; }

            [JsonProperty("start")]
            public int? Start { get; set; }

            [JsonProperty("end")]
            public int? End { get; set; }
        }

        public class GachaGroup
        {
            [JsonProperty("Id")]
            public int? Id { get; set; }

            [JsonProperty("ItemList")]
            public List<List<int?>> ItemList { get; set; }
        }

        public class Region
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("studentlevel_max")]
            public int? StudentlevelMax { get; set; }

            [JsonProperty("weaponlevel_max")]
            public int? WeaponlevelMax { get; set; }

            [JsonProperty("bondlevel_max")]
            public int? BondlevelMax { get; set; }

            [JsonProperty("gear1_max")]
            public int? Gear1Max { get; set; }

            [JsonProperty("gear2_max")]
            public int? Gear2Max { get; set; }

            [JsonProperty("gear3_max")]
            public int? Gear3Max { get; set; }

            [JsonProperty("campaign_max")]
            public int? CampaignMax { get; set; }

            [JsonProperty("events")]
            public List<int?> Events { get; set; }

            [JsonProperty("event_701_max")]
            public int? Event701Max { get; set; }

            [JsonProperty("event_701_challenge_max")]
            public int? Event701ChallengeMax { get; set; }

            [JsonProperty("commission_max")]
            public int? CommissionMax { get; set; }

            [JsonProperty("bounty_max")]
            public int? BountyMax { get; set; }

            [JsonProperty("schooldungeon_max")]
            public int? SchooldungeonMax { get; set; }

            [JsonProperty("current_gacha")]
            public List<CurrentGacha> CurrentGacha { get; set; }

            [JsonProperty("current_events")]
            public List<CurrentEvent> CurrentEvents { get; set; }

            [JsonProperty("current_raid")]
            public List<CurrentRaid> CurrentRaid { get; set; }
        }
    }
}
