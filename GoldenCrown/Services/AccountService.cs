using GoldenCrown.Data;
using GoldenCrown.Models;

namespace GoldenCrown.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAccountAsync(Guid userId)
        {
            _context.Accounts.Add(new Account
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Balance = 0m,
            });
            await _context.SaveChangesAsync();
        }
    }
}
