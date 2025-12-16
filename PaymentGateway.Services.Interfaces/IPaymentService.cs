using PaymentGateway.Services.Models;

namespace PaymentGateway.Services.Interfaces;

public interface IPaymentService
{
    public Task ProcessPayment(PaymentRequest paymentRequest);
}