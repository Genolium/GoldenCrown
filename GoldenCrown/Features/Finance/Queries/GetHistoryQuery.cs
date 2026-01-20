using MediatR;
using GoldenCrown.DTOs; 

namespace GoldenCrown.Features.Finance.Queries
{
    public class GetHistoryQuery : IRequest<List<TransactionResponse>>
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int Limit { get; set; } = 10;
        public int Offset { get; set; } = 0;
    }
}