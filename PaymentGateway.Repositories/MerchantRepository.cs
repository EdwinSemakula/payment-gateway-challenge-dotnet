using Microsoft.Extensions.Logging;

using PaymentGateway.Repositories.Interfaces;
using PaymentGateway.Repositories.Models;

namespace PaymentGateway.Repositories;

public class MerchantRepository(ILogger<MerchantRepository> logger) : IMerchantRepository
{
    public List<Merchant> Merchants = new();

    public void SaveMerchant(Merchant merchant)
    {
        Merchants.Add(merchant);
        
        logger.LogInformation("Merchant saved.");
    }

    public IEnumerable<Merchant> GetAllMerchants()
    {
        return Merchants;
    }
}