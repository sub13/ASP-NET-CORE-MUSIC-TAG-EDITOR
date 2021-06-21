using System.ComponentModel.DataAnnotations;

namespace MusicTagEditor.ViewModels.Auth
{
    public class LoginViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Не указан Email!")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Не указан Пароль!")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
