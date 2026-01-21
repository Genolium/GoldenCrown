using GoldenCrown.Data;
using GoldenCrown.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Features.Finance.Commands
{
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Unit>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateAccountCommandHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.Items["User"] as User;
            if (user == null) throw new Exception("Пользователь не авторизован");

            // Проверяем, нет ли уже такого счета
            var exists = await _context.Accounts
                .AnyAsync(a => a.UserId == user.Id && a.Currency == request.Currency, cancellationToken);

            if (exists) throw new Exception($"Счет в валюте {request.Currency} уже существует");

            var newAccount = new Account
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Currency = request.Currency,
                Balance = 0
            };

            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}