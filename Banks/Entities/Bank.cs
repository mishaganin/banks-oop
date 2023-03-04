using Banks.Entities.Accounts;
using Banks.Entities.Builders.Bank;
using Banks.Entities.Builders.Client;
using Banks.Entities.Commands;
using Banks.Entities.Observers;
using Banks.Entities.Observers.Accrue;
using Banks.Entities.Observers.Notify;
using Banks.Entities.Transactions;
using Banks.Models;
using INameBuilder = Banks.Entities.Builders.Bank.INameBuilder;

namespace Banks.Entities;

public class Bank : INotifyObservable, IAccrueObserver
{
    private List<Client> _clients;
    private List<DebitAccount> _debitAccounts;
    private List<DepositAccount> _depositAccounts;
    private List<CreditAccount> _creditAccounts;
    private List<Transaction> _transactionHistory;
    private List<INotifyObserver> _notifyObservers;
    private List<IAccrueObserver> _accrueObservers;
    private List<Tuple<ICommand, Guid>> _commandsHistory;
    private ICommand[] _transactionTypes;

    private Bank(
        string name,
        string address,
        decimal capital,
        decimal debitPercent,
        DateTime depositExpirationDate,
        List<decimal> depositPercents,
        List<decimal> depositLimits,
        decimal creditPercent,
        decimal withdrawLimit,
        decimal creditCommission)
    {
        Name = name;
        Address = address;
        Capital = capital;
        Id = Guid.NewGuid();
        DebitPercent = debitPercent;
        DepositExpirationDate = depositExpirationDate;
        DepositPercents = depositPercents;
        DepositLimits = depositLimits;
        CreditPercent = creditPercent;
        WithdrawLimit = withdrawLimit;
        CreditCommission = creditCommission;
        _clients = new List<Client>();
        _debitAccounts = new List<DebitAccount>();
        _depositAccounts = new List<DepositAccount>();
        _creditAccounts = new List<CreditAccount>();
        _transactionHistory = new List<Transaction>();
        _notifyObservers = new List<INotifyObserver>();
        _accrueObservers = new List<IAccrueObserver>();
        _commandsHistory = new List<Tuple<ICommand, Guid>>();
        _transactionTypes = new ICommand[3];
        for (int i = 0; i < _transactionTypes.Length; i++)
        {
            _transactionTypes[i] = new NoCommand();
        }

        SetCommands();
    }

    public static INameBuilder Builder => new BankBuilder();
    public string Name { get; private set; }
    public string Address { get; private set; }
    public decimal Capital { get; private set; }
    public Guid Id { get; }

    public decimal DebitPercent { get; private set; }
    public DateTime DepositExpirationDate { get; private set; }
    public List<decimal> DepositPercents { get; private set; }
    public List<decimal> DepositLimits { get; private set; }
    public decimal CreditPercent { get; private set; }
    public decimal WithdrawLimit { get; private set; }
    public decimal CreditCommission { get; private set; }

    public int DepositDays { get; }

    public void Update(int days)
    {
        NotifyAccrueObservers(days);
    }

    public void AddObserver(INotifyObserver o)
    {
        _notifyObservers.Add(o);
    }

    public void RemoveObserver(INotifyObserver o)
    {
        _notifyObservers.Remove(o);
    }

    public void AddObserver(IAccrueObserver o)
    {
        _accrueObservers.Add(o);
    }

    public void RemoveObserver(IAccrueObserver o)
    {
        _accrueObservers.Remove(o);
    }

    public void NotifyNotificationObservers(Notification notification)
    {
        foreach (INotifyObserver o in _notifyObservers)
        {
            o.Update(notification);
        }
    }

    public void NotifyAccrueObservers(int days)
    {
        foreach (IAccrueObserver o in _accrueObservers)
        {
            o.Update(days);
        }
    }

    public void IncreaseCapital(decimal amount)
    {
        Capital += amount;
    }

    public void DecreaseCapital(decimal amount)
    {
        if (Capital < amount)
        {
            throw new Exception(); // about bankruptcy
        }

        Capital -= amount;
    }

    public Client AddClient(string name, string surname, string? address = null, Passport? passport = null)
    {
        Client newClient = Client.Builder
                                 .WithName(name)
                                 .WithSurname(surname)
                                 .WithAddress(address)
                                 .WithPassport(passport)
                                 .Build();
        _clients.Add(newClient);
        _notifyObservers.Add(newClient);
        return newClient;
    }

