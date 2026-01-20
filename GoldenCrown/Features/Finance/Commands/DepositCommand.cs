using MediatR;

namespace GoldenCrown.Features.Finance.Commands
{
    public class DepositCommand : IRequest<Unit> // Unit = void 
    {
        public decimal Amount { get; set; }
    }
}