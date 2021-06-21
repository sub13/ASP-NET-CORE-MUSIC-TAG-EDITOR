using System.ComponentModel.DataAnnotations;

namespace MusicTagEditor.Data.Models
{
    public class Album
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int Year { get; set; }
    }
}
