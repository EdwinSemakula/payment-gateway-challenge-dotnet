using Microsoft.Extensions.Logging;

using Moq;

using PaymentGateway.Common.Exceptions;
using PaymentGateway.Repositories.Interfaces;
using PaymentGateway.Repositories.Models;
using PaymentGateway.Services.Interfaces;
using PaymentGateway.Services.Models;

using PaymentResponse = PaymentGateway.Services.Models.PaymentResponse;

namespace PaymentGateway.Services.UnitTests;

[TestFixture]
public class PaymentServiceTests
{
    private PaymentService _paymentService;
    private Mock<IPaymentRepository> _paymentRepositoryMock;
    private Mock<IBankService> _bankServiceMock;
    private Mock<ILogger<PaymentService>> _loggerMock;

    [SetUp]
    public void Setup()
    {
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _bankServiceMock = new Mock<IBankService>();
        _loggerMock = new Mock<ILogger<PaymentService>>();
        _paymentService = new PaymentService(
            _paymentRepositoryMock.Object,
            _bankServiceMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public Task WhenBankServiceThrowsException_ShouldThrowServiceException()
    {
        // Arrange
        _bankServiceMock
            .Setup(service => service.GetBankResponse(It.IsAny<PaymentRequest>()))
            .ThrowsAsync(new ServiceException("Bank service error"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<ServiceException>(async () =>
            await _paymentService.ProcessPayment(It.IsAny<PaymentRequest>()));
        Assert.That(ex.Message, Is.EqualTo("Bank service error"));
        return Task.CompletedTask;
    }

    [Test]
    public async Task WhenPaymentIsProcessed_ShouldSavePaymentResponse()
    {
        // Arrange
        var paymentRequest = new PaymentRequest
        {
            MerchantId = Guid.NewGuid(),
            Card = new Card
            {
                Number = "2345678901234562",
                ExpiryMonth = 12,
                ExpiryYear = 2028,
            },
            Currency = "USD",
            Amount = 100
        };
    
        var bankResponse = new BankResponse
        {
            Authorized = true
        };

        _bankServiceMock
            .Setup(service => service.GetBankResponse(It.IsAny<PaymentRequest>()))
            .ReturnsAsync(bankResponse);

        // Act
        await _paymentService.ProcessPayment(paymentRequest);

        // Assert
        _paymentRepositoryMock.Verify(repo => repo.SavePayment(It.IsAny<PaymentResponse>()), Times.Once);
    }
}