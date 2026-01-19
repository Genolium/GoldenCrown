using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.DTOs
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Логин обязателен")]
        [MinLength(3, ErrorMessage = "Логин должен быть длиннее 3 символов")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Имя обязательно")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(6, ErrorMessage = "Пароль должен быть длиннее 6 символов")]
        public string Password { get; set; }
    }
}
