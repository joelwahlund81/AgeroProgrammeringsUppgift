using System.Runtime.Serialization;
using System.Security.Principal;

namespace AgeroProgrammeringsUppgift.Models
{
    public class AllAccountsModel
    {
        public List<AccountModel>? Accounts { get; set; }

        public AllAccountsModel(List<AccountModel>? accounts)
        {
            Accounts = accounts;
        }

        public bool AccountsExist => Accounts?.Any() == true;
    }

    public class AddAccountModel
    {
        private string? _name;

        public required string Name
        {
            get => _name;
            set
            {
                if (value?.Length > 20)
                {
                    _name = value[..20];
                }
                else
                {
                    _name = value;
                }
            }
        }
        public required string Type { get; set; }
    }

    public class AddAcountModelResult
    {
        public bool Success { get; set; }
        public AccountModel? AccountModel { get; set; }
    }
    
    public class AccountModel
    {
        public decimal InitialBalance { get; set; } = 0;
        public required AccountTypes AccountType { get; set; }
        public string AccountTypeAsString => AccountType.GetEnumMemberValue();
        public List<BalanceTransaction>? BalanceTransactions { get; set; }
        public required string Name { get; set; }
        public decimal CurrentBalance { get; set; }

        public void AddBalanceTransaction(decimal amount)
        {
            BalanceTransactions?.Add(new BalanceTransaction { Amount = amount, PerformedAt = DateTime.UtcNow });
        }
    }

    public class AccountCurrentBalance 
    {
        public required string Name { get; set; }
        public required decimal CurrentBalance { get; set; }
    }

    public class BalanceTransaction
    {
        public DateTime PerformedAt { get; set; }
        public decimal Amount { get; set; }
    }

    public class AccountBalanceTransaction
    {
        public required string FromAccount { get; set; }
        public required string ToAccount { get; set; }
        public required decimal Amount { get; set; }
    }

    public class AccountBalanceTransactionResult 
    {
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }

    public enum AccountTypes
    {
        [EnumMember(Value = "Income")]
        income,
        [EnumMember(Value = "Expense")]
        expense,
        [EnumMember(Value = "Check")]
        check
    }
}
