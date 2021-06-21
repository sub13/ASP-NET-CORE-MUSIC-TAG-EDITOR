using MusicTagEditor.Data.Models;
using System.Threading.Tasks;

namespace MusicTagEditor.Businees.Servicess
{
    public interface IUserService
    {
        Task<User> GetCurrentUser();
    }
}
