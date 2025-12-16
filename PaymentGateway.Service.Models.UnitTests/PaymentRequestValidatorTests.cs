using PaymentGateway.Services.Models;
using PaymentGateway.Services.Models.Validators;

namespace PaymentGateway.Service.Models.UnitTests
{
    [TestFixture]
    public class PaymentRequestValidatorTests
    {
        private PaymentRequestValidator _validator;
        private PaymentRequest _request;

        [SetUp]
        public void Setup()
        {
            _validator = new PaymentRequestValidator();
            _request = CreatePaymentRequest();
        }

        [Test]
        public void WhenValidRequest_ShouldReturnSuccess()
        {
            // Act
            var validationResult = _validator.Validate(_request);

            // Assert
            Assert.That(validationResult.Success, Is.True);
            Assert.That(validationResult.Message, Is.Empty);
        }


        [Test]
        public void WhenInvalidRequestWithNull_ShouldReturnErrorMessage()
        {
            // Act
            var validationResult = _validator.Validate(null);

            // Assert
            Assert.That(validationResult.Success, Is.False);
            Assert.That(validationResult.Message, Is.EqualTo("Payment request must be supplied."));
        }

        [Test]
        public void WhenCardNumberNotGiven_ShouldReturnErrorMessage()
        {
            // Arrange
            _request.Card.Number = null;

            // Act
            var validationResult = _validator.Validate(_request);

            // Assert
            Assert.That(validationResult.Success, Is.False);
            Assert.That(validationResult.Message, Is.EqualTo("Card number must be supplied."));
        }

        [Test]
        public void WhenInvalidCardNumberLength_ShouldReturnErrorMessage()
        {
            // Arrange
            _request.Card.Number = "123";

            // Act
            var validationResult = _validator.Validate(_request);

            // Assert
            Assert.That(validationResult.Success, Is.False);
            Assert.That(validationResult.Message, Is.EqualTo("Card number must be 16 digits long."));
        }

        [Test]
        public void WhenCardNumberContainsLetters_ShouldReturnErrorMessage()
        {
            // Arrange
            _request.Card.Number = "22224E53432488B9"; 

            // Act
            var validationResult = _validator.Validate(_request);

            // Assert
            Assert.That(validationResult.Success, Is.False);
            Assert.That(validationResult.Message, Is.EqualTo("Card number must only contain digits."));
        }

        [Test]
        public void WhenExpiryMonthIsInvalid_ShouldReturnErrorMessage()
        {
            // Arrange
            _request.Card.ExpiryMonth = 20;

            // Act
            var validationResult = _validator.Validate(_request);

            // Assert
            Assert.That(validationResult.Success, Is.False);
            Assert.That(validationResult.Message, Is.EqualTo("Expiry month must be between 1 and 12."));
        }

        [Test]
        public void WhenExpiryYearIsInvalid_ShouldReturnErrorMessage()
        {
            // Arrange
            _request.Card.ExpiryYear = 2000;

            // Act
            var validationResult = _validator.Validate(_request);

            // Assert
            Assert.That(validationResult.Success, Is.False);
            Assert.That(validationResult.Message, Is.EqualTo("Expiry year cannot be in the past."));
        }

        [Test]
        public void WhenExpiryMontAndYearIsInvalid_ShouldReturnErrorMessage()
        {
            // Arrange
            _request.Card.ExpiryMonth = DateTime.Now.Month;
            _request.Card.ExpiryYear = DateTime.Now.Year;

            // Act
            var validationResult = _validator.Validate(_request);

            // Assert
            Assert.That(validationResult.Success, Is.False);
            Assert.That(validationResult.Message, Is.EqualTo("Card has already expired."));
        }

        [Test]
        public void WhenInvalidCVVLength_ShouldReturnErrorMessage()
        {
            // Arrange
            _request.Card.Cvv = "1234";

            // Act
            var validationResult = _validator.Validate(_request);

            // Assert
            Assert.That(validationResult.Success, Is.False);
            Assert.That(validationResult.Message, Is.EqualTo("CVV must be 3 digits long."));
        }

        [Test]
        public void WhenCVVContainsLetter_ShouldReturnErrorMessage()
        {
            // Arrange
            _request.Card.Cvv = "12E";

            // Act
            var validationResult = _validator.Validate(_request);

            // Assert
            Assert.That(validationResult.Success, Is.False);
            Assert.That(validationResult.Message, Is.EqualTo("CVV must only contain digits."));
        }

        private PaymentRequest CreatePaymentRequest()
        {
            return new PaymentRequest
            {
                Card = new Card()
                {
                    Number = "2222405343248879",
                    ExpiryMonth = 12,
                    ExpiryYear = 2030,
                    Cvv = "123"
                },
                MerchantId = Guid.NewGuid(),
                Currency = "USD",
                Amount = 100
            };
        }
    }
}