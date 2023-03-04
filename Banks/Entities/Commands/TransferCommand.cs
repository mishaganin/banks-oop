namespace Banks.Entities.Commands;

public class TransferCommand : ICommand
{
    private IAccount? _account;

    public TransferCommand()
    {
        _account = null;
        Id = Guid.NewGuid();
    }

    public Guid Id { get; }

    public void Execute(IAccount account, decimal amountOfMoney, IAccount? recipient = null)
    {
        _account = account;
        if (recipient != null)
        {
            if (account.Bank != recipient.Bank)
            {
                throw new Exception(); // can't transfer to another bank
            }

            if (account.IsQuestionable || recipient.IsQuestionable)
            {
                throw new Exception();
            }

            _account.Transfer(amountOfMoney, recipient);
        }
    }

    public void Undo()
    {
        _account?.CancelTransfer();
    }
}