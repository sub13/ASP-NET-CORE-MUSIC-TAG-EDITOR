using Microsoft.AspNetCore.Http;

namespace MusicTagEditor.ViewModels.Menu
{
    public class SongViewModel
    {
        public int Id { get; set; }

        public string Name  { get; set; }

        public string Artists { get; set; }

        public string Comment { get; set; }

        public string Compositors { get; set; }
        public int Disc { get; set; }

        public string FrameModel { get; set; }
        public string Genres { get; set; }

        public float Length { get; set; }
        public string Lyrics { get; set; }

        public IFormFile mainSongImage { get; set; }

        public string Album { get; set; }

        public int Track { get; set; }
        public string nameFileSong { get; set; }

        public string Year { get; set; }
    }
}
