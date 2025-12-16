using PaymentGateway.Common.Enums;
using PaymentGateway.Services.Models;

namespace PaymentGateway.Services.Helpers;

public static class PaymentResponseHelper
{
    public static PaymentResponse CreatePaymentResponse(PaymentRequest paymentRequest, bool isAuthorized)
    {
        return new PaymentResponse
        {
            Id = Guid.NewGuid(),
            MerchantId = paymentRequest.MerchantId,
            Status = ConvertToPaymentStatus(isAuthorized),
            LastFourDigits = GetLastFourDigits(paymentRequest.Card.Number),
            ExpiryMonth = paymentRequest.Card.ExpiryMonth,
            ExpiryYear = paymentRequest.Card.ExpiryYear,
            Currency = paymentRequest.Currency,
            Amount = paymentRequest.Amount
        };
    }

    private static PaymentStatus ConvertToPaymentStatus(bool isAuthorized)
    {
        return isAuthorized ? PaymentStatus.Authorized : PaymentStatus.Declined;
    }

    private static string GetLastFourDigits(string cardNumber)
    {
        return cardNumber.Substring(cardNumber.Length - 4);
    }
}