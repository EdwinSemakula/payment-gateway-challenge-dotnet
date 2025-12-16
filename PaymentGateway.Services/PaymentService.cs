using Microsoft.Extensions.Logging;

using PaymentGateway.Common.Exceptions;
using PaymentGateway.Repositories.Interfaces;
using PaymentGateway.Services.Helpers;
using PaymentGateway.Services.Interfaces;
using PaymentGateway.Services.Models;

namespace PaymentGateway.Services;

public class PaymentService(
    IPaymentRepository paymentResponseRepository,
    IBankService bankService,
    ILogger<PaymentService> logger)
    : IPaymentService
{
    public async Task ProcessPayment(PaymentRequest paymentRequest)
    {
        try
        {
            var bankResponse = await bankService.GetBankResponse(paymentRequest);
            var paymentResponse = PaymentResponseHelper.CreatePaymentResponse(paymentRequest, bankResponse.Authorized);
               
            paymentResponseRepository.SavePayment(paymentResponse);
            
        }
        catch (Exception exception)
        {
            logger.LogError("An error occurred while processing the payment.");
            throw new ServiceException(exception.Message, exception);
        }
    }
}