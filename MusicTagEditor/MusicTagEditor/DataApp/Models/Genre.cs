using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MusicTagEditor.DataApp.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

    }
}
