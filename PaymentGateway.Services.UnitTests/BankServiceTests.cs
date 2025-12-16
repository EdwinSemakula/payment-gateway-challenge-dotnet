using Microsoft.Extensions.Logging;

using Moq;

using PaymentGateway.Common.Exceptions;
using PaymentGateway.Repositories.Interfaces;
using PaymentGateway.Repositories.Models;
using PaymentGateway.Services.Models;

namespace PaymentGateway.Services.UnitTests;

[TestFixture]
public class BankServiceTests
{
    private BankService _bankService;
    private Mock<ILogger<BankService>> _loggerMock;
    private Mock<IBankAPiClient> _bankApiClientMock;

    BankResponse mockBankResponse = new BankResponse
    {
        Authorized = false,
        AuthorizationCode = "dummy"
    };

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<BankService>>();
        _bankApiClientMock = new Mock<IBankAPiClient>();
        _bankService = new BankService(_bankApiClientMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task WhenBankAPIClientReturnsException_ShouldThrowException()
    {
        // Arrange
        var paymentRequest = CreatePaymentRequest();

        _bankApiClientMock
            .Setup(client => client.ProcessPayment(It.IsAny<BankRequest>()))
            .ThrowsAsync(new RepositoryException("Unexpected error occurred while calling Bank API."));

        var exceptionMessage = "Unexpected error occurred while calling Bank API.";

        // Act & Assert
        var ex = Assert.ThrowsAsync<ServiceException>(async () =>
            await _bankService.GetBankResponse(paymentRequest));
        Assert.That(ex.Message, Is.EqualTo(exceptionMessage));
    }

    [Test]
    public async Task WhenPaymentRequestIsValid_ShouldReturnBankResponse()
    {
        // Arrange
        var paymentRequest = CreatePaymentRequest();

        _bankApiClientMock
            .Setup(client => client.ProcessPayment(It.IsAny<BankRequest>()))
            .ReturnsAsync(mockBankResponse);

        // Act
        var response = await _bankService.GetBankResponse(paymentRequest);

        // Assert
        Assert.IsInstanceOf<BankResponse>(response);
        Assert.That(response.Authorized, Is.False);
    }

    public PaymentRequest CreatePaymentRequest()
    {
        return new PaymentRequest
        {
            MerchantId = Guid.NewGuid(),
            Card = new Card
            {
                Number = "2345678901234560",
                ExpiryMonth = 12,
                ExpiryYear = 2028,
            },
            Currency = "USD",
            Amount = 100
        };
    }
}