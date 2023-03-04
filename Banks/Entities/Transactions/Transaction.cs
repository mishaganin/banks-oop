namespace Banks.Entities.Transactions;

public class Transaction
{
    public Transaction(string type, decimal amountOfMoney, IAccount initiator, IAccount? recipient)
    {
        Type = type;
        AmountOfMoney = amountOfMoney;
        Initiator = initiator;
        Recipient = recipient;
        CompletionDate = DateTime.Now;
        Id = Guid.NewGuid();
        IsCancelled = false;
    }

    public string Type { get; }
    public decimal AmountOfMoney { get; }
    public IAccount Initiator { get; }
    public IAccount? Recipient { get; }
    public bool IsCancelled { get; }
    public DateTime CompletionDate { get; }
    public Guid Id { get; }
}