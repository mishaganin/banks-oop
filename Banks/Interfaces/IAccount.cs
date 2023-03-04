using Banks.Entities.Observers.Accrue;
using Banks.Entities.Transactions;

namespace Banks.Entities;

public interface IAccount : IAccrueObserver
{
    public Bank Bank { get; }
    public decimal Balance { get; }
    public decimal Percent { get; }
    public Client Client { get; }
    public DateTime ExpirationDate { get; }
    public bool IsQuestionable { get; }
    public Guid Id { get; }

    public decimal AddMoney(decimal amount);
    public decimal ReduceMoney(decimal amount);
    public decimal Withdraw(decimal amount);
    public void CancelWithdraw();
    public decimal Replenish(decimal amount);
    public void CancelReplenish();
    public decimal Transfer(decimal amount, IAccount account);
    public void CancelTransfer();
    public void AccrueAllDeposits();
    public void NotEnoughQuestionable();
}