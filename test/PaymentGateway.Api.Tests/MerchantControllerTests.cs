using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Repositories.Interfaces;
using PaymentGateway.Repositories.Models;
using PaymentGateway.Services.Interfaces;

namespace PaymentGateway.Api.Controller.UnitTests;

[TestFixture]
public class MerchantControllerTests
{
    private MerchantController _controller;
    private Mock<IMerchantService> _merchantServiceMock;
    private Mock<IMerchantRepository> _merchantRepositoryMock;
    private Mock<ILogger<MerchantController>> _loggerMock;

    [SetUp]
    public void Setup()
    {
        _merchantServiceMock = new Mock<IMerchantService>();
        _merchantRepositoryMock = new Mock<IMerchantRepository>();
        _loggerMock = new Mock<ILogger<MerchantController>>();

        _controller = new MerchantController(
            _merchantServiceMock.Object,
            _merchantRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task CreateMerchantAsync_WhenSuccessful_ShouldReturnNoContent()
    {
        // Arrange
        var merchantName = "Test Merchant";
        _merchantServiceMock.Setup(s => s.CreateMerchant(merchantName));

        // Act
        var result = await _controller.CreateMerchantAsync(merchantName);

        // Assert
        Assert.IsInstanceOf<NoContentResult>(result);
        _merchantServiceMock.Verify(s => s.CreateMerchant(merchantName), Times.Once);
    }

    [Test]
    public async Task CreateMerchantAsync_WhenExceptionThrown_ShouldReturnInternalServerError()
    {
        // Arrange
        var merchantName = "Test Merchant";
        var expectedException = new Exception("Test exception");
        _merchantServiceMock.Setup(s => s.CreateMerchant(merchantName))
            .Throws(expectedException);

        // Act
        var result = await _controller.CreateMerchantAsync(merchantName);

        // Assert
        Assert.IsInstanceOf<ObjectResult>(result);
        var objectResult = (ObjectResult)result;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        Assert.That(objectResult.Value, Is.EqualTo("Error creating merchant."));
    }

    [Test]
    public async Task GetAllMerchantsAsync_ShouldReturnOkWithMerchantsList()
    {
        // Arrange
        var expectedMerchants = new List<Merchant>
        {
            new Merchant { Id = Guid.NewGuid(), Name = "Test Merchant 1" },
            new Merchant { Id = Guid.NewGuid(), Name = "Test Merchant 2" }
        };

        _merchantRepositoryMock.Setup(r => r.GetAllMerchants())
            .Returns(expectedMerchants);

        // Act
        var result = await _controller.GetAllMerchantsAsync();

        // Assert
        Assert.IsInstanceOf<ActionResult<List<Merchant>>>(result);
        var okResult = result.Result as OkObjectResult;
        var merchants = okResult.Value as List<Merchant>;
        Assert.That(merchants.Count, Is.EqualTo(expectedMerchants.Count));
    }

    [TearDown]
    public void TearDown()
    {
        _controller?.Dispose();
    }
}