using GoldenCrown.Attributes;
using GoldenCrown.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null)
            {
                await _next(context);
                return;
            }

            var authAttribute = endpoint.Metadata.GetMetadata<MyAuthorizeAttribute>();

            if (authAttribute != null)
            {
                // Читаем токен из заголовка
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { Message = "Токен отсутствует" });
                    return;
                }

                var session = await dbContext.Sessions
                    .Include(s => s.User)
                    .ThenInclude(u => u.Accounts)
                    .AsNoTracking() 
                    .FirstOrDefaultAsync(s => s.Token == token);

                if (session == null || session.ExpiresAt < DateTime.UtcNow)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { Message = "Токен невалиден или истек" });
                    return;
                }

                context.Items["User"] = session.User;
            }

            await _next(context);
        }
    }
}