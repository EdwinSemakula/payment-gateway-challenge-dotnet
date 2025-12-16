using PaymentResponse = PaymentGateway.Repositories.Models.PaymentResponse;
using Service = PaymentGateway.Services.Models;

namespace PaymentGateway.Repositories.Interfaces;

public interface IPaymentRepository
{
    public void SavePayment(Service.PaymentResponse payment);
    public IEnumerable<PaymentResponse> GetAllPayments();
    public IEnumerable<PaymentResponse> GetPaymentsByMerchantId(Guid merchantId);
    public PaymentResponse GetPaymentByIds(Guid merchantId, Guid paymentId);
}