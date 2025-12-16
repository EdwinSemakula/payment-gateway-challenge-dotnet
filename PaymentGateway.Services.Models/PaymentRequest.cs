namespace PaymentGateway.Services.Models;

public class PaymentRequest
{
    public Guid MerchantId { get; set; }
    public Card Card { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
}