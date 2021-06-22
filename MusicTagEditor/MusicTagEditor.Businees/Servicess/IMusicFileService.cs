using Microsoft.AspNetCore.Http;
using MusicTagEditor.Businees.Models;
using MusicTagEditor.Data.Models;
using System.IO;
using System.Threading.Tasks;

namespace MusicTagEditor.Businees.Servicess
{
    public interface IMusicFileService
    {
        Task<string> UploadMusicFiles(IFormFileCollection uploads);

        Task<FileStream> UpdateMusicFile(SongData songViewModel);

        Task<Song> GetMusicFileData(string name);
    }
}
