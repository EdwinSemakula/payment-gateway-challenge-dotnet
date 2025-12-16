using AutoMapper;

using Microsoft.Extensions.Logging;

using Moq;

using PaymentGateway.Common.Enums;
using PaymentGateway.Repositories.Models;
using Service = PaymentGateway.Services.Models;

namespace PaymentGateway.Repositories.UnitTests;

[TestFixture]
public class PaymentRepositoryTests
{
    private PaymentRepository _paymentRepository;
    private Mock<IMapper> _mapperMock;
    private Mock<ILogger<PaymentRepository>> _loggerMock;

    [SetUp]
    public void Setup()
    {
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<PaymentRepository>>();
        _paymentRepository = new PaymentRepository(_mapperMock.Object, _loggerMock.Object);
    }

    [Test]
    public void WhenSavePaymentCalled_ShouldSavePayment()
    {
        // Arrange
        var merchantId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        var servicePayment = CreateServicePayment(paymentId, merchantId, PaymentStatus.Authorized, "1234", 12, 2030, "USD", 100);
        SetupMapperWithStandardMapping();

        // Act
        _paymentRepository.SavePayment(servicePayment);

        // Assert
        var payments = _paymentRepository.GetAllPayments();

        Assert.That(payments, Has.Exactly(1).Matches<PaymentResponse>(p => 
            p.Id == servicePayment.Id &&
            p.MerchantId == servicePayment.MerchantId &&
            p.Status == servicePayment.Status &&
            p.LastFourDigits == servicePayment.LastFourDigits &&
            p.ExpiryMonth == servicePayment.ExpiryMonth &&
            p.ExpiryYear == servicePayment.ExpiryYear &&
            p.Currency == servicePayment.Currency &&
            p.Amount == servicePayment.Amount));
    }

    [Test]
    public void WhenValidRequestForPaymentsByMerchantId_ShouldReturnPayments()
    {
        // Arrange
        var merchantId = Guid.NewGuid();

        var servicePayments = new List<Service.PaymentResponse>()
        {
            CreateServicePayment(Guid.NewGuid(), merchantId, PaymentStatus.Authorized, "1234", 12, 2030, "USD", 100),
            CreateServicePayment(Guid.NewGuid(), Guid.NewGuid(), PaymentStatus.Declined, "5678", 11, 2030, "EUR", 200)
        };

        SetupMapperWithStandardMapping();

        foreach (var payment in servicePayments)
        {
            _paymentRepository.SavePayment(payment);
        }

        // Act
        var merchantPayments = _paymentRepository.GetPaymentsByMerchantId(merchantId);

        // Assert
        Assert.That(merchantPayments.Count(), Is.EqualTo(1));
        Assert.That(merchantPayments.First().Amount, Is.EqualTo(100));
        Assert.That(merchantPayments.First().MerchantId, Is.EqualTo(merchantId));
    }

    [Test]
    public void WhenValidRequestForPaymentsByIds_ShouldReturnPayment()
    {
        // Arrange
        var merchantId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        var servicePayment = CreateServicePayment(paymentId, merchantId, PaymentStatus.Authorized, "1234", 12, 2030, "USD", 100);
        
        SetupMapperWithStandardMapping();
        _paymentRepository.SavePayment(servicePayment);

        // Act
        var retrievedPayment = _paymentRepository.GetPaymentByIds(merchantId, paymentId);

        // Assert
        Assert.IsNotNull(retrievedPayment);
        Assert.That(retrievedPayment.Id, Is.EqualTo(paymentId));
        Assert.That(retrievedPayment.Amount, Is.EqualTo(100));
    }

    private Service.PaymentResponse CreateServicePayment(Guid id, Guid merchantId, PaymentStatus status, 
        string lastFourDigits, int expiryMonth, int expiryYear, string currency, int amount)
    {
        return new Service.PaymentResponse()
        {
            Id = id,
            MerchantId = merchantId,
            Status = status,
            LastFourDigits = lastFourDigits,
            ExpiryMonth = expiryMonth,
            ExpiryYear = expiryYear,
            Currency = currency,
            Amount = amount
        };
    }

    private void SetupMapperWithStandardMapping()
    {
        _mapperMock
            .Setup(mapper => mapper.Map<PaymentResponse>(It.IsAny<Service.PaymentResponse>()))
            .Returns((Service.PaymentResponse sp) => new PaymentResponse()
            {
                Id = sp.Id,
                MerchantId = sp.MerchantId,
                Status = sp.Status,
                LastFourDigits = sp.LastFourDigits,
                ExpiryMonth = sp.ExpiryMonth,
                ExpiryYear = sp.ExpiryYear,
                Currency = sp.Currency,
                Amount = sp.Amount
            });
    }
}