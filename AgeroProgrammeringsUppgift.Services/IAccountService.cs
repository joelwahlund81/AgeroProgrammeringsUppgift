using AgeroProgrammeringsUppgift.Models;

namespace AgeroProgrammeringsUppgift.Services
{
    public interface IAccountService
    {
        AddAcountModelResult Create(AddAccountModel model);
        bool CheckExists(string name);
        AllAccountsModel GetAllAccounts();
        AccountBalanceTransactionResult TryPerformTransaction(AccountBalanceTransaction accountBalanceTransaction);
    }
}
