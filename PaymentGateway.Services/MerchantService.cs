using Microsoft.Extensions.Logging;

using PaymentGateway.Repositories.Interfaces;
using PaymentGateway.Repositories.Models;
using PaymentGateway.Services.Interfaces;

namespace PaymentGateway.Services;

public class MerchantService(
    IMerchantRepository merchantRepository,
    ILogger<MerchantService> logger)
    : IMerchantService
{
    public void CreateMerchant(string merchantName)
    {
        var merchant = new Merchant()
        {
            Id = Guid.NewGuid(),
            Name = merchantName
        };
        
        logger.LogInformation($"Merchant created with ID: {merchant.Id} and Name: {merchant.Name}");

        merchantRepository.SaveMerchant(merchant);
    }
}