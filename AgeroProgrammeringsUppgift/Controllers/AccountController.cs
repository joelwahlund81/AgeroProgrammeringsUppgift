using AgeroProgrammeringsUppgift.Models;
using AgeroProgrammeringsUppgift.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgeroProgrammeringsUppgift.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    public IActionResult CreateAccount([FromBody] AddAccountModel account)
    {
        if (account == null)
        {
            return BadRequest("Invalid input: model is missing.");
        }

        if (string.IsNullOrWhiteSpace(account.Name))
        {
            return BadRequest("Invalid input: name cannot be null, empty or only consist of whitespace.");
        }

        if (!Enum.TryParse(account.Type?.ToLower(), out AccountTypes _))
        {
            return BadRequest("Invalid input: type must have a value of 'income', 'expense' or 'check'.");
        }

        var createdResult = _accountService.Create(account);

        if (!createdResult.Success)
        {
            return Conflict("An account with the same name already exists.");
        }

        return Ok(createdResult.AccountModel);
    }

    [HttpGet]
    public IActionResult GetAccounts()
    {
        var allAccounts = _accountService.GetAllAccounts();

        if (!allAccounts.AccountsExist)
        {
            return Ok(new List<AccountCurrentBalance>());
        }

        var accountsWithCurrentBalance = allAccounts.Accounts!.Select(a => new AccountCurrentBalance { Name = a.Name, CurrentBalance = a.CurrentBalance });

        return Ok(accountsWithCurrentBalance.OrderBy(a => a.Name).ToList());
    }

    
}
