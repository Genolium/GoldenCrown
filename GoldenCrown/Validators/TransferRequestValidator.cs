using FluentValidation;
using GoldenCrown.DTOs;

namespace GoldenCrown.Validators
{
    public class TransferRequestValidator : AbstractValidator<TransferRequest>
    {
        public TransferRequestValidator()
        {
            RuleFor(x => x.ReceiverLogin)
                .NotEmpty().WithMessage("Укажите логин получателя");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Сумма перевода должна быть больше нуля");
        }
    }
}