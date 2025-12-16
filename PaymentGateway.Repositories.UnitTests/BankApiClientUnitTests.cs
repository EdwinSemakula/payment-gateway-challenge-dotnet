using System.Net;
using System.Text.Json;

using FluentAssertions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

using PaymentGateway.Common.Exceptions;
using PaymentGateway.Repositories.Clients;
using PaymentGateway.Repositories.Clients.Settings;
using PaymentGateway.Repositories.Models;
using PaymentGateway.Repositories.UnitTests.Stubs;
using PaymentGateway.Services.Models;

namespace PaymentGateway.Repositories.UnitTests;

[TestFixture]
public class BankApiClientUnitTests
{
    private BankApiClient _bankClient;
    private BankApiSettings _bankApiSettings;
    private DelegatingHandlerStub _delegatingHandlerStub;
    private Mock<ILogger<BankApiClient>> _loggerMock;


    [SetUp]
    public void Setup()
    {
        _bankApiSettings = new BankApiSettings
        {
            BaseUrl = "http://localhost/payments",
        };

        var optionsMock = new Mock<IOptions<BankApiSettings>>();
        optionsMock.Setup(o => o.Value).Returns(_bankApiSettings);

        _delegatingHandlerStub = new DelegatingHandlerStub();
        var clientFactoryMock = new Mock<IHttpClientFactory>();
        clientFactoryMock.Setup(cf => cf.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(_delegatingHandlerStub));
        _loggerMock = new Mock<ILogger<BankApiClient>>();
        
        _bankClient = new BankApiClient(optionsMock.Object,
            clientFactoryMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task WhenBankApiReturnsBadRequest_ShouldThrowException()
    {
        // Arrange
        var bankRequest = CreateBankRequest();

        _delegatingHandlerStub.SetResponseMessage(HttpStatusCode.BadRequest);

        // Act
        Func<Task> action = async () => await _bankClient.ProcessPayment(bankRequest);

        // Assert
       await action.Should().ThrowAsync<RepositoryException>();
    }

    [Test]
    public async Task WhenBankApiReturnsServiceUnavailable_ShouldThrowException()
    {
        // Arrange
        var bankRequest = CreateBankRequest();

        _delegatingHandlerStub.SetResponseMessage(HttpStatusCode.ServiceUnavailable);

        // Act
        Func<Task> action = async () => await _bankClient.ProcessPayment(bankRequest);

        // Assert
        await action.Should().ThrowAsync<RepositoryException>();
    }

    [Test]
    public async Task WhenBankApiReturnsSuccessCodeNullContent_ShouldThrowException()
    {
        // Arrange
        var bankRequest = CreateBankRequest();
        BankResponse? expectedBankResponse = null;

        _delegatingHandlerStub.SetResponseMessage(HttpStatusCode.OK, JsonSerializer.Serialize(expectedBankResponse));

        // Act
        Func<Task> action = async () => await _bankClient.ProcessPayment(bankRequest);

        // Assert
        await action.Should().ThrowAsync<RepositoryException>();
    }

    [Test]
    public async Task WhenBankApiReturnsSuccessCodeWithContent_ShouldReturnBankResponse()
    {
        // Arrange
        var bankRequest = CreateBankRequest();
        var expectedBankResponse = new BankResponse()
        {
            Authorized = true,
            AuthorizationCode = "dummy"
        };

        _delegatingHandlerStub.SetResponseMessage(HttpStatusCode.OK, JsonSerializer.Serialize(expectedBankResponse));
        
        // Act
        var actualBankResponse = await _bankClient.ProcessPayment(bankRequest);
        
        // Assert
        actualBankResponse.Should().BeEquivalentTo(expectedBankResponse);
    }

    [TearDown]
    public void TearDown()
    {
        _delegatingHandlerStub?.Dispose();
    }

    private BankRequest CreateBankRequest()
    {
        return new BankRequest
        {
            CardNumber = "4111111111111111",
            ExpiryDate = "12/30",
            Currency = "USD",
            Amount = 1000,
            CVV = "123",
        };
    }
}