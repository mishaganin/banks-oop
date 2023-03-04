namespace Banks.Entities.Commands;

public class NoCommand : ICommand
{
    public NoCommand()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; }
    public void Execute(IAccount account, decimal amountOfMoney, IAccount? recipient = null)
    {
    }

    public void Undo()
    {
    }
}