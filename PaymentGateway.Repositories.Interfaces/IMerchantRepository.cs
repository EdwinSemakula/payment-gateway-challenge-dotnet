using PaymentGateway.Repositories.Models;

namespace PaymentGateway.Repositories.Interfaces;

public interface IMerchantRepository
{
    void SaveMerchant(Merchant merchant);
    IEnumerable<Merchant> GetAllMerchants();
}