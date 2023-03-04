using Banks.Entities;
using Banks.Entities.Observers;
using Banks.Entities.Observers.Accrue;
using Banks.Entities.Observers.Notify;

namespace Banks.Services;

public class CentralBank : ICentralBank, IAccrueObservable
{
    private static CentralBank? _connection;
    private List<Bank> _banks;
    private List<IAccrueObserver> _accrueObservers;

    private CentralBank()
    {
        _banks = new List<Bank>();
        _accrueObservers = new List<IAccrueObserver>();
    }

    public static CentralBank GetConnection()
    {
        if (_connection == null)
        {
            _connection = new CentralBank();
        }

        return _connection;
    }

    public Bank CreateBank(
        string name,
        string address,
        decimal capital,
        decimal debitPercent,
        List<decimal> depositPercents,
        List<decimal> depositLimits,
        decimal creditPercent,
        decimal withdrawLimit,
        decimal creditCommission,
        DateTime depositExpirationDate)
    {
        Bank newBank = Bank.Builder
                           .WithName(name)
                           .WithAddress(address)
                           .WithCapital(capital)
                           .WithDebitPercent(debitPercent)
                           .WithDepositPercents(depositPercents)
                           .WithDepositLimits(depositLimits)
                           .WithCreditPercent(creditPercent)
                           .WithWithdrawLimit(withdrawLimit)
                           .WithCreditCommission(creditCommission)
                           .WithDepositExpirationDate(depositExpirationDate)
                           .Build();
        _banks.Add(newBank);
        AddObserver(newBank);
        return newBank;
    }

    public void AddObserver(IAccrueObserver o)
    {
        _accrueObservers.Add(o);
    }

    public void RemoveObserver(IAccrueObserver o)
    {
        _accrueObservers.Remove(o);
    }

    public void NotifyAccrueObservers(int days)
    {
        foreach (IAccrueObserver o in _accrueObservers)
        {
            o.Update(days);
        }
    }

    public void PassOneDay()
    {
        NotifyAccrueObservers(1);
    }

    public void PassDays(int days)
    {
        NotifyAccrueObservers(days);
    }

    public void Transfer(IAccount sender, IAccount recipient, decimal amount)
    {
        if (sender.IsQuestionable || recipient.IsQuestionable)
        {
            throw new Exception();
        }

        sender.Withdraw(amount);
        recipient.Replenish(amount);
    }

    public Bank GetBankById(Guid id) => _banks.Single(bank => bank.Id == id);
}