using GoldenCrown.Data;
using GoldenCrown.DTOs;
using GoldenCrown.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Features.Finance.Queries
{
    public class GetHistoryQueryHandler : IRequestHandler<GetHistoryQuery, List<TransactionResponse>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetHistoryQueryHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<TransactionResponse>> Handle(GetHistoryQuery request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Items["User"] is not User user)
                throw new Exception("Пользователь не авторизован");

            var query = _context.Transactions
                .AsNoTracking()
                .Where(t => t.SenderId == user.Id || t.ReceiverId == user.Id);

            if (request.From.HasValue) query = query.Where(t => t.Date >= request.From.Value);
            if (request.To.HasValue) query = query.Where(t => t.Date <= request.To.Value);

            return await query
                .OrderByDescending(t => t.Date)
                .Skip(request.Offset)
                .Take(request.Limit)
                .Select(t => new TransactionResponse
                {
                    Id = t.Id,
                    Date = t.Date,
                    Amount = t.Amount,
                    SenderName = t.Sender != null ? t.Sender.Name : "System",
                    ReceiverName = t.Receiver.Name,
                    Type = t.SenderId == null ? "Deposit" : (t.SenderId == user.Id ? "Expense" : "Income")
                })
                .ToListAsync(cancellationToken);
        }
    }
}