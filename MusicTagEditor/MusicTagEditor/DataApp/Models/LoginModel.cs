using System.ComponentModel.DataAnnotations;

namespace MusicTagEditor.DataApp.Models
{
    public class LoginModel
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Не указан Email!")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Не указан Пароль!")]
        public string Password { get; set; }
    }
}
