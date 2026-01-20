using FluentValidation;
using GoldenCrown.DTOs;

namespace GoldenCrown.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty().WithMessage("Логин не может быть пустым")
                .MinimumLength(3).WithMessage("Минимальная длина логина - 3 символа");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя обязательно");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен")
                .MinimumLength(6).WithMessage("Пароль должен быть длиннее 6 символов");
        }
    }
}