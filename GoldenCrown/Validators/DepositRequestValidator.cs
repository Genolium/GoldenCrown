using FluentValidation;
using GoldenCrown.DTOs;

namespace GoldenCrown.Validators
{
    public class DepositRequestValidator : AbstractValidator<DepositRequest>
    {
        public DepositRequestValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Сумма пополнения должна быть больше нуля")
                .LessThan(100000000).WithMessage("Слишком большая сумма");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Укажите валюту")
                .Length(3).WithMessage("Код валюты должен быть 3 символа (USD, RUB...)");
        }
    }
}