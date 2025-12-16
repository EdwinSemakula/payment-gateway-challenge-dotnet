namespace PaymentGateway.Services.Models.Validators;

public interface IPaymentRequestValidator
{
    ValidationResult Validate(PaymentRequest paymentRequest);
}