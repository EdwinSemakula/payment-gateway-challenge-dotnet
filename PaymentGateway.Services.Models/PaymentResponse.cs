using PaymentGateway.Common.Enums;

namespace PaymentGateway.Services.Models;

public class PaymentResponse
{
    public Guid Id { get; set; }
    public Guid MerchantId { get; set; }
    public PaymentStatus Status { get; set; }
    public string LastFourDigits { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
}