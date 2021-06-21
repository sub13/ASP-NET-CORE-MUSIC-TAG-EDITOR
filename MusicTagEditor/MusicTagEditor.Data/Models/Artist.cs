using System.ComponentModel.DataAnnotations;

namespace MusicTagEditor.Data.Models
{
    public class Artist
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
