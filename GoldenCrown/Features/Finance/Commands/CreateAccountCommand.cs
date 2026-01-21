using MediatR;

namespace GoldenCrown.Features.Finance.Commands
{
    public class CreateAccountCommand : IRequest<Unit>
    {
        public string Currency { get; set; }
    }
}
