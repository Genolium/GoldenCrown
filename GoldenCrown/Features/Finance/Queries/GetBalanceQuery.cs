using MediatR;

namespace GoldenCrown.Features.Finance.Queries
{
    public class GetBalanceQuery : IRequest<decimal> { }
}