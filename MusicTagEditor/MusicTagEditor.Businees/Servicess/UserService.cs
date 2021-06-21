using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MusicTagEditor.Data.Models;
using System.Threading.Tasks;

namespace MusicTagEditor.Businees.Servicess
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccesssor;
        private readonly UserManager<User> _userManager;

        public UserService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _httpContextAccesssor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<User>  GetCurrentUser()
        {
            return await _userManager.GetUserAsync(_httpContextAccesssor.HttpContext.User);
        }
    }
}
