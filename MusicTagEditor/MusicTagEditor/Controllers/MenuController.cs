using AutoMapper;
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

            var pathToFiles = await _musicFileService.UploadMusicFiles(uploads);


            if (pathToFiles != null)
            {
                return RedirectToAction("Choosing", "Menu");
            }

            return View("General");       
        }

        
        public async Task<IActionResult> Choosing()
        {
            List<MusicFileModel> musicFilesModel = await _musicFileService.GetMusicModels();     
            return View(musicFilesModel);
        }

        [HttpPost]
        public async Task GetTagFromMusic(string name, string connectionId)
        {
            var songData = await _musicFileService.GetMusicFileData(name);
            await _hubContext.Clients.Client(connectionId).SendAsync("GetTagForm", songData);
        }

        [HttpPost]
        public async Task<IActionResult> SaveTag(SongViewModel songTag)
        {
            var songData = _mapper.Map<SongData>(songTag);
            
            var fileStream = await _musicFileService.UpdateMusicFile(songData);

            return File(fileStream, "text/plain", songTag.nameFileSong);
        }

        public async Task<IActionResult> _GetEdit(string path)
        {
            List<MusicFileModel> musicFilesModel = await _musicFileService.GetMusicModels();
            return PartialView("_Edit");
        }

        [HttpPost]
        public async Task<FileStreamResult> GetSongByName(string name)
        {
            var musicFileStream = await _musicFileService.GetSongByName(name);
            return File(musicFileStream, "text/plain", name);
        }
    }
}
