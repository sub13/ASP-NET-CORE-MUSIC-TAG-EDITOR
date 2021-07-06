using Microsoft.AspNetCore.Diagnostics;

namespace MusicTagEditor.ViewModels
{
    public class ErrorViewModels
    {
        public string ErrorMessage { get; set; }
        public string ExceptionPath { get; set; }
        public string StackTrace { get; set; }

        public static ErrorViewModels Create(IExceptionHandlerPathFeature exceptionDetails)
        {
            var errorViewModels = new ErrorViewModels()
            {
                ErrorMessage = exceptionDetails.Error.Message,
                ExceptionPath = exceptionDetails.Path,
                StackTrace = exceptionDetails.Error.StackTrace
            };

            return errorViewModels;
        }
    }
}
