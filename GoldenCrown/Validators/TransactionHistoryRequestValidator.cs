using FluentValidation;
using GoldenCrown.DTOs;

namespace GoldenCrown.Validators
{
    public class TransactionHistoryRequestValidator : AbstractValidator<TransactionHistoryRequest>
    {
        public TransactionHistoryRequestValidator()
        {
            RuleFor(x => x.Limit)
                .GreaterThan(0).WithMessage("Количество запрашиваемых операций должно быть больше 0 ")
                .LessThan(100000000).WithMessage("Слишком большое число запрашиваемых операций");

            RuleFor(x => x.Offset)
                .GreaterThanOrEqualTo(0).WithMessage("Смещение не может быть отрицательным");
        }
    }
}
