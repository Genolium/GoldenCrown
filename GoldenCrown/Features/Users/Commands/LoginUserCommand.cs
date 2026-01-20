using MediatR;

namespace GoldenCrown.Features.Users.Commands
{
    // Возвращает string (токен) или null (если ошибка)
    public class LoginUserCommand : IRequest<string?>
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}