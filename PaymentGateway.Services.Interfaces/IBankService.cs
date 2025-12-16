using PaymentGateway.Repositories.Models;
using PaymentGateway.Services.Models;

namespace PaymentGateway.Services.Interfaces;

public interface IBankService
{
    public Task<BankResponse> GetBankResponse(PaymentRequest paymentRequest);
}