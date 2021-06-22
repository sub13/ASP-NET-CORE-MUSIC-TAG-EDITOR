using System.ComponentModel.DataAnnotations;

namespace MusicTagEditor.DataApp.Models
{
    public class Compositor
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
