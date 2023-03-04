using Banks.Entities.Transactions;

namespace Banks.Entities.Commands;

public class WithdrawCommand : ICommand
{
    private IAccount? _account;

    public WithdrawCommand()
    {
        _account = null;
        Id = Guid.NewGuid();
    }

    public Guid Id { get; }

    public void Execute(IAccount account, decimal amountOfMoney, IAccount? recipient = null)
    {
        _account = account;
        _account.Withdraw(amountOfMoney);
    }

    public void Undo()
    {
        _account?.CancelWithdraw();
    }
}