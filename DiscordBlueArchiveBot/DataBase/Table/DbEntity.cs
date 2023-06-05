using System.ComponentModel.DataAnnotations;

namespace DiscordBlueArchiveBot.DataBase.Table
{
    public class DbEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime? DateAdded { get; set; } = DateTime.UtcNow;
    }
}
