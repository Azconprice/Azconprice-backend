namespace Application.Services
{
    public interface ITransactionService
    {
        Task<bool> CreateTransactionAsync(string userId, string companyId, double amount, string description);
    }
}
