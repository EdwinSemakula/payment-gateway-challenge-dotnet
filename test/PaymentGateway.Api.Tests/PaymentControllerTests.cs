using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Common.Enums;
using PaymentGateway.Repositories.Interfaces;
using PaymentGateway.Services.Interfaces;
using PaymentGateway.Services.Models;
using PaymentGateway.Services.Models.Validators;

using PaymentResponse = PaymentGateway.Repositories.Models.PaymentResponse;

namespace PaymentGateway.Api.Controller.UnitTests;

[TestFixture]
public class PaymentControllerTests
{
    private PaymentController _controller;
    private Mock<IPaymentRepository> _paymentsRepositoryMock;
    private Mock<IPaymentService> _paymentServiceMock;
    private Mock<IPaymentRequestValidator> _validatorMock;
    private Mock<ILogger<PaymentController>> _loggerMock;
 

    [SetUp]
    public void Setup()
    {
        _paymentsRepositoryMock = new Mock<IPaymentRepository>();
        _paymentServiceMock = new Mock<IPaymentService>();
        _validatorMock = new Mock<IPaymentRequestValidator>();
        _loggerMock = new Mock<ILogger<PaymentController>>();
        _controller = new PaymentController(
            _paymentsRepositoryMock.Object,
            _paymentServiceMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task WhenGetPaymentByIdsAsyncCalledWithPayment_ShouldReturnPayment()
    {
        // Arrange
        var merchantId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        var expectedPayment = CreateTestPaymentResponse(paymentId, merchantId);

        _paymentsRepositoryMock.Setup(repo =>
            repo.GetPaymentByIds(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(expectedPayment);

        // Act
        var result = await _controller.GetPaymentByIdsAsync(merchantId, paymentId);

        // Assert
        var actualPayment = AssertOkResult<PaymentResponse>(result);
        Assert.That(actualPayment.Id, Is.EqualTo(expectedPayment.Id));
    }

    [Test]
    public async Task WhenGetPaymentByIdsAsyncCalledWithNoPayment_ShouldReturnPayment()
    {
        // Arrange
        var merchantId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();

        _paymentsRepositoryMock.Setup(repo =>
                repo.GetPaymentByIds(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns((PaymentResponse)null);

        // Act
        var result = await _controller.GetPaymentByIdsAsync(merchantId, paymentId);

        // Assert
        AssertNotFoundResult(result.Result, "Payment not found.");
    }

    [Test]
    public async Task WhenGetPaymentsByMerchantIdAsyncCalledWithPayments_ShouldReturnPayments()
    {
        // Arrange
        var merchantId = Guid.NewGuid();
        var expectedPayments = new List<PaymentResponse>
        {
            CreateTestPaymentResponse(Guid.NewGuid(), merchantId)
        };

        _paymentsRepositoryMock.Setup(repo =>
                repo.GetPaymentsByMerchantId(It.IsAny<Guid>()))
            .Returns(expectedPayments);

        // Act
        var result = await _controller.GetPaymentsByMerchantIdAsync(merchantId);

        // Assert
        var actualPayments = AssertOkResult<IEnumerable<PaymentResponse>>(result) as List<PaymentResponse>;
        Assert.That(actualPayments.Count, Is.EqualTo(1));
        Assert.That(actualPayments.First().Amount, Is.EqualTo(expectedPayments.First().Amount));
    }

    [Test]
    public async Task WhenGetPaymentsByMerchantIdAsyncCalledWithNoPayments_ShouldReturnNotFoundObjectResult()
    {
        // Arrange
        var merchantId = Guid.NewGuid();

        _paymentsRepositoryMock.Setup(repo =>
                repo.GetPaymentsByMerchantId(It.IsAny<Guid>()))
            .Returns(new List<PaymentResponse>());

        // Act
        var result = await _controller.GetPaymentsByMerchantIdAsync(merchantId);

        // Assert
        AssertNotFoundResult(result.Result, "No payments found.");
    }

    [Test]
    public async Task WhenCreatePaymentAsyncCalledWithValidRequest_ShouldReturnNoContentResult()
    {
        // Arrange
        var merchantId = Guid.NewGuid();
        var paymentRequest = CreateTestPaymentRequest();

        _validatorMock.Setup(v => v.Validate(It.IsAny<Services.Models.PaymentRequest>()))
            .Returns(new ValidationResult() { Success = true });
        _paymentServiceMock.Setup(p => p.ProcessPayment(It.IsAny<Services.Models.PaymentRequest>()))
            .Returns(Task.CompletedTask);
        
        SetupControllerContext(merchantId);

        // Act
        var result = await _controller.CreatePaymentAsync(paymentRequest);

        // Assert
        Assert.IsInstanceOf<NoContentResult>(result);
        _paymentServiceMock.Verify(p => p.ProcessPayment(It.IsAny<Services.Models.PaymentRequest>()), Times.Once);
    }

    [Test]
    public async Task WhenCreatePaymentAsyncCalledWithInValidRequest_ShouldReturnBadObjectRequestResult()
    {
        // Arrange
        var merchantId = Guid.NewGuid();
        var paymentRequest = CreateTestPaymentRequest(amount: -100);
        var expectedErrorMessage = "Amount must be greater than zero.";

        _validatorMock.Setup(v => v.Validate(It.IsAny<Services.Models.PaymentRequest>()))
            .Returns(new ValidationResult() { Success = false, Message = expectedErrorMessage });
        
        SetupControllerContext(merchantId);

        // Act
        var result = await _controller.CreatePaymentAsync(paymentRequest);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.That(badRequestResult.Value, Is.EqualTo(expectedErrorMessage));
    }

    [TearDown]
    public void TearDown()
    {
        _controller?.Dispose();
    }

    private PaymentResponse CreateTestPaymentResponse(Guid paymentId, Guid merchantId)
    {
        return new PaymentResponse
        {
            Id = paymentId,
            MerchantId = merchantId,
            Status = PaymentStatus.Authorized,
            LastFourDigits = "1234",
            ExpiryMonth = 12,
            ExpiryYear = 2030,
            Currency = "USD",
            Amount = 1000
        };
    }

    private PaymentRequest CreateTestPaymentRequest(int amount = 1000)
    {
        return new PaymentRequest
        {
            Card = new Card
            {
                Number = "4111111111111111",
                ExpiryMonth = 12,
                ExpiryYear = 2030,
                Cvv = "123"
            },
            Currency = "USD",
            Amount = amount
        };
    }

    private void SetupControllerContext(Guid merchantId)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["merchantId"] = merchantId.ToString();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    private T AssertOkResult<T>(ActionResult<T> result)
    {
        Assert.IsInstanceOf<ActionResult<T>>(result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        return (T)okResult.Value;
    }

    private void AssertNotFoundResult(IActionResult result, string expectedMessage)
    {
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.That(notFoundResult.Value, Is.EqualTo(expectedMessage));
    }
}