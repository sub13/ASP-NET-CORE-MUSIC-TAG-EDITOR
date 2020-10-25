using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MusicTagEditor.DataApp.Models;
using TagLib;

namespace MusicTagEditor.Controllers
{
    public class MenuController : Controller
    {
        private SongsDbContext db;
        private IWebHostEnvironment _appEnvironment;
        private IHubContext<SendTagHub> _hubContext;
        public MenuController(SongsDbContext context, IWebHostEnvironment appEnvironment, IHubContext<SendTagHub> hubContext)
        {
            db = context;
            _appEnvironment = appEnvironment;
            _hubContext = hubContext;
        }
        public IActionResult General()
        {
            return View();
        }

        [HttpPost]
        [RequestSizeLimit(1000000000)] // 1gb
        public async Task<IActionResult> UploadMusicFiles(IFormFileCollection uploads)
        {
            StringBuilder _pathToUserCurrentDir;
            if (uploads != null)
            {
                string pathDirTemp = @$"{_appEnvironment.WebRootPath}\\TempFiles";
                DirectoryInfo dirTemp = new DirectoryInfo(pathDirTemp);
                DirectoryInfo _userDir = dirTemp.CreateSubdirectory(User.Identity.Name);
                _pathToUserCurrentDir = new StringBuilder($@"{pathDirTemp}\\{User.Identity.Name}\\");
                foreach (var musicFile in uploads)
                {
                    string pathToMusicFile = $@"{_pathToUserCurrentDir}\{musicFile.FileName}";
                    using (var fileStream = new FileStream(pathToMusicFile, FileMode.Create))
                    {
                        await Task.Run(() => musicFile.CopyToAsync(fileStream));
                    }
                }
                return RedirectToAction("Choosing", "Menu", new { path = _pathToUserCurrentDir.ToString() });
            }

            return View("General");       
        }


        private List<MusicFileModel> GetMusicModels(string path)
        {
            
            DirectoryInfo userDir = new DirectoryInfo(path);
            List<MusicFileModel> musicFilesModel = new List<MusicFileModel>();
            FileInfo[] musicFiles = userDir.GetFiles();

            foreach (var mFile in musicFiles)
            {
                MusicFileModel m = new MusicFileModel()
                {
                    Name = mFile.Name,
                    Path = mFile.FullName
                };
                musicFilesModel.Add(m);
            }
            return musicFilesModel;
        }

        public IActionResult Choosing(string path)
        {
            List<MusicFileModel> musicFilesModel = GetMusicModels(path);     
            return View(musicFilesModel);
        }

        [HttpPost]
        public async Task GetTagFromMusic(string name, string connectionId)
        {
            string path = @$"{_appEnvironment.WebRootPath}\\TempFiles\\{User.Identity.Name}\\{name}";
            TagLib.File musicFile = TagLib.File.Create(path);

            Song s = new Song()
            {
                Album = new Album() { Name = musicFile.Tag.Album, Year = (int)musicFile.Tag.Year },
                Name = musicFile.Tag.Title,
                Disc = (int)musicFile.Tag.Disc,
                Comment = musicFile.Tag.Comment,
                Track = (int)musicFile.Tag.Track,
                Lyrics = musicFile.Tag.Lyrics,
                //PictureData = musicFile.Tag.Pictures[0].Data.Data
            };

            // Добавление жанров
            List<Genre> genres = new List<Genre>();
            foreach (string genre in musicFile.Tag.Genres)
            {
                genres.Add(new Genre() { Name = genre });  
            }
            s.Genres = genres;

            // Добавление Артистов
            List<Artist> artists = new List<Artist>();
            foreach (string artist in musicFile.Tag.AlbumArtists)
            {
                artists.Add(new Artist() { Name = artist });
            }
            s.Artists = artists;

            // Добавление Композиторов
            List<Compositor> compositors = new List<Compositor>();
            foreach (string compositor in musicFile.Tag.Composers)
            {
                compositors.Add(new Compositor() { Name = compositor });
            }
            s.Compositors = compositors;
            if(musicFile.Tag.Pictures.Length != 0)
                s.PictureData = musicFile.Tag.Pictures[0].Data.Data;
            //MemoryStream ms = new MemoryStream(musicFile.Tag.Pictures[0].Data.Data);
            Picture pic = new Picture();

            await _hubContext.Clients.Client(connectionId).SendAsync("GetTagForm", s);
            //    TagLib.File musicFile = TagLib.File.Create(path);
            //    Song s = new Song()
            //    {
            //        album = new Album() { Name = musicFile.Tag.Album, Year = (int)musicFile.Tag.Year },
            //        Name = musicFile.Tag.Title,
            //        Disc = (int)musicFile.Tag.Disc,
            //        Comment = musicFile.Tag.Comment,
            //        Track = (int)musicFile.Tag.Track,
            //        genre = new Genre() { Name = musicFile.Tag.FirstGenre },
            //        Lyrics = musicFile.Tag.Lyrics
            //    };
            //    db.Songs.Add(s);
            //}
            
        }

        //[HtppPost]
        //public async Task SaveTag(SongViewModel j, IFormFile mainSongImage)
        //{
        //    var v = "";
        //}

        [HttpPost]
        public IActionResult SaveTag(SongViewModel songTag)
        {
            string path = @$"{_appEnvironment.WebRootPath}\\TempFiles\\{User.Identity.Name}\\{songTag.nameFileSong}";
            TagLib.File musicFile = TagLib.File.Create(path);

            // Запись изменений в файл

            if (songTag.Album == null)
                songTag.Album = "";
            musicFile.Tag.Album = songTag.Album;
            
            if(songTag.Artists == null)
                songTag.Artists = "";
            musicFile.Tag.AlbumArtists = songTag.Artists.Split(",");

            if(songTag.Comment == null)
                songTag.Comment = "";
            musicFile.Tag.Comment = songTag.Comment;

            if (songTag.Compositors == null)
                songTag.Compositors = "";
            musicFile.Tag.Composers = songTag.Compositors.Split(",");

            musicFile.Tag.Disc = (uint)songTag.Disc;


            if (songTag.Genres == null)
                songTag.Genres = "";
            musicFile.Tag.Genres = songTag.Genres.Split(",");

            if (songTag.Name == null)
                songTag.Name = "";
            musicFile.Tag.Title = songTag.Name;

            musicFile.Tag.Track = (uint)songTag.Track;

            if (songTag.Lyrics == null)
                songTag.Lyrics = "";
            musicFile.Tag.Lyrics = songTag.Lyrics;

            if (songTag.mainSongImage != null)
            {
                // Запись изображения
                using (var ms = new MemoryStream())
                {
                    songTag.mainSongImage.CopyToAsync(ms);
                    byte[] mainSongImageBytes = ms.ToArray();
                    musicFile.Tag.Pictures[0].Data = mainSongImageBytes;
                }
            }

            musicFile.Properties.MediaTypes.ToString();
            musicFile.Save();
            var fileStream = new FileStream(path, FileMode.Open);
            int posFormat = songTag.nameFileSong.LastIndexOf(".");
            string fileExtension = songTag.nameFileSong.Substring(posFormat + 1);
            return File(fileStream, "text/plain", songTag.nameFileSong);
        }

        public IActionResult _GetEdit(string path)
        {
            List<MusicFileModel> musicFilesModel = GetMusicModels(path);
            return PartialView("_Edit");
        }


    }
}
