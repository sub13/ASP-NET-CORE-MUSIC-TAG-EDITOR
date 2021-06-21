﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MusicTagEditor.Businees.Models;
using MusicTagEditor.Businees.Servicess;
using MusicTagEditor.Data.Models;
using MusicTagEditor.ViewModels.Menu;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MusicTagEditor.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        private SongsDbContext db;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly IHubContext<SendTagHub> _hubContext;
        private readonly IMusicFileService _musicFileService;
        private readonly IMapper _mapper;

        public MenuController(SongsDbContext context, 
            IWebHostEnvironment appEnvironment, 
            IHubContext<SendTagHub> hubContext, 
            IMusicFileService musicFileService,
            IMapper mapper)
        {
            db = context;
            _appEnvironment = appEnvironment;
            _hubContext = hubContext;
            _musicFileService = musicFileService;
            _mapper = mapper;
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
            var songData = await _musicFileService.GetMusicFileData(name);

            await _hubContext.Clients.Client(connectionId).SendAsync("GetTagForm", songData);
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

        [HttpPost]
        public async Task<IActionResult> SaveTag(SongViewModel songTag)
        {
            var songData = _mapper.Map<SongData>(songTag);
            
            var fileStream = await _musicFileService.UpdateMusicFile(songData);

            return File(fileStream, "text/plain", songTag.nameFileSong);
        }

        public IActionResult _GetEdit(string path)
        {
            List<MusicFileModel> musicFilesModel = GetMusicModels(path);
            return PartialView("_Edit");
        }
    }
}
