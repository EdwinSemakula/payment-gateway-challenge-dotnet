using AutoMapper;

using Microsoft.Extensions.Logging;

using PaymentGateway.Repositories.Interfaces;

using PaymentResponse = PaymentGateway.Repositories.Models.PaymentResponse;
using Service = PaymentGateway.Services.Models;

namespace PaymentGateway.Repositories;

public class PaymentRepository(IMapper mapper, ILogger<PaymentRepository> logger) : IPaymentRepository
{
    public List<PaymentResponse> Payments = new();

    public void SavePayment(Service.PaymentResponse payment)
    {
        var repoPayment = mapper.Map<PaymentResponse>(payment);
        Payments.Add(repoPayment);
        logger.LogInformation("Payment saved.");
    }

    public IEnumerable<PaymentResponse> GetAllPayments()
    {
        return Payments;
    }

    public IEnumerable<PaymentResponse> GetPaymentsByMerchantId(Guid merchantId)
    {
        return Payments.Where(p => p.MerchantId == merchantId);
    }

    public PaymentResponse GetPaymentByIds(Guid merchantId, Guid paymentId)
    {
        return Payments.FirstOrDefault(p => p.Id == paymentId && p.MerchantId == merchantId);
    }
}