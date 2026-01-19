using GoldenCrown.DTOs;
using GoldenCrown.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoldenCrown.Controllers
{
    [ApiController] 
    [Route("api/[controller]")] 
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _userService.RegisterAsync(request);

            if (!result.IsSuccess)
            {
                // Вернет 400 Bad Request с текстом ошибки
                return BadRequest(new { Error = result.ErrorMessage });
            }

            return Ok(new { Message = "Регистрация прошла успешно" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _userService.LoginAsync(request);

            if (token == null)
            {
                // Вернет 401 Unauthorized
                return Unauthorized(new { Message = "Неверный логин или пароль" });
            }

            // Вернет 200 OK и JSON с токеном
            return Ok(new LoginResponse { Token = token });
        }
    }
}
