using GoldenCrown.Data;
using GoldenCrown.Models;
using MediatR;

namespace GoldenCrown.Features.Finance.Commands
{
    public class DepositCommandHandler : IRequestHandler<DepositCommand, Unit>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DepositCommandHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(DepositCommand request, CancellationToken cancellationToken)
        {
            // Достаем юзера из контекста
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Items["User"] is not User user)
                throw new Exception("Пользователь не авторизован");

            // Прикрепляем юзера к контексту EF (так как он пришел из Middleware AsNoTracking)
            _context.Attach(user);

            // Логика пополнения
            var account = user.Accounts.FirstOrDefault(a => a.Currency == request.Currency);

            if (account == null)
                throw new Exception($"У вас нет счета в валюте {request.Currency}. Сначала создайте его.");

            account.Balance += request.Amount;

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = request.Amount,
                Date = DateTime.UtcNow,
                SenderId = null,
                ReceiverId = user.Id,
                Currency = request.Currency
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value; // Возвращаем "пустоту"
        }
    }
}