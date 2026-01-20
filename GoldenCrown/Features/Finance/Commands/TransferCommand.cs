using MediatR;

namespace GoldenCrown.Features.Finance.Commands
{
    public class TransferCommand : IRequest<Unit>
    {
        public string ReceiverLogin { get; set; }
        public decimal Amount { get; set; }
    }
}