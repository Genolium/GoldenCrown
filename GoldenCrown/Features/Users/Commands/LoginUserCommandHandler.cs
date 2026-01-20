using GoldenCrown.Data;
using GoldenCrown.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Features.Users.Commands
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, string?>
    {
        private readonly ApplicationDbContext _context;

        public LoginUserCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string?> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            // Ищем пользователя
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == request.Login, cancellationToken);

            // Проверяем пароль
            if (user == null || user.Password != request.Password)
            {
                return null;
            }

            // Удаляем старые сессии
            var oldSessions = await _context.Sessions
                .Where(s => s.UserId == user.Id)
                .ToListAsync(cancellationToken);

            if (oldSessions.Any())
            {
                _context.Sessions.RemoveRange(oldSessions);
            }

            // Создаем новую сессию
            var token = Guid.NewGuid().ToString();
            var session = new Session
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync(cancellationToken);

            return token;
        }
    }
}