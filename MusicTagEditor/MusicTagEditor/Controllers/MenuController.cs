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
using MusicTagEditor.DataApp.Models;
using TagLib;

namespace MusicTagEditor.Controllers
{
    public class MenuController : Controller
    {
        private SongsDbContext db;
        private IFormFileCollection _uploadsMusicFiles;
        private IWebHostEnvironment _appEnvironment;
        private DirectoryInfo _userDir;
        private List<string> _currentMusicPathCollection;

        public MenuController(SongsDbContext context, IWebHostEnvironment appEnvironment)
        {
            db = context;
            _appEnvironment = appEnvironment;
        }
        public IActionResult General()
        {
            return View();
        }

        [HttpPost]
        [RequestSizeLimit(1000000000)] // 1gb
        public async Task<IActionResult> UploadMusicFiles(IFormFileCollection uploads)
        {

            if (uploads != null)
            {
                _currentMusicPathCollection = new List<string>();
                string pathDirTemp = @$"{_appEnvironment.WebRootPath}\\TempFiles";
                DirectoryInfo dirTemp = new DirectoryInfo(pathDirTemp);
                _userDir = dirTemp.CreateSubdirectory(User.Identity.Name);
                StringBuilder fullPathSubDir = new StringBuilder($@"{pathDirTemp}\\{User.Identity.Name}\\");
                foreach (var musicFile in uploads)
                {
                    string pathToMusicFile = $@"{fullPathSubDir}\{musicFile.FileName}";
                    using (var fileStream = new FileStream(pathToMusicFile, FileMode.Create))
                    {
                        await Task.Run(() => musicFile.CopyToAsync(fileStream));
                    }
                    _currentMusicPathCollection.Add(pathToMusicFile);
                }
            }

            return RedirectToAction("General");
        }


        private void GetTagFromFiles()
        {
            if (_uploadsMusicFiles != null)
            {
                foreach (var musicFile in _uploadsMusicFiles)
                {
                    
                    //TagLib.File f = TagLib.File.Create(;
                    //TagLib
                    //_uploadsMusicFiles
                }
            }
        }

        public IActionResult Edit()
        {
            return View();
        }

    }
}
