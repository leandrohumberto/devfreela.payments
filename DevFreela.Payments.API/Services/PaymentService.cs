using DevFreela.Payments.API.Model;

namespace DevFreela.Payments.API.Services
{
    public class PaymentService : IPaymentService
    {
        public Task<bool> Process(PaymentInfoInputModel inputModel)
        {
            return Task.FromResult(true);
        }
    }
}
