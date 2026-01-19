using GoldenCrown.Data;
using GoldenCrown.DTOs;
using GoldenCrown.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountService _accountService;

        public UserService(ApplicationDbContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        async Task<(bool IsSuccess, string ErrorMessage)> IUserService.RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == request.Login);

            // проверить, существует ли пользователь с таким логином
            if (existingUser != null)
                return (false, "Пользователь с таким логином уже существует");

            // проверить сложность проля (минимум 6 символов)
            if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 6)
                return (false, "Пароль должен содержать минимум 6 символов");

            // создать нового пользователя
            var newUser = await _context.Users.AddAsync(new()
            {
                Id = Guid.NewGuid(),
                Login = request.Login,
                Name = request.Name,
                Password = request.Password
            });

            await _context.SaveChangesAsync();

            await _accountService.CreateAccountAsync(newUser.Entity.Id);

            return (true, null);
        }

        public async Task<string?> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == request.Login);

            if (user == null || user.Password != request.Password)
                return null;

            var oldSessions = await _context.Sessions
                .Where(s => s.UserId == user.Id)
                .ToListAsync();

            if (oldSessions.Any())
            {
                _context.Sessions.RemoveRange(oldSessions);
            }

            var token = Guid.NewGuid().ToString();
            var session = new Session
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            return token;
        }
    }
}
