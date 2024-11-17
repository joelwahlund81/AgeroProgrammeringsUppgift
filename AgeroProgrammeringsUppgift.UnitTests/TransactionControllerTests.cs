using AgeroProgrammeringsUppgift.Controllers;
using AgeroProgrammeringsUppgift.Models;
using AgeroProgrammeringsUppgift.Services;
using Microsoft.AspNetCore.Mvc;

public class TransactionControllerTests
{
    private readonly Mock<IAccountService> _accountServiceMock;
    private readonly TransactionController _controller;

    public TransactionControllerTests()
    {
        _accountServiceMock = new Mock<IAccountService>();
        _controller = new TransactionController(_accountServiceMock.Object);
    }

    public class AddTransaction : TransactionControllerTests
    {
        [Fact]
        public void AddTransaction_NullTransaction_ReturnsBadRequest()
        {
            // Act
            var result = _controller.AddTransaction(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input: model is missing.", badRequestResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void AddTransaction_InvalidFromAccount_ReturnsBadRequest(string fromAccountName)
        {
            // Arrange
            var transaction = new AccountBalanceTransaction { FromAccount = fromAccountName, ToAccount = "AccountB", Amount = 0 };

            // Act
            var result = _controller.AddTransaction(transaction);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input: from account cannot be null, empty or only consist of whitespace.", badRequestResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void AddTransaction_InvalidToAccount_ReturnsBadRequest(string toAccountName)
        {
            // Arrange
            var transaction = new AccountBalanceTransaction { FromAccount = "AccountA", ToAccount = toAccountName, Amount = 0 };

            // Act
            var result = _controller.AddTransaction(transaction);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input: to account cannot be null, empty or only consist of whitespace.", badRequestResult.Value);
        }

        [Fact]
        public void AddTransaction_AmountTooHigh_ReturnsBadRequest()
        {
            // Arrange
            var transaction = new AccountBalanceTransaction { FromAccount = "AccountA", ToAccount = "AccountB", Amount = 51000 };

            // Act
            var result = _controller.AddTransaction(transaction);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input: amount is too high, maximum is 50 000 per transaction.", badRequestResult.Value);
        }

        [Fact]
        public void AddTransaction_AmountIsNegative_ReturnsBadRequest()
        {
            // Arrange
            var transaction = new AccountBalanceTransaction { FromAccount = "AccountA", ToAccount = "AccountB", Amount = -10 };

            // Act
            var result = _controller.AddTransaction(transaction);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input: amount is too low, minimum is 0 per transaction.", badRequestResult.Value);
        }

        [Fact]
        public void AddTransaction_AmountIsNotInteger_ReturnsBadRequest()
        {
            // Arrange
            var transaction = new AccountBalanceTransaction { FromAccount = "AccountA", ToAccount = "AccountB", Amount = 10.5M };

            // Act
            var result = _controller.AddTransaction(transaction);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input: amount is not a integer.", badRequestResult.Value);
        }

        [Fact]
        public void AddTransaction_FromAccountDoesntExist_ReturnsBadRequest()
        {
            // Arrange
            _accountServiceMock
                .Setup(service => service.TryPerformTransaction(It.IsAny<AccountBalanceTransaction>()))
                .Returns(new AccountBalanceTransactionResult
                {
                    Message = "From account doesn't seem to exist.",
                    Success = false
                });

            var transaction = new AccountBalanceTransaction { FromAccount = "AccountA", ToAccount = "AccountB", Amount = 0 };

            // Act
            var result = _controller.AddTransaction(transaction);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("From account doesn't seem to exist.", badRequestResult.Value);
        }

        [Fact]
        public void AddTransaction_ToAccountDoesntExist_ReturnsBadRequest()
        {
            // Arrange
            _accountServiceMock
                .Setup(service => service.TryPerformTransaction(It.IsAny<AccountBalanceTransaction>()))
                .Returns(new AccountBalanceTransactionResult
                {
                    Message = "To account doesn't seem to exist.",
                    Success = false
                });

            var transaction = new AccountBalanceTransaction { FromAccount = "AccountA", ToAccount = "AccountB", Amount = 0 };

            // Act
            var result = _controller.AddTransaction(transaction);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("To account doesn't seem to exist.", badRequestResult.Value);
        }

        [Fact]
        public void AddTransaction_TransactionFails_ReturnsBadRequest()
        {
            // Arrange
            var transaction = new AccountBalanceTransaction { FromAccount = "AccountA", ToAccount = "AccountB", Amount = 0 };
            _accountServiceMock.Setup(service => service.TryPerformTransaction(transaction)).Returns(new AccountBalanceTransactionResult
            {
                Success = false,
                Message = "Insufficient funds"
            });

            // Act
            var result = _controller.AddTransaction(transaction);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Insufficient funds", badRequestResult.Value);
        }

        [Fact]
        public void AddTransaction_TransactionSucceeds_ReturnsOk()
        {
            // Arrange
            var transaction = new AccountBalanceTransaction { FromAccount = "AccountA", ToAccount = "AccountB", Amount = 0 };
            _accountServiceMock.Setup(service => service.TryPerformTransaction(transaction)).Returns(new AccountBalanceTransactionResult
            {
                Success = true,
                Message = "Transaction completed successfully"
            });

            // Act
            var result = _controller.AddTransaction(transaction);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Transaction completed successfully", okResult.Value);
        }
    }
}
