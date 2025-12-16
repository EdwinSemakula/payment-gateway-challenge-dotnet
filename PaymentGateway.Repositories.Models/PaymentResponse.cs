using System.Text.Json.Serialization;

using PaymentGateway.Common.Enums;

namespace PaymentGateway.Repositories.Models;

public class PaymentResponse
{
    public Guid Id { get; set; }
    [JsonIgnore]
    public Guid MerchantId { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PaymentStatus Status { get; set; }
    public string LastFourDigits { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
}