namespace GoldenCrown.Services
{
    public interface IAccountService
    {
        Task CreateAccountAsync(Guid userId);
    }
}
