using GoldenCrown.Data;
using GoldenCrown.DTOs;
using GoldenCrown.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services
{
    public class FinanceService : IFinanceService
    {
        private readonly ApplicationDbContext _context;

        public FinanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Ищем пользователя по токену
        private async Task<User> GetUserByTokenAsync(string token)
        {
            var session = await _context.Sessions
                .Include(s => s.User)
                .ThenInclude(u => u.Accounts) // Сразу подгружаем счета
                .FirstOrDefaultAsync(s => s.Token == token);

            if (session == null)
                throw new Exception("Сессия не найдена или токен неверен");

            if (session.ExpiresAt < DateTime.UtcNow)
                throw new Exception("Срок действия сессии истек");

            return session.User;
        }

        public async Task<decimal> GetBalanceAsync(string token)
        {
            var user = await GetUserByTokenAsync(token);
            var account = user.Accounts.FirstOrDefault();

            if (account == null)
                throw new Exception("Счет пользователя не найден");

            return account.Balance;
        }

        public async Task TransferAsync(TransferRequest request)
        {
            var sender = await GetUserByTokenAsync(request.Token);
            var senderAccount = sender.Accounts.FirstOrDefault();

            if (senderAccount == null)
                throw new Exception("Счет отправителя не найден");
            if (senderAccount.Balance < request.Amount)
                throw new Exception("Недостаточно средств");

            // Ищем получателя
            var receiver = await _context.Users
                .Include(u => u.Accounts)
                .FirstOrDefaultAsync(u => u.Login == request.ReceiverLogin);

            if (receiver == null) throw new Exception("Получатель не найден");

            var receiverAccount = receiver.Accounts.FirstOrDefault();
            if (receiverAccount == null) throw new Exception("Счет получателя не найден");

            // Перекидываем деньги
            senderAccount.Balance -= request.Amount;
            receiverAccount.Balance += request.Amount;

            // Записываем историю
            var transactionRecord = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = request.Amount,
                Date = DateTime.UtcNow,
                SenderId = sender.Id,
                ReceiverId = receiver.Id
            };

            _context.Transactions.Add(transactionRecord);
            await _context.SaveChangesAsync();
        }

        public async Task DepositAsync(DepositRequest request)
        {
            var user = await GetUserByTokenAsync(request.Token);
            var account = user.Accounts.FirstOrDefault();

            if (account == null)
                throw new Exception("Счет пользователя не найден");

            account.Balance += request.Amount;

            _context.Transactions.Add(new Transaction
            {
                Id = Guid.NewGuid(),
                SenderId = null,
                ReceiverId = user.Id,
                Date = DateTime.UtcNow,
                Amount = request.Amount
            });

            await _context.SaveChangesAsync();
        }

        public async Task<List<TransactionResponse>> GetHistoryAsync(TransactionHistoryRequest request)
        {
            var user = await GetUserByTokenAsync(request.Token);

            var query = _context.Transactions
                .AsNoTracking() 
                .Where(t => t.SenderId == user.Id || t.ReceiverId == user.Id);

            if (request.From.HasValue)
                query = query.Where(t => t.Date >= request.From.Value);

            if (request.To.HasValue)
                query = query.Where(t => t.Date <= request.To.Value);

            var transactions = await query
                .OrderByDescending(t => t.Date)
                .Skip(request.Offset)
                .Take(request.Limit)
                .Select(t => new TransactionResponse
                {
                    Id = Guid.Parse(t.Id.ToString()), 
                    Date = t.Date,
                    Amount = t.Amount,
                    SenderName = t.Sender != null ? t.Sender.Name : "Deposit",
                    ReceiverName = t.Receiver.Name,
                    Type = t.SenderId == null ? "Deposit" : (t.SenderId == user.Id ? "Expense" : "Income")
                })
                .ToListAsync();

            return transactions;
        }
    }
}
