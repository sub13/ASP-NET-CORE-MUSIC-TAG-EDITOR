using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MusicTagEditor.DataApp.Models
{
    public class RegisterModel
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Не указан Email!")]
        public string Email  { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Не указан Пароль!")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль потвержден неверно")]
        public string ConfirmPassword { get; set; }
    }
}
