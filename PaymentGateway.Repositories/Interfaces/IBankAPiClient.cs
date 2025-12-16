using PaymentGateway.Repositories.Models;
using PaymentGateway.Services.Models;

namespace PaymentGateway.Repositories.Interfaces;

public interface IBankAPiClient
{
    Task<BankResponse> ProcessPayment(BankRequest paymentRequest);
}