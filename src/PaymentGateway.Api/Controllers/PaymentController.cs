using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Repositories.Interfaces;
using PaymentGateway.Services.Interfaces;
using PaymentGateway.Services.Models;
using PaymentGateway.Services.Models.Validators;

using PaymentResponse = PaymentGateway.Repositories.Models.PaymentResponse;

namespace PaymentGateway.Api.Controllers;

[Route("payment")]
[ApiController]
public class PaymentController(
    IPaymentRepository paymentsRepository,
    IPaymentService paymentService,
    IPaymentRequestValidator validator,
    ILogger<PaymentController> logger)
    : Controller
{
    // For admin use only
    [HttpGet("all")]
    public async Task<ActionResult<PaymentResponse?>> GetAllPaymentsAsync()
    {
        var payments = paymentsRepository.GetAllPayments();

        return new OkObjectResult(payments);
    }

    [HttpGet("{merchantId:guid}/{paymentId:guid}")]
    public async Task<ActionResult<PaymentResponse?>> GetPaymentByIdsAsync(Guid merchantId, Guid paymentId)
    {
        var payment = paymentsRepository.GetPaymentByIds(merchantId, paymentId);

        if (payment == null)
        {
            return new NotFoundObjectResult("Payment not found.");
        }

        return new OkObjectResult(payment);
    }

    [HttpGet("{merchantId:guid}/all")]
    public async Task<ActionResult<IEnumerable<PaymentResponse?>>> GetPaymentsByMerchantIdAsync(Guid merchantId)
    {
        var payments = paymentsRepository.GetPaymentsByMerchantId(merchantId);

        if (!payments.Any())
        {
            return new NotFoundObjectResult("No payments found.");
        }

        return new OkObjectResult(payments);
    }

    [HttpPost("create/{merchantId:guid}")]
    public async Task<ActionResult> CreatePaymentAsync(PaymentRequest paymentRequest)
    {
        paymentRequest.MerchantId = Guid.Parse(HttpContext.Request.RouteValues["merchantId"]?.ToString() ?? string.Empty);

        try
        {
            var validationResult = validator.Validate(paymentRequest);
            if (!validationResult.Success)
            {
                return new BadRequestObjectResult(validationResult.Message);
            }

            await paymentService.ProcessPayment(paymentRequest);

            logger.LogInformation("Payment processed successfully.");
            return new NoContentResult();

        }
        catch (Exception exception)
        {
            logger.LogError(exception,"Error creating payment.");
            return StatusCode(500, "Error creating payment.");
        }
    }
}