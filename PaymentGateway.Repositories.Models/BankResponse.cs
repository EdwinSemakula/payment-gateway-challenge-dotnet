using System.Text.Json.Serialization;

namespace PaymentGateway.Repositories.Models;

public class BankResponse
{
    public bool Authorized { get; init; }
    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; init; }
}