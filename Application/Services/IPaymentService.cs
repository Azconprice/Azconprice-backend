namespace Application.Services
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentOrder();
        Task<string> CreatePaymentTransaction(string orderId, string status);
    }
}
