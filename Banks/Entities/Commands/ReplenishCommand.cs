namespace Banks.Entities.Commands;

public class ReplenishCommand : ICommand
{
    private IAccount? _account;

    public ReplenishCommand()
    {
        _account = null;
        Id = Guid.NewGuid();
    }

    public Guid Id { get; }
    public void Execute(IAccount account, decimal amountOfMoney, IAccount? recipient = null)
    {
        _account = account;
        _account.Replenish(amountOfMoney);
    }

    public void Undo()
    {
        _account?.CancelReplenish();
    }
}