    public void AddClientAddress(Guid clientId, string address)
    {
        Client client = GetClientById(clientId);

        for (int i = 0; i < _clients.Count; i++)
        {
            if (_clients[i].Id == clientId)
            {
                _clients[i] = client.AddAddress(address);
            }
        }

        if (client.Address == null || client.Passport == null) return;
        foreach (IAccount account in client.Accounts)
        {
            if (account.Bank == this)
            {
                account.NotEnoughQuestionable();
            }
        }
    }

    public void AddClientPassport(Guid clientId, Passport passport)
    {
        Client client = GetClientById(clientId);

        for (int i = 0; i < _clients.Count; i++)
        {
            if (_clients[i].Id == clientId)
            {
                _clients[i] = client.AddPassport(passport);
            }
        }

        if (client.Address == null || client.Passport == null) return;
        foreach (IAccount account in client.Accounts)
        {
            if (account.Bank == this)
            {
                account.NotEnoughQuestionable();
            }
        }
    }

    public DebitAccount CreateDebitAccount(decimal balance, Guid clientId)
    {
        Client client = GetClientById(clientId);
        bool isQuestionable = client.Address == null || client.Passport == null;
        DebitAccount debitAccount = new DebitAccount(this, balance, DebitPercent, WithdrawLimit, client, isQuestionable);
        _debitAccounts.Add(debitAccount);
        AddObserver(debitAccount);
        return debitAccount;
    }

    public DepositAccount CreateDepositAccount(decimal balance, Guid id)
    {
        Client client = GetClientById(id);
        bool isQuestionable = client.Address == null || client.Passport == null;
        decimal depositPercent = DepositPercents[^1];
        int index = 0;
        foreach (decimal depositLimit in DepositLimits)
        {
            if (balance < depositLimit)
            {
                depositPercent = DepositPercents[index];
            }
        }

        DepositAccount depositAccount = new DepositAccount(this, balance, depositPercent, WithdrawLimit, DepositExpirationDate, client, isQuestionable);
        _depositAccounts.Add(depositAccount);
        AddObserver(depositAccount);
        return depositAccount;
    }

    public CreditAccount CreateCreditAccount(decimal balance, Guid id)
    {
        Client client = GetClientById(id);
        bool isQuestionable = client.Address == null || client.Passport == null;
        CreditAccount creditAccount = new CreditAccount(this, balance, CreditPercent, WithdrawLimit, CreditCommission, client, isQuestionable);
        _creditAccounts.Add(creditAccount);
        AddObserver(creditAccount);
        return creditAccount;
    }

    public void AddTransaction(Transaction transaction)
    {
        _transactionHistory.Add(transaction);
    }

    public void SetCommands()
    {
        SetCommand(0, new WithdrawCommand());
        SetCommand(1, new ReplenishCommand());
        SetCommand(2, new TransferCommand());
    }

    public void SetCommand(int number, ICommand com)
    {
        _transactionTypes[number] = com;
    }

    public Guid MakeTransaction(int number, Guid accountId, decimal amountOfMoney, Guid? recipientId)
    {
        IAccount account = GetAccountById(accountId);
        IAccount? recipient = null;
        if (recipientId != null)
        {
            recipient = GetAccountById(recipientId ?? Guid.Empty);
        }

        _transactionTypes[number].Execute(account, amountOfMoney, recipient);
        Guid newId = Guid.NewGuid();
        var commandPair = new Tuple<ICommand, Guid>(_transactionTypes[number], newId);
        _commandsHistory.Add(commandPair);
        return newId;
    }

    public void UndoLastTransaction(Guid id)
    {
        if (_commandsHistory.Count > 0)
        {
            Tuple<ICommand, Guid> undoCommand = _commandsHistory.Single(command => command.Item2 == id) ?? throw new Exception("No such command with this id!");
            _commandsHistory.Remove(undoCommand);
            undoCommand.Item1.Undo();
        }
    }

    public void ChangeName(string name)
    {
        Name = name;
        var notification = new Notification("The bank name has changed");
        NotifyNotificationObservers(notification);
    }

    public void ChangeAddress(string address)
    {
        Address = address;
        var notification = new Notification("The bank address has changed");
        NotifyNotificationObservers(notification);
    }

    public void ChangeCapital(decimal capital)
    {
        Capital = capital;
        var notification = new Notification("The bank capital has changed");
        NotifyNotificationObservers(notification);
    }

    public void ChangeDebitPercent(decimal debitPercent)
    {
        DebitPercent = debitPercent;
        var notification = new Notification("The bank debit percent has changed");
        NotifyNotificationObservers(notification);
    }

    public void ChangeDepositExpirationDate(DateTime depositExpirationDate)
    {
        DepositExpirationDate = depositExpirationDate;
        var notification = new Notification("The bank deposit expiration date has changed");
        NotifyNotificationObservers(notification);
    }

