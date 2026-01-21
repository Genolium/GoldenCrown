using FluentValidation;
using GoldenCrown.Features.Finance.Commands;

namespace GoldenCrown.Validators
{
    public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountCommandValidator()
        {
            RuleFor(x => x.Currency)
                .NotEmpty()
                .Length(3)
                .Must(c => c == "USD" || c == "EUR" || c == "RUB") 
                .WithMessage("Допустимые валюты: USD, EUR, RUB");
        }
    }
}