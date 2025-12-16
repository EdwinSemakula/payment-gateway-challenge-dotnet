using Microsoft.Extensions.Logging;

using Moq;

using PaymentGateway.Repositories.Interfaces;

namespace PaymentGateway.Services.UnitTests;

[TestFixture]
public class MerchantServiceTests
{
    private MerchantService _merchantService;
    private Mock<IMerchantRepository> _merchantRepositoryMock;
    private Mock<ILogger<MerchantService>> _loggerMock;

    [SetUp]
    public void Setup()
    {
        _merchantRepositoryMock = new Mock<IMerchantRepository>();
        _loggerMock = new Mock<ILogger<MerchantService>>();
        _merchantService = new MerchantService(
            _merchantRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public void WhenCreateMerchantIsCalled_ShouldSaveMerchant()
    {
        // Arrange
        var merchantName = "Test Merchant";
        
        // Act
       _merchantService.CreateMerchant(merchantName);
        
        // Assert
        _merchantRepositoryMock.Verify(
            repo => repo.SaveMerchant(It.Is<Repositories.Models.Merchant>(
                m => m.Name == merchantName)),
            Times.Once);
    }
}