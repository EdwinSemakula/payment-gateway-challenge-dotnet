using Microsoft.Extensions.Logging;

using PaymentGateway.Common.Exceptions;
using PaymentGateway.Repositories.Interfaces;
using PaymentGateway.Repositories.Models;
using PaymentGateway.Services.Helpers;
using PaymentGateway.Services.Interfaces;
using PaymentGateway.Services.Models;

namespace PaymentGateway.Services;

public class BankService(IBankAPiClient bankApiClient, ILogger<BankService> logger) : IBankService
{
    public async Task<BankResponse> GetBankResponse(PaymentRequest paymentRequest)
    {
        try
        {
            var bankPaymentRequest = BankRequestHelper.ConvertToBankRequest(paymentRequest);
            var response = await bankApiClient.ProcessPayment(bankPaymentRequest);

            return response;
        }
        catch (Exception exception)
        {
            logger.LogError("An error occurred while processing this payment with the bank.");
            throw new ServiceException(exception.Message, exception);
        }
    }
}