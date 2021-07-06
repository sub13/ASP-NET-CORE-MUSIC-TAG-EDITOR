using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MusicTagEditor.ViewModels;

namespace MusicTagEditor.Controllers
{
    public class ErrorController : Controller
    {

        [Route("Error")]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            var errorViewModel = ErrorViewModels.Create(exceptionDetails);

            return View(errorViewModel);
        }
    }
}
