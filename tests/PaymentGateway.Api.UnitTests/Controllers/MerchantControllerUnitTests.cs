using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Repositories.Interfaces;
using PaymentGateway.Repositories.Models;
using PaymentGateway.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentGateway.Api.UnitTests.Controllers;

[TestFixture]
public class MerchantControllerTests
{
    private MerchantController _merchantController;
    private Mock<IMerchantService> _merchantServiceMock;
    private Mock<IMerchantRepository> _merchantRepositoryMock;
    private Mock<ILogger<MerchantController>> _loggerMock;

    [SetUp]
    public void Setup()
    {
        _merchantServiceMock = new Mock<IMerchantService>();
        _merchantRepositoryMock = new Mock<IMerchantRepository>();
        _loggerMock = new Mock<ILogger<MerchantController>>();

        _merchantController = new MerchantController(
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
        var result = await _merchantController.CreateMerchantAsync(merchantName);

        // Assert
        result.Should().BeOfType<NoContentResult>();
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
        var result = await _merchantController.CreateMerchantAsync(merchantName);

        // Assert
        var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
        statusCodeResult.Value.Should().Be("Error creating merchant.");
    }

    [Test]
    public async Task GetAllMerchantsAsync_ShouldReturnOkWithMerchantsList()
    {
        // Arrange
        var expectedMerchants = new List<Merchant>
        {
            new Merchant { Id = Guid.NewGuid(), Name = "Merchant 1" },
            new Merchant { Id = Guid.NewGuid(), Name = "Merchant 2" }
        };

        _merchantRepositoryMock.Setup(r => r.GetAllMerchants())
            .Returns(expectedMerchants);

        // Act
        var result = await _merchantController.GetAllMerchantsAsync();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var merchants = okResult.Value.Should().BeAssignableTo<IEnumerable<Merchant>>().Subject;
        merchants.Should().BeEquivalentTo(expectedMerchants);
        _merchantRepositoryMock.Verify(r => r.GetAllMerchants(), Times.Once);
    }

    [Test]
    public async Task GetAllMerchantsAsync_WhenNoMerchants_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        var expectedMerchants = new List<Merchant>();

        _merchantRepositoryMock.Setup(r => r.GetAllMerchants())
            .Returns(expectedMerchants);

        // Act
        var result = await _merchantController.GetAllMerchantsAsync();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var merchants = okResult.Value.Should().BeAssignableTo<IEnumerable<Merchant>>().Subject;
        merchants.Should().BeEmpty();
    }
}