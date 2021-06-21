using AutoMapper;
using MusicTagEditor.Businees.Models;
using MusicTagEditor.ViewModels.Menu;

namespace MusicTagEditor.Mappings
{
    public class SongDataProfile : Profile
    {
        public SongDataProfile()
        {
            CreateMap<SongViewModel, SongData>().ReverseMap();
        }
    }
}