    public void ChangeDepositPercents(List<decimal> depositPercents)
    {
        DepositPercents = depositPercents;
        var notification = new Notification("The bank deposit percents has changed");
        NotifyNotificationObservers(notification);
    }

    public void ChangeDepositLimits(List<decimal> depositLimits)
    {
        DepositLimits = depositLimits;
        var notification = new Notification("The bank deposit limits has changed");
        NotifyNotificationObservers(notification);
    }

    public void ChangeCreditPercent(decimal creditPercent)
    {
        CreditPercent = creditPercent;
        var notification = new Notification("The bank credit percent has changed");
        NotifyNotificationObservers(notification);
    }

    public void ChangeWithdrawLimit(decimal withdrawLimit)
    {
        WithdrawLimit = withdrawLimit;
        var notification = new Notification("The bank withdraw limit has changed");
        NotifyNotificationObservers(notification);
    }

    public void ChangeCreditCommission(decimal creditCommission)
    {
        CreditCommission = creditCommission;
        var notification = new Notification("The bank credit commission has changed");
        NotifyNotificationObservers(notification);
    }

    public IAccount GetAccountById(Guid id)
    {
        IAccount? account = _debitAccounts.SingleOrDefault(debitAccount => debitAccount.Id == id);
        if (account != null) return account;
        account = _depositAccounts.SingleOrDefault(depositAccount => depositAccount.Id == id);
        if (account != null) return account;
        account = _creditAccounts.Single(creditAccount => creditAccount.Id == id);
        return account;
    }

    public Client GetClientById(Guid id) => _clients.Single(client => client.Id == id);
    private Transaction GetTransactionById(Guid id) => _transactionHistory.Single(transaction => transaction.Id == id);

    private class BankBuilder
        : IAddressBuilder,
          IBankBuilder,
          ICapitalBuilder,
          ICreditCommissionBuilder,
          ICreditPercentBuilder,
          IDebitPercentBuilder,
          IDepositExpirationDateBuilder,
          IDepositLimitsBuilder,
          IDepositPercentsBuilder,
          INameBuilder,
          IWithdrawLimitBuilder
    {
        public BankBuilder()
        {
            Name = string.Empty;
            Address = string.Empty;
            Capital = 0;
            DebitPercent = 0;
            DepositPercents = new List<decimal>();
            DepositLimits = new List<decimal>();
            CreditPercent = 0;
            WithdrawLimit = 0;
            CreditCommission = 0;
            DepositExpirationDate = DateTime.Now;
        }

        private string Name { get; set; }
        private string Address { get; set; }
        private decimal Capital { get; set; }

        private decimal DebitPercent { get; set; }
        private List<decimal> DepositPercents { get; set; }
        private List<decimal> DepositLimits { get; set; }

        private decimal CreditPercent { get; set; }
        private decimal WithdrawLimit { get; set; }
        private decimal CreditCommission { get; set; }
        private DateTime DepositExpirationDate { get; set; }

        public ICapitalBuilder WithAddress(string address)
        {
            Address = address;
            return this;
        }

        public IDebitPercentBuilder WithCapital(decimal capital)
        {
            Capital = capital;
            return this;
        }

        public IDepositExpirationDateBuilder WithCreditCommission(decimal creditCommission)
        {
            CreditCommission = creditCommission;
            return this;
        }

        public IWithdrawLimitBuilder WithCreditPercent(decimal creditPercent)
        {
            CreditPercent = creditPercent;
            return this;
        }

        public IDepositPercentsBuilder WithDebitPercent(decimal debitPercent)
        {
            DebitPercent = debitPercent;
            return this;
        }

        public ICreditPercentBuilder WithDepositLimits(List<decimal> depositLimits)
        {
            DepositLimits = depositLimits;
            return this;
        }

        public IBankBuilder WithDepositExpirationDate(DateTime expirationDate)
        {
            DepositExpirationDate = expirationDate;
            return this;
        }

        public IDepositLimitsBuilder WithDepositPercents(List<decimal> depositPercents)
        {
            DepositPercents = depositPercents;
            return this;
        }

        public IAddressBuilder WithName(string name)
        {
            Name = name;
            return this;
        }

        public ICreditCommissionBuilder WithWithdrawLimit(decimal withdrawLimit)
        {
            WithdrawLimit = withdrawLimit;
            return this;
        }

        public Bank Build()
        {
            return new Bank(
                Name,
                Address,
                Capital,
                DebitPercent,
                DepositExpirationDate,
                DepositPercents,
                DepositLimits,
                CreditPercent,
                WithdrawLimit,
                CreditCommission);
        }
    }
}