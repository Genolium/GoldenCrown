using GoldenCrown.Data;
using GoldenCrown.Models; 
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Features.Users.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, string>
    {
        private readonly ApplicationDbContext _context;

        public RegisterUserCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Проверка на дубликат
            var exists = await _context.Users.AnyAsync(u => u.Login == request.Login, cancellationToken);
            if (exists) throw new Exception("Пользователь с таким логином уже существует");

            // Создаем пользователя
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Login = request.Login,
                Name = request.Name,
                Password = request.Password
            };

            // Создаем ему счет
            var newAccount = new Account
            {
                Id = Guid.NewGuid(),
                UserId = newUser.Id,
                Balance = 0
            };

            _context.Users.Add(newUser);
            _context.Accounts.Add(newAccount);

            await _context.SaveChangesAsync(cancellationToken);

            return "Пользователь успешно зарегистрирован";
        }
    }
}