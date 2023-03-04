using Banks.Entities.Commands;
using Banks.Entities.Observers.Accrue;
using Banks.Entities.Transactions;

namespace Banks.Entities.Accounts;

public class DebitAccount : IAccount, IAccrueObserver
{
    private const int DaysPerYear = 365;
    private const int DaysPerMonth = 30;
    private readonly List<string> _types = new List<string>() { "withdraw", "replenish", "transfer", };
    private List<Transaction> _transactionHistory;
    private Stack<Transaction> _withdrawHistory;
    private Stack<Transaction> _replenishHistory;
    private Stack<Transaction> _transferHistory;
    private List<decimal> _montlyDeposits;
    private int _daysUntilNextMonth;
    public DebitAccount(Bank bank, decimal balance, decimal percent, decimal withdrawLimit, Client client, bool isQuestionable)
    {
        Bank = bank;
        Balance = balance;
        Percent = percent / 100;
        WithdrawLimit = withdrawLimit;
        Client = client;
        IsQuestionable = isQuestionable;
        Id = Guid.NewGuid();
        _transactionHistory = new List<Transaction>();
        _montlyDeposits = new List<decimal>();
        _withdrawHistory = new Stack<Transaction>();
        _replenishHistory = new Stack<Transaction>();
        _transferHistory = new Stack<Transaction>();
        _daysUntilNextMonth = 0;
    }

    public Bank Bank { get; }
    public decimal Balance { get; private set; }
    public decimal Percent { get; }
    public decimal WithdrawLimit { get; }
    public Client Client { get; }
    public DateTime ExpirationDate { get; }
    public bool IsQuestionable { get; private set; }
    public Guid Id { get; }
    public decimal Accruals { get; private set; }
    public void Update(int days)
    {
        int totalDays = days + _daysUntilNextMonth;
        DailyAccruals(days);
        int months = totalDays / DaysPerMonth;
        for (int i = 0; i < months; i++)
        {
            MonthlyPayment();
        }

        _daysUntilNextMonth = totalDays % DaysPerMonth;
    }

    public decimal Withdraw(decimal amount)
    {
        if (IsQuestionable)
        {
            throw new Exception();
        }

        if (amount > Balance)
        {
            throw new Exception();
        }

        Balance -= amount;
        Bank.DecreaseCapital(amount);
        CreateTransaction(_types[0], amount, this);

        return Balance;
    }

    public void CancelWithdraw()
    {
        Transaction transaction = _withdrawHistory.Pop();
        Balance += transaction.AmountOfMoney;
        Bank.IncreaseCapital(transaction.AmountOfMoney);
    }

    public decimal Replenish(decimal amount)
    {
        Balance += amount;
        Bank.IncreaseCapital(amount);
        CreateTransaction(_types[1], amount, this);

        return Balance;
    }

    public void CancelReplenish()
    {
        Transaction transaction = _replenishHistory.Pop();
        Balance -= transaction.AmountOfMoney;
        Bank.DecreaseCapital(transaction.AmountOfMoney);
    }

    public decimal Transfer(decimal amount, IAccount account)
    {
        if (amount > Balance)
        {
            throw new Exception();
        }

        account.Replenish(amount);
        Balance -= amount;
        CreateTransaction(_types[1], amount, this, account);

        return Balance;
    }

    public void CancelTransfer()
    {
    }

    public decimal AddMoney(decimal amount)
    {
        Balance += amount;
        Bank.IncreaseCapital(amount);
        return Balance;
    }

    public decimal ReduceMoney(decimal amount)
    {
        Balance -= amount;
        Bank.DecreaseCapital(amount);
        return Balance;
    }

    public void MonthlyPayment()
    {
        _montlyDeposits.Add(Accruals);
        Accruals = 0;
    }

    public void AccrueAllDeposits()
    {
        foreach (decimal deposit in _montlyDeposits)
        {
            Balance += deposit;
            Bank.IncreaseCapital(deposit);
        }

        _montlyDeposits = new List<decimal>();
    }

    public void NotEnoughQuestionable()
    {
        IsQuestionable = false;
    }

    private void DailyAccruals(int days)
    {
        Accruals += (Percent / DaysPerYear) * days * Balance;
    }

    private Transaction CreateTransaction(string type, decimal amountOfMoney, IAccount initiator, IAccount? recipient = null)
    {
        Transaction newTransaction = new Transaction(type, amountOfMoney, initiator, recipient);
        _transactionHistory.Add(newTransaction);
        Bank.AddTransaction(newTransaction);
        switch (type)
        {
            case "withdraw":
                _withdrawHistory.Push(newTransaction);
                break;
            case "replenish":
                _replenishHistory.Push(newTransaction);
                break;
            case "transfer":
                _transferHistory.Push(newTransaction);
                break;
        }

        return newTransaction;
    }
}