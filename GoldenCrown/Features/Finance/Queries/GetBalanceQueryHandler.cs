using GoldenCrown.DTOs;
using GoldenCrown.Models;
using MediatR;

namespace GoldenCrown.Features.Finance.Queries
{
    public class GetBalanceQueryHandler : IRequestHandler<GetBalanceQuery, BalanceResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetBalanceQueryHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BalanceResponse> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
        {
            // Берем юзера из HttpContext (положил Middleware)
            var context = _httpContextAccessor.HttpContext;
            if (context?.Items["User"] is not User user)
                throw new Exception("Пользователь не авторизован");

            var accountsDto = user.Accounts.Select(a => new AccountDto
            {
                Currency = a.Currency,
                Balance = a.Balance
            }).ToList();

            return new BalanceResponse { Accounts = accountsDto };
        }
    }
}