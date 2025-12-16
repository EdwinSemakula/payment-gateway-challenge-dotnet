using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Repositories.Interfaces;
using PaymentGateway.Repositories.Models;
using PaymentGateway.Services.Interfaces;

namespace PaymentGateway.Api.Controllers;

[Route("merchant")]
[ApiController]
public class MerchantController(
    IMerchantService merchantService,
    IMerchantRepository merchantRepository,
    ILogger<MerchantController> logger)
    : Controller
{
    [HttpPost("create")]
    public async Task<ActionResult> CreateMerchantAsync(string merchantName)
    {
        try
        {
            merchantService.CreateMerchant(merchantName);

            return new NoContentResult();
        }
        catch(Exception exception)
        {
            logger.LogError(exception, "Error creating merchant.");
            return StatusCode(500, "Error creating merchant.");
        }
   
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<Merchant>>> GetAllMerchantsAsync()
    {
        var merchants = merchantRepository.GetAllMerchants();
        
        return new OkObjectResult(merchants);
    }
}