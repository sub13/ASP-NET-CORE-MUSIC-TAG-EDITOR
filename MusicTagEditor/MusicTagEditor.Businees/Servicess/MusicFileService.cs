﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MusicTagEditor.Businees.Models;
using MusicTagEditor.Data.Models;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MusicTagEditor.Businees.Servicess
{
    public class MusicFileService : IMusicFileService
    {
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly IUserService _userService;

        public MusicFileService(IWebHostEnvironment appEnvironment, IUserService userService)
        {
            _appEnvironment = appEnvironment;
            _userService = userService;
        }

        public async Task<string> UploadMusicFiles(IFormFileCollection uploads)
        {
            StringBuilder _pathToUserCurrentDir;
            var currentUser = await _userService.GetCurrentUser();

            if (uploads != null)
            {
                string pathDirTemp = @$"{_appEnvironment.WebRootPath}\\TempFiles";
                DirectoryInfo dirTemp = new DirectoryInfo(pathDirTemp);
                DirectoryInfo _userDir = dirTemp.CreateSubdirectory(currentUser.Email);
                _pathToUserCurrentDir = new StringBuilder($@"{pathDirTemp}\\{currentUser.Email}\\");

                foreach (var musicFile in uploads)
                {
                    string pathToMusicFile = $@"{_pathToUserCurrentDir}\{musicFile.FileName}";
                    using (var fileStream = new FileStream(pathToMusicFile, FileMode.Create))
                    {
                        await Task.Run(() => musicFile.CopyToAsync(fileStream));
                    }
                }

                return _pathToUserCurrentDir.ToString();
            }

            return null;
        }

        public async Task<FileStream> UpdateMusicFile(SongData songData)
        {
            var currentUser = await _userService.GetCurrentUser();

            string path = @$"{_appEnvironment.WebRootPath}\\TempFiles\\{currentUser.Email}\\{songData.nameFileSong}";
            TagLib.File musicFile = TagLib.File.Create(path);

            // Запись изменений в файл

            if (songData.Album == null)
                songData.Album = "";
            musicFile.Tag.Album = songData.Album;

            if (songData.Artists == null)
                songData.Artists = "";
            musicFile.Tag.AlbumArtists = songData.Artists.Split(",");

            if (songData.Comment == null)
                songData.Comment = "";
            musicFile.Tag.Comment = songData.Comment;

            if (songData.Compositors == null)
                songData.Compositors = "";
            musicFile.Tag.Composers = songData.Compositors.Split(",");

            musicFile.Tag.Disc = (uint)songData.Disc;


            if (songData.Genres == null)
                songData.Genres = "";
            musicFile.Tag.Genres = songData.Genres.Split(",");

            if (songData.Name == null)
                songData.Name = "";
            musicFile.Tag.Title = songData.Name;

            musicFile.Tag.Track = (uint)songData.Track;

            if (songData.Lyrics == null)
                songData.Lyrics = "";
            musicFile.Tag.Lyrics = songData.Lyrics;

            musicFile.Tag.Year = uint.Parse(songData.Year);

            if (songData.mainSongImage != null)
            {
                // Запись изображения
                using (var ms = new MemoryStream())
                {
                    await songData.mainSongImage.CopyToAsync(ms);
                    byte[] mainSongImageBytes = ms.ToArray();
                    //if (musicFile.Tag.Pictures[0] == null)
                    //   musicFile.Tag.Pictures[0] = new TagLib.Picture(PictureType.);
                    //musicFile.Tag.Pictures[0].Data = mainSongImageBytes;

                    musicFile.Tag.Pictures = new TagLib.IPicture[]
                    {
                        new TagLib.Picture(new TagLib.ByteVector(mainSongImageBytes))
                        {
                            Type = TagLib.PictureType.FrontCover,
                            Description = "Cover",
                            MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg
                        }
                     };
                }
            }

            musicFile.Properties.MediaTypes.ToString();
            musicFile.Save();
            return new FileStream(path, FileMode.Open);
        }

        public async Task<Song> GetMusicFileData(string name)
        {
            var currentUser = await _userService.GetCurrentUser();

            string path = @$"{_appEnvironment.WebRootPath}\\TempFiles\\{currentUser.Email}\\{name}";
            TagLib.File musicFile = TagLib.File.Create(path);

            Song s = new Song()
            {
                Album = new Album() { Name = musicFile.Tag.Album, Year = (int)musicFile.Tag.Year },
                Name = musicFile.Tag.Title,
                Disc = (int)musicFile.Tag.Disc,
                Comment = musicFile.Tag.Comment,
                Track = (int)musicFile.Tag.Track,
                Lyrics = musicFile.Tag.Lyrics,
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
            if (musicFile.Tag.Pictures.Length != 0)
                s.PictureData = musicFile.Tag.Pictures[0].Data.Data;

            return s;
        }
    }
}