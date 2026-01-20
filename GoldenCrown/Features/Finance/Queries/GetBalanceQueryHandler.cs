using MediatR;
using GoldenCrown.Models;

namespace GoldenCrown.Features.Finance.Queries
{
    public class GetBalanceQueryHandler : IRequestHandler<GetBalanceQuery, decimal>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetBalanceQueryHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<decimal> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
        {
            // Берем юзера из HttpContext (положил Middleware)
            var context = _httpContextAccessor.HttpContext;
            if (context?.Items["User"] is not User user)
                throw new Exception("Пользователь не авторизован");

            var account = user.Accounts.FirstOrDefault();
            if (account == null) throw new Exception("Счет не найден");

            return Task.FromResult(account.Balance);
        }
    }
}