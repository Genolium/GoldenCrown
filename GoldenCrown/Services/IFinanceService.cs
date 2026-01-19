using GoldenCrown.DTOs;
using GoldenCrown.Models;

namespace GoldenCrown.Services
{
    public interface IFinanceService
    {
        Task<decimal> GetBalanceAsync(string token);
        Task DepositAsync(DepositRequest request);
        Task TransferAsync(TransferRequest request);
        Task<List<TransactionResponse>> GetHistoryAsync(TransactionHistoryRequest request);
    }
}
