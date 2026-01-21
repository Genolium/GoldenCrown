using GoldenCrown.Data;
using GoldenCrown.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Features.Finance.Commands
{
    public class TransferCommandHandler : IRequestHandler<TransferCommand, Unit>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TransferCommandHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Items["User"] is not User sender)
                throw new Exception("Пользователь не авторизован");

            using var transactionScope = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                _context.Attach(sender);
                var senderAccount = sender.Accounts.FirstOrDefault(a => a.Currency == request.Currency);
                if (senderAccount == null) throw new Exception($"У вас нет счета в {request.Currency}");
                if (senderAccount.Balance < request.Amount) throw new Exception("Недостаточно средств");

                var receiver = await _context.Users.Include(u => u.Accounts)
                    .FirstOrDefaultAsync(u => u.Login == request.ReceiverLogin, cancellationToken);
                if (receiver == null) throw new Exception("Получатель не найден");

                var receiverAccount = receiver.Accounts.FirstOrDefault(a => a.Currency == request.Currency);
                if (receiverAccount == null) throw new Exception($"У получателя нет счета в {request.Currency}");

                senderAccount.Balance -= request.Amount;
                receiverAccount.Balance += request.Amount;

                var transactionRecord = new Transaction
                {
                    Id = Guid.NewGuid(),
                    Amount = request.Amount,
                    Date = DateTime.UtcNow,
                    SenderId = sender.Id,
                    ReceiverId = receiver.Id,
                    Currency = request.Currency
                };

                _context.Transactions.Add(transactionRecord);
                await _context.SaveChangesAsync(cancellationToken);
                await transactionScope.CommitAsync(cancellationToken);

                return Unit.Value;
            }
            catch
            {
                await transactionScope.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}