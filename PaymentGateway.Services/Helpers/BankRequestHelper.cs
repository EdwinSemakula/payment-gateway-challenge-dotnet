using PaymentGateway.Services.Models;

namespace PaymentGateway.Services.Helpers;

public static class BankRequestHelper
{
    public static BankRequest ConvertToBankRequest(PaymentRequest paymentRequest)
    {
        var card = paymentRequest.Card;
        return new BankRequest()
        {
            CardNumber = card.Number,
            ExpiryDate = GetExpiryDate(card.ExpiryMonth, card.ExpiryYear),
            Currency = paymentRequest.Currency,
            Amount = paymentRequest.Amount,
            CVV = card.Cvv
        };
    }

    private static string GetExpiryDate(int expiryMonth, int expiryYear)
    {
        return $"{expiryMonth:D2}/{expiryYear}";
    }
}