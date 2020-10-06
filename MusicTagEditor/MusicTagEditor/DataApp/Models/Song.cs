using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicTagEditor.DataApp.Models
{
    public class Song
    {
        public int Id { get; set; }

        public string Name  { get; set; }

        public List<Artist> Artists { get; set; }

        public string Comment { get; set; }

        public List<Compositor> Compositors { get; set; }
        public int Disc { get; set; }

        public string FrameModel { get; set; }
        public List<Genre> Genres { get; set; }

        public float Length { get; set; }
        public string Lyrics { get; set; }

        public byte[] PictureData { get; set; }

        public Album Album { get; set; }

        public int Track { get; set; }

    }
}
