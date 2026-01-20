using MediatR;

namespace GoldenCrown.Features.Users.Commands
{
    // Возвращает сообщение об успехе (string)
    public class RegisterUserCommand : IRequest<string>
    {
        public string Login { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }
}