using System.Text.RegularExpressions;

using PaymentGateway.Common.Enums;

namespace PaymentGateway.Services.Models.Validators;

public class PaymentRequestValidator : IPaymentRequestValidator
{
    public ValidationResult Validate(PaymentRequest paymentRequest)
    {
        var validationResult = new ValidationResult
        {
            Message = BuildErrorMessage(ValidatePaymentRequest(paymentRequest))
                      + BuildErrorMessage(ValidateCardDetails(paymentRequest))
                      + BuildErrorMessage(ValidateRequestParams(paymentRequest)),
        };

        validationResult.Success = string.IsNullOrEmpty(validationResult.Message);

        return validationResult;
    }

    private List<string> ValidatePaymentRequest(PaymentRequest paymentRequest)
    {
        var errors = new List<string>();
        if (paymentRequest == null)
        {
            errors.Add("Payment request must be supplied.");
        }
        return errors;
    }

    private List<string> ValidateCardDetails(PaymentRequest paymentRequest)
    {
        var errors = new List<string>();

        if (paymentRequest != null)
        {
            var card = paymentRequest.Card;
            
            if (card == null)
            {
                errors.Add("Card details must be supplied.");
            }
            else
            {
                if (string.IsNullOrEmpty(card.Number))
                {
                    errors.Add("Card number must be supplied.");
                    return errors;
                }
                if (card.Number.Length != 16)
                {
                    errors.Add("Card number must be 16 digits long.");
                }
                if (!IsNumbersOnly(card.Number))
                {
                    errors.Add("Card number must only contain digits.");
                }
                if (card.ExpiryMonth < 1 || card.ExpiryMonth > 12)
                {
                    errors.Add("Expiry month must be between 1 and 12.");
                }
                if (card.ExpiryYear < DateTime.Now.Year)
                {
                    errors.Add("Expiry year cannot be in the past.");
                }
                else if (card.ExpiryYear == DateTime.Now.Year && card.ExpiryMonth <= DateTime.Now.Month)
                {
                    errors.Add("Card has already expired.");
                }
                if (card.Cvv.Length != 3)
                {
                    errors.Add("CVV must be 3 digits long.");
                }
                if (!IsNumbersOnly(card.Cvv))
                {
                    errors.Add("CVV must only contain digits.");
                }
            }
        }

        return errors;
    }

    private List<string> ValidateRequestParams(PaymentRequest paymentRequest)
    {
        var errors = new List<string>();

        if (paymentRequest != null)
        {
            if (paymentRequest.MerchantId == Guid.Empty)
            {
                errors.Add("MerchantId must be supplied");
            }
            if (paymentRequest.Amount == null)
            {
                errors.Add("Amount must be supplied");
            }
            if (paymentRequest.Amount <= 0)
            {
                errors.Add("Amount must be greater than zero");
            }
            if (string.IsNullOrEmpty(paymentRequest.Currency))
            {
                errors.Add("Currency must be supplied");
            }
            else if (!Enum.GetNames(typeof(Currency))
                         .Any(x => x.Equals(paymentRequest.Currency, StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add("Currency supplied is invalid");
            }
        }

        return errors;
    }

    private static bool IsNumbersOnly(string value)
    {
        var pattern = @"^-?[0-9]+(?:\.[0-9]+)?$";
        var regex = new Regex(pattern);
        return regex.IsMatch(value);
    }

    private string BuildErrorMessage(List<string> errors)
    {
        if (errors != null && errors.Any())
        {
            return string.Join(Environment.NewLine, errors.Select((error, index) => index < errors.Count - 1 ? error + Environment.NewLine : error));
        }
        return string.Empty;
    }
}