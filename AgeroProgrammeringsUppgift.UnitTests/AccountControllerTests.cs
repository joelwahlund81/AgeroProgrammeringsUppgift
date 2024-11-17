using AgeroProgrammeringsUppgift.Controllers;
using AgeroProgrammeringsUppgift.Models;
using AgeroProgrammeringsUppgift.Services;
using Microsoft.AspNetCore.Mvc;

public class AccountControllerTests
{
    private readonly Mock<IAccountService> _accountServiceMock;
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        _accountServiceMock = new Mock<IAccountService>();
        _controller = new AccountController(_accountServiceMock.Object);
    }

    public class CreateAccount : AccountControllerTests
    {
        [Fact]
        public void CreateAccount_NullAccount_ReturnsBadRequest()
        {
            // Act
            var result = _controller.CreateAccount(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input: model is missing.", badRequestResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void CreateAccount_InvalidName_ReturnsBadRequest(string name)
        {
            // Arrange
            var account = new AddAccountModel
            {
                Name = name,
                Type = "Income"
            };

            // Act
            var result = _controller.CreateAccount(account);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input: name cannot be null, empty or only consist of whitespace.", badRequestResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Korv")]
        public void CreateAccount_InvalidAccountType_ReturnsBadRequest(string accountType)
        {
            // Arrange
            var account = new AddAccountModel
            {
                Name = "Say my name",
                Type = accountType
            };

            // Act
            var result = _controller.CreateAccount(account);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input: type must have a value of 'income', 'expense' or 'check'.", badRequestResult.Value);
        }

        [Fact]
        public void CreateAccount_AccountAlreadyExists_ReturnsConflict()
        {
            // Arrange
            var account = new AddAccountModel
            {
                Name = "Skuldindrivning",
                Type = "Income"
            };

            _accountServiceMock.Setup(service => service.Create(account)).Returns(new AddAcountModelResult { Success = false });

            // Act
            var result = _controller.CreateAccount(account);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("An account with the same name already exists.", conflictResult.Value);
        }

        [Fact]
        public void CreateAccount_ValidAccount_ReturnsOk()
        {
            // Arrange
            var newAccount = new AddAccountModel
            {
                Name = "Skuldindrivningskillen",
                Type = "Income"
            };

            _accountServiceMock.Setup(service => service.Create(newAccount)).Returns(new AddAcountModelResult { Success = true, AccountModel = new AccountModel { Name = newAccount.Name, AccountType = AccountTypes.income } });

            // Act
            var result = _controller.CreateAccount(newAccount);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var account = Assert.IsType<AccountModel>(okResult.Value);

            Assert.Equal("Skuldindrivningskill", account.Name);
        }
    }

    public class GetAccounts : AccountControllerTests
    {
        [Fact]
        public void GetAccounts_NoAccounts_ReturnsEmptyList()
        {
            // Arrange
            _accountServiceMock.Setup(service => service.GetAllAccounts()).Returns(new AllAccountsModel(null));

            // Act
            var result = _controller.GetAccounts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var accounts = Assert.IsType<List<AccountCurrentBalance>>(okResult.Value);
            Assert.Empty(accounts);
        }

        [Fact]
        public void GetAccounts_AccountsExist_ReturnsAccountsList()
        {
            // Arrange
            var accountsList = new List<AccountModel>
            {
                new AccountModel { Name = "John Doe", CurrentBalance = 100, InitialBalance = 0, AccountType = AccountTypes.income },
                new AccountModel { Name = "Jane Doe", CurrentBalance = 200, InitialBalance = 0, AccountType = AccountTypes.expense }
            };
            _accountServiceMock.Setup(service => service.GetAllAccounts()).Returns(new AllAccountsModel(accountsList));

            // Act
            var result = _controller.GetAccounts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var accounts = Assert.IsType<List<AccountCurrentBalance>>(okResult.Value);
            Assert.Equal(2, accounts?.Count);
            Assert.Equal("Jane Doe", accounts?[0].Name);
            Assert.Equal(200, accounts?[0].CurrentBalance);
            Assert.Equal("John Doe", accounts?[1].Name);
            Assert.Equal(100, accounts?[1].CurrentBalance);
        }
    }
}
