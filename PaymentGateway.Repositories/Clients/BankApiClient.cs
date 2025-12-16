using System.Net;
using System.Net.Http.Json;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using PaymentGateway.Common.Exceptions;
using PaymentGateway.Repositories.Clients.Settings;
using PaymentGateway.Repositories.Interfaces;
using PaymentGateway.Repositories.Models;
using PaymentGateway.Services.Models;

namespace PaymentGateway.Repositories.Clients;

public class BankApiClient : IBankAPiClient
{
    private readonly HttpClient _client;
    private readonly ILogger<BankApiClient> _logger;
    
    public BankApiClient(
        IOptions<BankApiSettings> options,
        IHttpClientFactory clientFactory,
        ILogger<BankApiClient> logger)
    {
        BankApiSettings bankApiSettings = options.Value;
        _client = clientFactory.CreateClient();
        _client.BaseAddress = new Uri(bankApiSettings.BaseUrl);
        _logger = logger;
    }

    public async Task<BankResponse> ProcessPayment(BankRequest paymentRequest)
    {
        var response = await _client.PostAsJsonAsync("", paymentRequest);

        if (response.IsSuccessStatusCode)
        {
            var bankResponse = await response.Content.ReadFromJsonAsync<BankResponse>();
            
            if(bankResponse == null)
            {
                _logger.LogError("Error, bank response empty");
                throw new RepositoryException("Bank Api call returned an empty response");
            }
            
            return bankResponse;
        }
       
        if(response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Error, there is a missing required field");
            throw new RepositoryException($"Bank Api call failed with Http status code: {response.StatusCode}");
        }

        if(response.StatusCode == HttpStatusCode.ServiceUnavailable)
        {
            _logger.LogError("Error, the bank service is unavailable");
            throw new RepositoryException($"Bank Api call failed with Http status code: {response.StatusCode}");
        }

        throw new RepositoryException("Unexpected error occurred while calling Bank API");
    }
}