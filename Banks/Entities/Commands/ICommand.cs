namespace Banks.Entities.Commands;

public interface ICommand
{
    public Guid Id { get; }
    void Execute(IAccount account, decimal amountOfMoney, IAccount? recipient);
    void Undo();
}