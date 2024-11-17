using AgeroProgrammeringsUppgift.Models;
using AgeroProgrammeringsUppgift.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgeroProgrammeringsUppgift.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly IAccountService _accountService;

    public TransactionController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    public IActionResult AddTransaction(AccountBalanceTransaction accountBalanceTransaction)
    {
        if (accountBalanceTransaction == null)
        {
            return BadRequest("Invalid input: model is missing.");
        }

        if (string.IsNullOrWhiteSpace(accountBalanceTransaction.FromAccount))
        {
            return BadRequest("Invalid input: from account cannot be null, empty or only consist of whitespace.");
        }

        if (string.IsNullOrWhiteSpace(accountBalanceTransaction.ToAccount))
        {
            return BadRequest("Invalid input: to account cannot be null, empty or only consist of whitespace.");
        }

        if (accountBalanceTransaction.Amount > 50000)
        {
            return BadRequest("Invalid input: amount is too high, maximum is 50 000 per transaction.");
        }

        if (accountBalanceTransaction.Amount < 0)
        {
            return BadRequest("Invalid input: amount is too low, minimum is 0 per transaction.");
        }

        if (accountBalanceTransaction.Amount % 1 != 0)
        {
            return BadRequest("Invalid input: amount is not a integer.");
        }

        var result = _accountService.TryPerformTransaction(accountBalanceTransaction);

        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Message);
    }
}

