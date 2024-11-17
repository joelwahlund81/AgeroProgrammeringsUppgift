using AgeroProgrammeringsUppgift.Models;
using AgeroProgrammeringsUppgift.Services;
using Newtonsoft.Json;

public class AccountService : IAccountService
{
    private readonly string _fileName = "accounts.json";
    private readonly List<AccountModel> _accounts;

    public AccountService()
    {
        _accounts = ReadFromFile();
    }

    public AddAcountModelResult Create(AddAccountModel account)
    {
        var result = new AddAcountModelResult
        {
            Success = false
        };

        if (!CheckExists(account.Name))
        {
            var newAccount = new AccountModel
            {
                Name = account.Name,
                AccountType = (AccountTypes)Enum.Parse(typeof(AccountTypes), account.Type.ToLower()),
                CurrentBalance = 0,
                InitialBalance = 0,
            };

            _accounts.Add(newAccount);

            WriteToFile();

            result.Success = true;
            result.AccountModel = newAccount;
        }

        return result;
    }

    public bool CheckExists(string username)
    {
        return _accounts.Any(a => a.Name == username);
    }

    public AllAccountsModel GetAllAccounts()
    {
        return new AllAccountsModel(_accounts);
    }

    public AccountBalanceTransactionResult TryPerformTransaction(AccountBalanceTransaction accountBalanceTransaction)
    {
        var result = new AccountBalanceTransactionResult
        {
            Success = false
        };

        if (!CheckExists(accountBalanceTransaction.FromAccount))
        {
            result.Message = "From account doesn't seem to exist.";
        }

        if (!CheckExists(accountBalanceTransaction.ToAccount))
        {
            result.Message = "To account doesn't seem to exist."; 
        }

        var fromAccount = _accounts.Single(a => a.Name == accountBalanceTransaction.FromAccount);
        var toAccount = _accounts.Single(a => a.Name == accountBalanceTransaction.ToAccount);

        fromAccount.AddBalanceTransaction(accountBalanceTransaction.Amount);
        toAccount.AddBalanceTransaction(accountBalanceTransaction.Amount);

        try
        {
            fromAccount.CurrentBalance -= accountBalanceTransaction.Amount;

            if (toAccount.AccountType == AccountTypes.check)
            {
                toAccount.CurrentBalance += accountBalanceTransaction.Amount;
            }
            else
            {
                toAccount.CurrentBalance -= accountBalanceTransaction.Amount;
            }

            result.Success = true;
            result.Message = $"Transaction complete from '{fromAccount.Name}' to '{toAccount.Name}' with amount of '{accountBalanceTransaction.Amount}'";

            WriteToFile();
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
        }
        
        return result;
    }

    private List<AccountModel> ReadFromFile()
    {
        if (!File.Exists(_fileName))
        {
            return new List<AccountModel>();
        }

        var jsonData = File.ReadAllText(_fileName);
        return JsonConvert.DeserializeObject<List<AccountModel>>(jsonData) ?? new List<AccountModel>();
    }

    private void WriteToFile()
    {
        var jsonData = JsonConvert.SerializeObject(_accounts, Formatting.Indented);
        File.WriteAllText(_fileName, jsonData);
    }
}
